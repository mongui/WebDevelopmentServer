using System;
using System.Collections.Generic;
using System.IO;
using System.Data;

namespace WDS
{
    class PHPConfigFile
    {
        
        private String PHPInstallation;

        public PHPConfigFile(String Installation)
        {
            this.PHPInstallation = Installation;

            if (!Directory.Exists(PHPInstallation + @"ext"))
            {
                Directory.CreateDirectory(PHPInstallation + @"ext");
            }
        }

        public void Write()
        {
            if (File.Exists(PHPInstallation + @"php.ini"))
            {
                File.Delete(PHPInstallation + @"php.ini");
            }

            try
            {
                StreamWriter WriteFile = new StreamWriter(PHPInstallation + @"php.ini");
                WriteFile.WriteLine("[PHP]");

                DataTable DBSettings = Globals.dbConn.GetDataTable("SELECT * FROM PHPSettings");
                if (DBSettings.Rows.Count > 0)
                {
                    String line;
                    foreach (DataRow row in DBSettings.Rows)
                    {
                        line = "";
                        if (row["Active"].ToString() != "1")
                        {
                            line = ";";
                        }
                        line += row["Param"] + " = " + row["Value"];
                        WriteFile.WriteLine(line);
                    }

                    WriteFile.WriteLine("");
                }

                DBSettings = Globals.dbConn.GetDataTable("SELECT * FROM ActiveModules WHERE ServerType LIKE 'PHP'");
                List<String> PHPModules = new List<String>();

                if (DBSettings.Rows.Count > 0)
                {
                    foreach (DataRow row in DBSettings.Rows)
                    {
                        PHPModules.Add(row["Module"].ToString());
                        WriteFile.WriteLine("extension = \"" + this.PHPInstallation + @"ext\" + row["Module"] + "\"");
                    }

                    WriteFile.WriteLine("");
                }

                String PHPMods = "Module LIKE '" + String.Join("' OR Module LIKE '", PHPModules.ToArray()) + "'";
                DBSettings = Globals.dbConn.GetDataTable("SELECT * FROM PHPModules WHERE " + PHPMods);
                if (DBSettings.Rows.Count > 0)
                {
                    foreach (DataRow row in DBSettings.Rows)
                    {
                        WriteFile.WriteLine("[" + row["Module"].ToString().Replace("php_", "").Replace(".dll", "") + "]");
                        WriteFile.WriteLine(row["Value"]);
                        WriteFile.WriteLine("");
                    }
                }

                WriteFile.Close();
            }
            catch (Exception ex)
            {
                Globals.Error.Show(ex.Message);
            }
        }
    }
}
