using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace WDS
{
    public static class Globals
    {
        public static String StartOnLaunch = "0";
        public static String StartInSystray = "0";
        public static String RewriteConfigFiles = "0";
        public static String LastServersDBMod = "0";

        public static RunServers Processes = new RunServers();
        public static ServerType Servers;

        public static List<VirtualHost> VHosts = new List<VirtualHost>();
        public static String AppFolder;

        public static String ActiveWebServerVersion;
        public static String ActivePHPVersion;
        public static String ActiveMySQLVersion;

        public static String WebServerStart;
        public static String PHPStart;
        public static String MySQLStart;

        public static String DataBasePass = "WebDevelopmentServer1";
        public static bool DataBasePassChange = false;

        public static SQLiteDatabase dbConn;
        public static SQLiteDatabase dbServ;

        public static Msg Error = new Msg();

        private static System.Windows.Forms.ListBox _list = new System.Windows.Forms.ListBox();

        public static System.Windows.Forms.ListBox list
        {
            get { return _list; }
            set
            {
                _list = value;
            }
        }

        public static void AddToLog(String[] Text)
        {
            try
            {
                foreach (String txt in Text)
                {
                    _list.Items.Add(txt);
                }
            }
            catch(Exception)
            {
                return;
            }
        }

        public static void AddToLog(String Text)
        {
            if (!String.IsNullOrEmpty(Text))
            {
                char[] charSeparators = new char[] { '\n' };
                String[] SplittedText = Text.Split(charSeparators, StringSplitOptions.RemoveEmptyEntries);
                AddToLog(SplittedText);
            }
        }
    }

    class Config
    {
        public static int LoadSettings()
        {
            DataTable recipe = new DataTable();
            try
            {
                recipe = Globals.dbConn.GetDataTable("SELECT * FROM General");
            }
            catch(Exception ex)
            {
                Globals.Error.Show("'Config.db' is not a valid database.\r\n" + ex.Message);
                return -1;
            }

            if (recipe.Rows.Count > 0)
            {
                foreach (DataRow row in recipe.Rows)
                {
                    try
                    {
                        var data = typeof(Globals).GetField(row["param"].ToString(), BindingFlags.Public | BindingFlags.Static);
                        data.SetValue(null, row["value"].ToString());
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            return recipe.Rows.Count;
        }

        public static bool SaveSetting(String key, String val)
        {
            DataTable recipe = Globals.dbConn.GetDataTable("SELECT * FROM General WHERE param LIKE '" + key + "'");
            int a;
            if (recipe.Rows.Count > 0)
            {
                a = Globals.dbConn.ExecuteNonQuery("UPDATE General SET value = '" + val + "' WHERE param LIKE '" + key + "'");
            }
            else
            {
                a = Globals.dbConn.ExecuteNonQuery("INSERT INTO General ('param', 'value') values('" + key + "', '" + val + "')");
            }

            return a >= 1;
        }
    }

    public class Msg
    {
        public void Show(String Message)
        {
            System.Windows.Forms.MessageBox.Show(Message);
        }
    }
}
