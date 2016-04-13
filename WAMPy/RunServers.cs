using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;

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
        public List<int> ProcessesPIDs = new List<int>();

        private String _stdErr = "";
        private String _stdOut = "";

        MySQLConfigFile thePass = new MySQLConfigFile();

        public String stdErr
        {
            get { return _stdErr; }
            set
            {
                this._stdErr = value;
                if (_stdErr != "")
                {
                    Globals.AddToLog(_stdErr);
                }
            }
        }

        public String stdOut
        {
            get { return _stdOut; }
            set
            {
                this._stdOut = value;
                if (_stdOut != "")
                {
                    Globals.AddToLog(_stdOut);
                }
            }
        }

        public bool WebServerStart(String Exe)
        {
            String WSDir = Globals.AppFolder + @"bin\WebServer\" + Globals.ActiveWebServerVersion + @"\";
            String ExeFile = Exe;
            String Opts = null;
            switch(Globals.Servers.WebServer.Type)
            {
                case WebServers.WSType.Apache:
                    if (Globals.RewriteConfigFiles == "1")
                    {
                        //ApacheConfigFile ApacheFile = new ApacheConfigFile(WSDir);
                        ApacheConfigFile ApacheFile = new ApacheConfigFile(Directory.GetParent(Path.GetDirectoryName(ExeFile)).ToString() + @"\");
                        ApacheFile.MakeItCompatible(Globals.Servers.WebServer.Version());
                        ApacheFile.UpdateData(this.InternalPHP);
                    }
                break;
                case WebServers.WSType.nginx:
                    if (Globals.RewriteConfigFiles == "1")
                    {
                        nginxConfigFile nginxFile = new nginxConfigFile(WSDir);
                        nginxFile.UpdateData(this.ExternalPHP);
                    }
                break;
                case WebServers.WSType.lighttpd:
                    if (Globals.RewriteConfigFiles == "1")
                    {
                        lighttpdConfigFile lighttpdeFile = new lighttpdConfigFile(WSDir);
                        lighttpdeFile.UpdateData(this.ExternalPHP);
                    }
                    Opts = "-Df " + WSDir + @"\conf\lighttpd.conf";
                break;
            }

            this.WebServer = this.ServiceStart(ExeFile, Opts);
            if (this.WebServer.Id >= 0)
            {
                this.WebServerID = this.WebServer.Id;
                return true;
            }
            else
            {
                Globals.AddToLog("Unable to start the Web Server.");
                return false;
            }
        }

        public bool PHPStart(String Exe, String Args)
        {
            this.PHP = this.ServiceStart(Exe, Args);

            if (this.PHP.Id >= 0)
            {
                this.PHPID = this.PHP.Id;
                return true;
            }
            else
            {
                Globals.AddToLog("Unable to start the PHP CGI Server.");
                return false;
            }
        }

        public bool DataBaseStart(String Exe)
        {
            if (Globals.DataBasePassChange)
            {
                this.thePass.ChangeRootPasswordFile();
                this.DataBase = this.ServiceStart(Exe, "--explicit_defaults_for_timestamp --init-file=\"\\mysqlWSD\"");
            }
            else
            {
                this.DataBase = this.ServiceStart(Exe, "--explicit_defaults_for_timestamp");
            }

            if (this.DataBase.Id >= 0)
            {
                this.DataBaseID = this.DataBase.Id;
                return true;
            }
            else
            {
                Globals.AddToLog("Unable to start the DataBase Server.");
                return false;
            }
        }

        private Process ServiceStart(String Exe, String Arguments = null)
        {
            var p = new Process();
            int pID = -1;
            ThreadStart ths = new ThreadStart(() =>
            {
                p.StartInfo.WorkingDirectory = Path.GetDirectoryName(Exe);
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.RedirectStandardError = true;
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.CreateNoWindow = true;

                p.OutputDataReceived += new DataReceivedEventHandler((s, e) => { Globals.Processes.stdOut = e.Data; });
                p.ErrorDataReceived += new DataReceivedEventHandler((s, e) => { Globals.Processes.stdErr = e.Data; });

                p.StartInfo.FileName = Exe;
                if (Arguments != null)
                {
                    p.StartInfo.Arguments = Arguments;
                }
                p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;

                try
                {
                    p.Start();

                    if (!this.ProcessesPIDs.Contains(p.Id))
                    {
                        this.ProcessesPIDs.Add(p.Id);

                        p.BeginOutputReadLine();
                        p.BeginErrorReadLine();
                    }
                    pID = p.Id;

                    p.WaitForExit();
                }
                catch (InvalidOperationException ex)
                {
                    Globals.AddToLog("Error: " + ex.Message);
                }
                catch (Exception ex)
                {
                    Globals.AddToLog("Error: " + ex.Message);
                }
            });

            Thread th = new Thread(ths);
            th.IsBackground = true;
            th.Start();

            while (pID < 0 && th.IsAlive)
            {
                Thread.Sleep(50);
            }

            return p;
        }

        public bool WebServerStop()
        {
            String[] processNames = { "httpd", "Httpd", "apache", "Apache", "nginx", "lighttpd", "node", "nodeJS", "nodejs" };

            bool killed = this.KillProcesses(processNames);

            if (!this.isActiveProcess(this.WebServerID))
            {
                this.ProcessesPIDs.Remove(this.WebServerID);
            }

            this.WebServerID = -1;

            return killed;
        }

        public bool PHPStop()
        {
            String[] processNames = { "php", "php-cgi", "php-win" };

            bool killed = this.KillProcesses(processNames);

            if (!this.isActiveProcess(this.PHPID))
            {
                this.ProcessesPIDs.Remove(this.PHPID);
            }

            this.PHPID = -1;

            return killed;
        }

        public bool DataBaseStop()
        {
            String[] processNames = { "mysqld" };

            bool killed = this.KillProcesses(processNames);


            if (!this.isActiveProcess(this.DataBaseID))
            {
                this.ProcessesPIDs.Remove(this.DataBaseID);
            }

            this.DataBaseID = -1;
            
            this.thePass.RemoveFile();
            return killed;
        }

        public bool isActiveProcess(int pID)
        {
            try
            {
                Process Proc = Process.GetProcessById(pID);
                return true;
            }
            catch(Exception)
            {
                return false;
            }
        }

        private bool KillProcesses(String[] processNames)
        {
            int totalProcesses = 0;
            foreach (String proc in processNames)
            {
                totalProcesses = 0;
                int count = 1;
                do
                {
                    try
                    {
                        Process[] ListOfProcs = Process.GetProcessesByName(proc);
                        totalProcesses = ListOfProcs.Length;

                        if (ListOfProcs.Length > 0)
                        {
                            foreach (Process Proc in ListOfProcs)
                            {
                                Proc.EnableRaisingEvents = true;
                                Proc.Exited += new EventHandler(ProcessExited);

                                if (!this.ProcessesPIDs.Contains(Proc.Id))
                                {
                                    this.ProcessesPIDs.Add(Proc.Id);
                                }
                                Proc.Kill();
                            }
                        }
                    }
                    catch (Exception)
                    {
                        return false;
                    }
                    Thread.Sleep(50);
                } while (count <= 5 && totalProcesses > 0);
            }
            if (totalProcesses > 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private void ProcessExited(object sender, EventArgs e)
        {
            if (this.ProcessesPIDs.Contains(((Process)sender).Id))
            {
                try
                {
                    this.ProcessesPIDs.Remove(((Process)sender).Id);
                }
                catch (Exception ex) { }
            }
        }
    }
}
