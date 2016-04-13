using System;
using System.Collections.Generic;
using System.IO;

namespace WDS.ServersConfig.ServerConfigFiles
{
    class nodeJSConfigFile
    {

        private String nodeJSInstallation;

        //public String PHPcgi;
        public bool PHPnative = true;

        public nodeJSConfigFile(String Installation)
        {
            this.nodeJSInstallation = Installation;
        }

        public void Write()
        {
            if (File.Exists(this.nodeJSInstallation + @"server.js"))
            {
                File.Delete(this.nodeJSInstallation + @"server.js");
            }

            try
            {
                StreamWriter WriteFile = new StreamWriter(this.nodeJSInstallation + @"server.js");

                WriteFile.WriteLine("server.document-root = \"" + this.nodeJSInstallation.Replace("\\", "/") + "\"");






                if (Globals.Servers.WebServer.ActiveModules.Count > 0)
                {
                    String Modules = "\"" + string.Join("\" , \"", Globals.Servers.WebServer.ActiveModules.ToArray()) + "\"";
                    Modules = Modules.Replace(".dll", "");
                    WriteFile.WriteLine("server.modules = ( " + Modules + " )");
                }

                WriteFile.WriteLine("server.document-root = \"" + this.nodeJSInstallation.Replace("\\", "/") + "\"");
                WriteFile.WriteLine("index-file.names = ( " + ((this.PHPnative)?"\"index.php\", ": "") + "\"index.html\", \"index.htm\", \"default.htm\" )");

                if (this.PHPnative)
                {
                    WriteFile.WriteLine("cgi.assign = ( \".php\" => \"" + Path.GetDirectoryName(Globals.Servers.PHP.Exe) + "/php-cgi.exe\" )");
                }

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

            if (!Directory.Exists(this.nodeJSInstallation))
            {
                Globals.Error.Show("Directory " + this.nodeJSInstallation + " not exist."); return false;
            }

            if (SetPHP == true)
            {
                this.PHPnative = true;
            }

            this.Write();

            return WhatToReturn;
        }
    }
}
