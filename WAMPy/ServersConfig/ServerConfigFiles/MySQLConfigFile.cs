using System;
using System.IO;

namespace WDS
{
    class MySQLConfigFile
    {
        public bool ChangeRootPasswordFile()
        {
            this.RemoveFile();
            try
            {
                StreamWriter WriteFile = new StreamWriter(@"\mysqlWSD");
                WriteFile.WriteLine("UPDATE mysql.user SET Password=PASSWORD('" + Globals.DataBasePass + "') WHERE User='root';");
                WriteFile.WriteLine("FLUSH PRIVILEGES;");
                WriteFile.Close();
            }
            catch(Exception)
            {
                this.RemoveFile();
                return false;
            }
            return true;
        }

        public void RemoveFile()
        {
            if (File.Exists(@"\mysqlWSD"))
            {
                File.Delete(@"\mysqlWSD");
            }
        }
    }
}
