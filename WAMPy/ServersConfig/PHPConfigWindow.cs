using System;
using System.Windows.Forms;

namespace WDS
{
    public partial class PHPConfigWindow : Form
    {
        private WebServerConfig Database = new WebServerConfig();
        private int SelSetting = -1;
        private int SelModule = -1;
        private String SelParam;
        private String SelModName;
        PHPConfig PHPSettings = new PHPConfig();

        public PHPConfigWindow()
        {
            InitializeComponent();
        }

        private void PHPConfigWindow_Load(object sender, EventArgs e)
        {
            try
            {
                Globals.Servers.PHP.AvailableModules = Globals.Servers.LoadAvailableModules("PHP");

                foreach (String ModuleName in Globals.Servers.PHP.AvailableModules)
                {
                    this.checkedListModules.Items.Add(ModuleName, Globals.Servers.PHP.ActiveModules.Contains(ModuleName));
                }
            }
            catch (Exception /*ex*/)
            {
                //Globals.Error.Show(ex.Message);
                this.tabPHP.TabPages.RemoveByKey("tabExtensions");
            }

            this.PHPSettings.LoadSettings();
            if (this.PHPSettings.Settings.Count > 0)
            {
                foreach (PHPSetting Setting in this.PHPSettings.Settings)
                {
                    this.checkedListSettings.Items.Add(Setting.Param, ((Setting.Active == 1) ? true : false));
                }
            }

            this.PHPSettings.LoadModules();
        }

        private void PHPConfigWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            Globals.Servers.PHP.ActiveModules.Clear();
            foreach (var item in this.checkedListModules.CheckedItems)
            {
                Globals.Servers.PHP.ActiveModules.Add(item.ToString());
            }
            this.Database.SaveActiveModules("PHP", Globals.Servers.PHP.ActiveModules);

            // Faltan guardar todos los cambios hechos en PHP Settings.
            this.checkedListSettings_SelectedIndexChanged(null, null);
            if (SelSetting >= 0)
            {
                this.PHPSettings.SaveSetting(SelSetting);
            }
            if (SelModule >= 0)
            {
                this.checkedListModules_SelectedIndexChanged(null, null);
            }
        }

        private void checkedListSettings_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (SelSetting >= 0)
            {
                int index = -1;
                if (this.checkedListSettings.Items[SelSetting].ToString() == SelParam)
                {
                    index = SelSetting;
                }
                else
                {
                    index = this.PHPSettings.FindParam(SelParam);
                }

                if (index >= 0)
                {
                    this.PHPSettings.Settings[index].Value = this.PHPSettingValue.Text;
                    this.PHPSettings.Settings[index].Active = ((this.checkedListSettings.GetItemCheckState(index) == CheckState.Checked) ? 1 : 0);
                    this.PHPSettings.SaveSetting(index);
                }
            }

            if (SelSetting != this.checkedListSettings.SelectedIndex)
            {
                SelSetting = this.checkedListSettings.SelectedIndex;
                SelParam = this.checkedListSettings.SelectedItem.ToString();
                this.PHPSettingValue.Text = this.PHPSettings.Settings[SelSetting].Value;

                if (this.PHPSettings.Settings[SelSetting].Active == 1)
                {
                    this.PHPSettingValue.Enabled = true;
                }
                else
                {
                    this.PHPSettingValue.Enabled = false;
                }
            }

            if (this.checkedListSettings.SelectedIndex >= 0)
            {
                this.RemoveSetting.Enabled = true;
            }
            else
            {
                this.RemoveSetting.Enabled = false;
            }
        }

        private void checkedListSettings_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (this.checkedListSettings.SelectedIndex >= 0)
            {
                this.PHPSettingValue.Enabled = !this.checkedListSettings.GetItemChecked(this.checkedListSettings.SelectedIndex);
            }
        }

        private void AddSetting_Click(object sender, EventArgs e)
        {
            Parameters Params = new Parameters();
            Params.Title = "PHP general settings";
            Params.textBox = "New";
            Params.textParameter1 = "New PHP general setting:";
            Params.textParameter2 = "New PHP value:";

            if (Params.ShowDialog() == DialogResult.OK)
            {
                PHPSetting newPHPSetting = new PHPSetting();
                newPHPSetting.Param = Params.Parameter1;
                newPHPSetting.Value = Params.Parameter2;
                newPHPSetting.Active = 1;

                this.PHPSettings.Settings.Add(newPHPSetting);
                this.checkedListSettings.Items.Add(newPHPSetting.Param, true);
                this.PHPSettings.SaveSetting(this.checkedListSettings.Items.Count - 1);
                this.PHPSettings.SaveSetting(this.checkedListSettings.Items.Count - 1, 1);
            }
            Params.Dispose();
        }

        private void RemoveSetting_Click(object sender, EventArgs e)
        {
            int selIndex = this.checkedListSettings.SelectedIndex;
            if (selIndex >= 0)
            {
                SelSetting = -1;
                this.PHPSettingValue.Text = "";
                this.PHPSettingValue.Enabled = false;

                // First, remove it from database.
                this.PHPSettings.SaveSetting(selIndex, 3);

                // Then, remove it from the ServerList variable.
                this.PHPSettings.Settings.RemoveAt(selIndex);
                this.checkedListSettings.Items.RemoveAt(selIndex);
            }
        }

        private void checkedListModules_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = -1;
            if (!String.IsNullOrEmpty(SelModName) && SelModName != "" && SelModName != this.checkedListModules.SelectedItem.ToString())
            {
                index = this.PHPSettings.FindModule(SelModName);
                if (index >= 0)
                {
                    if (this.ModulesConfig.Text != "")
                    {
                        this.PHPSettings.Modules[index].Value = this.ModulesConfig.Text;
                        this.PHPSettings.SaveModuleData(index);
                    }
                    else
                    {
                        this.PHPSettings.SaveModuleData(index, 3);
                        this.PHPSettings.Modules.RemoveAt(index);
                    }
                }
                else if (this.ModulesConfig.Text != "")
                {
                    PHPModule NewMod = new PHPModule();
                    NewMod.Module = SelModName;
                    NewMod.Value = this.ModulesConfig.Text;
                    this.PHPSettings.Modules.Add(NewMod);
                    this.PHPSettings.SaveModuleData(this.PHPSettings.Modules.Count-1, 1);
                }
            }

            // Ahora se cargan los nuevos datos.
            if (SelModule != this.checkedListModules.SelectedIndex)
            {
                SelModule = this.checkedListModules.SelectedIndex;
                SelModName = this.checkedListModules.SelectedItem.ToString();

                index = this.PHPSettings.FindModule(SelModName);

                this.ModuleTag.Text = "[" + SelModName.ToString().Replace("php_", "").Replace(".dll", "") + "]";
                if (index >= 0)
                {
                    string[] lines = this.PHPSettings.Modules[index].Value.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
                    this.ModulesConfig.Lines = lines;
                }
                else
                {
                    this.ModulesConfig.Text = "";
                }

                //if (this.PHPSettings.Settings[SelModule].Active == 1)
                if (this.checkedListModules.GetItemChecked(this.checkedListModules.SelectedIndex))
                {
                    this.ModulesConfig.Enabled = true;
                }
                else
                {
                    this.ModulesConfig.Enabled = false;
                }
            }

            if (this.checkedListModules.SelectedIndex >= 0)
            {
                this.ModulesInfo.Text = PHPSettings.GetModuleDescription(this.checkedListModules.SelectedItem.ToString());

                LinkLabel.Link link = new LinkLabel.Link();
                String URL = PHPSettings.GetModuleURL(this.checkedListModules.SelectedItem.ToString());
                link.LinkData = URL;
                ModulesLink.Text = URL;
                ModulesLink.Links.Clear();
                ModulesLink.Links.Add(link);
            }
        }

        private void ModulesLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(e.Link.LinkData.ToString());
        }
    }
}
