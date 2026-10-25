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
        /// <summary>
        /// Copies the columns of a row into the properties with the same name.
        /// NULL text columns become "" so the rest of the code never sees null.
        /// </summary>
        public static T FillFromRow<T>(DataRow row) where T : new()
        {
            T item = new T();
            foreach (DataColumn col in row.Table.Columns)
            {
                PropertyInfo pi = typeof(T).GetProperty(col.ColumnName);
                if (pi == null)
                {
                    continue;
                }

                object value = row[col];
                try
                {
                    if (value == DBNull.Value)
                    {
                        if (pi.PropertyType == typeof(String))
                        {
                            pi.SetValue(item, "", null);
                        }
                        continue;
                    }

                    if (pi.PropertyType == typeof(bool))
                    {
                        pi.SetValue(item, Convert.ToInt64(value) != 0, null);
                    }
                    else
                    {
                        pi.SetValue(item, Convert.ChangeType(value, pi.PropertyType), null);
                    }
                }
                catch (Exception)
                {
                    // Bad value in the database: keep the default.
                }
            }

            // String properties not present in the row are also set to "".
            foreach (PropertyInfo pi in typeof(T).GetProperties())
            {
                if (pi.PropertyType == typeof(String) && pi.CanWrite && pi.GetValue(item, null) == null)
                {
                    pi.SetValue(item, "", null);
                }
            }
            return item;
        }

        public void LoadVirtualHosts()
        {
            Globals.VHosts.Clear();

            DataTable DBVHosts = Globals.dbConn.GetDataTable("SELECT * FROM VirtualHosts ORDER BY VHostID");
            foreach (DataRow rowVH in DBVHosts.Rows)
            {
                VirtualHost VH = FillFromRow<VirtualHost>(rowVH);

                DataTable DBAliases = Globals.dbConn.GetDataTable("SELECT * FROM Aliases WHERE VHostID = @p0", VH.VHostID);
                foreach (DataRow rowAl in DBAliases.Rows)
                {
                    VH.Alias.Add(FillFromRow<Aliases>(rowAl));
                }

                DataTable DBDirectories = Globals.dbConn.GetDataTable("SELECT * FROM Directories WHERE VHostID = @p0", VH.VHostID);
                foreach (DataRow rowDir in DBDirectories.Rows)
                {
                    VH.Directory.Add(FillFromRow<Directories>(rowDir));
                }

                Globals.VHosts.Add(VH);
            }
        }

        /// <summary>
        /// True if the text is a valid TCP port (1-65535).
        /// </summary>
        public static bool IsValidPort(String port)
        {
            int p;
            return Int32.TryParse(port, out p) && p >= 1 && p <= 65535;
        }

        /* Add/Remove VirtualHosts/Aliases/Directories */
        /// <summary>
        /// Saves a virtual host. Returns its id (insert), the updated rows (update) or -1 on error.
        /// </summary>
        public int AddUpdateVirtualHost(VirtualHost VH)
        {
            if (!IsValidPort(VH.Port))
            {
                Globals.Error.Show("'" + VH.Port + "' is not a valid port (1-65535). The virtual server was not saved.");
                return -1;
            }

            try
            {
                if (VH.VHostID > 0)
                {
                    return Globals.dbConn.ExecuteNonQuery(
                        "UPDATE VirtualHosts SET DocumentRoot = @p0, ServerName = @p1, ServerAlias = @p2, IP = @p3, Port = @p4, Others = @p5 WHERE VHostID = @p6",
                        VH.DocumentRoot, VH.ServerName, VH.ServerAlias, VH.IP, Int32.Parse(VH.Port), VH.Others, VH.VHostID);
                }

                return (int)Globals.dbConn.ExecuteInsert(
                    "INSERT INTO VirtualHosts (ServerName, IP, Port, DocumentRoot, ServerAlias, Others) VALUES (@p0, @p1, @p2, @p3, @p4, @p5)",
                    VH.ServerName, VH.IP, Int32.Parse(VH.Port), VH.DocumentRoot, VH.ServerAlias ?? "", VH.Others ?? "");
            }
            catch (Exception ex)
            {
                Globals.Error.Show("Unable to save the virtual server", ex);
                return -1;
            }
        }

        public void DeleteVirtualHost(int VHostID)
        {
            Globals.dbConn.ExecuteNonQuery("DELETE FROM VirtualHosts WHERE VHostID = @p0", VHostID);
            Globals.dbConn.ExecuteNonQuery("DELETE FROM Aliases WHERE VHostID = @p0", VHostID);
            Globals.dbConn.ExecuteNonQuery("DELETE FROM Directories WHERE VHostID = @p0", VHostID);
        }

        public int AddUpdateAlias(Aliases Al)
        {
            try
            {
                if (Al.AliasID > 0)
                {
                    return Globals.dbConn.ExecuteNonQuery("UPDATE Aliases SET AliasFolder = @p0, AliasName = @p1 WHERE AliasID = @p2",
                        Al.AliasFolder, Al.AliasName, Al.AliasID);
                }

                return (int)Globals.dbConn.ExecuteInsert("INSERT INTO Aliases (AliasFolder, AliasName, VHostID) VALUES (@p0, @p1, @p2)",
                    Al.AliasFolder, Al.AliasName, Al.VHostID);
            }
            catch (Exception ex)
            {
                Globals.Error.Show("Unable to save the alias", ex);
                return -1;
            }
        }

        public void DeleteAlias(Aliases Al)
        {
            Globals.dbConn.ExecuteNonQuery("DELETE FROM Aliases WHERE AliasID = @p0", Al.AliasID);
        }

        public int AddUpdateDirectory(Directories Dir)
        {
            try
            {
                if (Dir.DirectoryID > 0)
                {
                    return Globals.dbConn.ExecuteNonQuery(
                        "UPDATE Directories SET VHostID = @p0, Others = @p1, AllowOverride = @p2, MultiViews = @p3, Indexes = @p4, Includes = @p5, SymLinks = @p6, DirectoryName = @p7 WHERE DirectoryID = @p8",
                        Dir.VHostID, Dir.Others ?? "", Dir.AllowOverride ? 1 : 0, Dir.MultiViews ? 1 : 0, Dir.Indexes ? 1 : 0,
                        Dir.Includes ? 1 : 0, Dir.SymLinks ? 1 : 0, Dir.DirectoryName, Dir.DirectoryID);
                }

                return (int)Globals.dbConn.ExecuteInsert(
                    "INSERT INTO Directories (VHostID, Others, AllowOverride, MultiViews, Indexes, Includes, SymLinks, DirectoryName) VALUES (@p0, @p1, @p2, @p3, @p4, @p5, @p6, @p7)",
                    Dir.VHostID, Dir.Others ?? "", Dir.AllowOverride ? 1 : 0, Dir.MultiViews ? 1 : 0, Dir.Indexes ? 1 : 0,
                    Dir.Includes ? 1 : 0, Dir.SymLinks ? 1 : 0, Dir.DirectoryName);
            }
            catch (Exception ex)
            {
                Globals.Error.Show("Unable to save the folder", ex);
                return -1;
            }
        }

        public void DeleteDirectory(Directories Dir)
        {
            Globals.dbConn.ExecuteNonQuery("DELETE FROM Directories WHERE DirectoryID = @p0", Dir.DirectoryID);
        }

        /* WebServer Modules Extra Settings */
        public void LoadModulesSettings()
        {
            DataTable DBSettings = Globals.dbConn.GetDataTable("SELECT * FROM ConfigModules WHERE ServerType = @p0", Globals.Servers.WebServer.Type.ToString());
            foreach (DataRow row in DBSettings.Rows)
            {
                Globals.Servers.WebServer.ModulesConfig.Add(FillFromRow<ConfigModules>(row));
            }
        }

        /// <summary>
        /// op: 0 = delete, 1 = update, 2 = insert.
        /// </summary>
        public int SaveModulesSettings(ConfigModules mod, int op)
        {
            String type = Globals.Servers.WebServer.Type.ToString();
            try
            {
                switch (op)
                {
                    case 0:
                        return Globals.dbConn.ExecuteNonQuery("DELETE FROM ConfigModules WHERE Module = @p0 AND ServerType = @p1", mod.Module, type);
                    case 1:
                        return Globals.dbConn.ExecuteNonQuery("UPDATE ConfigModules SET IfModule = @p0, IfModuleActive = @p1 WHERE Module = @p2 AND ServerType = @p3",
                            mod.IfModule, mod.IfModuleActive, mod.Module, type);
                    case 2:
                        return Globals.dbConn.ExecuteNonQuery("INSERT INTO ConfigModules (IfModule, IfModuleActive, Module, ServerType) VALUES (@p0, @p1, @p2, @p3)",
                            mod.IfModule, mod.IfModuleActive, mod.Module, type);
                    default:
                        return 0;
                }
            }
            catch (Exception ex)
            {
                Globals.LogError("Unable to save the module settings", ex);
                return 0;
            }
        }

        public void LoadCustomSettings(String ServerType)
        {
            DataTable DBSettings = Globals.dbConn.GetDataTable("SELECT Id, Param FROM CustomSettings WHERE ServerType = @p0", ServerType);
            foreach (DataRow row in DBSettings.Rows)
            {
                Globals.Servers.WebServer.CustomSettings.Add(FillFromRow<CustomSetting>(row));
            }
        }

        public int SaveCustomSettings(int op)
        {
            return (op == 2) ? this.SaveCustomSettings(op, 0, "") : -1;
        }

        public int SaveCustomSettings(int op, int Id)
        {
            return (op == 0) ? this.SaveCustomSettings(op, Id, "") : -1;
        }

        /// <summary>
        /// op: 0 = delete, 1 = update, 2 = insert a new empty setting.
        /// </summary>
        public int SaveCustomSettings(int op, int Id, String Param)
        {
            try
            {
                switch (op)
                {
                    case 0:
                        return Globals.dbConn.ExecuteNonQuery("DELETE FROM CustomSettings WHERE Id = @p0", Id);
                    case 1:
                        return Globals.dbConn.ExecuteNonQuery("UPDATE CustomSettings SET Param = @p0 WHERE Id = @p1", Param, Id);
                    case 2:
                        return Globals.dbConn.ExecuteNonQuery("INSERT INTO CustomSettings (Param, ServerType) VALUES ('New Setting', @p0)",
                            Globals.Servers.WebServer.Type.ToString());
                    default:
                        return -1;
                }
            }
            catch (Exception ex)
            {
                Globals.LogError("Unable to save the custom setting", ex);
                return -1;
            }
        }

        public void ClearConfig()
        {
            String[] SQLArray = {
                           "DELETE FROM ActiveModules  WHERE ServerType IN ('Apache', 'nginx', 'lighttpd', 'NodeJS', 'IIS')",
                           "DELETE FROM ConfigModules  WHERE ServerType IN ('Apache', 'nginx', 'lighttpd', 'NodeJS', 'IIS')",
                           "DELETE FROM CustomSettings WHERE ServerType IN ('Apache', 'nginx', 'lighttpd', 'NodeJS', 'IIS')",
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
                Globals.Error.Show("Unexpected error", e);
            }
        }

        public void SetDefaultConfig()
        {
            SetDefaultConfig(Globals.AppFolder + @"www", false);
        }

        public void SetDefaultConfig(Boolean phpMyAdmin)
        {
            SetDefaultConfig(Globals.AppFolder + @"www", phpMyAdmin);
        }

        public void SetDefaultConfig(String WebFolder, Boolean phpMyAdmin)
        {
            // Optional extra settings. Without them the program writes a working
            // configuration by itself (see the *ConfigFile classes).
            String ConfigFile = Globals.AppFolder + @"extras\default\WebServer.txt";

            ClearConfig();

            try
            {
                if (System.IO.File.Exists(ConfigFile))
                {
                    Globals.dbConn.ExecuteNonQuery(System.IO.File.ReadAllText(ConfigFile));
                }

                String docRoot = WebFolder.TrimEnd('\\') + @"\";
                CreateWebFolder(docRoot);
                // Main server: only reachable from this computer by default.
                Globals.dbConn.ExecuteNonQuery(
                    "INSERT INTO VirtualHosts (DocumentRoot, IP, Others, Port, ServerAlias, ServerName, VHostID) VALUES (@p0, '127.0.0.1', '', 80, '', 'localhost', 1)",
                    docRoot);

                if (phpMyAdmin)
                {
                    String pma = Globals.AppFolder + @"extras\phpMyAdmin\";
                    Globals.dbConn.ExecuteNonQuery(
                        "INSERT INTO Directories (DirectoryID, VHostID, Others, AllowOverride, MultiViews, Indexes, Includes, SymLinks, DirectoryName) VALUES (1, 1, @p0, 1, 1, 1, 1, 1, @p1)",
                        "Order allow,deny\r\nAllow from all", pma);
                    Globals.dbConn.ExecuteNonQuery(
                        "INSERT INTO Aliases (AliasID, AliasFolder, AliasName, VHostID) VALUES (1, @p0, '/phpmyadmin', 1)",
                        pma);
                }
            }
            catch (Exception e)
            {
                Globals.Error.Show("Unexpected error", e);
            }

            LoadVirtualHosts();
        }

        /// <summary>
        /// Without any virtual server the web server has nothing to listen on: create
        /// http://localhost/ on AppFolder\www (only reachable from this computer).
        /// </summary>
        public void EnsureDefaultVirtualHost()
        {
            if (Globals.VHosts.Count > 0)
            {
                return;
            }

            String docRoot = Globals.AppFolder + @"www\";
            try
            {
                CreateWebFolder(docRoot);
                Globals.dbConn.ExecuteNonQuery(
                    "INSERT INTO VirtualHosts (DocumentRoot, IP, Others, Port, ServerAlias, ServerName, VHostID) VALUES (@p0, '127.0.0.1', '', 80, '', 'localhost', 1)",
                    docRoot);
                Globals.AddToLog("No virtual server was configured: http://localhost/ now serves " + docRoot);
            }
            catch (Exception e)
            {
                Globals.LogError("Unable to create the default virtual server", e);
            }
            LoadVirtualHosts();
        }

        private static void CreateWebFolder(String docRoot)
        {
            try
            {
                System.IO.Directory.CreateDirectory(docRoot);
                if (System.IO.Directory.GetFileSystemEntries(docRoot).Length == 0)
                {
                    System.IO.File.WriteAllText(docRoot + "index.php", "<?php phpinfo(); ?>\r\n");
                    System.IO.File.WriteAllText(docRoot + "index.html", "<!doctype html><title>It works</title><p>The web server is running.</p>\r\n");
                }
            }
            catch (Exception e)
            {
                Globals.LogError("Unable to create " + docRoot + "", e);
            }
        }
    }
}
