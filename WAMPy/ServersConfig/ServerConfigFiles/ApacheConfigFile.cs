using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace WDS
{
    public class ApacheConfigFile
    {
        private String ApacheInstallation;

        // How PHP is joined to Apache; null = without PHP.
        private PairResult Link;

        public ApacheConfigFile(String Installation)
        {
            this.ApacheInstallation = Installation;

            Directory.CreateDirectory(ApacheInstallation + @"logs");
            Directory.CreateDirectory(ApacheInstallation + @"conf");
        }

        private static String Slashes(String path)
        {
            return (path ?? "").Replace("\\", "/");
        }

        private void AddModule(String module)
        {
            if (!Globals.Servers.WebServer.ActiveModules.Contains(module))
            {
                Globals.Servers.WebServer.ActiveModules.Add(module);
            }
        }

        /// <summary>
        /// Adds a module only if this Apache has it.
        /// </summary>
        private void AddModuleIfAvailable(String[] available, String module)
        {
            if (available.Contains(module, StringComparer.OrdinalIgnoreCase))
            {
                AddModule(module);
            }
        }

        public void Write()
        {
            try
            {
                using (StreamWriter WriteFile = new StreamWriter(ApacheInstallation + @"conf\httpd.conf", false))
                {
                    WriteContent(WriteFile);
                }
            }
            catch (Exception ex)
            {
                Globals.Error.Show("Unable to write httpd.conf", ex);
            }
        }

        private void WriteContent(StreamWriter WriteFile)
        {
            List<String> active = Globals.Servers.WebServer.ActiveModules;
            String[] Available = Globals.Servers.LoadAvailableModules("Apache") ?? new String[0];
            String[] NotAvailable = active.Except(Available, StringComparer.OrdinalIgnoreCase).ToArray();

            if (NotAvailable.Length > 0)
            {
                Globals.AddToLog("Module not found in this WebServer: " + string.Join(", ", NotAvailable));
                Globals.Servers.WebServer.ActiveModules = active = active.Except(NotAvailable, StringComparer.OrdinalIgnoreCase).ToList();
            }

            PhpLink method = (this.Link != null && this.Link.Compatible) ? this.Link.Link : PhpLink.None;
            String PHPpath = (method != PhpLink.None) ? Path.GetDirectoryName(Globals.Servers.PHP.Exe) : null;
            int phpMajor = Globals.Servers.PHP.VersionPart(0);

            // Modules every generated configuration needs.
            foreach (String mod in new String[] { "mod_alias.so", "mod_mime.so", "mod_dir.so", "mod_log_config.so", "mod_env.so" })
            {
                AddModuleIfAvailable(Available, mod);
            }

            int major = Globals.Servers.WebServer.VersionPart(0);
            int minor = Globals.Servers.WebServer.VersionPart(1);
            if (major > 2 || (major == 2 && minor >= 4))
            {
                AddModuleIfAvailable(Available, "mod_authz_core.so");
            }

            switch (method)
            {
                case PhpLink.Cgi:
                    AddModuleIfAvailable(Available, "mod_cgi.so");
                    AddModuleIfAvailable(Available, "mod_actions.so");
                    break;
                case PhpLink.ProxyFcgi:
                    AddModuleIfAvailable(Available, "mod_proxy.so");
                    AddModuleIfAvailable(Available, "mod_proxy_fcgi.so");
                    break;
                case PhpLink.Fcgid:
                    AddModuleIfAvailable(Available, "mod_fcgid.so");
                    break;
            }

            WriteFile.WriteLine("ServerRoot \"" + Slashes(ApacheInstallation).TrimEnd('/') + "\"");

            foreach (String Mod in Globals.Servers.WebServer.ActiveModules)
            {
                WriteFile.WriteLine("LoadModule " + Mod.Replace(".so", "_module").Replace("mod_", "") + " modules/" + Mod);
            }

            WriteFile.WriteLine("");
            WriteFile.WriteLine("ServerName localhost");
            WriteFile.WriteLine("PidFile logs/httpd.pid");
            WriteFile.WriteLine("ErrorLog logs/error.log");
            if (File.Exists(ApacheInstallation + @"conf\mime.types"))
            {
                WriteFile.WriteLine("<IfModule mime_module>");
                WriteFile.WriteLine("\tTypesConfig conf/mime.types");
                WriteFile.WriteLine("</IfModule>");
            }
            WriteFile.WriteLine("<IfModule dir_module>");
            WriteFile.WriteLine("\tDirectoryIndex " + (method != PhpLink.None ? "index.php " : "") + "index.html index.htm");
            WriteFile.WriteLine("</IfModule>");
            WriteFile.WriteLine("");

            String phpCgi = Path.Combine(PHPpath ?? "", "php-cgi.exe");
            if (PHPpath != null && !File.Exists(phpCgi))
            {
                phpCgi = Globals.Servers.PHP.Exe; // PHP 4: php.exe is the CGI binary.
            }

            switch (method)
            {
                case PhpLink.ModPHP:
                    String dll = this.Link.SapiDll;
                    String FileName = Path.GetFileName(dll).ToLowerInvariant();
                    String moduleName = "php5_module";
                    if (FileName.StartsWith("php8")) moduleName = "php_module"; // PHP 8 renamed it.
                    else if (FileName.StartsWith("php7")) moduleName = "php7_module";
                    else if (FileName.StartsWith("php4")) moduleName = "php4_module";

                    WriteFile.WriteLine("LoadModule " + moduleName + " \"" + Slashes(dll) + "\"");
                    WriteFile.WriteLine("PHPIniDir \"" + Slashes(PHPpath) + "\"");
                    if (phpMajor == 4)
                    {
                        WriteFile.WriteLine("AddType application/x-httpd-php .php");
                    }
                    else
                    {
                        WriteFile.WriteLine("AddHandler application/x-httpd-php .php");
                    }
                    break;

                case PhpLink.ProxyFcgi:
                    WriteFile.WriteLine("<FilesMatch \"\\.php$\">");
                    WriteFile.WriteLine("\tSetHandler \"proxy:fcgi://" + Compatibility.FastCgiAddress + "\"");
                    WriteFile.WriteLine("</FilesMatch>");
                    break;

                case PhpLink.Fcgid:
                    WriteFile.WriteLine("<IfModule fcgid_module>");
                    WriteFile.WriteLine("\tFcgidInitialEnv PHPRC \"" + Slashes(PHPpath) + "\"");
                    WriteFile.WriteLine("\tFcgidInitialEnv PHP_FCGI_MAX_REQUESTS 0");
                    WriteFile.WriteLine("\tAddHandler fcgid-script .php");
                    WriteFile.WriteLine("\tFcgidWrapper \"" + Slashes(phpCgi) + "\" .php");
                    WriteFile.WriteLine("</IfModule>");
                    break;

                case PhpLink.Cgi:
                    WriteFile.WriteLine("SetEnv PHPRC \"" + Slashes(PHPpath) + "\"");
                    WriteFile.WriteLine("ScriptAlias /php/ \"" + Slashes(PHPpath) + "/\"");
                    WriteFile.WriteLine("Action application/x-httpd-php \"/php/" + Path.GetFileName(phpCgi) + "\"");
                    WriteFile.WriteLine("AddHandler application/x-httpd-php .php");
                    break;
            }

            DataTable DBSettings = Globals.dbConn.GetDataTable("SELECT * FROM ConfigModules WHERE IfModuleActive > 0 AND ServerType = 'Apache'");
            if (DBSettings.Rows.Count > 0)
            {
                WriteFile.WriteLine("");
                foreach (DataRow row in DBSettings.Rows)
                {
                    String text = "<IfModule ";
                    if (row["IfModuleActive"].ToString() == "2")
                    {
                        text += "!";
                    }
                    text += row["Module"].ToString().Replace(".so", "_module").Replace("mod_", "") + ">\r\n";
                    text += row["IfModule"] + "\r\n";
                    text += "</IfModule>";

                    WriteFile.WriteLine(text + "\r\n");
                }
                WriteFile.WriteLine("");
            }

            DBSettings = Globals.dbConn.GetDataTable("SELECT * FROM CustomSettings WHERE ServerType = 'Apache'");
            foreach (DataRow row in DBSettings.Rows)
            {
                WriteFile.WriteLine(row["Param"] + "\r\n");
            }

            List<String> Listens = new List<String>();

            foreach (VirtualHost vh in Globals.VHosts)
            {
                if (!WebServerConfig.IsValidPort(vh.Port))
                {
                    Globals.AddToLog("Virtual server '" + vh.ServerName + "' skipped: invalid port '" + vh.Port + "'.");
                    continue;
                }

                bool main = (vh.VHostID == 1);
                String tab = main ? "" : "\t";
                bool anyIP = String.IsNullOrEmpty(vh.IP) || vh.IP == "*";
                String VServ = "";

                // "*" listens on every network card; any other IP only on that one.
                String listen = anyIP ? vh.Port : vh.IP + ":" + vh.Port;
                if (!Listens.Contains(listen))
                {
                    VServ += "Listen " + listen + "\r\n";
                    Listens.Add(listen);
                }

                String hostIP = anyIP ? "*" : vh.IP;
                if (!main)
                {
                    if (major == 2 && minor < 4)
                    {
                        VServ += "NameVirtualHost " + hostIP + ":" + vh.Port + "\r\n";
                    }
                    VServ += "<VirtualHost " + hostIP + ":" + vh.Port + ">\r\n";
                }
                if (!String.IsNullOrEmpty(vh.ServerName)) { VServ += tab + "ServerName \"" + vh.ServerName + "\"\r\n"; }
                if (!main && !String.IsNullOrEmpty(vh.ServerAlias)) { VServ += tab + "ServerAlias \"" + vh.ServerAlias + "\"\r\n"; }
                if (!String.IsNullOrEmpty(vh.DocumentRoot)) { VServ += tab + "DocumentRoot \"" + Slashes(vh.DocumentRoot) + "\"\r\n"; }
                if (!String.IsNullOrEmpty(vh.Others)) { VServ += vh.Others + "\r\n"; }
                foreach (Aliases alias in vh.Alias)
                {
                    VServ += tab + "Alias \"" + alias.AliasName + "\" \"" + Slashes(alias.AliasFolder) + "\"\r\n";
                }
                foreach (Directories dir in vh.Directory)
                {
                    VServ += tab + "<Directory \"" + Slashes(dir.DirectoryName) + "\">\r\n";
                    VServ += tab + tab + "AllowOverride " + ((dir.AllowOverride) ? "All" : "None") + "\r\n";
                    VServ += tab + tab +
                        "Options " + ((!dir.SymLinks) ? "-" : "+") + "FollowSymLinks " +
                        ((!dir.Includes) ? "-" : "+") + "Includes " +
                        ((!dir.Indexes) ? "-" : "+") + "Indexes " +
                        ((!dir.MultiViews) ? "-" : "+") + "MultiViews\r\n";
                    if (!String.IsNullOrEmpty(dir.Others)) { VServ += tab + tab + dir.Others.Trim() + "\r\n"; }
                    VServ += tab + "</Directory>\r\n";
                }

                if (!main) { VServ += "</VirtualHost>\r\n"; }

                WriteFile.WriteLine(VServ);
            }
        }

        /// <summary>
        /// Writes httpd.conf. Link says how PHP is joined to Apache (null = no PHP).
        /// </summary>
        public bool UpdateData(PairResult link)
        {
            if (!Directory.Exists(this.ApacheInstallation))
            {
                Globals.AddToLog("Directory " + this.ApacheInstallation + " does not exist.");
                return false;
            }

            this.Link = (link != null && link.Compatible && !String.IsNullOrEmpty(Globals.Servers.PHP.Exe)) ? link : null;
            this.Write();
            return true;
        }

        public void MakeItCompatible(int[] Version)
        {
            if (Version.Length < 2 || Version[0] != 2)
            {
                return;
            }

            switch (Version[1]) // Subversion.
            {
                case 0:
                    AddModule("mod_access.so"); // Makes 'Order' available.
                    break;
                case 2:
                    AddModule("mod_authz_host.so"); // Makes 'Order' available.
                    break;
                case 4:
                    AddModule("mod_authn_core.so"); // Avoids 500 Error.
                    AddModule("mod_access_compat.so"); // Makes 'Order' available.
                    break;
            }
        }
    }
}
