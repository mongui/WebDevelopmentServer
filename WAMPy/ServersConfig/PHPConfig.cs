using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace WDS
{
    class PHPSetting
    {
        public String Param { get; set; }
        public String Value { get; set; }
        public int Active { get; set; }
    }

    class PHPModule
    {
        public String Module { get; set; }
        public String Value { get; set; }
    }

    class PHPConfig : CommonServerConfig
    {
        public List<PHPSetting> Settings = new List<PHPSetting>();
        public List<PHPModule> Modules = new List<PHPModule>();

        public void LoadSettings()
        {
            DataTable SettingsDB = Globals.dbConn.GetDataTable("SELECT * FROM PHPSettings");

            foreach (DataRow row in SettingsDB.Rows)
            {
                PHPSetting Set = new PHPSetting();

                foreach (DataColumn col in SettingsDB.Columns)
                {
                    try
                    {
                        PropertyInfo pi = Set.GetType().GetProperty(col.ToString());
                        pi.SetValue(Set, Convert.ChangeType(row[col], pi.PropertyType), null);
                    }
                    catch (Exception /*ex*/)
                    {
                        //Globals.Error.Show("Column: " + col.ToString() + "\r\nContent: " + row[col].ToString() + "\r\n" + ex.Message);
                    }
                }

                Settings.Add(Set);
            }
        }


        public void LoadModules()
        {
            DataTable ModulesDB = Globals.dbConn.GetDataTable("SELECT * FROM PHPModules");

            foreach (DataRow row in ModulesDB.Rows)
            {
                PHPModule Mod = new PHPModule();

                foreach (DataColumn col in ModulesDB.Columns)
                {
                    try
                    {
                        PropertyInfo pi = Mod.GetType().GetProperty(col.ToString());
                        pi.SetValue(Mod, Convert.ChangeType(row[col], pi.PropertyType), null);
                    }
                    catch (Exception /*ex*/)
                    {
                        //Globals.Error.Show("Column: " + col.ToString() + "\r\nContent: " + row[col].ToString() + "\r\n" + ex.Message);
                    }
                }

                Modules.Add(Mod);
            }

            LoadModulesDescription();
        }

        public int FindParam(String Param)
        {
            int i = 0;
            foreach (PHPSetting Set in this.Settings)
            {
                if (Set.Param == Param)
                {
                    return i;
                }
                i++;
            }

            return -1;
        }

        public int FindModule(String Name)
        {
            int i = 0;
            foreach (PHPModule Mod in this.Modules)
            {
                if (Mod.Module == Name)
                {
                    return i;
                }
                i++;
            }

            return -1;
        }

        public int SaveSetting(String Param)
        {
            return this.SaveSetting(FindParam(Param));
        }

        public int SaveSetting(int index, int DoWhat = 2)
        {
            String sql = "";
            switch (DoWhat)
            {
                case 1:
                    sql = "INSERT INTO PHPSettings ('Active', 'Param', 'Value') VALUES (" + this.Settings[index].Active.ToString() + ", '" + this.Settings[index].Param + "', '" + this.Settings[index].Value + "')";
                    break;
                case 2:
                    if (this.Settings.Count > 0 && index >= 0 && this.Settings[index].Param != "")
                    {
                        sql = "UPDATE PHPSettings SET Value = '" + this.Settings[index].Value + "', Active = " + this.Settings[index].Active.ToString() + " WHERE Param LIKE '" + this.Settings[index].Param + "'";
                    }
                    break;
                case 3:
                    if (this.Settings.Count > 0 && index >= 0 && this.Settings[index].Param != "")
                    {
                        sql = "DELETE FROM PHPSettings WHERE Param = '" + this.Settings[index].Param + "'";
                    }
                    break;
                default:
                    return -1;
            }

            try
            {
                return Globals.dbConn.ExecuteNonQuery(sql);
            }
            catch (Exception ex)
            {
                Globals.Error.Show(ex.Message);
                return 0;
            }
        }

        public int SaveModuleData(int index, int DoWhat = 2)
        {
            if (!String.IsNullOrEmpty(this.Modules[index].Module))
            {
                String sql = "";
                switch (DoWhat)
                {
                    case 1:
                        sql = "INSERT INTO PHPModules ('Module', 'Value') VALUES ('" + this.Modules[index].Module + "', '" + this.Modules[index].Value + "')";
                        break;
                    case 2:
                        sql = "UPDATE PHPModules SET Value = '" + this.Modules[index].Value + "' WHERE Module LIKE '" + this.Modules[index].Module + "'";
                        break;
                    case 3:
                        sql = "DELETE FROM PHPModules WHERE Module = '" + this.Modules[index].Module + "'";
                        break;
                    default:
                        return -1;
                }

                try
                {
                    return Globals.dbConn.ExecuteNonQuery(sql);
                }
                catch (Exception ex)
                {
                    Globals.Error.Show(ex.Message);
                }
            }

            return 0;
        }

        public void ClearConfig()
        {
            String[] SQLArray = {
                           "DELETE FROM ActiveModules  WHERE ServerType LIKE 'PHP'",
                           "UPDATE PHPSettings SET Active = 0",
                           "DELETE FROM PHPModules"
                       };

            try
            {
                Globals.dbConn.ExecuteNonQueryBulk(SQLArray);
            }
            catch (Exception e)
            {
                Globals.Error.Show(e.Message);
            }
        }

        public void SetDefaultConfig()
        {
            String ConfigFile = Globals.AppFolder + @"extras\default\PHP.txt";
            if (!System.IO.File.Exists(ConfigFile))
            {
                Globals.Error.Show("The default config data file for this server doesn't exist.\r\nCan't set a new configuration.");
                return;
            }

            ClearConfig();
            String SQLBulk = System.IO.File.ReadAllText(ConfigFile);
            try
            {
                Globals.dbConn.ExecuteNonQuery(SQLBulk);
            }
            catch (Exception e)
            {
                Globals.Error.Show(e.Message);
            }
        }
    }
}
