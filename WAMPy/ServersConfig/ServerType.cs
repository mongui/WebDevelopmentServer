using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;

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
        public String CGIlinker;
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

        public int[] Version()
        {
            int[] a = { };
            return this.Version(a);
        }

        public int[] Version(int[] value)
        {
            if (value.Length > 0)
            {
                this._VerNum1 = value[0];
                this._VerNum2 = value[1];
                this._VerNum3 = value[2];
            }

            if (this._VerNum1 >= 0)
            {
                if (this._VerNum2 >= 0)
                {
                    if (this._VerNum3 >= 0) { return new int[] { this._VerNum1, this._VerNum2, this._VerNum3 }; }
                    else { return new int[] { this._VerNum1, this._VerNum2 }; }
                }
                else { return new int[] { this._VerNum1 }; }
            }
            else { return new int[] { -1 }; }
        }

        public int[] Version(String value)
        {
            if (value != null)
            {
                string[] val = value.Split('.');
                this._VerNum1 = (val[0].Length > 0) ? Convert.ToInt16(System.Text.RegularExpressions.Regex.Replace(val[0], "[^.0-9]", "")) : -1;
                this._VerNum2 = (val[1].Length > 0) ? Convert.ToInt16(System.Text.RegularExpressions.Regex.Replace(val[1], "[^.0-9]", "")) : -1;
                this._VerNum3 = (val[2].Length > 0) ? Convert.ToInt16(System.Text.RegularExpressions.Regex.Replace(val[2], "[^.0-9]", "")) : -1;
            }

            return this.Version();
        }

        private int _VerNum1;
        private int _VerNum2;
        private int _VerNum3;
    }

    public class ServerType
    {
        public WebServers WebServer = new WebServers();
        public PHPs PHP = new PHPs();
        public DataBases DataBase = new DataBases();

        public ServerType()
        {
            // Get servers executables/linkers.
            if (Globals.ActiveWebServerVersion.Length > 0)
                this.WebServer.Exe = this.FindServerFile(this.WebServer.AvailableFiles, Globals.AppFolder + @"bin\WebServer\" + Globals.ActiveWebServerVersion + @"\");
            if (Globals.ActivePHPVersion.Length > 0)
                this.PHP.Exe = this.FindServerFile(this.PHP.AvailableFiles, Globals.AppFolder + @"bin\PHP\" + Globals.ActivePHPVersion + @"\");
            if (Globals.ActiveMySQLVersion.Length > 0)
                this.DataBase.Exe = this.FindServerFile(this.DataBase.AvailableFiles, Globals.AppFolder + @"bin\MySQL\" + Globals.ActiveMySQLVersion + @"\");
            
            bool WSExeExists = (File.Exists(this.WebServer.Exe));
            bool PHPExeExists = (File.Exists(this.PHP.Exe));
            bool DBExeExists = (File.Exists(this.DataBase.Exe));

            if (WSExeExists)
            {
                // Get web server types.
                if (this.WebServer.Exe.ToLower().Contains("lighttpd.exe")) { this.WebServer.Type = WebServers.WSType.lighttpd; }
                else if (this.WebServer.Exe.ToLower().Contains("httpd.exe") || WebServer.Exe.ToLower().Contains("apache.exe")) { this.WebServer.Type = WebServers.WSType.Apache; }
                else if (this.WebServer.Exe.ToLower().Contains("nginx.exe")) { this.WebServer.Type = WebServers.WSType.nginx; }
                else if (this.WebServer.Exe.ToLower().Contains("node.exe")) { this.WebServer.Type = WebServers.WSType.NodeJS; }
                else if (this.WebServer.Exe.ToLower().Contains("iisexpress.exe")) { this.WebServer.Type = WebServers.WSType.IIS; }

                // Get server version.
                this.WebServer.Version(FileVersionInfo.GetVersionInfo(WebServer.Exe).ProductVersion);

                // Get architecture type.
                this.WebServer.Is64Bits = (bool)Is64Bit.GetBits.UnmanagedDllIs64Bit(WebServer.Exe);
                this.WebServer.Is32Bits = !this.WebServer.Is64Bits;
            }

            if (PHPExeExists)
            {
                // Get server version.
                this.PHP.Version(FileVersionInfo.GetVersionInfo(PHP.Exe).ProductVersion);

                // Get architecture type.
                this.PHP.Is64Bits = (bool)Is64Bit.GetBits.UnmanagedDllIs64Bit(PHP.Exe);
                this.PHP.Is32Bits = !this.PHP.Is64Bits;
            }
            if (DBExeExists)
            {
                // Get database types. (NOT FINISHED YET)
                if (this.DataBase.Exe.ToLower().Contains("mysqld.exe")) { this.DataBase.Type = DataBases.DBType.MySQL; }

                // Get server version.
                this.DataBase.Version(FileVersionInfo.GetVersionInfo(DataBase.Exe).ProductVersion);

                // Get architecture type.
                this.DataBase.Is64Bits = (bool)Is64Bit.GetBits.UnmanagedDllIs64Bit(DataBase.Exe);
                this.DataBase.Is32Bits = !this.DataBase.Is64Bits;

                // Stupid way to know if the database server is MySQL or MariaDB.
                String DBPath = Path.GetDirectoryName(DataBase.Exe.Replace(@"bin\mysql.exe", ""));
                if (File.Exists(DBPath + @"\README"))
                {
                    String line = "";
                    using (StreamReader reader = new StreamReader(DBPath + @"\README")) { line = reader.ReadLine(); }
                    if (line.Contains("MariaDB")) { this.DataBase.Type = DataBases.DBType.MariaDB; }
                    else { this.DataBase.Type = DataBases.DBType.MySQL; }
                }
            }
        }

        private List<DirectoryInfo> _Paths = new List<DirectoryInfo>();
        private String _FoundServerFile;

        private String FindServerFile(String[] ServerFiles, String ServerPath = null)
        {
            if (ServerPath != null && Directory.Exists(ServerPath))
            {
                this._Paths.Add(new DirectoryInfo(ServerPath));
            }

            if (this._Paths.Count <= 0)
            {
                return null;
            }

            DirectoryInfo Find;
            using (var iter = this._Paths.GetEnumerator())
            {
                iter.MoveNext();
                Find = iter.Current;
            }

            foreach (FileInfo File in Find.GetFiles())
            {
                foreach (String SF in ServerFiles)
                {
                    if (SF == File.Name.ToLower())
                    {
                        this._Paths.Clear();
                        this._FoundServerFile = File.FullName;
                        return File.FullName;
                    }
                }
            }

            foreach (DirectoryInfo subDirectory in Find.GetDirectories())
            {
                if (subDirectory.ToString() == "bin")
                {
                    this._FoundServerFile = Find.FullName.ToString();
                    this._Paths.Clear();
                }

                this._Paths.Add(subDirectory);
            }

            this._Paths.Remove(Find);
            String RtrnFile = this.FindServerFile(ServerFiles);
            return RtrnFile;
        }

        public String[] LoadAvailableModules(String ServerType)
        {
            String ModulesDir = "";
            switch(ServerType)
            {
                case "Apache":
                    ModulesDir = Directory.GetParent(Path.GetDirectoryName(this.WebServer.Exe)).ToString() + @"\modules\";
                    break;
                case "lighttpd":
                    ModulesDir = Path.GetDirectoryName(this.WebServer.Exe) + @"\modules\";
                    break;
                case "PHP":
                    ModulesDir = Path.GetDirectoryName(this.PHP.Exe).ToString() + @"\ext\";
                    break;
                default:
                    return null;
            }

            String[] AvailableMods = {};

            try
            {
                AvailableMods = Directory.GetFiles(ModulesDir);
            }
            catch(Exception ex)
            {
                Globals.Error.Show(ex.Message);
            }

            for (int i = 0; i < AvailableMods.Length; i++)
            {
                AvailableMods[i] = AvailableMods[i].Replace(ModulesDir, "");
            }

            return AvailableMods;
        }

        public bool WebServerLoadsPHP(WebServers.WSType SrvType)
        {
            Globals.Processes.ExternalPHP = false;
            Globals.Processes.InternalPHP = false;
            int[] PHPver = new int[3];
            try {
                PHPver = Globals.Servers.PHP.Version();
            }
            catch(Exception)
            {
                return false;
            }

            switch (SrvType)
            {
                case WebServers.WSType.Apache:
                    int[] WSver = Globals.Servers.WebServer.Version();

                    String PHPDir = Path.GetDirectoryName(Globals.Servers.PHP.Exe);

                    // SAPI.
                    String LinkerFile =
                        "php" + PHPver[0] +
                        "apache" + WSver[0] + "_" + WSver[1] +
                        "-php" + PHPver[0] + "." + PHPver[1] +
                        "." + ((Globals.Servers.WebServer.Is32Bits) ? "win32" : "win64") +
                        "*.dll";

                    String[] AvailableLinkers = Directory.GetFiles(Globals.AppFolder + @"extras\", LinkerFile);

                    if (AvailableLinkers.Length > 0)
                    {
                        Globals.Servers.WebServer.CGIlinker = AvailableLinkers[0];
                        Globals.Processes.InternalPHP = true;
                    }
                    else // FastCGI.
                    {
                        LinkerFile =
                            "mod_fcgid.so.apache" + ((WSver[1] >= 4) ? WSver[0].ToString() + "_" + WSver[1].ToString() : WSver[0].ToString()) +
                            "." + ((Globals.Servers.WebServer.Is32Bits) ? "win32" : "win64") +
                            "*";

                        AvailableLinkers = Directory.GetFiles(Globals.AppFolder + @"extras\", LinkerFile);
                        
                        if (AvailableLinkers.Length > 0)
                        {
                            Globals.Servers.WebServer.CGIlinker = AvailableLinkers[0];
                            Globals.Processes.InternalPHP = true;
                        }
                        else // CGI.
                        {
                            Globals.AddToLog("It is not possible to connect Apache " + WSver[0] + "." + WSver[1] + " version with PHP " + PHPver[0] + "." + PHPver[1] + ".");

                            Globals.Servers.WebServer.CGIlinker = Path.GetFullPath(Path.GetDirectoryName(Globals.Servers.WebServer.Exe) + @"\..\modules\mod_cgi.so");
                            
                            if (File.Exists(Globals.Servers.WebServer.CGIlinker))
                            {
                                Globals.AddToLog("Trying with CGI module (Use this only for development and testing purposes).");
                                Globals.Processes.InternalPHP = true; 
                            }
                        }
                    }
                    break;
                case WebServers.WSType.nginx:
                    if (PHPver[0] == 5) // FastCGI.
                    {
                        Globals.Processes.ExternalPHP = true;
                    }
                    break;
                case WebServers.WSType.lighttpd:
                    if (PHPver[0] < 5) // CGI.
                    {
                        Globals.Processes.InternalPHP = true;
                    }
                    else // FastCGI.
                    {
                        Globals.Processes.ExternalPHP = true;
                    }
                    break;
                default:
                    Globals.Processes.ExternalPHP = true;
                    break;
            }

            return Globals.Processes.InternalPHP;
        }
    }
}
