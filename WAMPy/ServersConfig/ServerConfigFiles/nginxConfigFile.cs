using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;

namespace WDS
{
    class nginxConfigFile
    {
        private String nginxInstallation;
        public bool PHPcgi = false;
        private int tabn;

        public nginxConfigFile(String Installation)
        {
            this.nginxInstallation = Installation;

            if (!Directory.Exists(nginxInstallation + @"logs"))
            {
                Directory.CreateDirectory(nginxInstallation + @"logs");
            }
            if (!Directory.Exists(nginxInstallation + @"conf"))
            {
                Directory.CreateDirectory(nginxInstallation + @"conf");
            }
        }

        public void UpdateData(bool SetPHP = false)
        {
            this.PHPcgi = SetPHP;
            this.Write();
        }
        
        private String[] GetFileSection(String Section = "")
        {
            List<String> Settings = new List<String>();
            String Start = "";

            if (Section != "")
            {
                Start = Section + ".";
            }
            

            String Word;
            foreach (CustomSetting cs in Globals.Servers.WebServer.CustomSettings)
            {
                Word = cs.Param.Split(' ')[0];
                if (Word.StartsWith(Start) && Word.Substring(Start.Length).Split('.').Length <= 1)
                {
                    Settings.Add(cs.Param.Substring(Start.Length));
                }
            }

            return Settings.ToArray();
        }

        public void Write()
        {
            tabn = 0;
            if (File.Exists(this.nginxInstallation + @"conf\nginx.conf"))
            {
                File.Delete(this.nginxInstallation + @"conf\nginx.conf");
            }

            try
            {
                using (StreamWriter WriteFile = new StreamWriter(this.nginxInstallation + @"conf\nginx.conf"))
                {

                foreach (String cs in this.GetFileSection(""))
                {
                    WriteFile.WriteLine(cs.Replace("\n", Environment.NewLine));
                }

                // nginx refuses to start without an events block.
                String[] data = this.GetFileSection("events");
                WriteFile.WriteLine();
                WriteFile.WriteLine("events {");
                tabn++;
                if (data.Length == 0)
                {
                    WriteFile.WriteLine(tab(tabn) + "worker_connections 1024;");
                }
                foreach (String cs in data)
                {
                    WriteFile.WriteLine(tab(tabn) + cs.Replace("\n", Environment.NewLine));
                }
                tabn--;
                WriteFile.WriteLine("}");

                WriteFile.WriteLine();
                WriteFile.WriteLine("http {");
                tabn++;

                data = this.GetFileSection("http");
                bool hasTypes = false;
                foreach (String cs in data)
                {
                    if (cs.TrimStart().StartsWith("include mime.types") || cs.TrimStart().StartsWith("types")) hasTypes = true;
                    WriteFile.WriteLine(tab(tabn) + cs.Replace("\n", Environment.NewLine));
                }
                if (!hasTypes && File.Exists(this.nginxInstallation + @"conf\mime.types"))
                {
                    WriteFile.WriteLine(tab(tabn) + "include mime.types;");
                    WriteFile.WriteLine(tab(tabn) + "default_type application/octet-stream;");
                }

                if (Globals.VHosts.Count > 0)
                {
                    String fullPath;

                    foreach (VirtualHost vh in Globals.VHosts)
                    {
                        if (!WebServerConfig.IsValidPort(vh.Port) || String.IsNullOrEmpty(vh.DocumentRoot))
                        {
                            Globals.AddToLog("Virtual server '" + vh.ServerName + "' skipped: invalid port or empty document root.");
                            continue;
                        }

                        fullPath = Path.GetFullPath(Path.Combine(this.nginxInstallation, vh.DocumentRoot)).Replace('\\', '/').TrimEnd('/');
                        bool anyIP = String.IsNullOrEmpty(vh.IP) || vh.IP == "*";

                        WriteFile.WriteLine(tab(tabn) + "server {");
                        tabn++;
                        WriteFile.WriteLine(tab(tabn) + "listen " + (anyIP ? "" : vh.IP + ":") + vh.Port + ";");
                        if (!String.IsNullOrEmpty(vh.ServerName)) { WriteFile.WriteLine(tab(tabn) + "server_name " + vh.ServerName + ";"); }
                        else if (!String.IsNullOrEmpty(vh.ServerAlias)) { WriteFile.WriteLine(tab(tabn) + "server_name " + vh.ServerAlias + ";"); }
                        WriteFile.WriteLine(tab(tabn) + "index " + ((this.PHPcgi) ? "index.php " : "") + "index.html index.htm;");
                        WriteFile.WriteLine();
                        WriteFile.WriteLine(tab(tabn) + "location / {");
                        tabn++;
                        WriteFile.WriteLine(tab(tabn) + "root " + fullPath + ";");
                        tabn--;
                        WriteFile.WriteLine(tab(tabn) + "}");
                        WriteFile.WriteLine();
                        WriteFile.WriteLine(tab(tabn) + "error_page 500 502 503 504  /50x.html;");
                        WriteFile.WriteLine(tab(tabn) + "location = /50x.html {");
                        tabn++;
                        WriteFile.WriteLine(tab(tabn) + "root " + fullPath + ";");
                        tabn--;
                        WriteFile.WriteLine(tab(tabn) + "}");

                        List<String> DirPaths = new List<String>();

                        String af;
                        String dp;
                        String an;
                        foreach (Aliases alias in vh.Alias)
                        {
                            if (String.IsNullOrEmpty(alias.AliasFolder) || String.IsNullOrEmpty(alias.AliasName))
                            {
                                continue;
                            }
                            af = alias.AliasFolder.Replace("\\", "/");
                            af += (af[af.Length - 1] != '/') ? "/" : "";
                            an = alias.AliasName.Replace("\\", "/");
                            an += (an[an.Length - 1] != '/') ? "/" : "";

                            WriteFile.WriteLine(tab(tabn) + "location " + an + " {");
                            tabn++;
                            WriteFile.WriteLine(tab(tabn) + "alias " + af + ";");
                            DirPaths.Add(af);

                            foreach (Directories dir in vh.Directory)
                            {
                                if (dir.DirectoryName.Replace("\\", "/").TrimEnd('/').Equals(af.TrimEnd('/'), StringComparison.OrdinalIgnoreCase))
                                {
                                    // This is the same code...
                                    WriteFile.WriteLine(tab(tabn) + "disable_symlinks " + ((dir.SymLinks) ? "off" : "on") + ";");
                                    WriteFile.WriteLine(tab(tabn) + "autoindex " + ((dir.Indexes) ? "on" : "off") + ";");
                                    WriteFile.WriteLine(tab(tabn) + "ssi " + ((dir.Includes) ? "on" : "off") + ";");
                                    if (dir.MultiViews)
                                    {
                                        WriteFile.WriteLine(tab(tabn) + "try_files $uri $1?$args $1.php?$args;");
                                    }
                                }
                            }
                            tabn--;
                            WriteFile.WriteLine(tab(tabn) + "}");

                            if (this.PHPcgi)
                            {
                                WriteFile.WriteLine(tab(tabn) + "location ~ ^" + an + "(.+\\.php)$ {");
                                tabn++;
                                WriteFile.WriteLine(tab(tabn) + "alias " + af + "$1;");
                                WriteFile.WriteLine(tab(tabn) + "fastcgi_pass 127.0.0.1:8866;");
                                WriteFile.WriteLine(tab(tabn) + "fastcgi_index index.php;");
                                WriteFile.WriteLine(tab(tabn) + "fastcgi_param SCRIPT_FILENAME $document_root$fastcgi_script_name;");
                                WriteFile.WriteLine(tab(tabn) + "include fastcgi_params;");
                                tabn--;
                                WriteFile.WriteLine(tab(tabn) + "}");
                            }
                        }

                        DirPaths = DirPaths.ConvertAll(d => d.TrimEnd('/').ToLower());
                        foreach (Directories dir in vh.Directory) {
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
                                WriteFile.WriteLine(tab(tabn) + "location " + af + " {");
                                tabn++;
                                // ... As this one. Sorry. :(
                                WriteFile.WriteLine(tab(tabn) + "disable_symlinks " + ((dir.SymLinks) ? "off" : "on") + ";");
                                WriteFile.WriteLine(tab(tabn) + "autoindex " + ((dir.Indexes) ? "on" : "off") + ";");
                                WriteFile.WriteLine(tab(tabn) + "ssi " + ((dir.Includes) ? "on" : "off") + ";");
                                if (dir.MultiViews)
                                {
                                    WriteFile.WriteLine(tab(tabn) + "try_files $uri $1?$args $1.php?$args;");
                                }
                                tabn--;
                                WriteFile.WriteLine(tab(tabn) + "}");
                            }
                        }

                        if (this.PHPcgi)
                        {
                            WriteFile.WriteLine();
                            WriteFile.WriteLine(tab(tabn) + "location ~ \\.php$ {");
                            tabn++;
                            WriteFile.WriteLine(tab(tabn) + "root " + fullPath + ";");
                            WriteFile.WriteLine(tab(tabn) + "fastcgi_pass 127.0.0.1:8866;");
                            WriteFile.WriteLine(tab(tabn) + "fastcgi_index index.php;");
                            WriteFile.WriteLine(tab(tabn) + "fastcgi_param SCRIPT_FILENAME $document_root$fastcgi_script_name;");
                            //WriteFile.WriteLine(tab(tabn) + "try_files $uri $uri/ /index.php?$args;");
                            WriteFile.WriteLine(tab(tabn) + "try_files $uri $uri/ /index.php?$request_uri;");
                            WriteFile.WriteLine(tab(tabn) + "include fastcgi_params;");
                            tabn--;
                            WriteFile.WriteLine(tab(tabn) + "}");
                        }

                        tabn--;
                        WriteFile.WriteLine(tab(tabn) + "}");
                    }
                }
                tabn--;
                WriteFile.WriteLine(tab(tabn) + "}");
                WriteFile.WriteLine("");
                }
            }
            catch (Exception ex)
            {
                Globals.Error.Show("Unexpected error", ex);
            }
        }

        private string tab(int n)
        {
            return new String('\t', n);
        }
    }
}
