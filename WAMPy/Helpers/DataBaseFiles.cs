using System;
using System.ComponentModel;
using System.Data.SQLite;
using System.IO;
using System.Net;

namespace WDS
{
    class DataBaseFiles
    {
        private WebClient client;
        String URL = "http://www.mongui-programmer.com/WSD/WSDServers.zip";
        DateTime LastModified;

        public bool CheckLastModified()
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(this.URL);
            HttpWebResponse res = (HttpWebResponse)req.GetResponse();

            this.LastModified = res.LastModified;

            if (Globals.LastServersDBMod == null || Convert.ToDateTime(Globals.LastServersDBMod) < res.LastModified)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public Boolean DownloadUpdate()
        {
            this.DeleteFile(Globals.AppFolder + "tmp.zip");

            try
            {
                this.client = new WebClient();
                this.client.DownloadFileCompleted += new AsyncCompletedEventHandler(DownloadFileCompleted);
                this.client.DownloadFileAsync(new Uri(this.URL), Globals.AppFolder + "tmp.zip");
                return true;
            }
            catch (Exception e)
            {
                Globals.AddToLog(e.Message);
                this.DeleteFile(Globals.AppFolder + "tmp.zip");
                return false;
            }
        }

        private void DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            this.client = null;
            this.DeleteFile(Globals.AppFolder + "tmp");
            
            if (e.Error != null)
            {
                Globals.Error.Show(e.Error.Message);
            }
            else
            {
                Decompress.ExtractZip(Globals.AppFolder, "tmp.zip", true);

                Globals.LastServersDBMod = this.LastModified.ToString();
                Config.SaveSetting("LastServersDBMod", Globals.LastServersDBMod);

                LoadDataBase(Globals.AppFolder + @"servers.db", ref Globals.dbServ);

                Globals.AddToLog("Servers database successfully updated.");
            }


            string[] fileList = Directory.GetFiles(Globals.AppFolder + "tmp\\");

            string file2;
            foreach (string file in fileList)
            {
                file2 = Path.GetFileName(file);

                if (System.IO.File.Exists(Globals.AppFolder + file2))
                {
                    File.Delete(Globals.AppFolder + file2);
                }

                File.Move(Globals.AppFolder + "tmp\\" + file2, Globals.AppFolder + file2);
            }

            this.DeleteFile(Globals.AppFolder + "tmp.zip");
            this.DeleteFile(Globals.AppFolder + "tmp");
        }

        private void DeleteFile(String Fl)
        {
            if (File.Exists(Fl))
            {
                try
                {
                    File.Delete(Fl);
                }
                catch (Exception e)
                {
                    Globals.Error.Show(e.Message);
                }
            }
            else if (Directory.Exists(Fl))
            {
                try
                {
                    Directory.Delete(Fl);
                }
                catch (Exception e)
                {
                    Globals.Error.Show(e.Message);
                }
            }
        }

        public bool LoadDataBase(String FileName, ref SQLiteDatabase Target)
        {
            if (!System.IO.File.Exists(FileName))
            {
                //Globals.Error.Show("Error: Unable to find the database file at:\r\n\r\n" + Directory.GetParent(FileName));
                return false;
            }

            Target = new SQLiteDatabase(FileName);
            return true;
        }

        public Boolean CreateConfigFile()
        {
            String dbFile = Globals.AppFolder + @"config.db";
            SQLiteDatabase Target = new SQLiteDatabase(dbFile);
            String[] SQLs = new String[]
            {
                "CREATE TABLE General (param TEXT, value TEXT);",
                "INSERT INTO General VALUES('WebServerStart',0);",
                "INSERT INTO General VALUES('MySQLStart',0);",
                "INSERT INTO General VALUES('PHPStart',0);",
                "INSERT INTO General VALUES('DataBasePass','WebDevelopmentServer1');",
                "INSERT INTO General VALUES('StartOnLaunch',0);",
                "INSERT INTO General VALUES('StartInSystray',0);",
                "INSERT INTO General VALUES('RewriteConfigFiles',1);",
                "INSERT INTO General VALUES('ActiveWebServerVersion','null');",
                "INSERT INTO General VALUES('ActivePHPVersion','null');",
                "INSERT INTO General VALUES('ActiveMySQLVersion','null');",
                "INSERT INTO General VALUES('LastServersDBMod','01/01/2000 00:00:00');",
                "CREATE TABLE ActiveModules (Module TEXT, ServerType TEXT);",
                "CREATE TABLE Aliases (AliasID INTEGER PRIMARY KEY, AliasFolder TEXT, AliasName TEXT, VHostID NUMERIC);",
                "CREATE TABLE ConfigModules (ServerType TEXT, IfModule TEXT, IfModuleActive NUMERIC, Module TEXT);",
                "CREATE TABLE CustomSettings (ServerType TEXT, Id INTEGER PRIMARY KEY, Param TEXT);",
                "CREATE TABLE Directories (DirectoryID INTEGER PRIMARY KEY, VHostID NUMERIC, Others TEXT, AllowOverride NUMERIC, MultiViews NUMERIC, Indexes NUMERIC, Includes NUMERIC, SymLinks NUMERIC, DirectoryName TEXT);",
                "CREATE TABLE PHPModules (Module TEXT, Value TEXT);",
                "CREATE TABLE PHPSettings (Active NUMERIC, Param TEXT, Value TEXT);",
                "CREATE TABLE VirtualHosts (DocumentRoot TEXT, IP TEXT, Others TEXT, Port NUMERIC, ServerAlias TEXT, ServerName TEXT, VHostID INTEGER PRIMARY KEY);"
            };

            try
            {
                SQLiteConnection.CreateFile(dbFile);
                Target.ExecuteNonQueryBulk(SQLs);
                Globals.AddToLog("This is the first time you run this program.\r\nIt's highly recommended that you run the Configuration Wizard before you start using it.");
                return true;
            }
            catch(Exception)
            {
                Globals.Error.Show("Error: Unable to create the database file at:\r\n\r\n" + Directory.GetParent(dbFile));
                return false;
            }
        }
    }
}
