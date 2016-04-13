using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

namespace WDS
{
    public partial class WebServerConfigWindow : Form
    {
        private WebServerConfig Database = new WebServerConfig();
        private int SelVHost = -1;
        private int SelModule = -1;
        private int SelCustom = -1;

        public WebServerConfigWindow()
        {
            InitializeComponent();
        }

        private void WebServerConfig_Load(object sender, EventArgs e)
        {
            String SName = "";
            foreach (VirtualHost vh in Globals.VHosts)
            {
                SName = (vh.ServerName != "") ? vh.ServerName + " - " : "";
                SName += vh.IP + ":" + vh.Port;
                if (vh.VHostID == 1)
                {
                    SName += " - Main Server";
                }

                this.ServerList.Items.Add(SName);
            }
            Globals.Servers.WebServer.AvailableModules = Globals.Servers.LoadAvailableModules(Globals.Servers.WebServer.Type.ToString());

            try
            {
                foreach (String ModuleName in Globals.Servers.WebServer.AvailableModules)
                {
                    this.checkedListModules.Items.Add(ModuleName, Globals.Servers.WebServer.ActiveModules.Contains(ModuleName));
                }
            }
            catch(Exception /*ex*/)
            {
                //Globals.Error.Show(ex.Message);
                this.tabsWebServer.TabPages.RemoveByKey("tabModules");
            }

            try
            {
                foreach (CustomSetting CSet in Globals.Servers.WebServer.CustomSettings)
                {
                    this.CustomList.Items.Add(this.SplitCustomListString(CSet.Param));
                }

                this.tabsWebServer.TabPages["tabCustom"].Text = Globals.Servers.WebServer.Type.ToString() + " Settings";
            }
            catch(Exception /*ex*/)
            {
                //Globals.Error.Show(ex.Message);
                this.tabsWebServer.TabPages.RemoveByKey("tabCustom");
            }

            Database.LoadModulesDescription();
        }

        /*
         * 'ServerList' settings management.
         */

        private void ServerList_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Update the VirtualHost list.
            if (this.SelVHost >= 0 && this.ServerList.SelectedIndex != this.SelVHost)
            {
                this.SaveVHost();

                String UpdateText = (Globals.VHosts[this.SelVHost].ServerName != "") ? Globals.VHosts[this.SelVHost].ServerName + " - " : "";
                UpdateText += Globals.VHosts[this.SelVHost].IP + ":" + Globals.VHosts[this.SelVHost].Port;
                if (Globals.VHosts[this.SelVHost].VHostID == 1)
                {
                    UpdateText += " - Main Server";
                }
                this.ServerList.Items[this.SelVHost] = UpdateText;
            }

            this.RemoveAlias.Enabled = false;
            this.RemoveFolder.Enabled = false;
            this.RemoveServer.Enabled = false;
            if (this.ServerList.SelectedIndex >= 0 && this.SelVHost != this.ServerList.SelectedIndex)
            {
                this.AddAlias.Enabled = true;
                this.AddFolder.Enabled = true;
                if (Globals.VHosts[this.ServerList.SelectedIndex].VHostID != 1)
                {
                    this.RemoveServer.Enabled = true;
                    this.HostIP.Enabled = true;
                    this.ServerAlias.Enabled = true;
                }
                else
                {
                    this.HostIP.Enabled = false;
                    this.ServerAlias.Enabled = false;
                }

                this.HostIP.Text = Globals.VHosts[this.ServerList.SelectedIndex].IP;
                this.Port.Text = Globals.VHosts[this.ServerList.SelectedIndex].Port;
                this.ServerName.Text = Globals.VHosts[this.ServerList.SelectedIndex].ServerName;
                this.ServerAlias.Text = Globals.VHosts[this.ServerList.SelectedIndex].ServerAlias;
                this.DocumentRoot.Text = Globals.VHosts[this.ServerList.SelectedIndex].DocumentRoot;

                this.Alias.Items.Clear();
                foreach (Aliases alias in Globals.VHosts[this.ServerList.SelectedIndex].Alias)
                {
                    this.Alias.Items.Add(alias.AliasName);
                }

                this.Folders.Items.Clear();
                foreach (Directories folder in Globals.VHosts[this.ServerList.SelectedIndex].Directory)
                {
                    this.Folders.Items.Add(folder.DirectoryName);
                }

                this.Others.Text = Globals.VHosts[this.ServerList.SelectedIndex].Others;
            }
            else if (this.ServerList.SelectedIndex < 0)
            {
                this.AddAlias.Enabled = false;
                this.AddFolder.Enabled = false;
            }

            this.SelVHost = this.ServerList.SelectedIndex;
        }

        private void AddServer_Click(object sender, EventArgs e)
        {
            VirtualHost newVHost = new VirtualHost();
            newVHost.ServerName = "New virtual server";
            newVHost.IP = "*";
            newVHost.Port = "80";
            newVHost.DocumentRoot = Globals.AppFolder;

            newVHost.VHostID = this.Database.AddUpdateVirtualHost(newVHost);

            Globals.VHosts.Add(newVHost);

            this.ServerList.Items.Add(newVHost.ServerName);
            this.ServerList.SelectedIndex = this.ServerList.Items.Count - 1;
        }

        private void RemoveServer_Click(object sender, EventArgs e)
        {
            if (this.ServerList.SelectedIndex >= 0)
            {
                this.SelVHost = -1;

                // First, remove it from database.
                int VHostID = Globals.VHosts[this.ServerList.SelectedIndex].VHostID;
                this.Database.DeleteVirtualHost(VHostID);
                
                // Then, remove it from the ServerList variable.
                Globals.VHosts.RemoveAt(this.ServerList.SelectedIndex);
                this.ServerList.Items.RemoveAt(this.ServerList.SelectedIndex);
            }
        }

        /*
         * 'Alias' settings management.
         */
        private void Alias_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.Alias.SelectedIndex >= 0)
            {
                this.RemoveAlias.Enabled = true;
            }
            else
            {
                this.RemoveAlias.Enabled = false;
            }
        }

        private void Alias_DoubleClick(object sender, EventArgs e)
        {
            if (this.ServerList.SelectedIndex >= 0 && this.Alias.SelectedIndex >= 0)
            {
                Parameters Params = new Parameters();
                Params.Title = "Add/edit alias";
                Params.textBox = "Configuration";
                Params.textParameter1 = "Alias name:";
                Params.Parameter1 = Globals.VHosts[this.ServerList.SelectedIndex].Alias[this.Alias.SelectedIndex].AliasName;
                Params.textParameter2 = "Alias folder:";
                Params.Parameter2 = Globals.VHosts[this.ServerList.SelectedIndex].Alias[this.Alias.SelectedIndex].AliasFolder;
                if (Params.ShowDialog() == DialogResult.OK)
                {
                    this.Alias.Items[this.Alias.SelectedIndex] = Params.Parameter1;
                    Globals.VHosts[this.ServerList.SelectedIndex].Alias[this.Alias.SelectedIndex].AliasName = Params.Parameter1;
                    Globals.VHosts[this.ServerList.SelectedIndex].Alias[this.Alias.SelectedIndex].AliasFolder = Params.Parameter2;

                    Database.AddUpdateAlias(Globals.VHosts[this.ServerList.SelectedIndex].Alias[this.Alias.SelectedIndex]);
                }
                Params.Dispose();
            }
        }

        private void AddAlias_Click(object sender, EventArgs e)
        {
            if (this.ServerList.SelectedIndex >= 0)
            {
                Aliases newAlias = new Aliases();
                newAlias.AliasName = "/www_folder";
                newAlias.AliasFolder = Globals.AppFolder;
                newAlias.VHostID = Globals.VHosts[this.ServerList.SelectedIndex].VHostID;

                newAlias.AliasID = Database.AddUpdateAlias(newAlias);
                
                Globals.VHosts[this.ServerList.SelectedIndex].Alias.Add(newAlias);
                this.Alias.Items.Add(newAlias.AliasName);
                this.Alias.SelectedIndex = this.Alias.Items.Count - 1;

                this.Alias_DoubleClick(null, null);
            }
        }

        private void RemoveAlias_Click(object sender, EventArgs e)
        {
            if (this.ServerList.SelectedIndex >= 0 && this.Alias.SelectedIndex >= 0)
            {
                Database.DeleteAlias(Globals.VHosts[this.ServerList.SelectedIndex].Alias[this.Alias.SelectedIndex]);

                Globals.VHosts[this.ServerList.SelectedIndex].Alias.RemoveAt(this.Alias.SelectedIndex);
                this.Alias.Items.RemoveAt(this.Alias.SelectedIndex);
            }
        }

        /*
         * 'Folders' settings management.
         */
        private void Folders_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.Folders.SelectedIndex >= 0)
            {
                this.RemoveFolder.Enabled = true;
            }
            else
            {
                this.RemoveFolder.Enabled = false;
            }
        }
        
        private void Folders_DoubleClick(object sender, EventArgs e)
        {
            if (this.ServerList.SelectedIndex >= 0 && this.Folders.SelectedIndex >= 0)
            {
                Parameters Params = new Parameters();
                Params.Title = "Add/edit folder";
                Params.textBox = "Configuration";
                Params.textParameter1 = "Folder path:";
                Params.Parameter1 = this.Folders.GetItemText(this.Folders.SelectedItem);
                Params.textParameter2 = "More options:";
                Params.Parameter2 = Globals.VHosts[this.ServerList.SelectedIndex].Directory[this.Folders.SelectedIndex].Others;

                Params.SymLinks = Globals.VHosts[this.ServerList.SelectedIndex].Directory[this.Folders.SelectedIndex].SymLinks;
                Params.Includes = Globals.VHosts[this.ServerList.SelectedIndex].Directory[this.Folders.SelectedIndex].Includes;
                Params.Indexes = Globals.VHosts[this.ServerList.SelectedIndex].Directory[this.Folders.SelectedIndex].Indexes;
                Params.MultiViews = Globals.VHosts[this.ServerList.SelectedIndex].Directory[this.Folders.SelectedIndex].MultiViews;
                Params.AllowOverride = Globals.VHosts[this.ServerList.SelectedIndex].Directory[this.Folders.SelectedIndex].AllowOverride;

                Params.Folders = true;

                if (Params.ShowDialog() == DialogResult.OK)
                {
                    this.Folders.Items[this.Folders.SelectedIndex] = Params.Parameter1;
                    Globals.VHosts[this.ServerList.SelectedIndex].Directory[this.Folders.SelectedIndex].DirectoryName = Params.Parameter1;
                    Globals.VHosts[this.ServerList.SelectedIndex].Directory[this.Folders.SelectedIndex].Others = Params.Parameter2;
                    Globals.VHosts[this.ServerList.SelectedIndex].Directory[this.Folders.SelectedIndex].SymLinks = Params.SymLinks;
                    Globals.VHosts[this.ServerList.SelectedIndex].Directory[this.Folders.SelectedIndex].Includes = Params.Includes;
                    Globals.VHosts[this.ServerList.SelectedIndex].Directory[this.Folders.SelectedIndex].Indexes = Params.Indexes;
                    Globals.VHosts[this.ServerList.SelectedIndex].Directory[this.Folders.SelectedIndex].MultiViews = Params.MultiViews;
                    Globals.VHosts[this.ServerList.SelectedIndex].Directory[this.Folders.SelectedIndex].AllowOverride = Params.AllowOverride;

                    Database.AddUpdateDirectory(Globals.VHosts[this.ServerList.SelectedIndex].Directory[this.Folders.SelectedIndex]);
                }
                Params.Dispose();
            }
        }

        private void AddFolder_Click(object sender, EventArgs e)
        {
            if (this.ServerList.SelectedIndex >= 0)
            {
                Directories newFolder = new Directories();
                newFolder.DirectoryName = Globals.AppFolder;
                newFolder.VHostID = Globals.VHosts[this.ServerList.SelectedIndex].VHostID;

                newFolder.DirectoryID = Database.AddUpdateDirectory(newFolder);

                Globals.VHosts[this.ServerList.SelectedIndex].Directory.Add(newFolder);
                this.Folders.Items.Add(newFolder.DirectoryName);
                this.Folders.SelectedIndex = this.Folders.Items.Count - 1;

                this.Folders_DoubleClick(null, null);
            }
        }

        private void RemoveFolder_Click(object sender, EventArgs e)
        {
            if (this.ServerList.SelectedIndex >= 0 && this.Folders.SelectedIndex >= 0)
            {
                Database.DeleteDirectory(Globals.VHosts[this.ServerList.SelectedIndex].Directory[this.Folders.SelectedIndex]);

                Globals.VHosts[this.ServerList.SelectedIndex].Directory.RemoveAt(this.Folders.SelectedIndex);
                this.Folders.Items.RemoveAt(this.Folders.SelectedIndex);
            }
        }

        private void WebServerConfigWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.SelVHost >= 0 )
            {
                this.SaveVHost();
            }

            try
            {
                if (Globals.Servers.WebServer.AvailableModules != null)
                {
                    Globals.Servers.WebServer.ActiveModules.Clear();
                    foreach (var item in this.checkedListModules.CheckedItems)
                    {
                        Globals.Servers.WebServer.ActiveModules.Add(item.ToString());
                    }

                    this.Database.SaveActiveModules(Globals.Servers.WebServer.Type.ToString(), Globals.Servers.WebServer.ActiveModules);
                    if (SelModule >= 0)
                    {
                        this.SaveModuleConf();
                    }
                }
            }
            catch(Exception ex)
            {
                Globals.Error.Show(ex.Message);
            }
        }

        private void SaveVHost()
        {
            Globals.VHosts[this.SelVHost].ServerName = this.ServerName.Text.Trim();
            Globals.VHosts[this.SelVHost].IP = this.HostIP.Text.Trim();
            Globals.VHosts[this.SelVHost].Port = this.Port.Text.Trim();
            Globals.VHosts[this.SelVHost].ServerAlias = this.ServerAlias.Text.Trim();
            Globals.VHosts[this.SelVHost].DocumentRoot = this.DocumentRoot.Text.Trim();
            Globals.VHosts[this.SelVHost].Others = this.Others.Text.Trim();
                
            this.Database.AddUpdateVirtualHost(Globals.VHosts[this.SelVHost]);
        }

        private void checkedListModules_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.SelModule >= 0)
            {
                this.SaveModuleConf();
            }
            this.SelModule = this.checkedListModules.SelectedIndex;

            String Module = this.checkedListModules.SelectedItem.ToString();
            bool exists = false;

            foreach(ConfigModules Mdl in Globals.Servers.WebServer.ModulesConfig)
            {
                if (Mdl.Module == Module)
                {
                    exists = true;
                    string[] lines = Mdl.IfModule.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
                    this.ExtraConfig.Lines = lines;

                    switch(Mdl.IfModuleActive)
                    {
                        case 0:
                            this.ModuleConfDisabled.Checked = true;
                            break;
                        case 1:
                            this.ModuleConfIfActive.Checked = true;
                            break;
                        case 2:
                            this.ModuleConfIfNotActive.Checked = true;
                            break;
                    }
                }
            }

            if (exists == false)
            {
                    this.ExtraConfig.Text = "";
                    this.ModuleConfDisabled.Checked = true;
            }


            if (this.checkedListModules.SelectedIndex >= 0)
            {
                this.ModulesInfo.Text = Database.GetModuleDescription(this.checkedListModules.SelectedItem.ToString());

                String URL = Database.GetModuleURL(this.checkedListModules.SelectedItem.ToString());

                LinkLabel.Link link = new LinkLabel.Link();
                link.LinkData = URL;

                ModulesLink.Text = URL;
                ModulesLink.Links.Clear();
                ModulesLink.Links.Add(link);
            }
        }

        private void SaveModuleConf()
        {
            String Module = this.checkedListModules.Items[this.SelModule].ToString();
            int exists = -1;

            for (int i = 0; i < Globals.Servers.WebServer.ModulesConfig.Count; i++)
            {
                if (Globals.Servers.WebServer.ModulesConfig[i].Module == Module)
                {
                    exists = i;
                }
            }

            String lines = string.Join("\r\n", this.ExtraConfig.Lines);

            ConfigModules thisMod = new ConfigModules();

            thisMod.IfModule = lines;
            thisMod.Module = Module;
            if (this.ModuleConfDisabled.Checked == true)
                thisMod.IfModuleActive = 0;
            else if (this.ModuleConfIfActive.Checked == true)
                thisMod.IfModuleActive = 1;
            else if (this.ModuleConfIfNotActive.Checked == true)
                thisMod.IfModuleActive = 2;
            else
                thisMod.IfModuleActive = 0;

            if (exists >= 0)
            {
                if (lines.Trim() == "")
                {
                    // DELETE
                    Globals.Servers.WebServer.ModulesConfig.RemoveAt(exists);
                    this.Database.SaveModulesSettings(thisMod, 0);
                }
                else
                {
                    // UPDATE
                    Globals.Servers.WebServer.ModulesConfig[exists] = thisMod;
                    this.Database.SaveModulesSettings(thisMod, 1);
                }
            }
            else if (lines.Trim() != "")
            {
                // INSERT
                Globals.Servers.WebServer.ModulesConfig.Add(thisMod);
                this.Database.SaveModulesSettings(thisMod, 2);
            }
        }

        private void ModuleConfIfActive_CheckedChanged(object sender, EventArgs e)
        {
            if (this.ModuleConfIfActive.Checked == true)
            {
                this.ExtraConfig.ReadOnly = false;
            }
        }

        private void ModuleConfDisabled_CheckedChanged(object sender, EventArgs e)
        {
            if (this.ModuleConfDisabled.Checked == true)
            {
                this.ExtraConfig.ReadOnly = true;
            }
        }

        private void ModuleConfIfNotActive_CheckedChanged(object sender, EventArgs e)
        {
            if (this.ModuleConfIfNotActive.Checked == true)
            {
                this.ExtraConfig.ReadOnly = false;
            }
        }

        private string SplitCustomListString(String str)
        {
            int len = str.IndexOf("\n");
            if (len > 0 && len < 30)
            {
                return str.Substring(0, len);
            }
            else if (str.Length >= 30)
            {
                return str.Substring(0, 30) + "...";
            }
            else
            {
                return str;
            }
        }

        private void CustomList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.SelCustom >= 0)
            {
                Globals.Servers.WebServer.CustomSettings[this.SelCustom].Param = string.Join(Environment.NewLine, this.CustomEdit.Lines);
                this.Database.SaveCustomSettings(1, Globals.Servers.WebServer.CustomSettings[this.SelCustom].Id, this.CustomEdit.Text);
                this.CustomList.Items[this.SelCustom] = this.SplitCustomListString(this.CustomEdit.Lines[0]);
            }

            this.SelCustom = this.CustomList.SelectedIndex;

            if (this.SelCustom >= 0)
            {
                this.CustomEdit.Text = Globals.Servers.WebServer.CustomSettings[this.SelCustom].Param;
                this.CustomEdit.Enabled = true;
                this.RemoveCustom.Enabled = true;
            }
            else
            {
                this.RemoveCustom.Enabled = false;
                this.CustomEdit.Enabled = false;
            }
        }

        private void AddCustom_Click(object sender, EventArgs e)
        {
            this.Database.SaveCustomSettings(2);

            this.CustomList.Items.Clear();
            Globals.Servers.WebServer.CustomSettings.Clear();

            this.Database.LoadCustomSettings(Globals.Servers.WebServer.Type.ToString());

            foreach (CustomSetting CSet in Globals.Servers.WebServer.CustomSettings)
            {
                this.CustomList.Items.Add(this.SplitCustomListString(CSet.Param));
            }
        }

        private void RemoveCustom_Click(object sender, EventArgs e)
        {
            int rtrn = this.Database.SaveCustomSettings(0, Globals.Servers.WebServer.CustomSettings[this.SelCustom].Id);
            if (rtrn >= 0)
            {
                this.CustomList.Items.Clear();
                Globals.Servers.WebServer.CustomSettings.Clear();

                this.Database.LoadCustomSettings(Globals.Servers.WebServer.Type.ToString());

                foreach (CustomSetting CSet in Globals.Servers.WebServer.CustomSettings)
                {
                    this.CustomList.Items.Add(this.SplitCustomListString(CSet.Param));
                }
            }
        }

        private void SelectFolder_Click(object sender, EventArgs e)
        {
            try
            {
                using (FolderBrowserDialog FolderDialog = new FolderBrowserDialog())
                {
                    FolderDialog.Description = "Open a folder";
                    FolderDialog.ShowNewFolderButton = true;
                    FolderDialog.RootFolder = Environment.SpecialFolder.MyComputer;
                    if (FolderDialog.ShowDialog() == DialogResult.OK && this.DocumentRoot.Enabled == true)
                    {
                        this.DocumentRoot.Text = FolderDialog.SelectedPath;
                    }
                }
            }
            catch (Exception)
            {
            }
        }
    }
}
