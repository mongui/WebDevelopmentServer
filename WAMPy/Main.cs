using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using WDS.Resources;

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
            // Need to modify manually inside the AssemblyInfo.cs file (AssemblyFileVersion).
            this.Text = String.Format("Web Development Server {0}", Application.ProductVersion);

            // Add the link to GitHub in the About tab.
            LinkLabel.Link link = new LinkLabel.Link();
            link.LinkData = "https://github.com/mongui";
            tGithub.Links.Add(link);

            this.CreateCompatibilityLabel();

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

            // No valid root password yet: create a random one. It is applied the next
            // time the database starts and can be seen/changed in the Database settings.
            if (Globals.DataBasePass.Length < 8)
            {
                new DataBaseConfig().SetDefaultConfig(Globals.GeneratePassword());
                Globals.AddToLog("A random root password was generated for the database. You can see it in the database settings.");
            }

            // Use the current servers list (if any) and look for updates in the background.
            dbFiles.LoadDataBase(Globals.AppFolder + @"servers.db", ref Globals.dbServ);
#if !DEBUG
            this.CheckServersUpdate(dbFiles);
#endif

            try
            {
                Directory.CreateDirectory(Globals.AppFolder + @"bin\WebServer");
                Directory.CreateDirectory(Globals.AppFolder + @"bin\PHP");
                Directory.CreateDirectory(Globals.AppFolder + @"bin\MySQL");
            }
            catch (Exception ex)
            {
                Globals.Error.Show("Unable to create the 'bin' folders", ex);
            }

            this.LoadCombos();
            this.LoadCheckboxes();
            this.ChecksActive();

            this.LoadServerData(true, true, true);

            this.NotifyIcon.Icon = WDS.Properties.Resources.ServerRed;

            // Warn about Windows services that may use the same ports. They are not
            // stopped: they belong to other installations.
            Services Svc = new Services();
            List<String> running = Svc.RunningServices("Apache");
            running.AddRange(Svc.RunningServices("MySQL"));
            running.AddRange(Svc.RunningServices("MariaDB"));
            foreach (String name in running)
            {
                Globals.AddToLog("Warning: the Windows service '" + name + "' is running and may use the same ports.");
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
        }

        private void CheckServersUpdate(DataBaseFiles dbFiles)
        {
            if (!DataBaseFiles.UpdatesEnabled)
            {
                if (Globals.dbServ == null)
                {
                    Globals.AddToLog("No servers list (servers.db) and no 'ServersListURL' set in config.db: the component downloader is not available.");
                }
                return;
            }

            bool missing = !File.Exists(Globals.AppFolder + @"servers.db");

            BackgroundWorker check = new BackgroundWorker();
            check.DoWork += (s, a) => { a.Result = missing || dbFiles.CheckLastModified(); };
            check.RunWorkerCompleted += (s, a) =>
            {
                if (a.Error == null && (bool)a.Result && !this.IsDisposed)
                {
                    dbFiles.DownloadUpdate();
                }
                check.Dispose();
            };
            check.RunWorkerAsync();
        }

        public void LoadServerData(bool WebServer = false, bool PHP = false, bool DataBase = false)
        {
            Globals.Servers = new ServerType();

            // Load/Update WebServer data.
            WebServerConfig LoadVH = new WebServerConfig();
            LoadVH.LoadVirtualHosts();

            this.InfoWSVer.Text = Globals.Servers.WebServer.VersionText();
            this.InfoPHPVer.Text = Globals.Servers.PHP.VersionText();

            this.InfoWSArch.Text = (Globals.Servers.WebServer.Is64Bits) ? "64 bits" : "32 bits";
            this.InfoPHPArch.Text = (Globals.Servers.PHP.Is64Bits) ? "64 bits" : "32 bits";


            // Load active modules of the selected servers.
            Globals.Servers.WebServer.ActiveModules = LoadVH.LoadActiveModules(Globals.Servers.WebServer.Type.ToString());
            Globals.Servers.PHP.ActiveModules = LoadVH.LoadActiveModules("PHP");
            LoadVH.LoadModulesSettings();
            LoadVH.LoadCustomSettings(Globals.Servers.WebServer.Type.ToString());

            this.UpdateCompatibilityLabel();
        }

        private LinkLabel CompatibilityLabel;

        /// <summary>
        /// Line above the log that says whether the selected versions can work together.
        /// </summary>
        private void CreateCompatibilityLabel()
        {
            this.LogList.Top += 26;
            this.LogList.Height -= 26;

            this.CompatibilityLabel = new LinkLabel();
            this.CompatibilityLabel.Location = new System.Drawing.Point(6, this.LogList.Top - 25);
            this.CompatibilityLabel.Size = new System.Drawing.Size(this.LogList.Width, 22);
            this.CompatibilityLabel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            this.CompatibilityLabel.AutoEllipsis = true;
            this.CompatibilityLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.CompatibilityLabel.LinkBehavior = LinkBehavior.HoverUnderline;
            this.CompatibilityLabel.LinkClicked += (s, e) =>
            {
                using (CompatibilityWindow w = new CompatibilityWindow())
                {
                    w.ShowDialog(this);
                }
            };
            this.tabStart.Controls.Add(this.CompatibilityLabel);
            this.CompatibilityTip = new ToolTip();
            this.CompatibilityTip.AutoPopDelay = 30000;
        }

        private ToolTip CompatibilityTip;

        private void UpdateCompatibilityLabel()
        {
            if (this.CompatibilityLabel == null || Globals.AppFolder == null)
            {
                return;
            }

            // The versions that will start; if none is checked, the selected ones.
            bool any = this.checkWebServer.Checked || this.checkPHP.Checked || this.checkMySQL.Checked;
            Analysis a = Compatibility.Analyze(
                (!any || this.checkWebServer.Checked) ? Compatibility.Active(ComponentKind.WebServer) : null,
                (!any || this.checkPHP.Checked) ? Compatibility.Active(ComponentKind.PHP) : null,
                (!any || this.checkMySQL.Checked) ? Compatibility.Active(ComponentKind.DataBase) : null);

            List<String> errors = new List<String>(a.Errors());
            List<String> warnings = new List<String>(a.Warnings());
            List<String> lines = new List<String>();
            System.Drawing.Color color;
            String text;

            if (a.Web == null && a.Php == null && a.Db == null)
            {
                text = "Compatibility: select the versions to use. Click to see all combinations.";
                color = System.Drawing.SystemColors.GrayText;
            }
            else if (errors.Count > 0)
            {
                text = "✖ " + errors[0];
                color = System.Drawing.Color.Firebrick;
                lines.AddRange(errors);
            }
            else
            {
                List<String> parts = new List<String>();
                if (a.WebPhp != null) parts.Add(a.WebPhp.A.Title + " + " + a.WebPhp.B.Title + ": " + a.WebPhp.Method);
                if (a.PhpDb != null) parts.Add(a.PhpDb.B.Title + (a.PhpDb.Auth == DbAuthMode.Default ? "" : a.PhpDb.Auth == DbAuthMode.Native ? " (native passwords)" : " (old passwords)"));
                if (parts.Count == 0)
                {
                    foreach (SoftwareComponent c in new SoftwareComponent[] { a.Web, a.Php, a.Db })
                    {
                        if (c != null) parts.Add(c.Title);
                    }
                }
                bool limited = warnings.Count > 0 || (a.WebPhp != null && (a.WebPhp.Link == PhpLink.Cgi || a.WebPhp.Notes.Count > 0));
                text = (limited ? "⚠ " : "✔ ") + String.Join(" · ", parts.ToArray());
                color = limited ? System.Drawing.Color.DarkOrange : System.Drawing.Color.ForestGreen;
                lines.AddRange(warnings);
                foreach (PairResult p in new PairResult[] { a.WebPhp, a.PhpDb })
                {
                    if (p == null) continue;
                    lines.AddRange(p.AutoConfig);
                    lines.AddRange(p.Notes);
                }
            }

            this.CompatibilityLabel.Text = text;
            this.CompatibilityLabel.LinkColor = color;
            this.CompatibilityLabel.ActiveLinkColor = color;
            this.CompatibilityLabel.VisitedLinkColor = color;
            lines.Add("Click to see every combination and the rules.");
            this.CompatibilityTip.SetToolTip(this.CompatibilityLabel, String.Join("\r\n", lines.ToArray()));
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
            this.StopUptime();
            if (Globals.AppFolder == null)
            {
                return;
            }
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
            using (ComponentDownloader ComponentDialog = new ComponentDownloader())
            {
                ComponentDialog.SelTypes = type;
                ComponentDialog.ShowDialog();
            }

            LoadCombos();
            this.UpdateCompatibilityLabel();
        }

        private void LoadCheckboxes()
        {
            this.checkWebServer.Checked = (Globals.WebServerStart == "1" && this.comboWebServerVersion.Text != "");
            this.checkPHP.Checked = (Globals.PHPStart == "1" && this.comboPHPVersion.Text != "");
            this.checkMySQL.Checked = (Globals.MySQLStart == "1" && this.comboMySQLVersion.Text != "");
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

            try
            {
                Directory.CreateDirectory(Dir);
                foreach (var item in Directory.GetDirectories(Dir))
                {
                    String ItmName = Path.GetFileName(item);
                    Items.Add(ItmName);
                    if (ItmName == Selected)
                    {
                        Combo.Text = ItmName;
                    }
                }
            }
            catch (Exception ex)
            {
                Globals.LogError("Unable to read " + Dir + "", ex);
            }
        }

        private void SaveStartFlag(CheckBox check, ComboBox combo, String key, ref String current, int tab)
        {
            string status;
            if (combo.Text != "")
            {
                status = (check.Checked) ? "1" : "0";
            }
            else
            {
                check.Checked = false;
                status = "0";
            }

            if (status != current)
            {
                Config.SaveSetting(key, status);
                current = status;
            }

            this.ChangeTabIcons(tab);
            this.ChecksActive();
        }

        private void checkWebServer_CheckedChanged(object sender, EventArgs e)
        {
            SaveStartFlag(this.checkWebServer, this.comboWebServerVersion, "WebServerStart", ref Globals.WebServerStart, 1);
        }

        private void checkPHP_CheckedChanged(object sender, EventArgs e)
        {
            SaveStartFlag(this.checkPHP, this.comboPHPVersion, "PHPStart", ref Globals.PHPStart, 2);
        }

        private void checkMySQL_CheckedChanged(object sender, EventArgs e)
        {
            SaveStartFlag(this.checkMySQL, this.comboMySQLVersion, "MySQLStart", ref Globals.MySQLStart, 3);
        }

        private void ChecksActive()
        {
            bool anyChecked =
                (this.checkWebServer.Checked && this.comboWebServerVersion.Text != "") ||
                (this.checkPHP.Checked && this.comboPHPVersion.Text != "") ||
                (this.checkMySQL.Checked && this.comboMySQLVersion.Text != "");

            this.bstart.Enabled = anyChecked && Globals.Processes.RunningCount == 0;
            this.UpdateCompatibilityLabel();
        }

        private void SaveVersion(ComboBox combo, String key, ref String current)
        {
            if (combo.SelectedItem == null)
            {
                return;
            }

            string status = combo.SelectedItem.ToString();
            if (status != current)
            {
                Config.SaveSetting(key, status);
                current = status;

                this.LoadServerData();
            }
        }

        private void comboWebServerVersion_SelectedIndexChanged(object sender, EventArgs e)
        {
            SaveVersion(this.comboWebServerVersion, "ActiveWebServerVersion", ref Globals.ActiveWebServerVersion);
        }

        private void comboPHPVersion_SelectedIndexChanged(object sender, EventArgs e)
        {
            SaveVersion(this.comboPHPVersion, "ActivePHPVersion", ref Globals.ActivePHPVersion);
        }

        private void comboMySQLVersion_SelectedIndexChanged(object sender, EventArgs e)
        {
            SaveVersion(this.comboMySQLVersion, "ActiveMySQLVersion", ref Globals.ActiveMySQLVersion);
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

        // Runs on the UI thread, so it can update the label directly.
        private Timer UpTimeTimer;
        private DateTime StartTime;

        private void UpTimeTick(object sender, EventArgs e)
        {
            TimeSpan up = DateTime.Now - this.StartTime;
            this.Uptime.Text = String.Format("{0:00}:{1:00}:{2:00}", (int)up.TotalHours, up.Minutes, up.Seconds);
        }

        private void StopUptime()
        {
            if (this.UpTimeTimer != null)
            {
                this.UpTimeTimer.Stop();
                this.UpTimeTimer.Dispose();
                this.UpTimeTimer = null;
            }
            if (!this.IsDisposed)
            {
                this.Uptime.Text = "--:--:--";
            }
        }

        /// <summary>
        /// Runs an action on the UI thread. Process events arrive on other threads.
        /// </summary>
        private void OnUI(Action action)
        {
            if (this.IsDisposed || Globals.list == null)
            {
                return;
            }
            try
            {
                this.BeginInvoke(action);
            }
            catch (InvalidOperationException)
            {
                // The window is closing.
            }
        }

        private void bstart_Click(object sender, EventArgs e)
        {
            // First, stop the servers started before.
            this.bstop_Click(null, null);

            // Check how the selected versions can be joined and set it up automatically.
            bool startWS = this.checkWebServer.Checked && Globals.Servers.WebServer.Type != WebServers.WSType.none;
            bool startPHP = this.checkPHP.Checked;
            bool startDB = this.checkMySQL.Checked;

            Analysis plan = Compatibility.Analyze(
                startWS ? Compatibility.Active(ComponentKind.WebServer) : null,
                startPHP ? Compatibility.Active(ComponentKind.PHP) : null,
                startDB ? Compatibility.Active(ComponentKind.DataBase) : null);
            plan.Log();

            if (plan.Web != null && plan.HasError(plan.Web)) { startWS = false; Globals.AddToLog("The web server was not started."); }
            if (plan.Php != null && plan.HasError(plan.Php)) { startPHP = false; Globals.AddToLog("PHP was not started."); }
            if (plan.Db != null && plan.HasError(plan.Db)) { startDB = false; Globals.AddToLog("The database was not started."); }

            Globals.WebPhpLink = null;
            Globals.PhpDbLink = null;
            if (startWS && startPHP)
            {
                if (plan.WebPhp != null && plan.WebPhp.Compatible)
                {
                    Globals.WebPhpLink = plan.WebPhp;
                }
                else
                {
                    startPHP = false;
                    Globals.AddToLog("PHP was not started: it cannot work with this web server. The web server starts without PHP.");
                }
            }
            if (startPHP && startDB && plan.PhpDb != null)
            {
                if (plan.PhpDb.Compatible)
                {
                    Globals.PhpDbLink = plan.PhpDb;
                }
                else
                {
                    Globals.AddToLog("The database starts, but this PHP will not be able to use it.");
                }
            }

            if (startWS)
            {
                new WebServerConfig().EnsureDefaultVirtualHost();
            }

            // Then, start selected servers.
            Globals.Processes.InternalPHP = false;
            Globals.Processes.ExternalPHP = false;
            if (startPHP)
            {
                if (String.IsNullOrEmpty(Globals.Servers.PHP.Exe) || !File.Exists(Globals.Servers.PHP.Exe))
                {
                    Globals.AddToLog("PHP was not started: php-cgi.exe or php.exe not found in the selected version folder.");
                }
                else
                {
                    // Without a web server, PHP runs as a FastCGI server for other programs.
                    PhpLink link = (Globals.WebPhpLink != null) ? Globals.WebPhpLink.Link : PhpLink.FastCgi;
                    Globals.Processes.InternalPHP = (link == PhpLink.ModPHP || link == PhpLink.Cgi || link == PhpLink.Fcgid);

                    String PHPDir = Path.GetDirectoryName(Globals.Servers.PHP.Exe) + @"\";
                    if (Globals.RewriteConfigFiles == "1")
                    {
                        new PHPConfigFile(PHPDir).Write();
                    }

                    if (!Globals.Processes.InternalPHP)
                    {
                        Globals.Processes.ExternalPHP = true;
                        if (Globals.Processes.PHPStart(Globals.Servers.PHP.Exe, "-b 127.0.0.1:8866 -c \"" + PHPDir + "php.ini\""))
                        {
                            Process php = Globals.Processes.PHP;
                            php.Exited += (sndr, earg) => OnUI(() =>
                            {
                                // Ignore it if this process was already stopped/replaced.
                                if (Globals.Processes.PHP != php) return;
                                Globals.Processes.PHPStop();
                                this.ChangeTabIcons(2);
                                this.CheckStartStopButtons();
                            });
                        }
                    }
                }
                this.ChangeTabIcons(2);
            }

            if (startWS)
            {
                if (Globals.Processes.WebServerStart(Globals.Servers.WebServer.Exe))
                {
                    Process ws = Globals.Processes.WebServer;
                    ws.Exited += (sndr, earg) => OnUI(() =>
                    {
                        if (Globals.Processes.WebServer != ws) return;
                        Globals.Processes.WebServerStop();
                        this.ChangeTabIcons(1);
                        this.ChangeTabIcons(2);
                        this.CheckStartStopButtons();
                    });
                }
                this.ChangeTabIcons(1);
                this.ChangeTabIcons(2);
            }

            if (startDB)
            {
                if (Globals.Servers.DataBase.Type == DataBases.DBType.none)
                {
                    Globals.AddToLog("The database was not started: no MySQL/MariaDB server found in the selected version folder.");
                }
                else if (Globals.Processes.DataBaseStart(Globals.Servers.DataBase.Exe, Globals.PhpDbLink))
                {
                    Process db = Globals.Processes.DataBase;
                    db.Exited += (sndr, earg) => OnUI(() =>
                    {
                        if (Globals.Processes.DataBase != db) return;
                        Globals.Processes.DataBaseStop();
                        this.ChangeTabIcons(3);
                        this.CheckStartStopButtons();
                    });
                }
                this.ChangeTabIcons(3);
            }

            this.CheckStartStopButtons();

            if (Globals.Processes.RunningCount > 0)
            {
                this.StartTime = DateTime.Now;
                this.UpTimeTimer = new Timer();
                this.UpTimeTimer.Interval = 1000;
                this.UpTimeTimer.Tick += UpTimeTick;
                this.UpTimeTimer.Start();
            }
        }

        private void CheckStartStopButtons()
        {
            bool running = Globals.Processes.RunningCount > 0;
            this.bstop.Enabled = running;
            if (running)
            {
                this.bstart.Enabled = false;
            }
            else
            {
                this.StopUptime();
                this.ChecksActive();
            }
            this.UpdateTrayIcon();
        }

        private void bstop_Click(object sender, EventArgs e)
        {
            this.StopUptime();

            Globals.Processes.WebServerStop();
            Globals.Processes.PHPStop();
            Globals.Processes.DataBaseStop();

            this.ChangeTabIcons(1);
            this.ChangeTabIcons(2);
            this.ChangeTabIcons(3);

            this.CheckStartStopButtons();
        }

        private void UpdateTrayIcon()
        {
            this.NotifyIcon.Icon = (Globals.Processes.RunningCount > 0)
                ? WDS.Properties.Resources.ServerGreen
                : WDS.Properties.Resources.ServerRed;
        }

        private void ChangeTabIcons(int Server)
        {
            // If the form is closed, there is no need to change its images.
            if (Globals.list == null || this.IsDisposed)
            {
                return;
            }

            int PID = -1;
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

            string Status;
            string StatusImg;
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

            this.UpdateTrayIcon();
        }

        private void buttonWebServerVersion_Click(object sender, EventArgs e)
        {
            using (WebServerConfigWindow ConfigWindow = new WebServerConfigWindow())
            {
                ConfigWindow.ShowDialog();
            }
        }

        private void buttonPHPVersion_Click(object sender, EventArgs e)
        {
            using (PHPConfigWindow ConfigWindow = new PHPConfigWindow())
            {
                ConfigWindow.ShowDialog();
            }
        }

        private void buttonMySQLVersion_Click(object sender, EventArgs e)
        {
            using (DatabaseConfigWindow ConfigWindow = new DatabaseConfigWindow())
            {
                ConfigWindow.ShowDialog();
            }
        }

        private void bPreferences_Click(object sender, EventArgs e)
        {
            using (PreferencesWindow Preferences = new PreferencesWindow())
            {
                if (Preferences.ShowDialog() == DialogResult.OK)
                {
                    Globals.StartOnLaunch = Preferences.StartOnLaunch ? "1" : "0";
                    Globals.StartInSystray = Preferences.StartInSystray ? "1" : "0";
                    Globals.RewriteConfigFiles = Preferences.RewriteWhenStart ? "1" : "0";

                    Config.SaveSetting("StartOnLaunch", Globals.StartOnLaunch);
                    Config.SaveSetting("StartInSystray", Globals.StartInSystray);
                    Config.SaveSetting("RewriteConfigFiles", Globals.RewriteConfigFiles);
                }
            }
        }

        private void bWizard_Click(object sender, MouseEventArgs e)
        {
            using (WizardWindow Wizard = new WizardWindow())
            {
                if (Wizard.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                LoadCombos();

                if (Wizard.CheckboxWS.Checked)
                {
                    this.comboWebServerVersion.Text = Wizard.ComboboxWSVersion.Text;
                }
                this.checkWebServer.Checked = Wizard.CheckboxWS.Checked;

                if (Wizard.CheckboxPHP.Checked)
                {
                    this.comboPHPVersion.Text = Wizard.ComboboxPHPVersion.Text;
                }
                this.checkPHP.Checked = Wizard.CheckboxPHP.Checked;

                if (Wizard.CheckboxDB.Checked)
                {
                    this.comboMySQLVersion.Text = Wizard.ComboboxDBVersion.Text;
                }
                this.checkMySQL.Checked = Wizard.CheckboxDB.Checked;
            }
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
            if (!this.IsDisposed)
            {
                this.PublicAddress.Text = (e.Error == null && e.Result != null) ? e.Result.ToString() : "Not available";
            }
        }

        private void GetPublicIPAddress(object sender, DoWorkEventArgs e)
        {
            try
            {
                System.Net.HttpWebRequest req = (System.Net.HttpWebRequest)System.Net.WebRequest.Create("https://icanhazip.com");
                req.Timeout = 5000;
                using (System.Net.WebResponse res = req.GetResponse())
                using (StreamReader objReader = new StreamReader(res.GetResponseStream()))
                {
                    e.Result = objReader.ReadLine();
                }
            }
            catch (Exception)
            {
                e.Result = "Not available";
            }
        }

        private void GetLocalIPAddress(object sender, DoWorkEventArgs e)
        {
            try
            {
                string localIP = "Not available";
                System.Net.IPHostEntry host = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());
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
            catch (Exception)
            {
                e.Result = "Not available";
            }
        }

        private void GetLocalIPDone(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!this.IsDisposed)
            {
                this.LocalAddress.Text = (e.Error == null && e.Result != null) ? e.Result.ToString() : "Not available";
            }
        }

        private void tGithub_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(e.Link.LinkData.ToString());
        }

        private void LogList_DoubleClick(object sender, EventArgs e)
        {
            LogList.Items.Clear();
        }

        private void bInfoPHPFiles_Click(object sender, EventArgs e)
        {
            if (this.PHPFileOpener.Visible)
            {
                this.PHPFileOpener.Hide();
            }
            else
            {
                Button btnSender = (Button)sender;
                System.Drawing.Point ptLowerLeft = new System.Drawing.Point(0, btnSender.Height);
                ptLowerLeft = btnSender.PointToScreen(ptLowerLeft);
                this.PHPFileOpener.Show(ptLowerLeft);
            }
        }
    }
}
