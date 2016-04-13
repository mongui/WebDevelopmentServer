using System;
using System.ComponentModel;
using System.Windows.Forms;
using WDS.Resources;
using System.IO;

namespace WDS
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }

        private void Main_Load(object sender, EventArgs e)
        {
            CheckForIllegalCrossThreadCalls = false;
            // Need to modify manually inside the AssemblyInfo.cs file (AssemblyFileVersion).
            this.Text = String.Format("Web Development Server {0}", Application.ProductVersion);

            // Add the link to GitHub in the About tab.
            LinkLabel.Link link = new LinkLabel.Link();
            link.LinkData = "https://github.com/mongui";
            tGithub.Links.Add(link);

            Globals.list = this.LogList;
            Globals.AddToLog("Thanks for using Web Development Server v." + Application.ProductVersion + ".");
            Globals.AddToLog("------------------------------------------------------------------------------");

#if DEBUG
            Globals.AppFolder = @"c:\tests\";
#else
            this.NetworkStatus();
            Globals.AppFolder = AppDomain.CurrentDomain.BaseDirectory;
#endif

            // Check if the databases are available.
            DataBaseFiles dbFiles = new DataBaseFiles();
            if (!dbFiles.LoadDataBase(Globals.AppFolder + @"config.db", ref Globals.dbConn))
            {
                if (!dbFiles.CreateConfigFile())
                {
                    this.Close();
                    return;
                }
                dbFiles.LoadDataBase(Globals.AppFolder + @"config.db", ref Globals.dbConn);
            }

            if (Config.LoadSettings() < 0)
            {
                this.Close();
                return;
            }
            if (Globals.DataBasePass.Length < 8)
            {
                Globals.DataBasePass = "WebDevelopmentServer1";
                Config.SaveSetting("DataBasePass", Globals.DataBasePass);

                DataBaseConfig DBConfig = new DataBaseConfig();
                DBConfig.SetDefaultConfig(Globals.DataBasePass);
            }

            Boolean isDownloading = false;
#if !DEBUG
            if (!File.Exists(Globals.AppFolder + @"servers.db") || dbFiles.CheckLastModified())
            {
                isDownloading = dbFiles.DownloadUpdate();
            }
#endif

            if (!isDownloading && !dbFiles.LoadDataBase(Globals.AppFolder + @"servers.db", ref Globals.dbServ))
            {
                this.Close();
                return;
            }

            if (!Directory.Exists(Globals.AppFolder + @"bin"))
                Directory.CreateDirectory(Globals.AppFolder + @"bin");
            if (!Directory.Exists(Globals.AppFolder + @"bin\WebServer"))
                Directory.CreateDirectory(Globals.AppFolder + @"bin\WebServer");
            if (!Directory.Exists(Globals.AppFolder + @"bin\PHP"))
                Directory.CreateDirectory(Globals.AppFolder + @"bin\PHP");
            if (!Directory.Exists(Globals.AppFolder + @"bin\MySQL"))
                Directory.CreateDirectory(Globals.AppFolder + @"bin\MySQL");

            this.LoadCombos();
            this.LoadCheckboxes();
            this.ChecksActive();

            this.LoadServerData(true, true, true);

            if (Globals.Processes.ProcessesPIDs.Count == 0)
            {
                this.NotifyIcon.Icon = WDS.Properties.Resources.ServerRed;
            }

            // Start in the systray and start servers if the options are checked.
            if (Globals.StartInSystray == "1")
            {
                this.WindowState = FormWindowState.Minimized;
            }
            if (Globals.StartOnLaunch == "1")
            {
                this.bstart_Click(null, null);
            }

            // Check if there is any non-wanted service running.
            Services Svc = new Services();
            String SvcName = Svc.DoesServiceExist("Apache");
            if (Svc.StopService(SvcName))
                Globals.AddToLog("Found an Apache service running. Shutting down the webserver...");
            
            SvcName = Svc.DoesServiceExist("MySQL");
            if (Svc.StopService(SvcName))
                Globals.AddToLog("Found an MySQLd service running. Shutting down the database manager...");
        }

        public void LoadServerData(bool WebServer = false, bool PHP = false, bool DataBase = false)
        {
            Globals.Servers = new ServerType();

            // Load/Update WebServer data.
            WebServerConfig LoadVH = new WebServerConfig();
            LoadVH.LoadVirtualHosts();

            // Load active modules of the selected servers.
            Globals.Servers.WebServer.ActiveModules = LoadVH.LoadActiveModules(Globals.Servers.WebServer.Type.ToString());
            Globals.Servers.PHP.ActiveModules = LoadVH.LoadActiveModules("PHP");
            LoadVH.LoadModulesSettings();
            LoadVH.LoadCustomSettings(Globals.Servers.WebServer.Type.ToString());

            // Load/Update PHP data.
            PHPConfig LoadPHP = new PHPConfig();

            // Load/Update DataBase data.
            DataBaseConfig LoadDB = new DataBaseConfig();
        }

        private void MinimizeButton_Click(object sender, EventArgs e)
        {
            this.MinimizeToTray();
        }

        private void MinimizeToTray()
        {
            if (FormWindowState.Minimized == this.WindowState)
            {
                this.Hide();
            }

            else if (FormWindowState.Normal == this.WindowState)
            {
                this.Show();
            }
        }

        private void NotifyIcon_DoubleClick(object sender, EventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            Globals.list = null;
        }

        private void Main_FormClosed(object sender, FormClosedEventArgs e)
        {
            Globals.Processes.WebServerStop();
            Globals.Processes.PHPStop();
            Globals.Processes.DataBaseStop();
        }

        private void buttonInstallWebServer_Click(object sender, EventArgs e)
        {
            this.InstallComponentsWindow(ComponentDownloader.ComponentType.WebServer);
        }

        private void buttonInstallPHP_Click(object sender, EventArgs e)
        {
            this.InstallComponentsWindow(ComponentDownloader.ComponentType.PHP);
        }

        private void buttonInstallMySQL_Click(object sender, EventArgs e)
        {
            this.InstallComponentsWindow(ComponentDownloader.ComponentType.DataBase);
        }

        private void InstallComponentsWindow(ComponentDownloader.ComponentType type)
        {
            ComponentDownloader ComponentDialog = new ComponentDownloader();
            ComponentDialog.SelTypes = type;

            ComponentDialog.ShowDialog();
            ComponentDialog.Dispose();

            LoadCombos();
        }

        private void LoadCheckboxes()
        {
            this.checkWebServer.Checked = (Globals.WebServerStart == "1" && this.comboWebServerVersion.Text != "") ? true : false;
            this.checkPHP.Checked = (Globals.PHPStart == "1" && this.comboPHPVersion.Text != "") ? true : false;
            this.checkMySQL.Checked = (Globals.MySQLStart == "1" && this.comboMySQLVersion.Text != "") ? true : false;
        }

        private void LoadCombos()
        {
            this.FillDirectoryLists(Globals.AppFolder + @"bin\WebServer\", this.comboWebServerVersion, Globals.ActiveWebServerVersion);
            this.FillDirectoryLists(Globals.AppFolder + @"bin\PHP\", this.comboPHPVersion, Globals.ActivePHPVersion);
            this.FillDirectoryLists(Globals.AppFolder + @"bin\MySQL\", this.comboMySQLVersion, Globals.ActiveMySQLVersion);
        }

        private void FillDirectoryLists(String Dir, ComboBox Combo, String Selected)
        {
            var Items = Combo.Items;
            Items.Clear();

            if (!Directory.Exists(Dir))
            {
                Directory.CreateDirectory(Dir);
            }

            String ItmName;
            foreach (var item in Directory.GetDirectories(Dir))
            {
                ItmName = item.Remove(0, Dir.Length);
                Items.Add(ItmName);
                if (ItmName == Selected)
                {
                    Combo.Text = ItmName;
                }
            }
        }

        private void checkWebServer_CheckedChanged(object sender, EventArgs e)
        {
            string status;
            if (this.comboWebServerVersion.Text != "")
            {
                status = (this.checkWebServer.Checked) ? "1" : "0";
            }
            else
            {
                this.checkWebServer.Checked = false;
                status = "0";
            }

            if (status != Globals.WebServerStart)
            {
                Config.SaveSetting("WebServerStart", status);
                Globals.WebServerStart = status;
            }

            this.ChangeTabIcons(1);

            this.ChecksActive();
        }

        private void checkPHP_CheckedChanged(object sender, EventArgs e)
        {
            string status;
            if (this.comboPHPVersion.Text != "")
            {
                status = (this.checkPHP.Checked) ? "1" : "0";
            }
            else
            {
                this.checkPHP.Checked = false;
                status = "0";
            }

            if (status != Globals.PHPStart)
            {
                Config.SaveSetting("PHPStart", status);
                Globals.PHPStart = status;
            }

            this.ChangeTabIcons(2);

            this.ChecksActive();
        }

        private void checkMySQL_CheckedChanged(object sender, EventArgs e)
        {
            string status;
            if (this.comboMySQLVersion.Text != "")
            {
                status = (this.checkMySQL.Checked) ? "1" : "0";
            }
            else
            {
                this.checkMySQL.Checked = false;
                status = "0";
            }

            if (status != Globals.MySQLStart)
            {
                Config.SaveSetting("MySQLStart", status);
                Globals.MySQLStart = status;
            }

            this.ChangeTabIcons(3);

            this.ChecksActive();
        }

        private void ChecksActive()
        {
            if (
                (this.checkWebServer.Checked && this.comboWebServerVersion.Text != "") || 
                (this.checkPHP.Checked && this.comboPHPVersion.Text != "") || 
                (this.checkMySQL.Checked && this.comboMySQLVersion.Text != "")
               )
            {
                this.bstart.Enabled = true;
            }
            else
            {
                this.bstart.Enabled = false;
            }
        }
        
        private void comboWebServerVersion_SelectedIndexChanged(object sender, EventArgs e)
        {
            string status = comboWebServerVersion.SelectedItem.ToString();
            if (status != Globals.ActiveWebServerVersion)
            {
                Config.SaveSetting("ActiveWebServerVersion", status);
                Globals.ActiveWebServerVersion = status;

                this.LoadServerData();
            }
        }

        private void comboPHPVersion_SelectedIndexChanged(object sender, EventArgs e)
        {
            string status = comboPHPVersion.SelectedItem.ToString();
            if (status != Globals.ActivePHPVersion)
            {
                Config.SaveSetting("ActivePHPVersion", status);
                Globals.ActivePHPVersion = status;

                this.LoadServerData();
            }
        }

        private void comboMySQLVersion_SelectedIndexChanged(object sender, EventArgs e)
        {
            string status = comboMySQLVersion.SelectedItem.ToString();
            if (status != Globals.ActiveMySQLVersion)
            {
                Config.SaveSetting("ActiveMySQLVersion", status);
                Globals.ActiveMySQLVersion = status;

                this.LoadServerData();
            }
        }

        private void comboWebServerVersion_TextUpdate(object sender, EventArgs e)
        {
            if (comboWebServerVersion.Text == "")
            {
                this.checkWebServer.Checked = false;
            }
        }

        private void comboPHPVersion_TextUpdate(object sender, EventArgs e)
        {
            if (comboPHPVersion.Text == "")
            {
                this.checkPHP.Checked = false;
            }
        }

        private void comboMySQLVersion_TextUpdate(object sender, EventArgs e)
        {
            if (comboMySQLVersion.Text == "")
            {
                this.checkMySQL.Checked = false;
            }
        }

        private long FirstTick;
        private long CurrentTick;
        System.Timers.Timer UpTime;

        private void DisplayTimeEvent(object source, System.Timers.ElapsedEventArgs e)
        {
            if (Globals.list != null)
            {
                CurrentTick = DateTime.Now.Ticks - this.FirstTick;
                this.Uptime.Text = string.Format("{0:HH:mm:ss tt}", new DateTime(CurrentTick));
            }
        }

        private void bstart_Click(object sender, EventArgs e)
        {
            // First, kill already working servers.
            this.bstop_Click(null, null);

            // Then, start selected servers.
            if (this.checkPHP.Checked)
            {
                if (this.checkWebServer.Checked)
                {
                    Globals.Servers.WebServerLoadsPHP(Globals.Servers.WebServer.Type);
                }

                PHPConfigFile WritePHPConfigFile = new PHPConfigFile(Path.GetDirectoryName(Globals.Servers.PHP.Exe) + @"\");
                if (Globals.RewriteConfigFiles == "1")
                {
                    WritePHPConfigFile.Write();
                }

                if (Globals.Processes.InternalPHP == false)
                {
                    Globals.Processes.ExternalPHP = true;
                    Globals.Processes.PHPStart(Globals.Servers.PHP.Exe, "-b 127.0.0.1:8866 -c " + Path.GetDirectoryName(Globals.Servers.PHP.Exe) + @"\php.ini");

                    if (!Globals.Processes.PHP.HasExited)
                    {
                        Globals.Processes.PHP.EnableRaisingEvents = true;
                        Globals.Processes.PHP.Exited += (sndr, earg) =>
                        {
                            Globals.Processes.PHPStop();
                            this.ChangeTabIcons(2);
                            this.CheckStartStopButtons();

                            if (Globals.Processes.ProcessesPIDs.Count == 0)
                            {
                                this.bstop.Enabled = false;
                            }
                        };
                    }

                    this.ChangeTabIcons(2);
                }
            }

            if (this.checkWebServer.Checked && Globals.Servers.WebServer.Type != WebServers.WSType.none)
            {
                Globals.Processes.WebServerStart(Globals.Servers.WebServer.Exe);

                if (!Globals.Processes.WebServer.HasExited)
                {
                    Globals.Processes.WebServer.EnableRaisingEvents = true;
                    Globals.Processes.WebServer.Exited += (sndr, earg) =>
                    {
                        Globals.Processes.WebServerStop();
                        this.ChangeTabIcons(1);
                        this.CheckStartStopButtons();
                    };

                    this.ChangeTabIcons(1);
                    this.ChangeTabIcons(2);
                }
            }
            
            if (this.checkMySQL.Checked && Globals.Servers.DataBase.Type != DataBases.DBType.none)
            {
                Globals.Processes.DataBaseStart(Globals.Servers.DataBase.Exe);
                if (!Globals.Processes.DataBase.HasExited)
                {
                    Globals.Processes.DataBase.EnableRaisingEvents = true;
                    Globals.Processes.DataBase.Exited += (sndr, earg) =>
                    {
                        Globals.Processes.DataBaseStop();
                        this.ChangeTabIcons(3);
                        this.CheckStartStopButtons();
                    };
                    Globals.DataBasePassChange = false;
                    this.bstop.Enabled = true;
                }

                this.ChangeTabIcons(3);
            }

            this.CheckStartStopButtons();

            // Start the Uptime timer.
            FirstTick = DateTime.Now.Ticks;
            this.UpTime = new System.Timers.Timer();
            this.UpTime.Elapsed += new System.Timers.ElapsedEventHandler(DisplayTimeEvent);
            this.UpTime.Interval = 1000;
            this.UpTime.Start();
        }

        private void CheckStartStopButtons()
        {
            if (Globals.Processes.ProcessesPIDs.Count == 0)
            {
                this.bstop.Enabled = false;
            }
            else
            {
                this.bstop.Enabled = true;
            }

            if (Globals.Processes.WebServerID >= 0 || Globals.Processes.PHPID >= 0 || Globals.Processes.DataBaseID >= 0)
            {
                this.bstart.Enabled = false;
            }
        }

        private void bstop_Click(object sender, EventArgs e)
        {
            if (this.UpTime != null)
            {
                this.UpTime.Stop();
                this.UpTime = null;
                this.Uptime.Text = "--:--:--";
            }

            this.bstart.Enabled = true;
            Globals.Processes.WebServerStop();
            Globals.Processes.PHPStop();
            Globals.Processes.DataBaseStop();
            
            this.ChangeTabIcons(1);
            this.ChangeTabIcons(2);
            this.ChangeTabIcons(3);

            if (Globals.Processes.ProcessesPIDs.Count == 0)
            {
                this.bstop.Enabled = false;
            }
        }

        private void ChangeTabIcons(int Server)
        {
            // If the form is closed, there is no need to change its images.
            if (Globals.list == null)
            {
                return;
            }

            int PID = -1;
            string Status = "";
            string StatusImg = "";
            bool isChecked = false;
             
            switch (Server)
            {
                case 1:
                    PID = Globals.Processes.WebServerID;
                    isChecked = this.checkWebServer.Checked;
                    break;
                case 2:
                    if (Globals.Processes.InternalPHP)
                    {
                        PID = Globals.Processes.WebServerID;
                    }
                    else if (Globals.Processes.ExternalPHP)
                    {
                        PID = Globals.Processes.PHPID;
                    }
                    isChecked = this.checkPHP.Checked;
                    break;
                case 3:
                    PID = Globals.Processes.DataBaseID;
                    isChecked = this.checkMySQL.Checked;
                    break;
            }

            if (PID > 0)
            {
                StatusImg = "green";
                Status = "Online";
            }
            else if (isChecked)
            {
                StatusImg = "red";
                Status = "Disconnected";
            }
            else
            {
                StatusImg = "grey";
                Status = "Disabled";
            }

            switch (Server)
            {
                case 1:
                    this.tWebServerStatus.Text = Status;
                    this.pictureWebServer.Image = this.tabIcons.Images[StatusImg];
                    this.tabWebServer.ImageKey = StatusImg;
                    break;
                case 2:
                    this.tPHPStatus.Text = Status;
                    this.picturePHP.Image = this.tabIcons.Images[StatusImg];
                    this.tabPHP.ImageKey = StatusImg;
                    break;
                case 3:
                    this.tDatabaseStatus.Text = Status;
                    this.pictureMySQL.Image = this.tabIcons.Images[StatusImg];
                    this.tabDatabase.ImageKey = StatusImg;
                    break;
            }

            try
            {
                if (Globals.Processes.ProcessesPIDs.Count <= 0)
                {
                    this.NotifyIcon.Icon = WDS.Properties.Resources.ServerRed;
                }
                else
                {
                    this.NotifyIcon.Icon = WDS.Properties.Resources.ServerGreen; 
                }
            }
            catch(Exception /*ex*/)
            {
                //Globals.Error.Show(ex.Message);
            }
        }

        private void buttonWebServerVersion_Click(object sender, EventArgs e)
        {
            WebServerConfigWindow ConfigWindow = new WebServerConfigWindow();
            ConfigWindow.ShowDialog();
            ConfigWindow.Dispose();
        }

        private void buttonPHPVersion_Click(object sender, EventArgs e)
        {
            PHPConfigWindow ConfigWindow = new PHPConfigWindow();
            ConfigWindow.ShowDialog();
            ConfigWindow.Dispose();
        }

        private void buttonMySQLVersion_Click(object sender, EventArgs e)
        {
            DatabaseConfigWindow ConfigWindow = new DatabaseConfigWindow();
            ConfigWindow.ShowDialog();
            ConfigWindow.Dispose();
        }
        
        private void bPreferences_Click(object sender, EventArgs e)
        {
            PreferencesWindow Preferences = new PreferencesWindow();

            if (Preferences.ShowDialog() == DialogResult.OK)
            {
                Globals.StartOnLaunch = Preferences.StartOnLaunch ? "1" : "0";
                Globals.StartInSystray = Preferences.StartInSystray ? "1" : "0";
                Globals.RewriteConfigFiles = Preferences.RewriteWhenStart ? "1" : "0";

                Config.SaveSetting("StartOnLaunch", Globals.StartOnLaunch);
                Config.SaveSetting("StartInSystray", Globals.StartInSystray);
                Config.SaveSetting("RewriteConfigFiles", Globals.RewriteConfigFiles);
            }

            Preferences.Dispose();
        }

        private void bWizard_Click(object sender, MouseEventArgs e)
        {
            WizardWindow Wizard = new WizardWindow();

            if (Wizard.ShowDialog() == DialogResult.OK)
            {
                LoadCombos();

                if (Wizard.CheckboxWS.Checked)
                {
                    this.comboWebServerVersion.Text = Wizard.ComboboxWSVersion.Text;
                    this.checkWebServer.Checked = true;
                }
                else
                {
                    this.checkWebServer.Checked = false;
                }
                if (Wizard.CheckboxPHP.Checked)
                {
                    this.comboPHPVersion.Text = Wizard.ComboboxPHPVersion.Text;
                    this.checkPHP.Checked = true;
                }
                else
                {
                    this.checkPHP.Checked = false;
                }
                if (Wizard.CheckboxDB.Checked)
                {
                    this.comboMySQLVersion.Text = Wizard.ComboboxDBVersion.Text;
                    this.checkMySQL.Checked = true;
                }
                else
                {
                    this.checkMySQL.Checked = false;
                }
            }
            
            Wizard.Dispose();
        }

        private void OKButton_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        /* Check Network Status */
        private void NetworkStatus()
        {
            if (System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            {
                this.EnabledNetwork.Text = "Yes";
                BackgroundWorker GetPublicIP = new BackgroundWorker();
                GetPublicIP.DoWork += GetPublicIPAddress;
                GetPublicIP.RunWorkerCompleted += GetPublicIPDone;
                GetPublicIP.RunWorkerAsync();

                BackgroundWorker GetPrivateIP = new BackgroundWorker();
                GetPrivateIP.DoWork += GetLocalIPAddress;
                GetPrivateIP.RunWorkerCompleted += GetLocalIPDone;
                GetPrivateIP.RunWorkerAsync();
            }
            else
            {
                this.EnabledNetwork.Text = "No";
                this.PublicAddress.Text = "Not available";
                this.LocalAddress.Text = "Not available";
            }
        }

        private void GetPublicIPDone(object sender, RunWorkerCompletedEventArgs e)
        {
            this.PublicAddress.Text = e.Result.ToString();
        }

        private void GetPublicIPAddress(object sender, DoWorkEventArgs e)
        {
            try
            {
                string sURL = "http://icanhazip.com";

                System.Net.WebRequest wrGETURL = System.Net.WebRequest.Create(sURL);
                Stream objStream = wrGETURL.GetResponse().GetResponseStream();
                StreamReader objReader = new StreamReader(objStream);

                e.Result = objReader.ReadLine();
            }
            catch (Exception /*ex*/)
            {
                e.Result = "Not available";
            }
        }

        private void GetLocalIPAddress(object sender, DoWorkEventArgs e)
        {
            try
            {
                System.Net.IPHostEntry host;
                string localIP = "";
                host = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());
                foreach (System.Net.IPAddress ip in host.AddressList)
                {
                    if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        localIP = ip.ToString();
                        break;
                    }
                }
                e.Result = localIP;
            }
            catch (Exception /*ex*/)
            {
                e.Result = "Not available";
            }
        }

        private void GetLocalIPDone(object sender, RunWorkerCompletedEventArgs e)
        {
            this.LocalAddress.Text = e.Result.ToString();
        }

        private void tGithub_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(e.Link.LinkData.ToString());
        }

        private void LogList_DoubleClick(object sender, EventArgs e)
        {
            LogList.Items.Clear();
        }

        private void bInfoPHPFiles_Click(object sender, EventArgs e)
        {
            if (this.contextMenuFileOpener.Visible)
            {
                this.contextMenuFileOpener.Hide();
            }
            else
            {
                Button btnSender = (Button)sender;
                System.Drawing.Point ptLowerLeft = new System.Drawing.Point(0, btnSender.Height);
                ptLowerLeft = btnSender.PointToScreen(ptLowerLeft);
                this.contextMenuFileOpener.Show(ptLowerLeft);
            }
        }
    }
}
