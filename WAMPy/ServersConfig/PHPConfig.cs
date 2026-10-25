using System;
using System.Collections.Generic;
using System.Data;

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
                Settings.Add(WebServerConfig.FillFromRow<PHPSetting>(row));
            }
        }

        public void LoadModules()
        {
            DataTable ModulesDB = Globals.dbConn.GetDataTable("SELECT * FROM PHPModules");
            foreach (DataRow row in ModulesDB.Rows)
            {
                Modules.Add(WebServerConfig.FillFromRow<PHPModule>(row));
            }

            LoadModulesDescription();
        }

        public int FindParam(String Param)
        {
            return this.Settings.FindIndex(s => s.Param == Param);
        }

        public int FindModule(String Name)
        {
            return this.Modules.FindIndex(m => m.Module == Name);
        }

        public int SaveSetting(String Param)
        {
            return this.SaveSetting(FindParam(Param));
        }

        /// <summary>
        /// DoWhat: 1 = insert, 2 = update, 3 = delete.
        /// </summary>
        public int SaveSetting(int index, int DoWhat = 2)
        {
            if (index < 0 || index >= this.Settings.Count || String.IsNullOrEmpty(this.Settings[index].Param))
            {
                return -1;
            }

            PHPSetting s = this.Settings[index];
            try
            {
                switch (DoWhat)
                {
                    case 1:
                        return Globals.dbConn.ExecuteNonQuery("INSERT INTO PHPSettings (Active, Param, Value) VALUES (@p0, @p1, @p2)", s.Active, s.Param, s.Value);
                    case 2:
                        return Globals.dbConn.ExecuteNonQuery("UPDATE PHPSettings SET Value = @p0, Active = @p1 WHERE Param = @p2", s.Value, s.Active, s.Param);
                    case 3:
                        return Globals.dbConn.ExecuteNonQuery("DELETE FROM PHPSettings WHERE Param = @p0", s.Param);
                    default:
                        return -1;
                }
            }
            catch (Exception ex)
            {
                Globals.Error.Show("Unexpected error", ex);
                return 0;
            }
        }

        /// <summary>
        /// DoWhat: 1 = insert, 2 = update, 3 = delete.
        /// </summary>
        public int SaveModuleData(int index, int DoWhat = 2)
        {
            if (index < 0 || index >= this.Modules.Count || String.IsNullOrEmpty(this.Modules[index].Module))
            {
                return 0;
            }

            PHPModule m = this.Modules[index];
            try
            {
                switch (DoWhat)
                {
                    case 1:
                        return Globals.dbConn.ExecuteNonQuery("INSERT INTO PHPModules (Module, Value) VALUES (@p0, @p1)", m.Module, m.Value);
                    case 2:
                        return Globals.dbConn.ExecuteNonQuery("UPDATE PHPModules SET Value = @p0 WHERE Module = @p1", m.Value, m.Module);
                    case 3:
                        return Globals.dbConn.ExecuteNonQuery("DELETE FROM PHPModules WHERE Module = @p0", m.Module);
                    default:
                        return -1;
                }
            }
            catch (Exception ex)
            {
                Globals.Error.Show("Unexpected error", ex);
                return 0;
            }
        }

        public void ClearConfig()
        {
            String[] SQLArray = {
                           "DELETE FROM ActiveModules WHERE ServerType = 'PHP'",
                           "UPDATE PHPSettings SET Active = 0",
                           "DELETE FROM PHPModules"
                       };

            try
            {
                Globals.dbConn.ExecuteNonQueryBulk(SQLArray);
            }
            catch (Exception e)
            {
                Globals.Error.Show("Unexpected error", e);
            }
        }

        public void SetDefaultConfig()
        {
            // Optional extra settings; php.ini gets the required ones anyway (PHPConfigFile).
            String ConfigFile = Globals.AppFolder + @"extras\default\PHP.txt";

            ClearConfig();
            if (!System.IO.File.Exists(ConfigFile))
            {
                return;
            }
            try
            {
                Globals.dbConn.ExecuteNonQuery(System.IO.File.ReadAllText(ConfigFile));
            }
            catch (Exception e)
            {
                Globals.Error.Show("Unexpected error", e);
            }
        }
    }
}
