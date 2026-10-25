using System;
using System.ComponentModel;
using System.Data.SQLite;
using System.Globalization;
using System.IO;
using System.Net;

namespace WDS
{
    class DataBaseFiles
    {
        private WebClient client;
        private DateTime LastModified;

        // Comes from the 'ServersListURL' setting in config.db. Empty = no updates.
        private static String URL
        {
            get { return (Globals.ServersListURL ?? "").Trim(); }
        }

        public static bool UpdatesEnabled
        {
            get { return URL.Length > 0; }
        }

        /// <summary>
        /// Called on the UI thread when a new servers.db has been installed and loaded.
        /// </summary>
        public event EventHandler ServersUpdated;

        /// <summary>
        /// True if the server has a newer servers.db. Returns false (and logs why)
        /// if there is no connection or the server does not answer.
        /// </summary>
        public bool CheckLastModified()
        {
            try
            {
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(URL);
                req.Method = "HEAD";
                req.Timeout = 5000;
                using (HttpWebResponse res = (HttpWebResponse)req.GetResponse())
                {
                    this.LastModified = res.LastModified.ToUniversalTime();
                }
            }
            catch (Exception ex)
            {
                Globals.LogError("Unable to check for servers list updates", ex);
                return false;
            }

            DateTime saved;
            if (!DateTime.TryParse(Globals.LastServersDBMod, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal, out saved))
            {
                return true;
            }
            return saved < this.LastModified;
        }

        /// <summary>
        /// Starts downloading the servers list in the background.
        /// </summary>
        public Boolean DownloadUpdate()
        {
            String zip = Globals.AppFolder + "tmp.zip";
            this.DeleteFile(zip);

            try
            {
                this.client = new WebClient();
                this.client.DownloadFileCompleted += new AsyncCompletedEventHandler(DownloadFileCompleted);
                this.client.DownloadFileAsync(new Uri(URL), zip);
                Globals.AddToLog("Downloading the servers list...");
                return true;
            }
            catch (Exception e)
            {
                Globals.LogError("Unable to download the servers list", e);
                this.DeleteFile(zip);
                return false;
            }
        }

        private void DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (this.client != null)
            {
                this.client.Dispose();
                this.client = null;
            }

            String zip = Globals.AppFolder + "tmp.zip";
            String tmpDir = Globals.AppFolder + "tmp";

            try
            {
                if (e.Cancelled || e.Error != null)
                {
                    Globals.AddToLog("Unable to download the servers list: " + (e.Error != null ? e.Error.Message : "cancelled"));
                    return;
                }

                this.DeleteFile(tmpDir);
                if (!Decompress.ExtractZip(Globals.AppFolder, "tmp.zip", true))
                {
                    return;
                }

                // Only replace the current list if the new one is a valid database.
                String newDb = Path.Combine(tmpDir, "servers.db");
                if (!File.Exists(newDb) || !new SQLiteDatabase(newDb).HasTable("ServerNames"))
                {
                    Globals.AddToLog("The downloaded servers list is not valid. The current one is kept.");
                    return;
                }
                SQLiteConnection.ClearAllPools();

                foreach (String file in Directory.GetFiles(tmpDir))
                {
                    String target = Globals.AppFolder + Path.GetFileName(file);
                    if (File.Exists(target))
                    {
                        File.Delete(target);
                    }
                    File.Move(file, target);
                }

                if (this.LastModified == DateTime.MinValue)
                {
                    this.LastModified = DateTime.UtcNow;
                }
                Globals.LastServersDBMod = this.LastModified.ToString("o", CultureInfo.InvariantCulture);
                Config.SaveSetting("LastServersDBMod", Globals.LastServersDBMod);

                LoadDataBase(Globals.AppFolder + @"servers.db", ref Globals.dbServ);
                Globals.AddToLog("Servers database successfully updated.");

                if (ServersUpdated != null)
                {
                    ServersUpdated(this, EventArgs.Empty);
                }
            }
            catch (Exception ex)
            {
                Globals.LogError("Unable to update the servers list", ex);
            }
            finally
            {
                this.DeleteFile(zip);
                this.DeleteFile(tmpDir);
            }
        }

        private void DeleteFile(String Fl)
        {
            try
            {
                if (File.Exists(Fl))
                {
                    File.Delete(Fl);
                }
                else if (Directory.Exists(Fl))
                {
                    Directory.Delete(Fl, true);
                }
            }
            catch (Exception e)
            {
                Globals.LogError("Unable to delete " + Fl + "", e);
            }
        }

        public bool LoadDataBase(String FileName, ref SQLiteDatabase Target)
        {
            if (!File.Exists(FileName))
            {
                return false;
            }

            Target = new SQLiteDatabase(FileName);
            return true;
        }

        public Boolean CreateConfigFile()
        {
            String dbFile = Globals.AppFolder + @"config.db";
            String[] SQLs = new String[]
            {
                "CREATE TABLE General (param TEXT, value TEXT);",
                "INSERT INTO General VALUES('WebServerStart','0');",
                "INSERT INTO General VALUES('MySQLStart','0');",
                "INSERT INTO General VALUES('PHPStart','0');",
                "INSERT INTO General VALUES('DataBasePass','');",
                "INSERT INTO General VALUES('DataBasePassPending','0');",
                "INSERT INTO General VALUES('StartOnLaunch','0');",
                "INSERT INTO General VALUES('StartInSystray','0');",
                "INSERT INTO General VALUES('RewriteConfigFiles','1');",
                "INSERT INTO General VALUES('ActiveWebServerVersion','');",
                "INSERT INTO General VALUES('ActivePHPVersion','');",
                "INSERT INTO General VALUES('ActiveMySQLVersion','');",
                "INSERT INTO General VALUES('LastServersDBMod','2000-01-01T00:00:00.0000000Z');",
                "INSERT INTO General VALUES('ServersListURL','');",
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
                new SQLiteDatabase(dbFile).ExecuteNonQueryBulk(SQLs);
                Globals.AddToLog("This is the first time you run this program.");
                Globals.AddToLog("It's highly recommended that you run the Configuration Wizard before you start using it.");
                return true;
            }
            catch (Exception ex)
            {
                Globals.Error.Show("Error: Unable to create the database file at:\r\n\r\n" + Directory.GetParent(dbFile), ex);
                return false;
            }
        }
    }
}
