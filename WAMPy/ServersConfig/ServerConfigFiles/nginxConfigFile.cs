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
                StreamWriter WriteFile = new StreamWriter(this.nginxInstallation + @"conf\nginx.conf");

                foreach (String cs in this.GetFileSection(""))
                {
                    WriteFile.WriteLine(cs.Replace("\n", Environment.NewLine));
                }

                String[] data = this.GetFileSection("events");
                if (data.Length > 0)
                {
                    WriteFile.WriteLine();
                    WriteFile.WriteLine("events {");
                    tabn++;
                    foreach (String cs in data)
                    {
                        WriteFile.WriteLine(tab(tabn) + cs.Replace("\n", Environment.NewLine));
                    }
                    tabn--;
                    WriteFile.WriteLine("}");
                }

                WriteFile.WriteLine();
                WriteFile.WriteLine("http {");
                tabn++;

                data = this.GetFileSection("http");
                foreach (String cs in data)
                {
                    WriteFile.WriteLine(tab(tabn) + cs.Replace("\n", Environment.NewLine));
                }

                if (Globals.VHosts.Count > 0)
                {
                    String fullPath;

                    foreach (VirtualHost vh in Globals.VHosts)
                    {
                        fullPath = Path.Combine(this.nginxInstallation, vh.DocumentRoot);
                        fullPath = Path.GetFullPath((new Uri(fullPath)).LocalPath).Replace('\\', '/').ToLower();

                        WriteFile.WriteLine(tab(tabn) + "server {");
                        tabn++;
                        WriteFile.WriteLine(tab(tabn) + "listen " + vh.Port + ";");
                        if (vh.ServerName != "") { WriteFile.WriteLine(tab(tabn) + "server_name " + vh.ServerName + ";"); }
                        else if (vh.ServerAlias != "") { WriteFile.WriteLine(tab(tabn) + "server_name " + vh.ServerAlias + ";"); }
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
                                if (dir.DirectoryName.ToLower().Equals(af.ToLower()))
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

                        DirPaths = DirPaths.ConvertAll(d => d.ToLower());
                        foreach (Directories dir in vh.Directory) {
                            dp = dir.DirectoryName.ToLower().Replace("\\", "/");
                            if (!DirPaths.Contains(dir.DirectoryName.ToLower()) && dp != fullPath)
                            {
                                af = "/" + dp.Replace(fullPath, "");
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
                            WriteFile.WriteLine(tab(tabn) + "location ~ .php$ {");
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

                WriteFile.Close();
            }
            catch (Exception ex)
            {
                Globals.Error.Show(ex.Message);
            }
        }

        private string tab(int n)
        {
            return new String('\t', n);
        }
    }
}
