using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace WDS
{
    public class WebServers : Versions
    {
        public enum WSType
        {
            none,
            Apache,
            nginx,
            lighttpd,
            NodeJS,
            IIS
        };
        public WSType Type = WSType.none;
        public String[] AvailableFiles = { "httpd.exe", "apache.exe", "nginx.exe", "lighttpd.exe", "node.exe", "iisexpress.exe" };

        public String[] AvailableModules;
        public List<String> ActiveModules = new List<String>();
        public List<ConfigModules> ModulesConfig = new List<ConfigModules>();
        public List<CustomSetting> CustomSettings = new List<CustomSetting>();
    }

    public class PHPs : Versions
    {
        public String[] AvailableFiles = { "php-cgi.exe", "php.exe" };

        public String[] AvailableModules;
        public List<String> ActiveModules = new List<String>();
    }

    public class DataBases : Versions
    {
        public enum DBType
        {
            none,
            MySQL,
            MariaDB
        }
        public DBType Type = DBType.none;
        public String[] AvailableFiles = { "mysqld.exe", "mariadbd.exe" };
    }

    public class Versions
    {
        public String Exe;
        public bool Is32Bits;
        public bool Is64Bits;

        // -1 = unknown.
        private int _VerNum1 = -1;
        private int _VerNum2 = -1;
        private int _VerNum3 = -1;

        /// <summary>
        /// Known version numbers (1 to 3 of them), or { -1 } if unknown.
        /// </summary>
        public int[] Version()
        {
            if (this._VerNum1 < 0) { return new int[] { -1 }; }
            if (this._VerNum2 < 0) { return new int[] { this._VerNum1 }; }
            if (this._VerNum3 < 0) { return new int[] { this._VerNum1, this._VerNum2 }; }
            return new int[] { this._VerNum1, this._VerNum2, this._VerNum3 };
        }

        public String VersionText()
        {
            int[] v = this.Version();
            return (v[0] < 0) ? "Unknown" : String.Join(".", Array.ConvertAll(v, n => n.ToString()));
        }

        /// <summary>
        /// Version number at a position, or 0 if unknown.
        /// </summary>
        public int VersionPart(int index)
        {
            int[] v = this.Version();
            return (index < v.Length && v[index] >= 0) ? v[index] : 0;
        }

        public int[] Version(String value)
        {
            this._VerNum1 = this._VerNum2 = this._VerNum3 = -1;
            if (value != null)
            {
                // Accepts "2.4.17", "5.6", "10.4.12-MariaDB", "8.0.33.0"...
                Match m = Regex.Match(value, @"(\d+)(?:\.(\d+))?(?:\.(\d+))?");
                if (m.Success)
                {
                    this._VerNum1 = Int32.Parse(m.Groups[1].Value);
                    if (m.Groups[2].Success) this._VerNum2 = Int32.Parse(m.Groups[2].Value);
                    if (m.Groups[3].Success) this._VerNum3 = Int32.Parse(m.Groups[3].Value);
                }
            }

            return this.Version();
        }

        /// <summary>
        /// Reads version and architecture of the executable.
        /// </summary>
        protected void ReadExeInfo()
        {
            try
            {
                FileVersionInfo info = FileVersionInfo.GetVersionInfo(this.Exe);
                this.Version(info.ProductVersion ?? info.FileVersion);
            }
            catch (Exception)
            {
                this.Version((String)null);
            }

            bool? is64 = null;
            try
            {
                is64 = Is64Bit.GetBits.UnmanagedDllIs64Bit(this.Exe);
            }
            catch (Exception)
            {
            }
            this.Is64Bits = (is64 == true);
            this.Is32Bits = (is64 == false);
        }

        public void LoadExe(String exe)
        {
            this.Exe = exe;
            if (File.Exists(exe))
            {
                this.ReadExeInfo();
            }
        }
    }

    public class ServerType
    {
        public WebServers WebServer = new WebServers();
        public PHPs PHP = new PHPs();
        public DataBases DataBase = new DataBases();

        public ServerType()
        {
            // Get servers executables.
            if (!String.IsNullOrEmpty(Globals.ActiveWebServerVersion))
                this.WebServer.LoadExe(FindServerFile(this.WebServer.AvailableFiles, Globals.AppFolder + @"bin\WebServer\" + Globals.ActiveWebServerVersion + @"\"));
            if (!String.IsNullOrEmpty(Globals.ActivePHPVersion))
                this.PHP.LoadExe(FindServerFile(this.PHP.AvailableFiles, Globals.AppFolder + @"bin\PHP\" + Globals.ActivePHPVersion + @"\"));
            if (!String.IsNullOrEmpty(Globals.ActiveMySQLVersion))
                this.DataBase.LoadExe(FindServerFile(this.DataBase.AvailableFiles, Globals.AppFolder + @"bin\MySQL\" + Globals.ActiveMySQLVersion + @"\"));

            if (File.Exists(this.WebServer.Exe))
            {
                String exe = Path.GetFileName(this.WebServer.Exe).ToLowerInvariant();
                switch (exe)
                {
                    case "lighttpd.exe": this.WebServer.Type = WebServers.WSType.lighttpd; break;
                    case "httpd.exe":
                    case "apache.exe": this.WebServer.Type = WebServers.WSType.Apache; break;
                    case "nginx.exe": this.WebServer.Type = WebServers.WSType.nginx; break;
                    case "node.exe": this.WebServer.Type = WebServers.WSType.NodeJS; break;
                    case "iisexpress.exe": this.WebServer.Type = WebServers.WSType.IIS; break;
                }
            }

            if (File.Exists(this.DataBase.Exe))
            {
                this.DataBase.Type = IsMariaDB(this.DataBase.Exe) ? DataBases.DBType.MariaDB : DataBases.DBType.MySQL;
            }
        }

        internal static bool IsMariaDB(String exe)
        {
            if (Path.GetFileName(exe).Equals("mariadbd.exe", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            try
            {
                FileVersionInfo info = FileVersionInfo.GetVersionInfo(exe);
                String text = (info.ProductName ?? "") + " " + (info.CompanyName ?? "") + " " + (info.ProductVersion ?? "");
                if (text.IndexOf("MariaDB", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return true;
                }
            }
            catch (Exception)
            {
            }

            // mysqld.exe is in <root>\bin; MariaDB packages have a README that names it.
            try
            {
                DirectoryInfo root = Directory.GetParent(Path.GetDirectoryName(exe));
                foreach (String name in new String[] { "README", "README.md", "README-wsrep" })
                {
                    String readme = Path.Combine(root.FullName, name);
                    if (File.Exists(readme))
                    {
                        using (StreamReader reader = new StreamReader(readme))
                        {
                            String line = reader.ReadLine();
                            if (line != null && line.IndexOf("MariaDB", StringComparison.OrdinalIgnoreCase) >= 0)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
            return false;
        }

        /// <summary>
        /// Breadth-first search of the first known executable inside ServerPath.
        /// </summary>
        internal static String FindServerFile(String[] ServerFiles, String ServerPath)
        {
            if (!Directory.Exists(ServerPath))
            {
                return null;
            }

            Queue<DirectoryInfo> pending = new Queue<DirectoryInfo>();
            pending.Enqueue(new DirectoryInfo(ServerPath));

            while (pending.Count > 0)
            {
                DirectoryInfo dir = pending.Dequeue();
                try
                {
                    // Respect the order of ServerFiles (php-cgi.exe before php.exe...).
                    foreach (String SF in ServerFiles)
                    {
                        String candidate = Path.Combine(dir.FullName, SF);
                        if (File.Exists(candidate))
                        {
                            return candidate;
                        }
                    }

                    foreach (DirectoryInfo sub in dir.GetDirectories())
                    {
                        pending.Enqueue(sub);
                    }
                }
                catch (Exception)
                {
                    // Unreadable folder: skip it.
                }
            }
            return null;
        }

        public String[] LoadAvailableModules(String ServerType)
        {
            String ModulesDir;
            try
            {
                switch (ServerType)
                {
                    case "Apache":
                        ModulesDir = Directory.GetParent(Path.GetDirectoryName(this.WebServer.Exe)).FullName + @"\modules\";
                        break;
                    case "lighttpd":
                        ModulesDir = Path.GetDirectoryName(this.WebServer.Exe) + @"\modules\";
                        break;
                    case "PHP":
                        ModulesDir = Path.GetDirectoryName(this.PHP.Exe) + @"\ext\";
                        break;
                    default:
                        return null;
                }
            }
            catch (Exception)
            {
                return null;
            }

            if (!Directory.Exists(ModulesDir))
            {
                return new String[0];
            }

            String[] AvailableMods = Directory.GetFiles(ModulesDir);
            for (int i = 0; i < AvailableMods.Length; i++)
            {
                AvailableMods[i] = Path.GetFileName(AvailableMods[i]);
            }
            return AvailableMods;
        }
    }
}
