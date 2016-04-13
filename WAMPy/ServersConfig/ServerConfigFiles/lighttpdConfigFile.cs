using System;
using System.Collections.Generic;
using System.IO;

namespace WDS
{
    class lighttpdConfigFile
    {
        private String lighttpdInstallation;

        public bool PHPcgi = false;
        private int tabn;

        public lighttpdConfigFile(String Installation)
        {
            this.lighttpdInstallation = Installation;

            if (!Directory.Exists(this.lighttpdInstallation + @"logs"))
            {
                Directory.CreateDirectory(this.lighttpdInstallation + @"logs");
            }
            if (!Directory.Exists(this.lighttpdInstallation + @"conf"))
            {
                Directory.CreateDirectory(this.lighttpdInstallation + @"conf");
            }
        }

        public void Write()
        {
            tabn = 0;
            if (File.Exists(this.lighttpdInstallation + @"conf\lighttpd.conf"))
            {
                File.Delete(this.lighttpdInstallation + @"conf\lighttpd.conf");
            }

            try
            {
                StreamWriter WriteFile = new StreamWriter(this.lighttpdInstallation + @"conf\lighttpd.conf");
                
                bool AddSSI = false;
                String fullPath;

                foreach(VirtualHost vh in Globals.VHosts)
                {
                    fullPath = Path.Combine(this.lighttpdInstallation, vh.DocumentRoot);
                    fullPath = Path.GetFullPath((new Uri(fullPath)).LocalPath).Replace('\\', '/').ToLower();

                    if (vh.IP == "*" || vh.IP == "") { vh.IP = "127.0.0.1"; }
                    if (vh.VHostID != 1)
                    {
                        WriteFile.WriteLine(tab(tabn) + "$SERVER[\"socket\"] == \"" + vh.IP + ":" + vh.Port + "\" {");
                        tabn++;
                    }
                    WriteFile.WriteLine(tab(tabn) + "server.document-root = \"" + vh.DocumentRoot.Replace("\\", "/") + "\"");
                    WriteFile.WriteLine(tab(tabn) + "index-file.names = ( " + ((this.PHPcgi) ? "\"index.php\", " : "") + "\"index.html\", \"index.htm\", \"default.htm\" )");

                    List<String> DirPaths = new List<String>();
                    int aliasCount = 1;
                    String an;
                    String dp;
                    String af;

                    foreach (Aliases alias in vh.Alias)
                    {
                        an = alias.AliasName.Replace("\\", "/");
                        an += (an[an.Length - 1] != '/') ? "/" : "";
                        af = alias.AliasFolder.Replace("\\", "/");
                        af += (af[af.Length - 1] != '/') ? "/" : "";
                        DirPaths.Add(af);
                        if (aliasCount == 1)
                        {
                            WriteFile.WriteLine(tab(tabn) + "alias.url = ( \"" + an + "\" => \"" + af + "\" )");
                        }
                        else
                        {
                            WriteFile.WriteLine(tab(tabn) + "alias.url += ( \"" + an + "\" => \"" + af + "\" )");
                        }

                        foreach (Directories dir in vh.Directory)
                        {
                            dp = dir.DirectoryName.Replace("\\", "/");
                            if (dp.ToLower().Equals(af.ToLower()))
                            {
                                // This is the same code...
                                WriteFile.WriteLine(tab(tabn) + "$HTTP[\"url\"] =~ \"^" + an + "\" {");
                                tabn++;
                                WriteFile.WriteLine(tab(tabn) + "server.follow-symlink=\"" + ((dir.SymLinks) ? "enable" : "disable") + "\"");
                                WriteFile.WriteLine(tab(tabn) + "dir-listing.activate=\"" + ((dir.Indexes) ? "enable" : "disable") + "\"");
                                if (dir.Includes)
                                {
                                    AddSSI = true;
                                    WriteFile.WriteLine(tab(tabn) + "ssi.extension = ( \".shtml\" )");
                                }
                                // MultiViews not supported. Activate "Mod_Magnet.dll" module and implement it manually.
                                tabn--;
                                WriteFile.WriteLine(tab(tabn) + "}");
                            }
                        }

                        aliasCount++;
                    }
                    
                    DirPaths = DirPaths.ConvertAll(d => d.ToLower());
                    foreach (Directories dir in vh.Directory)
                    {
                        dp = dir.DirectoryName.ToLower().Replace("\\", "/");
                        if (!DirPaths.Contains(dp))
                        {
                            af = "/" + dp.Replace(fullPath, "");
                            // ... As this one. Sorry. :(
                            WriteFile.WriteLine(tab(tabn) + "$HTTP[\"url\"] =~ \"^" + af + "\" {");
                            tabn++;
                            WriteFile.WriteLine(tab(tabn) + "server.follow-symlink=\"" + ((dir.SymLinks) ? "enable" : "disable") + "\"");
                            WriteFile.WriteLine(tab(tabn) + "dir-listing.activate=\"" + ((dir.Indexes) ? "enable" : "disable") + "\"");
                            if (dir.Includes)
                            {
                                AddSSI = true;
                                WriteFile.WriteLine(tab(tabn) + "ssi.extension = ( \".shtml\" )");
                            }
                            // MultiViews not supported. Activate "Mod_Magnet.dll" module and implement it manually.
                            tabn--;
                            WriteFile.WriteLine(tab(tabn) + "}");
                        }
                    }
                    


                    if (vh.VHostID != 1)
                    {
                        tabn--;
                        WriteFile.WriteLine(tab(tabn) + "}");
                    }
                    WriteFile.WriteLine("");
                }

                if ((Globals.Processes.InternalPHP || Globals.Processes.ExternalPHP) && !Globals.Servers.WebServer.ActiveModules.Contains("mod_cgi.dll")) { Globals.Servers.WebServer.ActiveModules.Add("mod_cgi.dll"); }

                if (this.PHPcgi && !Globals.Servers.WebServer.ActiveModules.Contains("mod_fastcgi.dll")) { Globals.Servers.WebServer.ActiveModules.Add("mod_fastcgi.dll"); }
                if (!Globals.Servers.WebServer.ActiveModules.Contains("mod_alias.dll")) { Globals.Servers.WebServer.ActiveModules.Add("mod_alias.dll"); }
                if (!Globals.Servers.WebServer.ActiveModules.Contains("mod_simple_vhost.dll")) { Globals.Servers.WebServer.ActiveModules.Add("mod_simple_vhost.dll"); }
                if (AddSSI && !Globals.Servers.WebServer.ActiveModules.Contains("mod_ssi.dll")) { Globals.Servers.WebServer.ActiveModules.Add("mod_ssi.dll"); }

                String Modules = "\"" + string.Join("\" , \"", Globals.Servers.WebServer.ActiveModules.ToArray()) + "\"";
                Modules = Modules.Replace(".dll", "");
                WriteFile.WriteLine("server.modules = ( " + Modules + " )");
                WriteFile.WriteLine("");

                if (Globals.Processes.ExternalPHP) // FastCGI.
                {
                    WriteFile.WriteLine(tab(tabn) + "fastcgi.server = ( \".php\" =>");
                    tabn++;
                    WriteFile.WriteLine(tab(tabn) + "( \"localhost\" =>");
                    tabn++;
                    WriteFile.WriteLine(tab(tabn) + "(");
                    tabn++;
                    WriteFile.WriteLine(tab(tabn) + "\"host\" => \"127.0.0.1\",");
                    WriteFile.WriteLine(tab(tabn) + "\"port\" => 8866,");
                    tabn--;
                    WriteFile.WriteLine(tab(tabn) + ")");
                    tabn--;
                    WriteFile.WriteLine(tab(tabn) + ")");
                    tabn--;
                    WriteFile.WriteLine(tab(tabn) + ")");

                    WriteFile.WriteLine("fastcgi.map-extensions = (");
                    tabn++;
                    WriteFile.WriteLine(tab(tabn) + "\".php3\" => \".php\",");
                    WriteFile.WriteLine(tab(tabn) + "\".php4\" => \".php\",");
                    WriteFile.WriteLine(tab(tabn) + "\".php5\" => \".php\",");
                    WriteFile.WriteLine(tab(tabn) + "\".phps\" => \".php\",");
                    WriteFile.WriteLine(tab(tabn) + "\".phtml\" => \".php\"");
                    tabn--;
                    WriteFile.WriteLine(")");
                }
                else if (Globals.Processes.InternalPHP) // CGI.
                {
                    WriteFile.WriteLine(tab(tabn) + "cgi.assign = ( \".php\" => \"" + Globals.Servers.PHP.Exe + "\" )");
                }

                WriteFile.WriteLine("include \"" + this.lighttpdInstallation.Replace("\\", "/") + "conf/variables.conf\"");
                WriteFile.WriteLine("include \"" + this.lighttpdInstallation.Replace("\\", "/") + "conf/mimetype.conf\"");
                WriteFile.WriteLine("");

                foreach (CustomSetting Setting in Globals.Servers.WebServer.CustomSettings)
                {
                    WriteFile.WriteLine(Setting.Param.Replace("\n", Environment.NewLine));
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
            bool WhatToReturn = true;

            if (!Directory.Exists(this.lighttpdInstallation))
            {
                Globals.Error.Show("Directory " + this.lighttpdInstallation + " not exist.");
                return false;
            }

            this.PHPcgi = SetPHP;
            this.Write();

            return WhatToReturn;
        }
        
        private string tab(int n)
        {
            return new String('\t', n);
        }
    }
}
