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
                using (StreamWriter WriteFile = new StreamWriter(this.lighttpdInstallation + @"conf\lighttpd.conf"))
                {
                bool AddSSI = false;
                String fullPath;

                foreach(VirtualHost vh in Globals.VHosts)
                {
                    if (!WebServerConfig.IsValidPort(vh.Port) || String.IsNullOrEmpty(vh.DocumentRoot))
                    {
                        Globals.AddToLog("Virtual server '" + vh.ServerName + "' skipped: invalid port or empty document root.");
                        continue;
                    }

                    fullPath = Path.GetFullPath(Path.Combine(this.lighttpdInstallation, vh.DocumentRoot)).Replace('\\', '/').TrimEnd('/');

                    bool anyIP = String.IsNullOrEmpty(vh.IP) || vh.IP == "*";
                    if (vh.VHostID == 1)
                    {
                        // variables.conf is included without its own server.bind/server.port
                        // (see WriteVariablesCopy), so these lines are the only ones.
                        WriteFile.WriteLine("server.bind = \"" + (anyIP ? "0.0.0.0" : vh.IP) + "\"");
                        WriteFile.WriteLine("server.port = " + vh.Port);
                    }
                    else
                    {
                        WriteFile.WriteLine(tab(tabn) + "$SERVER[\"socket\"] == \"" + (anyIP ? "" : vh.IP) + ":" + vh.Port + "\" {");
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
                        if (String.IsNullOrEmpty(alias.AliasFolder) || String.IsNullOrEmpty(alias.AliasName))
                        {
                            continue;
                        }
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
                            dp = (dir.DirectoryName ?? "").Replace("\\", "/").TrimEnd('/');
                            if (dp.Equals(af.TrimEnd('/'), StringComparison.OrdinalIgnoreCase))
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
                    
                    DirPaths = DirPaths.ConvertAll(d => d.TrimEnd('/').ToLower());
                    foreach (Directories dir in vh.Directory)
                    {
                        if (String.IsNullOrEmpty(dir.DirectoryName))
                        {
                            continue;
                        }
                        dp = dir.DirectoryName.Replace("\\", "/").TrimEnd('/');
                        // Only folders inside the document root can be mapped to a URL.
                        if (!DirPaths.Contains(dp.ToLower()) &&
                            dp.StartsWith(fullPath + "/", StringComparison.OrdinalIgnoreCase))
                        {
                            af = dp.Substring(fullPath.Length) + "/";
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
                    WriteFile.WriteLine(tab(tabn) + "cgi.assign = ( \".php\" => \"" + (Globals.Servers.PHP.Exe ?? "").Replace('\\', '/') + "\" )");
                }

                if (WriteVariablesCopy())
                {
                    WriteFile.WriteLine("include \"" + this.lighttpdInstallation.Replace("\\", "/") + "conf/variables-wds.conf\"");
                }
                WriteFile.WriteLine("include \"" + this.lighttpdInstallation.Replace("\\", "/") + "conf/mimetype.conf\"");
                WriteFile.WriteLine("");

                foreach (CustomSetting Setting in Globals.Servers.WebServer.CustomSettings)
                {
                    WriteFile.WriteLine(Setting.Param.Replace("\n", Environment.NewLine));
                }
                }
            }
            catch (Exception ex)
            {
                Globals.Error.Show("Unexpected error", ex);
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
        
        /// <summary>
        /// Copies conf\variables.conf to conf\variables-wds.conf without the
        /// server.bind/server.port lines, which this program writes itself
        /// (lighttpd refuses a variable set twice). Returns false if there is no variables.conf.
        /// </summary>
        private bool WriteVariablesCopy()
        {
            String source = this.lighttpdInstallation + @"conf\variables.conf";
            if (!File.Exists(source))
            {
                return false;
            }

            List<String> lines = new List<String>();
            foreach (String line in File.ReadAllLines(source))
            {
                String t = line.TrimStart();
                if (t.StartsWith("server.bind", StringComparison.OrdinalIgnoreCase) ||
                    t.StartsWith("server.port", StringComparison.OrdinalIgnoreCase))
                {
                    lines.Add("# (set by Web Development Server) " + line);
                }
                else
                {
                    lines.Add(line);
                }
            }
            File.WriteAllLines(this.lighttpdInstallation + @"conf\variables-wds.conf", lines.ToArray());
            return true;
        }

        private string tab(int n)
        {
            return new String('\t', n);
        }
    }
}
