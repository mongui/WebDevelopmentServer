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

            // Load Active Modules from database.
            DataTable DBModules = Globals.dbConn.GetDataTable("SELECT Module FROM ActiveModules WHERE ServerType='" + ServerType + "'");

            // Remove their path.
            foreach (DataRow rowMod in DBModules.Rows)
            {
                try
                {
                    ActiveMods.Add(rowMod[0].ToString());
                }
                catch (Exception /*ex*/)
                {
                    //Globals.Error.Show(ex.Message);
                }
            }

            return ActiveMods;
        }

        public int SaveActiveModules(String ServerType, List<String> ActiveMods)
        {
            // Remove all entries of the old server config.
            Globals.dbConn.ExecuteNonQuery("DELETE FROM ActiveModules WHERE ServerType LIKE '" + ServerType + "'");

            // Add new entries.
            List<String> InsertString = new List<String>();

            if (ActiveMods.Count > 0)
            {
                foreach (String module in ActiveMods)
                {
                    InsertString.Add("('" + module + "', '" + ServerType + "')");
                }
                return Globals.dbConn.ExecuteNonQuery("INSERT INTO ActiveModules ('Module', 'ServerType') VALUES " + String.Join(", ", InsertString.ToArray()));
            }
            else
            {
                return 0;
            }
        }

        public String GetModuleDescription(String Module)
        {
            foreach (DataRow row in this.ModulesDescription.Rows)
            {
                if (String.Compare(row["Name"].ToString(), Module, true) == 0)
                {
                    return row["Description"].ToString().Replace("\n", Environment.NewLine);
                }
            }
            return "";
        }

        public String GetModuleURL(String Module)
        {
            foreach (DataRow row in this.ModulesDescription.Rows)
            {
                if (String.Compare(row["Name"].ToString(), Module, true) == 0)
                {
                    return row["URL"].ToString();
                }
            }
            return "";
        }

        public void LoadModulesDescription()
        {
            try
            {
                ModulesDescription = Globals.dbServ.GetDataTable("SELECT * FROM ModulesDescription");
            }
            catch (Exception)
            {
                Globals.Error.Show("'Servers.db' is not a valid database.");
            }
        }
    }
}
