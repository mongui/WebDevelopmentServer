using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace WDS
{
    public class RunServers
    {
        public Process WebServer;
        public Process PHP;
        public Process DataBase;

        public bool InternalPHP = false;
        public bool ExternalPHP = false;

        public int WebServerID = -1;
        public int PHPID = -1;
        public int DataBaseID = -1;

        private readonly List<int> _pids = new List<int>();
        // Processes this program is stopping: their exit is not reported as a failure.
        private readonly HashSet<int> _stopping = new HashSet<int>();
        private readonly object _lock = new object();

        MySQLConfigFile thePass = new MySQLConfigFile();

        /// <summary>
        /// Number of servers started by this program that are still running.
        /// </summary>
        public int RunningCount
        {
            get { lock (_lock) { return _pids.Count; } }
        }

        public bool WebServerStart(String Exe)
        {
            if (String.IsNullOrEmpty(Exe) || !File.Exists(Exe))
            {
                Globals.AddToLog("Unable to start the Web Server: executable not found.");
                return false;
            }

            String ExeDir = Path.GetDirectoryName(Exe) + @"\";
            String Opts = null;
            switch (Globals.Servers.WebServer.Type)
            {
                case WebServers.WSType.Apache:
                    if (Globals.RewriteConfigFiles == "1")
                    {
                        // httpd.exe lives in <ServerRoot>\bin.
                        ApacheConfigFile ApacheFile = new ApacheConfigFile(Directory.GetParent(Path.GetDirectoryName(Exe)).FullName + @"\");
                        ApacheFile.MakeItCompatible(Globals.Servers.WebServer.Version());
                        ApacheFile.UpdateData((this.InternalPHP || this.ExternalPHP) ? Globals.WebPhpLink : null);
                    }
                    break;
                case WebServers.WSType.nginx:
                    if (Globals.RewriteConfigFiles == "1")
                    {
                        nginxConfigFile nginxFile = new nginxConfigFile(ExeDir);
                        nginxFile.UpdateData(this.ExternalPHP);
                    }
                    Opts = "-p \"" + ExeDir.Replace('\\', '/') + "\" -c conf/nginx.conf";
                    break;
                case WebServers.WSType.lighttpd:
                    if (Globals.RewriteConfigFiles == "1")
                    {
                        lighttpdConfigFile lighttpdFile = new lighttpdConfigFile(ExeDir);
                        lighttpdFile.UpdateData(this.ExternalPHP);
                    }
                    Opts = "-D -f \"" + ExeDir + "conf\\lighttpd.conf\"";
                    break;
                default:
                    Globals.AddToLog(Globals.Servers.WebServer.Type + " is not supported yet. The Web Server was not started.");
                    return false;
            }

            // The PHP module loads DLLs from the PHP folder (php8ts.dll, extensions...).
            Dictionary<String, String> env = null;
            if (Globals.WebPhpLink != null && Globals.WebPhpLink.Link == PhpLink.ModPHP && this.InternalPHP)
            {
                env = new Dictionary<String, String>();
                env["PATH"] = Path.GetDirectoryName(Globals.Servers.PHP.Exe) + ";" + Environment.GetEnvironmentVariable("PATH");
            }

            // Apache and nginx write their startup errors to logs\error.log.
            String errorLog = null;
            if (Globals.Servers.WebServer.Type == WebServers.WSType.Apache)
                errorLog = Path.Combine(Directory.GetParent(Path.GetDirectoryName(Exe)).FullName, @"logs\error.log");
            else if (Globals.Servers.WebServer.Type == WebServers.WSType.nginx)
                errorLog = Path.Combine(ExeDir, @"logs\error.log");

            this.WebServer = this.ServiceStart(Exe, Opts, env, errorLog, false);
            if (this.WebServer != null)
            {
                this.WebServerID = this.WebServer.Id;
                return true;
            }

            Globals.AddToLog("Unable to start the Web Server.");
            return false;
        }

        public bool PHPStart(String Exe, String Args)
        {
            if (String.IsNullOrEmpty(Exe) || !File.Exists(Exe))
            {
                Globals.AddToLog("Unable to start PHP: executable not found.");
                return false;
            }

            // php-cgi.exe exits after 500 requests by default; 0 = never.
            Dictionary<String, String> env = new Dictionary<String, String>();
            env["PHP_FCGI_MAX_REQUESTS"] = "0";

            this.PHP = this.ServiceStart(Exe, Args, env);
            if (this.PHP != null)
            {
                this.PHPID = this.PHP.Id;
                return true;
            }

            Globals.AddToLog("Unable to start the PHP CGI Server.");
            return false;
        }

        /// <summary>
        /// Starts MySQL/MariaDB. phpLink (may be null) says which password format PHP needs.
        /// </summary>
        public bool DataBaseStart(String Exe, PairResult phpLink = null)
        {
            if (String.IsNullOrEmpty(Exe) || !File.Exists(Exe))
            {
                Globals.AddToLog("Unable to start the DataBase Server: executable not found.");
                return false;
            }

            SoftwareComponent db = Compatibility.FromFolder(ComponentKind.DataBase, Compatibility.BinFolder(ComponentKind.DataBase) + Globals.ActiveMySQLVersion);
            if (!this.CreateDataFolder(db))
            {
                return false;
            }

            bool mariaDB = (db.Product == Product.MariaDB);
            DbAuthMode mode = (phpLink != null && phpLink.Compatible) ? phpLink.Auth : DbAuthMode.Default;

            // Only listen on this machine.
            String Args = "--bind-address=127.0.0.1";
            // Unknown options stop mysqld, so only add them where they exist.
            if (mariaDB ? db.AtLeast(10, 1, 8) : db.AtLeast(5, 6, 6))
            {
                Args += " --explicit_defaults_for_timestamp";
            }
            switch (mode)
            {
                case DbAuthMode.Native:
                    Args += db.AtLeast(8, 4) ? " --mysql-native-password=ON" : " --default-authentication-plugin=mysql_native_password";
                    break;
                case DbAuthMode.Old:
                    Args += " --old-passwords=1";
                    if (!mariaDB && db.AtLeast(5, 6, 5)) Args += " --secure-auth=OFF";
                    break;
            }
            if (phpLink != null && phpLink.Compatible && phpLink.Utf8Fallback)
            {
                Args += " --character-set-server=utf8 --collation-server=utf8_general_ci";
            }

            // mysqld writes its messages to a file, not to the console: use a known one,
            // so its errors can be copied to the program log.
            String root = Compatibility.DataBaseRoot(Exe);
            this.DataBaseErrorLog = Path.Combine(root, @"data\wds-mysqld.err");
            long errorLogStart = 0;
            try { if (File.Exists(this.DataBaseErrorLog)) errorLogStart = new FileInfo(this.DataBaseErrorLog).Length; } catch (Exception) { }
            Args += " --log-error=\"" + this.DataBaseErrorLog.Replace('\\', '/') + "\"";

            // The data folder remembers which password (and format) root has. A new data
            // folder, a new password or another format means it has to be set again.
            String marker = Path.Combine(root, @"data\wds-root-password.txt");
            String markerText = null;
            try { if (File.Exists(marker)) markerText = File.ReadAllText(marker).Trim(); } catch (Exception) { }
            bool needInit = Globals.DataBasePassPending == "1" || markerText != mode.ToString();

            bool changingPass = false;

            if (needInit)
            {
                String InitFile = this.thePass.ChangeRootPasswordFile(mode, db);
                if (InitFile != null)
                {
                    Args += " --init-file=\"" + InitFile.Replace('\\', '/') + "\"";
                    changingPass = true;
                }
                else
                {
                    Globals.AddToLog("Unable to write the file that changes the root password. The password was not changed.");
                }
            }

            this.DataBase = this.ServiceStart(Exe, Args, null, this.DataBaseErrorLog, true);
            if (this.DataBase != null)
            {
                this.DataBaseID = this.DataBase.Id;
                this.WatchDataBaseStart(this.DataBase, errorLogStart, changingPass ? marker : null, mode);
                return true;
            }

            this.thePass.RemoveFile();
            Globals.AddToLog("Unable to start the DataBase Server.");
            return false;
        }

        /// <summary>
        /// Error log of the running database server.
        /// </summary>
        public String DataBaseErrorLog;

        /// <summary>
        /// Copies the [ERROR] lines written since 'from' in the database error log to the program log.
        /// </summary>
        public void CopyDataBaseErrors(long from)
        {
            CopyServerLog(this.DataBaseErrorLog, from, true);
        }

        /// <summary>
        /// Copies what a server wrote to its own log since 'from' to the program log:
        /// only [ERROR]/Aborting lines (onlyErrors) or the last 20 lines.
        /// </summary>
        private static void CopyServerLog(String file, long from, bool onlyErrors)
        {
            try
            {
                if (String.IsNullOrEmpty(file) || !File.Exists(file)) return;
                List<String> lines = ReadFrom(file, from);
                if (onlyErrors)
                {
                    lines = lines.FindAll(l => l.IndexOf("[ERROR]", StringComparison.OrdinalIgnoreCase) >= 0 ||
                                               l.IndexOf("Aborting", StringComparison.OrdinalIgnoreCase) >= 0);
                }
                if (lines.Count > 20) lines = lines.GetRange(lines.Count - 20, 20);
                if (lines.Count == 0) return;

                Globals.AddToLog("Last messages in " + file + ":");
                foreach (String line in lines)
                {
                    Globals.AddToLog("  " + line);
                }
            }
            catch (Exception ex)
            {
                Globals.LogError("Unable to read " + file, ex);
            }
        }

        private static List<String> ReadFrom(String file, long from)
        {
            List<String> lines = new List<String>();
            using (FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete))
            {
                if (from > fs.Length) from = 0;
                fs.Seek(from, SeekOrigin.Begin);
                using (StreamReader reader = new StreamReader(fs))
                {
                    String line;
                    while ((line = reader.ReadLine()) != null) lines.Add(line);
                }
            }
            return lines;
        }

        /// <summary>
        /// Waits until mysqld says "ready for connections": by then it has run the --init-file,
        /// so the file (with the password in plain text) can be deleted and the new password
        /// marked as applied. If mysqld stops first, its errors are copied to the log.
        /// </summary>
        private void WatchDataBaseStart(Process db, long errorLogStart, String marker, DbAuthMode mode)
        {
            System.Threading.Thread t = new System.Threading.Thread(() =>
            {
                bool ready = false;
                DateTime limit = DateTime.Now.AddMinutes(3);
                while (DateTime.Now < limit)
                {
                    try
                    {
                        if (File.Exists(this.DataBaseErrorLog))
                        {
                            foreach (String line in ReadFrom(this.DataBaseErrorLog, errorLogStart))
                            {
                                if (line.IndexOf("ready for connections", StringComparison.OrdinalIgnoreCase) >= 0) { ready = true; break; }
                            }
                        }
                    }
                    catch (Exception) { }
                    if (ready) break;

                    bool exited;
                    try { exited = db.HasExited; } catch (Exception) { exited = true; }
                    if (exited) break;

                    System.Threading.Thread.Sleep(500);
                }

                if (ready)
                {
                    Globals.AddToLog("The database is ready for connections.");
                    if (marker != null)
                    {
                        try { File.WriteAllText(marker, mode.ToString()); }
                        catch (Exception ex) { Globals.LogError("Unable to write " + marker + " (the password will be set again on the next start)", ex); }
                        Globals.DataBasePassPending = "0";
                        Config.SaveSetting("DataBasePassPending", "0");
                        Globals.AddToLog("The root password of the database has been updated.");
                    }
                }
                else
                {
                    bool exited;
                    try { exited = db.HasExited; } catch (Exception) { exited = true; }
                    if (!exited)
                    {
                        // If it stopped, the exit handler already copied its errors.
                        Globals.AddToLog("The database did not say 'ready for connections' after 3 minutes.");
                        this.CopyDataBaseErrors(errorLogStart);
                    }
                }

                if (marker != null)
                {
                    this.thePass.RemoveFile();
                }
            });
            t.IsBackground = true;
            t.Start();
        }

        /// <summary>
        /// MySQL 5.7+ and recent MariaDB packages have no data folder: it is created here
        /// once, with an empty root password that is changed on the first start.
        /// </summary>
        private bool CreateDataFolder(SoftwareComponent db)
        {
            if (db.Exe == null || !Compatibility.NeedsDataInit(db.Exe))
            {
                return true;
            }

            String tool = Compatibility.DataInitTool(db);
            String root = Compatibility.DataBaseRoot(db.Exe);
            String data = Path.Combine(root, "data");
            if (tool == null)
            {
                Globals.AddToLog("The database has no data folder (" + data + ") and no tool to create it.");
                return false;
            }

            try
            {
                // --initialize needs an empty (or missing) data folder.
                // A folder without system tables is left over from a failed start: keep it
                // aside (nothing is deleted) and create a new one.
                if (Directory.Exists(data) && Directory.GetFileSystemEntries(data).Length > 0)
                {
                    String aside = data + ".old-" + DateTime.Now.ToString("yyyyMMdd-HHmmss");
                    Directory.Move(data, aside);
                    Globals.AddToLog("The folder " + data + " had no system tables (a failed start). It was moved to " + aside + " and a new one is created.");
                }

                String args = (db.Product == Product.MySQL)
                    ? "--initialize-insecure --basedir=\"" + root + "\" --datadir=\"" + data + "\" --console"
                    : "--datadir=\"" + data + "\"";

                Globals.AddToLog("Creating the database data folder. This can take a minute...");
                System.Windows.Forms.Application.DoEvents();

                using (Process p = new Process())
                {
                    p.StartInfo.FileName = tool;
                    p.StartInfo.Arguments = args;
                    p.StartInfo.WorkingDirectory = Path.GetDirectoryName(tool);
                    p.StartInfo.UseShellExecute = false;
                    p.StartInfo.CreateNoWindow = true;
                    p.StartInfo.RedirectStandardOutput = true;
                    p.StartInfo.RedirectStandardError = true;
                    p.OutputDataReceived += (s, e) => Globals.AddToLog(e.Data);
                    p.ErrorDataReceived += (s, e) => Globals.AddToLog(e.Data);
                    p.Start();
                    p.BeginOutputReadLine();
                    p.BeginErrorReadLine();
                    if (!p.WaitForExit(300000))
                    {
                        try { p.Kill(); } catch (Exception) { }
                        Globals.AddToLog("Creating the data folder took too long and was cancelled.");
                        return false;
                    }
                    p.WaitForExit();
                    if (p.ExitCode != 0 || Compatibility.NeedsDataInit(db.Exe))
                    {
                        Globals.AddToLog("Unable to create the data folder (exit code " + p.ExitCode + ").");
                        String blocked = Compatibility.ControlledFolderWarning(db.Exe);
                        if (blocked != null) Globals.AddToLog("Probable cause: " + blocked);
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Globals.LogError("Unable to create the data folder", ex);
                return false;
            }

            // New system tables: root has no password yet.
            Globals.DataBasePassPending = "1";
            Config.SaveSetting("DataBasePassPending", "1");
            Globals.AddToLog("Database data folder created.");
            return true;
        }


        /// <summary>
        /// Starts a process with redirected output. Returns null if it could not be started.
        /// </summary>
        private Process ServiceStart(String Exe, String Arguments = null, Dictionary<String, String> env = null,
                                     String errorLog = null, bool onlyErrors = false)
        {
            long errorLogStart = 0;
            try { if (errorLog != null && File.Exists(errorLog)) errorLogStart = new FileInfo(errorLog).Length; } catch (Exception) { }

            // The full command, for the log file (it never contains passwords).
            Globals.AddToLogFile("Starting: \"" + Exe + "\" " + (Arguments ?? ""));

            Process p = new Process();
            p.StartInfo.FileName = Exe;
            p.StartInfo.WorkingDirectory = Path.GetDirectoryName(Exe);
            if (Arguments != null)
            {
                p.StartInfo.Arguments = Arguments;
            }
            if (env != null)
            {
                foreach (KeyValuePair<String, String> kv in env)
                {
                    p.StartInfo.EnvironmentVariables[kv.Key] = kv.Value;
                }
            }
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;

            p.OutputDataReceived += (s, e) => Globals.AddToLog(e.Data);
            p.ErrorDataReceived += (s, e) => Globals.AddToLog(e.Data);

            try
            {
                if (!p.Start())
                {
                    p.Dispose();
                    return null;
                }
            }
            catch (Exception ex)
            {
                Globals.LogError("Error starting " + Path.GetFileName(Exe) + "", ex);
                p.Dispose();
                return null;
            }

            int pid = p.Id;
            lock (_lock)
            {
                _pids.Add(pid);
            }

            p.EnableRaisingEvents = true;
            String name = Path.GetFileName(Exe);
            p.Exited += (s, e) =>
            {
                bool expected;
                lock (_lock)
                {
                    _pids.Remove(pid);
                    expected = _stopping.Remove(pid);
                }
                // An exit right after starting is reported by ServiceStart itself.
                try { if ((p.ExitTime - p.StartTime).TotalMilliseconds < 400) expected = true; } catch (Exception) { }
                if (!expected)
                {
                    int code = 0;
                    try { code = p.ExitCode; } catch (Exception) { }
                    Globals.AddToLog("ERROR: " + name + " stopped by itself (exit code " + code + ").");
                    CopyServerLog(errorLog, errorLogStart, onlyErrors);
                }
            };

            p.BeginOutputReadLine();
            p.BeginErrorReadLine();

            // A process that exits at once usually failed (bad config, port in use...).
            if (p.WaitForExit(300))
            {
                Globals.AddToLog("ERROR: " + Path.GetFileName(Exe) + " exited right after starting (exit code " + p.ExitCode + ").");
                lock (_lock) { _stopping.Add(pid); } // Already reported here.
                CopyServerLog(errorLog, errorLogStart, onlyErrors);
                return null;
            }

            Globals.AddToLog(name + " started (PID " + pid + ").");
            return p;
        }

        public bool WebServerStop()
        {
            bool killed = this.StopProcess(ref this.WebServer);
            this.WebServerID = -1;
            this.KillLeftovers(new String[] { "httpd", "apache", "nginx", "lighttpd" });
            return killed;
        }

        public bool PHPStop()
        {
            bool killed = this.StopProcess(ref this.PHP);
            this.PHPID = -1;
            this.KillLeftovers(new String[] { "php", "php-cgi", "php-win" });
            return killed;
        }

        public bool DataBaseStop()
        {
            bool killed = this.StopProcess(ref this.DataBase);
            this.DataBaseID = -1;
            this.KillLeftovers(new String[] { "mysqld", "mariadbd" });
            this.thePass.RemoveFile();
            return killed;
        }

        /// <summary>
        /// Kills a process started by this program together with its child processes.
        /// </summary>
        private bool StopProcess(ref Process proc)
        {
            if (proc == null)
            {
                return true;
            }

            bool ok = true;
            try
            {
                if (!proc.HasExited)
                {
                    Globals.AddToLog("Stopping " + Path.GetFileName(proc.StartInfo.FileName) + " (PID " + proc.Id + ")...");
                    lock (_lock) { _stopping.Add(proc.Id); }
                    KillTree(proc.Id);
                    ok = proc.WaitForExit(5000);
                }
            }
            catch (Exception ex)
            {
                Globals.LogError("Error stopping a server", ex);
                ok = false;
            }

            lock (_lock)
            {
                try { _pids.Remove(proc.Id); } catch (Exception) { }
            }
            proc.Dispose();
            proc = null;
            return ok;
        }

        private static void KillTree(int pid)
        {
            using (Process taskkill = new Process())
            {
                taskkill.StartInfo.FileName = "taskkill";
                taskkill.StartInfo.Arguments = "/PID " + pid + " /T /F";
                taskkill.StartInfo.UseShellExecute = false;
                taskkill.StartInfo.CreateNoWindow = true;
                taskkill.StartInfo.RedirectStandardOutput = true;
                taskkill.StartInfo.RedirectStandardError = true;
                taskkill.Start();
                taskkill.StandardOutput.ReadToEnd();
                taskkill.StandardError.ReadToEnd();
                taskkill.WaitForExit(5000);
            }
        }

        /// <summary>
        /// Kills processes left by a previous run of this program (for example after a crash).
        /// Only processes whose executable is inside this program's "bin" folder are touched,
        /// so servers of other programs keep running.
        /// </summary>
        private void KillLeftovers(String[] processNames)
        {
            String binFolder = Path.GetFullPath(Globals.AppFolder + @"bin\");

            foreach (String name in processNames)
            {
                Process[] procs;
                try
                {
                    procs = Process.GetProcessesByName(name);
                }
                catch (Exception)
                {
                    continue;
                }

                foreach (Process proc in procs)
                {
                    try
                    {
                        String file = proc.MainModule.FileName;
                        if (file.StartsWith(binFolder, StringComparison.OrdinalIgnoreCase))
                        {
                            proc.Kill();
                            proc.WaitForExit(5000);
                        }
                    }
                    catch (Exception)
                    {
                        // No access to that process: it is not ours.
                    }
                    finally
                    {
                        proc.Dispose();
                    }
                }
            }
        }
    }
}
