using System;
using System.IO;

namespace WDS
{
    class MySQLConfigFile
    {
        // Lives in the user's temp folder (not the root of the drive) and is
        // deleted when the database server stops.
        private static readonly String InitFile = Path.Combine(Path.GetTempPath(), "wds-mysql-init.sql");

        /// <summary>
        /// Writes the SQL that sets the root password. Returns the file path, or null on error.
        /// </summary>
        public String ChangeRootPasswordFile(DbAuthMode mode, SoftwareComponent db)
        {
            this.RemoveFile();
            try
            {
                String pass = Escape(Globals.DataBasePass);
                bool mariaDB = (db.Product == Product.MariaDB);

                // ALTER USER exists in MySQL 5.7.6+ and MariaDB 10.2+.
                // MySQL 8 removed PASSWORD() and the 'Password' column.
                bool alterUser = mariaDB ? db.AtLeast(10, 2) : db.AtLeast(5, 7, 6);

                using (StreamWriter WriteFile = new StreamWriter(InitFile))
                {
                    if (mode == DbAuthMode.Old)
                    {
                        // Pre-4.1 hash, the only one PHP 4 understands.
                        WriteFile.WriteLine("SET PASSWORD FOR 'root'@'localhost' = OLD_PASSWORD('" + pass + "');");
                    }
                    else if (mode == DbAuthMode.Native && !mariaDB && db.AtLeast(8))
                    {
                        WriteFile.WriteLine("ALTER USER 'root'@'localhost' IDENTIFIED WITH mysql_native_password BY '" + pass + "';");
                    }
                    else if (!mariaDB && db.AtLeast(8))
                    {
                        WriteFile.WriteLine("ALTER USER 'root'@'localhost' IDENTIFIED WITH caching_sha2_password BY '" + pass + "';");
                    }
                    else if (alterUser)
                    {
                        WriteFile.WriteLine("ALTER USER 'root'@'localhost' IDENTIFIED BY '" + pass + "';");
                    }
                    else
                    {
                        WriteFile.WriteLine("SET PASSWORD FOR 'root'@'localhost' = PASSWORD('" + pass + "');");
                    }
                    WriteFile.WriteLine("FLUSH PRIVILEGES;");
                }
            }
            catch (Exception ex)
            {
                Globals.LogError("Unable to write the file that sets the root password (" + InitFile + ")", ex);
                this.RemoveFile();
                return null;
            }
            return InitFile;
        }

        private static String Escape(String value)
        {
            return (value ?? "").Replace("\\", "\\\\").Replace("'", "''");
        }

        public void RemoveFile()
        {
            try
            {
                if (File.Exists(InitFile))
                {
                    File.Delete(InitFile);
                }
            }
            catch (Exception ex)
            {
                // mysqld may still hold the file; it is deleted on the next stop.
                Globals.AddToLogFile("Unable to delete " + InitFile + " now (it is retried on the next stop): " + ex.Message);
            }
        }
    }
}
