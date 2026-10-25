using System;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace WDS
{
    class PHPConfigFile
    {
        private String PHPInstallation;

        public PHPConfigFile(String Installation)
        {
            this.PHPInstallation = Installation;
            Directory.CreateDirectory(PHPInstallation + @"ext");
        }

        public void Write()
        {
            try
            {
                using (StreamWriter WriteFile = new StreamWriter(PHPInstallation + @"php.ini", false))
                {
                    WriteFile.WriteLine("[PHP]");

                    HashSet<String> written = new HashSet<String>(StringComparer.OrdinalIgnoreCase);
                    DataTable DBSettings = Globals.dbConn.GetDataTable("SELECT * FROM PHPSettings");
                    foreach (DataRow row in DBSettings.Rows)
                    {
                        bool on = (row["Active"].ToString() == "1");
                        String line = on ? "" : ";";
                        line += row["Param"] + " = " + row["Value"];
                        WriteFile.WriteLine(line);
                        if (on) written.Add(row["Param"].ToString().Trim());
                    }

                    // Settings every web server connection needs, unless the user set them.
                    String extDir = this.PHPInstallation + "ext";
                    if (!Directory.Exists(extDir) && Directory.Exists(this.PHPInstallation + "extensions"))
                    {
                        extDir = this.PHPInstallation + "extensions"; // PHP 4.
                    }
                    if (!written.Contains("extension_dir")) WriteFile.WriteLine("extension_dir = \"" + extDir + "\"");
                    if (!written.Contains("cgi.force_redirect")) WriteFile.WriteLine("cgi.force_redirect = 0");
                    if (!written.Contains("cgi.fix_pathinfo")) WriteFile.WriteLine("cgi.fix_pathinfo = 1");
                    WriteFile.WriteLine("");

                    DBSettings = Globals.dbConn.GetDataTable("SELECT Module FROM ActiveModules WHERE ServerType = 'PHP'");
                    List<String> modules = new List<String>();
                    foreach (DataRow row in DBSettings.Rows)
                    {
                        modules.Add(row["Module"].ToString());
                    }

                    // The database is used: enable the MySQL extensions this PHP has.
                    if (Globals.PhpDbLink != null && Globals.PhpDbLink.Compatible)
                    {
                        foreach (String ext in new String[] { "php_pdo.dll", "php_mysqli.dll", "php_pdo_mysql.dll", "php_mysql.dll" })
                        {
                            if (File.Exists(Path.Combine(extDir, ext)) && !modules.Exists(m => m.Equals(ext, StringComparison.OrdinalIgnoreCase)))
                            {
                                modules.Add(ext);
                            }
                        }
                        // PHP 5.0-5.2: php_pdo.dll must be loaded before php_pdo_mysql.dll.
                        int pdo = modules.FindIndex(m => m.Equals("php_pdo.dll", StringComparison.OrdinalIgnoreCase));
                        if (pdo > 0)
                        {
                            String m0 = modules[pdo];
                            modules.RemoveAt(pdo);
                            modules.Insert(0, m0);
                        }
                    }

                    HashSet<String> PHPModules = new HashSet<String>(StringComparer.OrdinalIgnoreCase);
                    foreach (String module in modules)
                    {
                        if (PHPModules.Add(module))
                        {
                            WriteFile.WriteLine("extension = \"" + module + "\"");
                        }
                    }
                    WriteFile.WriteLine("");

                    // Extra settings of the active extensions only.
                    DBSettings = Globals.dbConn.GetDataTable("SELECT * FROM PHPModules");
                    foreach (DataRow row in DBSettings.Rows)
                    {
                        String module = row["Module"].ToString();
                        if (!PHPModules.Contains(module))
                        {
                            continue;
                        }
                        WriteFile.WriteLine("[" + module.Replace("php_", "").Replace(".dll", "") + "]");
                        WriteFile.WriteLine(row["Value"]);
                        WriteFile.WriteLine("");
                    }
                }
            }
            catch (Exception ex)
            {
                Globals.Error.Show("Unable to write php.ini", ex);
            }
        }
    }
}
