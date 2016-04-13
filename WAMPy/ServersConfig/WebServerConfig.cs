using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace WDS
{
    public class VirtualHost
    {
        public int VHostID { get; set; }
        public String IP { get; set; }
        public String Port { get; set; }
        public String ServerName { get; set; }
        public String ServerAlias { get; set; }
        public String DocumentRoot { get; set; }
        public String Others { get; set; }

        public List<Aliases> Alias = new List<Aliases>();
        public List<Directories> Directory = new List<Directories>();
    }

    public class Aliases
    {
        public int AliasID { get; set; }
        public int VHostID { get; set; }
        public String AliasName { get; set; }
        public String AliasFolder { get; set; }
    }

    public class Directories
    {
        public int DirectoryID { get; set; }
        public int VHostID { get; set; }
        public String DirectoryName { get; set; }
        public bool SymLinks { get; set; }
        public bool Includes { get; set; }
        public bool Indexes { get; set; }
        public bool MultiViews { get; set; }
        public bool AllowOverride { get; set; }
        public String Others { get; set; }
    }

    public class ConfigModules
    {
        public String Module { get; set; }
        public int IfModuleActive { get; set; }
        public String IfModule { get; set; }
    }

    public class CustomSetting
    {
        public int Id { get; set; }
        public String Param { get; set; }
    }

    class WebServerConfig : CommonServerConfig
    {
        public void LoadVirtualHosts()
        {
            Globals.VHosts.Clear();
            
            // Load Virtual Hosts info from database.
            DataTable DBVHosts = Globals.dbConn.GetDataTable("SELECT * FROM VirtualHosts");
            
            foreach (DataRow rowVH in DBVHosts.Rows)
            {
                VirtualHost VH = new VirtualHost();

                foreach (DataColumn colVH in DBVHosts.Columns)
                {
                    try
                    {
                        PropertyInfo pi = VH.GetType().GetProperty(colVH.ToString());
                        pi.SetValue(VH, Convert.ChangeType(rowVH[colVH], pi.PropertyType), null);
                        //Object collection = pi.GetValue(VH, null);
                    }
                    catch (Exception /*ex*/)
                    {
                        //Globals.Error.Show("Column: " + colVH.ToString() + "\r\nContent: " + rowVH[colVH].ToString() + "\r\n" + ex.Message);
                    }
                }

                // Load Aliases info for each Virtual Host from database.
                DataTable DBAliases = Globals.dbConn.GetDataTable("SELECT * FROM Aliases WHERE VHostID = " + rowVH["VHostID"].ToString());

                foreach (DataRow rowAl in DBAliases.Rows)
                {
                    Aliases Alias = new Aliases();
                    foreach (DataColumn colAl in DBAliases.Columns)
                    {
                        try
                        {
                            PropertyInfo pi = Alias.GetType().GetProperty(colAl.ToString());
                            pi.SetValue(Alias, Convert.ChangeType(rowAl[colAl], pi.PropertyType), null);
                        }
                        catch (Exception)
                        {
                        }
                    }

                    VH.Alias.Add(Alias);
                }

                // Load Directories info for each Virtual Host from database.
                DataTable DBDirectories = Globals.dbConn.GetDataTable("SELECT * FROM Directories WHERE VHostID = " + rowVH["VHostID"].ToString());

                foreach (DataRow rowDir in DBDirectories.Rows)
                {
                    Directories Directory = new Directories();
                    foreach (DataColumn colDir in DBDirectories.Columns)
                    {
                        try
                        {
                            PropertyInfo pi = Directory.GetType().GetProperty(colDir.ToString());
                            pi.SetValue(Directory, Convert.ChangeType(rowDir[colDir], pi.PropertyType), null);
                        }
                        catch (Exception)
                        {
                        }
                    }

                    VH.Directory.Add(Directory);
                }

                Globals.VHosts.Add(VH);
            }
        }

        /* Add/Remove VirtualHosts/Aliases/Directories */
        public int AddUpdateVirtualHost(VirtualHost VH)
        {
            if (VH.VHostID > 0)
            {
                try
                {
                    return Globals.dbConn.ExecuteNonQuery("UPDATE VirtualHosts SET " +
                        "DocumentRoot = '" + VH.DocumentRoot + "', " +
                        "ServerName = '" + VH.ServerName + "', " +
                        "ServerAlias = '" + VH.ServerAlias + "', " +
                        "IP = '" + VH.IP + "', " +
                        "Port = " + VH.Port + ", " +
                        "Others = '" + VH.Others + "' " +
                        "WHERE VHostID = " + VH.VHostID);
                }
                catch(Exception /*ex*/)
                {
                    //Globals.Error.Show(ex.Message);
                    return -1;
                }
            }
            else
            {
                Globals.dbConn.ExecuteNonQuery("INSERT INTO VirtualHosts ('ServerName', 'IP', 'Port', 'DocumentRoot') VALUES ('" + VH.ServerName + "', '" + VH.IP + "', " + VH.Port + ", '" + VH.DocumentRoot + "')");
                return Convert.ToInt32(Globals.dbConn.ExecuteScalar("SELECT MAX(VHostID) from VirtualHosts"));
            }
        }

        public void DeleteVirtualHost(int VHostID)
        {
            Globals.dbConn.ExecuteNonQuery("DELETE FROM VirtualHosts WHERE VHostID = " + VHostID);
            Globals.dbConn.ExecuteNonQuery("DELETE FROM Aliases WHERE VHostID = " + VHostID);
            Globals.dbConn.ExecuteNonQuery("DELETE FROM Directories WHERE VHostID = " + VHostID);
        }

        public int AddUpdateAlias(Aliases Al)
        {
            if (Al.AliasID > 0)
            {
                try
                {
                    return Globals.dbConn.ExecuteNonQuery("UPDATE Aliases SET AliasFolder = '" + Al.AliasFolder + "', AliasName = '" + Al.AliasName + "' WHERE AliasID = " + Al.AliasID);
                }
                catch (Exception /*ex*/)
                {
                    //Globals.Error.Show(ex.Message);
                    return -1;
                }
            }
            else
            {
                Globals.dbConn.ExecuteNonQuery("INSERT INTO Aliases ('AliasFolder', 'AliasName', 'VHostID') VALUES ('" + Al.AliasFolder + "', '" + Al.AliasName + "', " + Al.VHostID + ")");
                return Convert.ToInt32(Globals.dbConn.ExecuteScalar("SELECT MAX(AliasID) from Aliases"));
            }
        }

        public void DeleteAlias(Aliases Al)
        {
            Globals.dbConn.ExecuteNonQuery("DELETE FROM Aliases WHERE AliasID = " + Al.AliasID);
        }

        public int AddUpdateDirectory(Directories Dir)
        {
            if (Dir.DirectoryID > 0)
            {
                try
                {
                    return Globals.dbConn.ExecuteNonQuery("UPDATE Directories SET " +
                        "VHostID = " + Dir.VHostID + ", " +
                        "Others = '" + Dir.Others + "', " +
                        "AllowOverride = " + ((Dir.AllowOverride == true) ? "1" : "0") + ", " +
                        "MultiViews = " + ((Dir.MultiViews==true) ? "1" : "0") + ", " +
                        "Indexes = " + ((Dir.Indexes == true) ? "1" : "0") + ", " +
                        "Includes = " + ((Dir.Includes == true) ? "1" : "0") + ", " +
                        "SymLinks = " + ((Dir.SymLinks == true) ? "1" : "0") + ", " +
                        "DirectoryName = '" + Dir.DirectoryName + "' " +
                        "WHERE VHostID = " + Dir.VHostID);
                }
                catch (Exception /*ex*/)
                {
                    //Globals.Error.Show(ex.Message);
                    return -1;
                }
            }
            else
            {
                Globals.dbConn.ExecuteNonQuery("INSERT INTO Directories ('VHostID', 'Others', 'AllowOverride', 'MultiViews', 'Indexes', 'Includes', 'SymLinks', 'DirectoryName') VALUES (" +
                        Dir.VHostID + ", " +
                        "'" + Dir.Others + "', " +
                        ((Dir.AllowOverride == true) ? "1" : "0") + ", " +
                        ((Dir.MultiViews == true) ? "1" : "0") + ", " +
                        ((Dir.Indexes == true) ? "1" : "0") + ", " +
                        ((Dir.Includes == true) ? "1" : "0") + ", " +
                        ((Dir.SymLinks == true) ? "1" : "0") + ", " +
                        "'" + Dir.DirectoryName + "'" +
                    ")");

                return Convert.ToInt32(Globals.dbConn.ExecuteScalar("SELECT MAX(DirectoryID) from Directories"));
            }
        }

        public void DeleteDirectory(Directories Dir)
        {
            Globals.dbConn.ExecuteNonQuery("DELETE FROM Directories WHERE DirectoryID = " + Dir.DirectoryID);
        }

        /* WebServer Modules Extra Settings */
        public void LoadModulesSettings()
        {
            DataTable DBSettings = Globals.dbConn.GetDataTable("SELECT * FROM ConfigModules WHERE ServerType LIKE '" + Globals.Servers.WebServer.Type.ToString() + "'");

            if (DBSettings.Rows.Count > 0)
            {
                foreach (DataRow row in DBSettings.Rows)
                {
                    ConfigModules ModCnf = new ConfigModules();
                    foreach (DataColumn col in DBSettings.Columns)
                    {
                        try
                        {
                            PropertyInfo pi = ModCnf.GetType().GetProperty(col.ToString());
                            pi.SetValue(ModCnf, Convert.ChangeType(row[col], pi.PropertyType), null);
                        }
                        catch (Exception)
                        {
                        }
                    }

                    Globals.Servers.WebServer.ModulesConfig.Add(ModCnf);
                }
            }
        }

        public int SaveModulesSettings(ConfigModules mod, int op)
        {
            String sql = "";

            switch (op)
            {
                case 0:
                    sql = "DELETE FROM ConfigModules WHERE Module LIKE '" + mod.Module + "'";
                    break;
                case 1:
                    sql = "UPDATE ConfigModules SET IfModule = '" + mod.IfModule + "', IfModuleActive = " + mod.IfModuleActive + " WHERE Module LIKE '" + mod.Module + "'";
                    break;
                case 2:
                    sql = "INSERT INTO ConfigModules ('IfModule', 'IfModuleActive', 'Module', 'ServerType') VALUES ('" + mod.IfModule + "', " + mod.IfModuleActive + ", '" + mod.Module + "', '" + Globals.Servers.WebServer.Type.ToString() + "')";
                    break;
            }

            try
            {
                return Globals.dbConn.ExecuteNonQuery(sql);
            }
            catch(Exception)
            {
                return 0;
            }
        }
        
        public void LoadCustomSettings(String ServerType)
        {
            DataTable DBSettings = Globals.dbConn.GetDataTable("SELECT Id, Param FROM CustomSettings WHERE ServerType='" + ServerType + "'");

            if (DBSettings.Rows.Count > 0)
            {
                foreach (DataRow row in DBSettings.Rows)
                {
                    CustomSetting CSett = new CustomSetting();
                    foreach (DataColumn col in DBSettings.Columns)
                    {
                        try
                        {
                            PropertyInfo pi = CSett.GetType().GetProperty(col.ToString());
                            pi.SetValue(CSett, Convert.ChangeType(row[col], pi.PropertyType), null);
                        }
                        catch (Exception)
                        {
                        }
                    }

                    Globals.Servers.WebServer.CustomSettings.Add(CSett);
                }
            }
        }

        public int SaveCustomSettings(int op)
        {
            if (op == 2)
            {
                return this.SaveCustomSettings(op, 0, "");
            }
            else
            {
                return -1;
            }
        }
        public int SaveCustomSettings(int op, int Id)
        {
            if (op == 0)
            {
                return this.SaveCustomSettings(op, Id, "");
            }
            else
            {
                return -1;
            }
        }

        public int SaveCustomSettings(int op, int Id, String Param)
        {
            String sql = "";

            switch (op)
            {
                case 0:
                    sql = "DELETE FROM CustomSettings WHERE Id = " + Id;
                    break;
                case 1:
                    sql = "UPDATE CustomSettings SET Param = '" + Param + "' WHERE Id = " + Id;
                    break;
                case 2:
                    sql = "INSERT INTO CustomSettings ('Param', 'ServerType') VALUES ('New Setting', '" + Globals.Servers.WebServer.Type.ToString() + "');";
                    break;
            }

            try
            {
                return Globals.dbConn.ExecuteNonQuery(sql);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public void ClearConfig()
        {
            String[] SQLArray = {
                           "DELETE FROM ActiveModules  WHERE ServerType LIKE 'Apache' OR ServerType LIKE 'nginx' OR ServerType LIKE 'lighttpd' OR ServerType LIKE 'NodeJS' OR ServerType LIKE 'IIS'",
                           "DELETE FROM ConfigModules  WHERE ServerType LIKE 'Apache' OR ServerType LIKE 'nginx' OR ServerType LIKE 'lighttpd' OR ServerType LIKE 'NodeJS' OR ServerType LIKE 'IIS'",
                           "DELETE FROM CustomSettings WHERE ServerType LIKE 'Apache' OR ServerType LIKE 'nginx' OR ServerType LIKE 'lighttpd' OR ServerType LIKE 'NodeJS' OR ServerType LIKE 'IIS'",
                           
                           "DELETE FROM Aliases",
                           "DELETE FROM Directories",
                           "DELETE FROM VirtualHosts"
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
            SetDefaultConfig(Globals.AppFolder + @"www\", false);
        }

        public void SetDefaultConfig(Boolean phpMyAdmin)
        {
            SetDefaultConfig(Globals.AppFolder + @"www\", phpMyAdmin);
        }

        public void SetDefaultConfig(String WebFolder, Boolean phpMyAdmin)
        {
            String ConfigFile = Globals.AppFolder + @"extras\default\WebServer.txt";
            if (!System.IO.File.Exists(ConfigFile))
            {
                Globals.Error.Show("The default config data file for this server doesn't exist.\r\nCan't set a new configuration.");
                return;
            }

            ClearConfig();
            String[] SQLArray;
            if(phpMyAdmin)
            {
                SQLArray = new String[] {
                                    System.IO.File.ReadAllText(ConfigFile),
                                    "INSERT INTO VirtualHosts VALUES('" + WebFolder + "\\','*','',80,'','127.0.0.1',1)",
                                    "INSERT INTO Directories VALUES(1,1,'Order allow,deny\r\nAllow from all',1,1,1,1,1,'" + Globals.AppFolder + @"extras\phpMyAdmin\" + "')",
                                    @"INSERT INTO Aliases VALUES(1,'" + Globals.AppFolder + @"extras\phpMyAdmin\" + "','/phpmyadmin',1)"
                                };
            }
            else
            {
                SQLArray = new String[] {
                                    System.IO.File.ReadAllText(ConfigFile),
                                    "INSERT INTO VirtualHosts VALUES('" + WebFolder + "\\','*','',80,'','127.0.0.1',1)"
                                };
            }

            try
            {
                Globals.dbConn.ExecuteNonQueryBulk(SQLArray);
            }
            catch (Exception e)
            {
                Globals.Error.Show(e.Message);
            }

            LoadVirtualHosts();
        }
    }
}
