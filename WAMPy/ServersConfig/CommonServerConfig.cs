using System;
using System.Collections.Generic;
using System.Data;

namespace WDS
{
    class CommonServerConfig
    {
        public DataTable ModulesDescription = new DataTable();

        /* Load/Save Modules */
        public List<String> LoadActiveModules(String ServerType)
        {
            List<String> ActiveMods = new List<String>();

            DataTable DBModules = Globals.dbConn.GetDataTable("SELECT Module FROM ActiveModules WHERE ServerType = @p0", ServerType);
            foreach (DataRow rowMod in DBModules.Rows)
            {
                if (rowMod[0] != DBNull.Value)
                {
                    ActiveMods.Add(rowMod[0].ToString());
                }
            }

            return ActiveMods;
        }

        public int SaveActiveModules(String ServerType, List<String> ActiveMods)
        {
            List<String> sqls = new List<String>();
            sqls.Add("DELETE FROM ActiveModules WHERE ServerType = '" + ServerType.Replace("'", "''") + "'");
            foreach (String module in ActiveMods)
            {
                sqls.Add("INSERT INTO ActiveModules (Module, ServerType) VALUES ('" + module.Replace("'", "''") + "', '" + ServerType.Replace("'", "''") + "')");
            }

            // One transaction: either all modules are saved or none.
            try
            {
                return Globals.dbConn.ExecuteNonQueryBulk(sqls.ToArray());
            }
            catch (Exception ex)
            {
                Globals.Error.Show("Unable to save the active modules", ex);
                return 0;
            }
        }

        private DataRow FindDescription(String Module)
        {
            if (!this.ModulesDescription.Columns.Contains("Name"))
            {
                return null;
            }
            foreach (DataRow row in this.ModulesDescription.Rows)
            {
                if (String.Compare(row["Name"].ToString(), Module, true) == 0)
                {
                    return row;
                }
            }
            return null;
        }

        public String GetModuleDescription(String Module)
        {
            DataRow row = FindDescription(Module);
            return (row != null) ? row["Description"].ToString().Replace("\n", Environment.NewLine) : "";
        }

        public String GetModuleURL(String Module)
        {
            DataRow row = FindDescription(Module);
            return (row != null) ? row["URL"].ToString() : "";
        }

        public void LoadModulesDescription()
        {
            if (Globals.dbServ == null)
            {
                return;
            }

            try
            {
                ModulesDescription = Globals.dbServ.GetDataTable("SELECT * FROM ModulesDescription");
            }
            catch (Exception)
            {
                Globals.AddToLog("'Servers.db' is not a valid database.");
            }
        }
    }
}
