using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using WDS.Resources;

namespace WDS
{
    public partial class WizardWindow : Form
    {
        private int Tab;

        private ComponentDownloader CompWS = new ComponentDownloader();
        private ComponentDownloader CompPHP = new ComponentDownloader();
        private ComponentDownloader CompDB = new ComponentDownloader();

        public WizardWindow()
        {
            InitializeComponent();
            Bitmap bmp = WDS.Properties.Resources.wand;
            this.Icon = Icon.FromHandle(bmp.GetHicon());
        }

        private void WizardWindow_Load(object sender, EventArgs e)
        {
            // Second panel.
            this.CheckboxWS.Checked = true;
            this.CheckboxPHP.Checked = true;
            this.CheckboxDB.Checked = true;

            ListServers();

            // Third panel.
            this.wwwFolder.Text = Globals.AppFolder + @"www";

            // Where to start.
            this.Tab = 1;
            Next_Back();
        }

        private void CheckboxWS_CheckedChanged(object sender, EventArgs e)
        {
            ComboboxWSType.Enabled = CheckboxWS.Checked;
            ComboboxWSVersion.Enabled = CheckboxWS.Checked;

            bNext_Available();
        }

        private void CheckboxPHP_CheckedChanged(object sender, EventArgs e)
        {
            ComboboxPHPType.Enabled = CheckboxPHP.Checked;
            ComboboxPHPVersion.Enabled = CheckboxPHP.Checked;

            bNext_Available();
        }

        private void CheckboxDB_CheckedChanged(object sender, EventArgs e)
        {
            ComboboxDBType.Enabled = CheckboxDB.Checked;
            ComboboxDBVersion.Enabled = CheckboxDB.Checked;

            bNext_Available();
        }

        private void bNext_Available()
        {
            if (!CheckboxWS.Checked && !CheckboxPHP.Checked && !CheckboxDB.Checked)
            {
                bNext.Enabled = false;
            }
            else
            {
                bNext.Enabled = true;
            }
        }

        private void ButtonBrowse_Click(object sender, EventArgs e)
        {
            BrowseFolder.Description = "Select the place where your site will be stored:";

            BrowseFolder.SelectedPath = wwwFolder.Text;
            if (BrowseFolder.ShowDialog() == DialogResult.OK)
            {
                wwwFolder.Text = BrowseFolder.SelectedPath;
            }
        }

        private void bNext_Click(object sender, EventArgs e)
        {
            ViewTab(1);
        }

        private void bBack_Click(object sender, EventArgs e)
        {
            ViewTab(-1);
        }

        private void ViewTab(int MoreOrLess)
        {
            this.Tab += MoreOrLess;

            if (
                (this.Tab == 3 && !this.CheckboxWS.Checked) || 
                (this.Tab == 4 && !this.CheckboxPHP.Checked) || 
                (this.Tab == 5 && !this.CheckboxDB.Checked)
            ) {
                ViewTab(MoreOrLess);
            }
            else
            {
                Next_Back();
            }
        }

        private void Next_Back()
        {
            bBack.Enabled = true;
            bNext.Enabled = true;
            bNext.Text = "&Next >";

            panel1.Visible = false;
            panel2.Visible = false;
            panel3.Visible = false;
            panel4.Visible = false;
            panel5.Visible = false;
            panel6.Visible = false;
            panel7.Visible = false;
            panel8.Visible = false;

            switch (this.Tab)
            {
                case 1:
                    panel1.Visible = true;
                    bBack.Enabled = false;
                    break;
                case 2:
                    panel2.Visible = true;
                    break;
                case 3:
                    panel3.Visible = true;
                    break;
                case 4:
                    panel4.Visible = true;
                    break;
                case 5:
                    panel5.Visible = true;
                    CheckDBPassword();
                    break;
                case 6:
                    panel6.Visible = true;
                    bNext.Text = "&Finish";
                    DoSummary();
                    break;
                case 7:
                    panel7.Visible = true;
                    bNext.Enabled = false; // DON'T FORGET TO UNCOMMENT THIS LINE ONCE THE DOWNLOADING/EDITING PART IS DONE.
                    bBack.Enabled = false;
                    bCancel.Enabled = false;

                    DoConfig();
                    break;
                case 8:
                    panel8.Visible = true;
                    bNext.Text = "C&lose";
                    bBack.Enabled = false;
                    bCancel.Enabled = false;
                    break;
                case 9:
                    DialogResult = DialogResult.OK;
                    Close();
                    break;
                default:
                    panel1.Visible = true;
                    break;
            }
        }

        private void ListServers()
        {
            // Web Servers.
            CompWS.SelTypes = ComponentDownloader.ComponentType.WebServer;
            if (CompWS.LoadServerList())
            {
                this.ComboboxWSType.Items.Clear();

                if (CompWS.SvrList.Rows.Count > 0)
                {
                    foreach (DataRow row in CompWS.SvrList.Rows)
                    {
                        this.ComboboxWSType.Items.Add(row["FullName"].ToString());
                    }

                    this.ComboboxWSType.SelectedIndex = 0;
                }
            }

            // PHP Modules.
            CompPHP.SelTypes = ComponentDownloader.ComponentType.PHP;
            if (CompPHP.LoadServerList())
            {
                this.ComboboxPHPType.Items.Clear();

                if (CompPHP.SvrList.Rows.Count > 0)
                {
                    foreach (DataRow row in CompPHP.SvrList.Rows)
                    {
                        this.ComboboxPHPType.Items.Add(row["FullName"].ToString());
                    }

                    this.ComboboxPHPType.SelectedIndex = 0;
                }
            }

            // Database.
            CompDB.SelTypes = ComponentDownloader.ComponentType.DataBase;
            if (CompDB.LoadServerList())
            {
                this.ComboboxDBType.Items.Clear();

                if (CompDB.SvrList.Rows.Count > 0)
                {
                    foreach (DataRow row in CompDB.SvrList.Rows)
                    {
                        this.ComboboxDBType.Items.Add(row["FullName"].ToString());
                    }

                    this.ComboboxDBType.SelectedIndex = 0;
                }
            }
        }

        private void ListComponentFiles(ComboBox Input, ComboBox Output, ComponentDownloader.ComponentType ServerType)
        {
            ComponentDownloader Comp = new ComponentDownloader();
            // Selected Server/Module files.
            if (ServerType == ComponentDownloader.ComponentType.WebServer)
                Comp = CompWS;
            if (ServerType == ComponentDownloader.ComponentType.PHP)
                Comp = CompPHP;
            if (ServerType == ComponentDownloader.ComponentType.DataBase)
                Comp = CompDB;

            Comp.SelTypes = ServerType;
            int ServerId = 0;

            if (Comp.LoadServerList())
            {
                foreach (DataRow Row in Comp.SvrList.Rows)
                {
                    if (Row["FullName"].ToString().CompareTo(Input.SelectedItem.ToString()) == 0)
                    {
                        ServerId = Convert.ToInt32(Row["Id"]);
                        break;
                    }
                }
            }
            else
            {
                return;
            }

            if (Comp.LoadServerFileList(ServerId))
            {
                Output.Items.Clear();

                if (Comp.SvrURLs.Rows.Count > 0)
                {
                    foreach (DataRow row in Comp.SvrURLs.Rows)
                    {
                        try
                        {
                            Output.Items.Add(Comp.CleanURL(row["URL"].ToString()));
                        }
                        catch(Exception ex)
                        {
                            Globals.Error.Show(ex.Message);
                        }
                    }

                    Output.SelectedIndex = 0;
                }
            }
        }

        private void ComboboxWSType_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListComponentFiles(this.ComboboxWSType, this.ComboboxWSVersion, ComponentDownloader.ComponentType.WebServer);
        }

        private void ComboboxPHPType_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListComponentFiles(this.ComboboxPHPType, this.ComboboxPHPVersion, ComponentDownloader.ComponentType.PHP);
        }

        private void ComboboxDBType_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListComponentFiles(this.ComboboxDBType, this.ComboboxDBVersion, ComponentDownloader.ComponentType.DataBase);
        }

        private void DBPasswd_TextChanged(object sender, EventArgs e)
        {
            CheckDBPassword();
        }

        private void CheckDBPassword()
        {
            DataBaseConfig DBc = new DataBaseConfig();
            Boolean pass = DBc.CheckRegex(this.DBPasswd.Text);

            if (pass)
            {
                bNext.Enabled = true;
            }
            else
            {
                bNext.Enabled = false;
            }
        }

        private void DoSummary()
        {
            String Summary = "";

            if (this.CheckboxWS.Checked)
            {
                Summary += "Web Server:\r\n";
                Summary += "Download package: " + this.ComboboxWSVersion.Text + "\r\n";
                Summary += "Set the website folder to: " + this.wwwFolder.Text + "\r\n";
                Summary += ((RadiobuttonWSConfigTrue.Checked)? "G":"Do not g") + "enerate a default configuration for this component.\r\n";
                Summary += ((RadiobuttonWSConfigTrue.Checked) ? "E" : "Do not e") + "nable the server for its use.\r\n";
                Summary += "\r\n";
            }

            if (this.CheckboxPHP.Checked)
            {
                Summary += "PHP Module:\r\n";
                Summary += "Download package: " + this.ComboboxPHPVersion.Text + "\r\n";
                Summary += ((RadiobuttonPHPConfigTrue.Checked)? "G":"Do not g") + "enerate a default configuration for this component.\r\n";
                Summary += ((RadiobuttonPHPConfigTrue.Checked) ? "E" : "Do not e") + "nable the module for its use.\r\n";
                Summary += "\r\n";
            }

            if (this.CheckboxDB.Checked)
            {
                Summary += "Database Server:\r\n";
                Summary += "Download package: " + this.ComboboxDBVersion.Text + "\r\n";
                Summary += ((RadiobuttonDBConfigTrue.Checked)? "G":"Do not g") + "enerate a default configuration for this component.\r\n";
                Summary += "Set password to the Root account.\r\n";
                Summary += ((RadiobuttonDBConfigTrue.Checked) ? "E" : "Do not e") + "nable the server for its use.";
            }

            this.tSummary.Text = Summary;
        }

        private void DoConfig()
        {
            bool DownWS = false, DownPHP = false, DownDB = false;

            if (this.CheckboxWS.Checked)
            {
                if (DownWS = !ServerInstalled(this.ComboboxWSVersion.Text, Globals.AppFolder + @"bin\WebServer\"))
                {
                    Steps++;
                }
                if (RadiobuttonWSConfigTrue.Checked)
                {
                    Steps++;
                }
            }

            if (this.CheckboxPHP.Checked)
            {
                if (DownPHP = !ServerInstalled(this.ComboboxPHPVersion.Text, Globals.AppFolder + @"bin\PHP\"))
                {
                    Steps++;
                }
                if (RadiobuttonPHPConfigTrue.Checked)
                {
                    Steps++;
                }
            }

            if (this.CheckboxDB.Checked)
            {
                if (DownDB = !ServerInstalled(this.ComboboxDBVersion.Text, Globals.AppFolder + @"bin\MySQL\"))
                {
                    Steps++;
                }
                if (RadiobuttonDBConfigTrue.Checked)
                {
                    Steps++;
                }
            }

            if (DownWS)
            {
                if (CompWS == null)
                {
                    ListServers();
                }

                DownloadList WS = new DownloadList();
                WS.URL = this.ComboboxWSVersion.Text;
                WS.URLlist = CompWS.SvrURLs;
                WS.Target = Globals.AppFolder + @"bin\WebServer\";
                ListOfDownloads.Add(WS);
            }

            if (DownPHP)
            {
                if (CompPHP == null)
                {
                    ListServers();
                }

                DownloadList PHP = new DownloadList();
                PHP.URL = this.ComboboxPHPVersion.Text;
                PHP.URLlist = CompPHP.SvrURLs;
                PHP.Target = Globals.AppFolder + @"bin\PHP\";
                ListOfDownloads.Add(PHP);
            }

            if (DownDB)
            {
                if (CompDB == null)
                {
                    ListServers();
                }

                DownloadList DB = new DownloadList();
                DB.URL = this.ComboboxDBVersion.Text;
                DB.URLlist = CompDB.SvrURLs;
                DB.Target = Globals.AppFolder + @"bin\MySQL\";
                ListOfDownloads.Add(DB);
            }

            if (ListOfDownloads.Count > 0)
            {
                StartDownloadList();
            }
            else
            {
                StartConfiguringServers();
            }
        }

        private String GetURLfromFile(String FileName, DataTable Table)
        {
            if (Table.Rows.Count > 0)
            {
                foreach (DataRow Row in Table.Rows)
                {
                    if (Row["URL"].ToString().Contains(FileName))
                    {
                        return Row["URL"].ToString();
                    }
                }
            }

            return null;
        }

        private Boolean ServerInstalled(String Target, String Folder)
        {
            ComponentDownloader CD = new ComponentDownloader();
            List<string> possible = CD.GetPossibleServers(Folder);

            foreach (string srvr in possible)
            {
                if (Target.CompareTo(srvr) == 0)
                {
                    return true;
                }
            }
            return false;
        }

        List<DownloadList> ListOfDownloads = new List<DownloadList>();
        int Steps = 0, Step = 0;

        private void StartDownloadList()
        {
            Step++;
            this.p7label3.Text = "Step " + Step + " of " + Steps + "...";
            this.p7label4.Text = "Downloading " + ListOfDownloads[0].URL + "...";
            this.ProgressbarGlobal.Value = Convert.ToInt32((Step-1)*100/Steps);

            String[] DownloadFile = { GetURLfromFile(ListOfDownloads[0].URL, ListOfDownloads[0].URLlist) };
            DownloaderWindow Down = new DownloaderWindow(DownloadFile, ListOfDownloads[0].Target);
            Down.client.DownloadFileCompleted += new System.ComponentModel.AsyncCompletedEventHandler(DownloadFileCompleted);
            Down.client.DownloadProgressChanged += new System.Net.DownloadProgressChangedEventHandler(DownloadProgressChanged);
            Down.StartDownload();
        }

        class DownloadList
        {
            public String URL = "";
            public DataTable URLlist = null;
            public String Target = "";
        }

        void DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            ListOfDownloads.RemoveAt(0);
            if (ListOfDownloads.Count > 0)
            {
                StartDownloadList();
            }
            else
            {
                StartConfiguringServers();
            }
        }

        private void DownloadProgressChanged(object sender, System.Net.DownloadProgressChangedEventArgs e)
        {
            this.ProgressbarStep.Value = e.ProgressPercentage;
        }

        private void StartConfiguringServers()
        {
            if (this.CheckboxWS.Checked && this.RadiobuttonWSConfigTrue.Checked)
            {
                Step++;
                this.p7label3.Text = "Step " + Step + " of " + Steps + "...";
                this.p7label4.Text = "Configuring WebServer...";
                WebServerConfig WSConfig = new WebServerConfig();

                if (this.CheckboxPHP.Checked && this.CheckboxDB.Checked)
                {   // With phpMyAdmin.
                    WSConfig.SetDefaultConfig(this.wwwFolder.Text, true);
                }
                else
                {   // Without it.
                    WSConfig.SetDefaultConfig(this.wwwFolder.Text, false);
                }
            }
            if (this.CheckboxPHP.Checked && this.RadiobuttonPHPConfigTrue.Checked)
            {
                Step++;
                this.p7label3.Text = "Step " + Step + " of " + Steps + "...";
                this.p7label4.Text = "Configuring PHP Module...";
                PHPConfig PHPConfig = new PHPConfig();
                PHPConfig.SetDefaultConfig();
            }
            if (this.CheckboxDB.Checked && this.RadiobuttonDBConfigTrue.Checked)
            {
                Step++;
                this.p7label3.Text = "Step " + Step + " of " + Steps + "...";
                this.p7label4.Text = "Configuring Database...";
                DataBaseConfig DBConfig = new DataBaseConfig();
                DBConfig.SetDefaultConfig(this.DBPasswd.Text);
            }

            this.Tab++;
            Next_Back();
        }
    }
}
