using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Security.Cryptography;
using System.Windows.Forms;

namespace WDS
{
    public static class Globals
    {
        public static String StartOnLaunch = "0";
        public static String StartInSystray = "0";
        public static String RewriteConfigFiles = "0";
        public static String LastServersDBMod = "0";
        // Where the servers list (zip with servers.db) is downloaded from.
        // The original server (mongui-programmer.com) no longer exists. Set this
        // setting in config.db to a new address, or leave it empty to skip updates.
        public static String ServersListURL = "";

        public static RunServers Processes = new RunServers();
        public static ServerType Servers;

        public static List<VirtualHost> VHosts = new List<VirtualHost>();
        public static String AppFolder;

        public static String ActiveWebServerVersion = "";
        public static String ActivePHPVersion = "";
        public static String ActiveMySQLVersion = "";

        public static String WebServerStart = "0";
        public static String PHPStart = "0";
        public static String MySQLStart = "0";

        public static String DataBasePass = "";
        // "1" while a new root password still has to be applied to the database server.
        // Saved in config.db, so a pending change survives a restart of the app.
        public static String DataBasePassPending = "0";
        // "<database folder>|<DbAuthMode>": password format last applied to the root user.
        public static String DataBaseAuthApplied = "";

        // How the running servers are joined (set on every start).
        public static PairResult WebPhpLink;
        public static PairResult PhpDbLink;

        public static SQLiteDatabase dbConn;
        public static SQLiteDatabase dbServ;

        public static Msg Error = new Msg();

        private static ListBox _list;

        public static ListBox list
        {
            get { return _list; }
            set { _list = value; }
        }

        /// <summary>
        /// Adds lines to the log. Safe to call from any thread.
        /// </summary>
        public static void AddToLog(String[] Text)
        {
            WriteLogFile(Text);
            AddToList(Text);
        }

        /// <summary>
        /// Writes only to the log file (details that would clutter the window).
        /// </summary>
        public static void AddToLogFile(String Text)
        {
            if (!String.IsNullOrEmpty(Text))
            {
                WriteLogFile(Text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries));
            }
        }

        /// <summary>
        /// An error: one line in the window, the full exception (with stack trace) in the file.
        /// </summary>
        public static void LogError(String context, Exception ex)
        {
            AddToList(new String[] { "ERROR: " + context + (ex != null ? ": " + ex.Message : "") });
            WriteLogFile(new String[] { "ERROR: " + context });
            if (ex != null)
            {
                WriteLogFile(ex.ToString().Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries));
            }
        }

        private static readonly object _logLock = new object();
        private static readonly List<String> _pendingLog = new List<String>();

        /// <summary>
        /// Appends the lines to AppFolder\logs\wds-yyyy-MM-dd.log. Lines logged before
        /// AppFolder is known are kept and written with the first ones after it.
        /// </summary>
        private static void WriteLogFile(String[] Text)
        {
            String stamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            lock (_logLock)
            {
                foreach (String txt in Text)
                {
                    _pendingLog.Add(stamp + "  " + txt);
                }
                if (AppFolder == null)
                {
                    return;
                }

                try
                {
                    String dir = System.IO.Path.Combine(AppFolder, "logs");
                    System.IO.Directory.CreateDirectory(dir);
                    System.IO.File.AppendAllLines(System.IO.Path.Combine(dir, "wds-" + DateTime.Now.ToString("yyyy-MM-dd") + ".log"), _pendingLog);
                    _pendingLog.Clear();
                }
                catch (Exception)
                {
                    // Keep them for the next try, but never grow without limit.
                    if (_pendingLog.Count > 1000) _pendingLog.RemoveRange(0, _pendingLog.Count - 1000);
                }
            }
        }

        private static void AddToList(String[] Text)
        {
            ListBox target = _list;
            if (target == null || target.IsDisposed)
            {
                return;
            }

            try
            {
                if (target.InvokeRequired)
                {
                    target.BeginInvoke(new Action<String[]>(AddToList), new object[] { Text });
                    return;
                }

                foreach (String txt in Text)
                {
                    target.Items.Add(txt);
                }
                target.TopIndex = Math.Max(0, target.Items.Count - 1);
            }
            catch (Exception)
            {
                // The form is closing; the log is no longer needed.
            }
        }

        public static void AddToLog(String Text)
        {
            if (!String.IsNullOrEmpty(Text))
            {
                char[] charSeparators = new char[] { '\r', '\n' };
                String[] SplittedText = Text.Split(charSeparators, StringSplitOptions.RemoveEmptyEntries);
                AddToLog(SplittedText);
            }
        }

        /// <summary>
        /// Random password that passes DataBaseConfig.CheckRegex.
        /// </summary>
        public static String GeneratePassword()
        {
            const String upper = "ABCDEFGHJKLMNPQRSTUVWXYZ";
            const String lower = "abcdefghijkmnopqrstuvwxyz";
            const String digits = "23456789";
            const String all = upper + lower + digits;

            byte[] bytes = new byte[16];
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(bytes);
            }

            char[] pass = new char[16];
            pass[0] = upper[bytes[0] % upper.Length];
            pass[1] = lower[bytes[1] % lower.Length];
            pass[2] = digits[bytes[2] % digits.Length];
            for (int i = 3; i < pass.Length; i++)
            {
                pass[i] = all[bytes[i] % all.Length];
            }
            return new String(pass);
        }
    }

    class Config
    {
        public static int LoadSettings()
        {
            DataTable recipe;
            try
            {
                recipe = Globals.dbConn.GetDataTable("SELECT * FROM General");
            }
            catch (Exception ex)
            {
                Globals.Error.Show("'Config.db' is not a valid database.", ex);
                return -1;
            }

            foreach (DataRow row in recipe.Rows)
            {
                FieldInfo data = typeof(Globals).GetField(row["param"].ToString(), BindingFlags.Public | BindingFlags.Static);
                if (data != null && data.FieldType == typeof(String))
                {
                    data.SetValue(null, row["value"].ToString());
                }
            }

            // Old config files stored the text 'null' for "no version selected".
            if (Globals.ActiveWebServerVersion == "null") Globals.ActiveWebServerVersion = "";
            if (Globals.ActivePHPVersion == "null") Globals.ActivePHPVersion = "";
            if (Globals.ActiveMySQLVersion == "null") Globals.ActiveMySQLVersion = "";

            return recipe.Rows.Count;
        }

        public static bool SaveSetting(String key, String val)
        {
            try
            {
                int a = Globals.dbConn.ExecuteNonQuery("UPDATE General SET value = @p1 WHERE param = @p0", key, val);
                if (a == 0)
                {
                    a = Globals.dbConn.ExecuteNonQuery("INSERT INTO General (param, value) VALUES (@p0, @p1)", key, val);
                }
                return a >= 1;
            }
            catch (Exception ex)
            {
                Globals.AddToLog("Unable to save the setting '" + key + "': " + ex.Message);
                return false;
            }
        }
    }

    public class Msg
    {
        public void Show(String Message)
        {
            // Every message box is also kept in the log.
            Globals.AddToLog("ERROR: " + Message);
            MessageBox.Show(Message, "Web Development Server");
        }

        /// <summary>
        /// Shows the message and logs the full exception.
        /// </summary>
        public void Show(String Message, Exception ex)
        {
            Globals.LogError(Message, ex);
            MessageBox.Show(Message + (ex != null ? "\r\n" + ex.Message : ""), "Web Development Server");
        }
    }
}
