using System;
using System.Collections.Generic;
using System.IO;
using System.Data;
using System.Linq;

namespace WDS
{
    public class ApacheConfigFile
    {
        private String ApacheInstallation;
        public String[] FileData;

        public String PHPmodule;
        public String ErrorLog;
        public String AccessLog;
        public ApacheConfigFile(String Installation)
        {
            this.ApacheInstallation = Installation;

            if (!Directory.Exists(ApacheInstallation + @"logs"))
            {
                Directory.CreateDirectory(ApacheInstallation + @"logs");
            }
            if (!Directory.Exists(ApacheInstallation + @"conf"))
            {
                Directory.CreateDirectory(ApacheInstallation + @"conf");
            }
        }

        public void Write()
        {
            if (File.Exists(ApacheInstallation + @"conf\httpd.conf"))
            {
                File.Delete(ApacheInstallation + @"conf\httpd.conf");
            }

            try
            {
                StreamWriter WriteFile = new StreamWriter(ApacheInstallation + @"conf\httpd.conf");

                String[] Available = Globals.Servers.LoadAvailableModules("Apache");
                String[] NotAvailable = Globals.Servers.WebServer.ActiveModules.Except(Available).ToArray();

                if (NotAvailable.Length > 0)
                {
                    Globals.AddToLog("Module not found in this WebServer: " + string.Join(", ", NotAvailable));
                    Globals.Servers.WebServer.ActiveModules = Globals.Servers.WebServer.ActiveModules.Except(NotAvailable).ToList<String>();
                }

                if (!Globals.Servers.WebServer.ActiveModules.Contains("mod_alias.so"))
                {
                    Globals.Servers.WebServer.ActiveModules.Add("mod_alias.so");
                }

                if (Globals.Servers.WebServer.Type == WebServers.WSType.Apache && !Globals.Servers.WebServer.ActiveModules.Contains("mod_authz_core.so"))
                {
                    int[] ver = Globals.Servers.WebServer.Version();
                    if (ver[0] >= 2 && ver[1] >= 4)
                    {
                        Globals.Servers.WebServer.ActiveModules.Add("mod_authz_core.so");
                    }
                }

                foreach (String Mod in Globals.Servers.WebServer.ActiveModules)
                {
                    WriteFile.WriteLine("LoadModule " + Mod.Replace(".so", "_module").Replace("mod_", "") + " " + "modules/" + Mod);
                }
                if (!string.IsNullOrEmpty(this.PHPmodule) && File.Exists(this.PHPmodule))
                {
                    // SAPI:
                    if (this.PHPmodule.Contains(".dll"))
                    {
                        String PHPpath = Path.GetDirectoryName(Globals.Servers.PHP.Exe);
                        String FileName = Path.GetFileName(Globals.Servers.WebServer.CGIlinker);

                        File.Copy(Globals.Servers.WebServer.CGIlinker, PHPpath + @"\" + FileName, true);
                        this.PHPmodule = PHPpath + @"\" + FileName;

                        if (this.PHPmodule.Contains("php7apache")) { WriteFile.WriteLine("LoadModule php7_module \"" + this.PHPmodule + "\""); }
                        else if (this.PHPmodule.Contains("php5apache")) { WriteFile.WriteLine("LoadModule php5_module \"" + this.PHPmodule + "\""); }
                        else if (this.PHPmodule.Contains("php4apache")) { WriteFile.WriteLine("LoadModule php4_module \"" + this.PHPmodule + "\""); }
                        WriteFile.WriteLine("PHPIniDir \"" + Globals.AppFolder.Replace("\\", "/") + "bin/PHP/" + Globals.ActivePHPVersion + "\"");
                        WriteFile.WriteLine("AddHandler application/x-httpd-php .php");
                    }
                    // FastCGI:
                    else if (this.PHPmodule.Contains("fcgid.so"))
                    {
                        File.Copy(Globals.Servers.WebServer.CGIlinker, ApacheInstallation + @"\modules\mod_fcgid.so", true);
                        this.PHPmodule = ApacheInstallation.Replace("\\", "/") + "modules/mod_fcgid.so";

                        WriteFile.WriteLine("LoadModule fcgid_module modules/mod_fcgid.so");
                        WriteFile.WriteLine("<IfModule fcgid_module>");
                        WriteFile.WriteLine("\tFcgidInitialEnv PHPRC \"" + Globals.AppFolder.Replace("\\", "/") + "bin/PHP/" + Globals.ActivePHPVersion + "/php\"");
                        WriteFile.WriteLine("\tAddHandler fcgid-script .php");
                        WriteFile.WriteLine("\tFcgidWrapper \"" + Globals.AppFolder.Replace("\\", "/") + "bin/PHP/" + Globals.ActivePHPVersion + "/php-cgi.exe\" .php");
                        WriteFile.WriteLine("</IfModule>");
                    }
                    // CGI:
                    else if (this.PHPmodule.Contains("cgi.so"))
                    {
                        if (!Globals.Servers.WebServer.ActiveModules.Any("mod_cgi.so".Contains))
                            WriteFile.WriteLine("LoadModule cgi_module modules/mod_cgi.so");
                        if (!Globals.Servers.WebServer.ActiveModules.Any("mod_cgi.so".Contains))
                            WriteFile.WriteLine("LoadModule actions_module modules/mod_actions.so");

                        WriteFile.WriteLine("SetEnv PHPRC \"" + Path.GetDirectoryName(Globals.Servers.PHP.Exe) + "\"");
                        WriteFile.WriteLine("ScriptAlias /php/ \"" + Path.GetDirectoryName(Globals.Servers.PHP.Exe) + "/\"");
                        WriteFile.WriteLine("Action application/x-httpd-php \"/php/php.exe\"");
                        WriteFile.WriteLine("AddHandler application/x-httpd-php .php");
                    }
                }

                DataTable DBSettings = Globals.dbConn.GetDataTable("SELECT * FROM ConfigModules WHERE IfModuleActive > 0 AND ServerType LIKE 'Apache'");
                if (DBSettings.Rows.Count > 0)
                {
                    WriteFile.WriteLine("");
                    String text;
                    foreach (DataRow row in DBSettings.Rows)
                    {
                        text = "<IfModule ";
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

                DBSettings = Globals.dbConn.GetDataTable("SELECT * FROM CustomSettings WHERE ServerType LIKE 'Apache'");

                if (DBSettings.Rows.Count > 0)
                {
                    foreach (DataRow row in DBSettings.Rows)
                    {
                        WriteFile.WriteLine(row["Param"] + "\r\n");
                    }
                    WriteFile.WriteLine("");
                }

                String VServ;
                String tab;
                List<String> Ports = new List<String>();
                
                foreach(VirtualHost vh in Globals.VHosts)
                {
                    if (vh.VHostID != 1) { tab = "\t"; }
                    else { tab = ""; }
                    VServ = "";

                    if (!Ports.Contains(vh.Port)) {
                        VServ += "Listen " + vh.Port + "\r\n";
                        Ports.Add(vh.Port);
                    }

                    if (vh.VHostID != 1 && vh.Port.Length > 0)
                    {
                        VServ += "NameVirtualHost \"" + vh.IP + ":" + vh.Port + "\"\r\n";
                        VServ += "<VirtualHost " + vh.IP + ":" + vh.Port + ">" + "\r\n";
                    }
                    if (vh.ServerName != null && vh.ServerName != "") { VServ += tab + "ServerName \"" + vh.ServerName + "\"" + "\r\n"; }
                    if (vh.VHostID != 1 && vh.ServerAlias != null && vh.ServerAlias != "") { VServ += tab + "ServerAlias \"" + vh.ServerAlias + "\"" + "\r\n"; }
                    if (vh.DocumentRoot != null && vh.DocumentRoot != "") { VServ += tab + "DocumentRoot \"" + vh.DocumentRoot.Replace("\\", "/") + "\"" + "\r\n"; }
                    if (vh.Others != null && vh.Others != "") { VServ += vh.Others + "\r\n"; }
                    foreach (Aliases alias in vh.Alias) { VServ += tab + "Alias \"" + alias.AliasName + "\" \"" + alias.AliasFolder.Replace("\\", "/") + "\"" + "\r\n"; }    
                    foreach (Directories dir in vh.Directory) {
                        VServ += tab + "<Directory \"" + dir.DirectoryName.Replace("\\", "/") + "\">" + "\r\n";
                        VServ += tab + tab + "AllowOverride " + ((dir.AllowOverride) ? "All" : "None") + "\r\n";
                        VServ += tab + tab + 
                            "Options " + ((!dir.SymLinks) ? "-" : "+") + "FollowSymLinks " + 
                            ((!dir.Includes) ? "-" : "+") + "Includes " +
                            ((!dir.Indexes) ? "-" : "+") + "Indexes " +
                            ((!dir.MultiViews) ? "-" : "+") + "MultiViews\r\n";
                        if (dir.Others.Length > 0) { VServ += tab + tab + dir.Others.Trim() + "\r\n"; }
                        VServ += tab + "</Directory>\r\n";
                    }

                    if (vh.VHostID != 1 && vh.Port.Length > 0) { VServ += "</VirtualHost>\r\n"; }

                    WriteFile.WriteLine(VServ);
                }

                WriteFile.Close();
            }
            catch (Exception ex)
            {
                Globals.Error.Show(ex.Message);
            }
        }

        public bool UpdateData(bool SetPHP = false)
        {
            if (!Directory.Exists(this.ApacheInstallation))
            {
                Globals.AddToLog("Directory " + this.ApacheInstallation + " not exist.");
                return false;
            }

            this.PHPmodule = null;

            if (SetPHP)
            {
                String PHPpath = Path.GetDirectoryName(Globals.Servers.PHP.Exe);

                if (string.IsNullOrEmpty(Globals.Servers.WebServer.CGIlinker) && !Directory.Exists(PHPpath))
                {
                    Globals.AddToLog("Couldn't start Apache with PHP.");
                    return false;
                }
                else
                {
                    this.PHPmodule = Globals.Servers.WebServer.CGIlinker;
                }
            }

            this.Write();
            return true;
        }

        public void MakeItCompatible(int[] Version)
        {
            switch(Version[0]) // Main version.
            {
                case 0:
                case 1:

                    break;
                case 2:
                    switch(Version[1]) // Subversion.
                    {
                        case 0:
                            if (!Globals.Servers.WebServer.ActiveModules.Any("mod_access.so".Contains)) // Makes 'Order' available.
                                Globals.Servers.WebServer.ActiveModules.Add("mod_access.so");
                            break;
                        case 2:
                            if (!Globals.Servers.WebServer.ActiveModules.Any("mod_authz_host.so".Contains)) // Makes 'Order' available.
                                Globals.Servers.WebServer.ActiveModules.Add("mod_authz_host.so");
                            break;
                        case 4:
                            if (!Globals.Servers.WebServer.ActiveModules.Any("mod_authn_core.so".Contains)) // Avoids 500 Error.
                                Globals.Servers.WebServer.ActiveModules.Add("mod_authn_core.so");
                            if (!Globals.Servers.WebServer.ActiveModules.Any("mod_access_compat.so".Contains)) // Makes 'Order' available.
                                Globals.Servers.WebServer.ActiveModules.Add("mod_access_compat.so");
                            break;
                        default:
                            return;
                    }
                    break;
                default:
                    return;
            }
        }
    }
}