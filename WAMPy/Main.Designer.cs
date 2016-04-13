namespace WDS
{
    partial class Main
    {
        /// <summary>
        /// Variable del diseñador requerida.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén utilizando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido del método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.bMinimize = new System.Windows.Forms.Button();
            this.bPreferences = new System.Windows.Forms.Button();
            this.TabList = new System.Windows.Forms.TabControl();
            this.tabStart = new System.Windows.Forms.TabPage();
            this.LogList = new System.Windows.Forms.ListBox();
            this.StartNetworkBox = new System.Windows.Forms.GroupBox();
            this.Uptime = new System.Windows.Forms.Label();
            this.tUptime = new System.Windows.Forms.Label();
            this.LocalAddress = new System.Windows.Forms.Label();
            this.tLocalAddress = new System.Windows.Forms.Label();
            this.PublicAddress = new System.Windows.Forms.Label();
            this.tPublicAddress = new System.Windows.Forms.Label();
            this.EnabledNetwork = new System.Windows.Forms.Label();
            this.tEnabledNetwork = new System.Windows.Forms.Label();
            this.StartServersBox = new System.Windows.Forms.GroupBox();
            this.bstop = new System.Windows.Forms.Button();
            this.bstart = new System.Windows.Forms.Button();
            this.tDatabaseStatus = new System.Windows.Forms.Label();
            this.pictureMySQL = new System.Windows.Forms.PictureBox();
            this.tPHPStatus = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.picturePHP = new System.Windows.Forms.PictureBox();
            this.label4 = new System.Windows.Forms.Label();
            this.tWebServerStatus = new System.Windows.Forms.Label();
            this.pictureWebServer = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tabWebServer = new System.Windows.Forms.TabPage();
            this.WebServerConfigBox = new System.Windows.Forms.GroupBox();
            this.buttonWebServerVersion = new System.Windows.Forms.Button();
            this.buttonInstallWebServer = new System.Windows.Forms.Button();
            this.tWebServerVersion = new System.Windows.Forms.Label();
            this.comboWebServerVersion = new System.Windows.Forms.ComboBox();
            this.checkWebServer = new System.Windows.Forms.CheckBox();
            this.tabPHP = new System.Windows.Forms.TabPage();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.bInfoPHPFiles = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.InfoPHPVer = new System.Windows.Forms.Label();
            this.tInfoPHPVer = new System.Windows.Forms.Label();
            this.PHPConfigBox = new System.Windows.Forms.GroupBox();
            this.buttonPHPVersion = new System.Windows.Forms.Button();
            this.buttonInstallPHP = new System.Windows.Forms.Button();
            this.tPHPVersion = new System.Windows.Forms.Label();
            this.comboPHPVersion = new System.Windows.Forms.ComboBox();
            this.checkPHP = new System.Windows.Forms.CheckBox();
            this.tabDatabase = new System.Windows.Forms.TabPage();
            this.DataBaseConfigBox = new System.Windows.Forms.GroupBox();
            this.buttonMySQLVersion = new System.Windows.Forms.Button();
            this.buttonInstallMySQL = new System.Windows.Forms.Button();
            this.tMySQLVersion = new System.Windows.Forms.Label();
            this.comboMySQLVersion = new System.Windows.Forms.ComboBox();
            this.checkMySQL = new System.Windows.Forms.CheckBox();
            this.tabAbout = new System.Windows.Forms.TabPage();
            this.tGithub = new System.Windows.Forms.LinkLabel();
            this.label14 = new System.Windows.Forms.Label();
            this.tabIcons = new System.Windows.Forms.ImageList(this.components);
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.NotifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.bWizard = new System.Windows.Forms.Button();
            this.contextMenuFileOpener = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.asdfasToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.phpiniToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.TabList.SuspendLayout();
            this.tabStart.SuspendLayout();
            this.StartNetworkBox.SuspendLayout();
            this.StartServersBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureMySQL)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picturePHP)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureWebServer)).BeginInit();
            this.tabWebServer.SuspendLayout();
            this.WebServerConfigBox.SuspendLayout();
            this.tabPHP.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.PHPConfigBox.SuspendLayout();
            this.tabDatabase.SuspendLayout();
            this.DataBaseConfigBox.SuspendLayout();
            this.tabAbout.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.contextMenuFileOpener.SuspendLayout();
            this.SuspendLayout();
            // 
            // bMinimize
            // 
            this.bMinimize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bMinimize.Location = new System.Drawing.Point(357, 446);
            this.bMinimize.Name = "bMinimize";
            this.bMinimize.Size = new System.Drawing.Size(100, 23);
            this.bMinimize.TabIndex = 11;
            this.bMinimize.Text = "&OK";
            this.bMinimize.UseVisualStyleBackColor = true;
            this.bMinimize.Click += new System.EventHandler(this.OKButton_Click);
            // 
            // bPreferences
            // 
            this.bPreferences.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bPreferences.Location = new System.Drawing.Point(275, 446);
            this.bPreferences.Name = "bPreferences";
            this.bPreferences.Size = new System.Drawing.Size(75, 23);
            this.bPreferences.TabIndex = 10;
            this.bPreferences.Text = "&Preferences";
            this.bPreferences.UseVisualStyleBackColor = true;
            this.bPreferences.Click += new System.EventHandler(this.bPreferences_Click);
            // 
            // TabList
            // 
            this.TabList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TabList.Controls.Add(this.tabStart);
            this.TabList.Controls.Add(this.tabWebServer);
            this.TabList.Controls.Add(this.tabPHP);
            this.TabList.Controls.Add(this.tabDatabase);
            this.TabList.Controls.Add(this.tabAbout);
            this.TabList.ImageList = this.tabIcons;
            this.TabList.ItemSize = new System.Drawing.Size(42, 23);
            this.TabList.Location = new System.Drawing.Point(2, 74);
            this.TabList.Name = "TabList";
            this.TabList.SelectedIndex = 0;
            this.TabList.Size = new System.Drawing.Size(459, 366);
            this.TabList.TabIndex = 8;
            // 
            // tabStart
            // 
            this.tabStart.Controls.Add(this.LogList);
            this.tabStart.Controls.Add(this.StartNetworkBox);
            this.tabStart.Controls.Add(this.StartServersBox);
            this.tabStart.Location = new System.Drawing.Point(4, 27);
            this.tabStart.Name = "tabStart";
            this.tabStart.Padding = new System.Windows.Forms.Padding(3);
            this.tabStart.Size = new System.Drawing.Size(451, 335);
            this.tabStart.TabIndex = 0;
            this.tabStart.Text = "Start";
            this.tabStart.UseVisualStyleBackColor = true;
            // 
            // LogList
            // 
            this.LogList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.LogList.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.LogList.FormattingEnabled = true;
            this.LogList.HorizontalScrollbar = true;
            this.LogList.Location = new System.Drawing.Point(6, 215);
            this.LogList.Name = "LogList";
            this.LogList.Size = new System.Drawing.Size(437, 106);
            this.LogList.TabIndex = 3;
            this.LogList.DoubleClick += new System.EventHandler(this.LogList_DoubleClick);
            // 
            // StartNetworkBox
            // 
            this.StartNetworkBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.StartNetworkBox.Controls.Add(this.Uptime);
            this.StartNetworkBox.Controls.Add(this.tUptime);
            this.StartNetworkBox.Controls.Add(this.LocalAddress);
            this.StartNetworkBox.Controls.Add(this.tLocalAddress);
            this.StartNetworkBox.Controls.Add(this.PublicAddress);
            this.StartNetworkBox.Controls.Add(this.tPublicAddress);
            this.StartNetworkBox.Controls.Add(this.EnabledNetwork);
            this.StartNetworkBox.Controls.Add(this.tEnabledNetwork);
            this.StartNetworkBox.Location = new System.Drawing.Point(6, 121);
            this.StartNetworkBox.Name = "StartNetworkBox";
            this.StartNetworkBox.Size = new System.Drawing.Size(437, 81);
            this.StartNetworkBox.TabIndex = 1;
            this.StartNetworkBox.TabStop = false;
            this.StartNetworkBox.Text = "Network";
            // 
            // Uptime
            // 
            this.Uptime.AutoSize = true;
            this.Uptime.Location = new System.Drawing.Point(143, 26);
            this.Uptime.Name = "Uptime";
            this.Uptime.Size = new System.Drawing.Size(31, 13);
            this.Uptime.TabIndex = 7;
            this.Uptime.Text = "--:--:--";
            // 
            // tUptime
            // 
            this.tUptime.AutoSize = true;
            this.tUptime.Location = new System.Drawing.Point(36, 26);
            this.tUptime.Name = "tUptime";
            this.tUptime.Size = new System.Drawing.Size(46, 13);
            this.tUptime.TabIndex = 6;
            this.tUptime.Text = "Up time:";
            // 
            // LocalAddress
            // 
            this.LocalAddress.AutoSize = true;
            this.LocalAddress.Location = new System.Drawing.Point(324, 49);
            this.LocalAddress.Name = "LocalAddress";
            this.LocalAddress.Size = new System.Drawing.Size(69, 13);
            this.LocalAddress.TabIndex = 5;
            this.LocalAddress.Text = "Not available";
            // 
            // tLocalAddress
            // 
            this.tLocalAddress.AutoSize = true;
            this.tLocalAddress.Location = new System.Drawing.Point(217, 49);
            this.tLocalAddress.Name = "tLocalAddress";
            this.tLocalAddress.Size = new System.Drawing.Size(89, 13);
            this.tLocalAddress.TabIndex = 4;
            this.tLocalAddress.Text = "Local IP address:";
            // 
            // PublicAddress
            // 
            this.PublicAddress.AutoSize = true;
            this.PublicAddress.Location = new System.Drawing.Point(324, 26);
            this.PublicAddress.Name = "PublicAddress";
            this.PublicAddress.Size = new System.Drawing.Size(69, 13);
            this.PublicAddress.TabIndex = 3;
            this.PublicAddress.Text = "Not available";
            // 
            // tPublicAddress
            // 
            this.tPublicAddress.AutoSize = true;
            this.tPublicAddress.Location = new System.Drawing.Point(217, 26);
            this.tPublicAddress.Name = "tPublicAddress";
            this.tPublicAddress.Size = new System.Drawing.Size(92, 13);
            this.tPublicAddress.TabIndex = 2;
            this.tPublicAddress.Text = "Public IP address:";
            // 
            // EnabledNetwork
            // 
            this.EnabledNetwork.AutoSize = true;
            this.EnabledNetwork.Location = new System.Drawing.Point(143, 49);
            this.EnabledNetwork.Name = "EnabledNetwork";
            this.EnabledNetwork.Size = new System.Drawing.Size(21, 13);
            this.EnabledNetwork.TabIndex = 1;
            this.EnabledNetwork.Text = "No";
            // 
            // tEnabledNetwork
            // 
            this.tEnabledNetwork.AutoSize = true;
            this.tEnabledNetwork.Location = new System.Drawing.Point(36, 49);
            this.tEnabledNetwork.Name = "tEnabledNetwork";
            this.tEnabledNetwork.Size = new System.Drawing.Size(91, 13);
            this.tEnabledNetwork.TabIndex = 0;
            this.tEnabledNetwork.Text = "Network enabled:";
            // 
            // StartServersBox
            // 
            this.StartServersBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.StartServersBox.Controls.Add(this.bstop);
            this.StartServersBox.Controls.Add(this.bstart);
            this.StartServersBox.Controls.Add(this.tDatabaseStatus);
            this.StartServersBox.Controls.Add(this.pictureMySQL);
            this.StartServersBox.Controls.Add(this.tPHPStatus);
            this.StartServersBox.Controls.Add(this.label6);
            this.StartServersBox.Controls.Add(this.picturePHP);
            this.StartServersBox.Controls.Add(this.label4);
            this.StartServersBox.Controls.Add(this.tWebServerStatus);
            this.StartServersBox.Controls.Add(this.pictureWebServer);
            this.StartServersBox.Controls.Add(this.label1);
            this.StartServersBox.Location = new System.Drawing.Point(6, 7);
            this.StartServersBox.Name = "StartServersBox";
            this.StartServersBox.Size = new System.Drawing.Size(438, 108);
            this.StartServersBox.TabIndex = 0;
            this.StartServersBox.TabStop = false;
            this.StartServersBox.Text = "Servers";
            // 
            // bstop
            // 
            this.bstop.Enabled = false;
            this.bstop.Location = new System.Drawing.Point(328, 68);
            this.bstop.Name = "bstop";
            this.bstop.Size = new System.Drawing.Size(75, 23);
            this.bstop.TabIndex = 7;
            this.bstop.Text = "S&top";
            this.bstop.UseVisualStyleBackColor = true;
            this.bstop.Click += new System.EventHandler(this.bstop_Click);
            // 
            // bstart
            // 
            this.bstart.Enabled = false;
            this.bstart.Location = new System.Drawing.Point(328, 23);
            this.bstart.Name = "bstart";
            this.bstart.Size = new System.Drawing.Size(75, 39);
            this.bstart.TabIndex = 6;
            this.bstart.Text = "&Start";
            this.bstart.UseVisualStyleBackColor = true;
            this.bstart.Click += new System.EventHandler(this.bstart_Click);
            // 
            // tDatabaseStatus
            // 
            this.tDatabaseStatus.AutoSize = true;
            this.tDatabaseStatus.Location = new System.Drawing.Point(185, 74);
            this.tDatabaseStatus.Name = "tDatabaseStatus";
            this.tDatabaseStatus.Size = new System.Drawing.Size(48, 13);
            this.tDatabaseStatus.TabIndex = 5;
            this.tDatabaseStatus.Text = "Disabled";
            // 
            // pictureMySQL
            // 
            this.pictureMySQL.Image = global::WDS.Properties.Resources.grey;
            this.pictureMySQL.Location = new System.Drawing.Point(172, 75);
            this.pictureMySQL.Name = "pictureMySQL";
            this.pictureMySQL.Size = new System.Drawing.Size(10, 10);
            this.pictureMySQL.TabIndex = 4;
            this.pictureMySQL.TabStop = false;
            // 
            // tPHPStatus
            // 
            this.tPHPStatus.AutoSize = true;
            this.tPHPStatus.Location = new System.Drawing.Point(185, 51);
            this.tPHPStatus.Name = "tPHPStatus";
            this.tPHPStatus.Size = new System.Drawing.Size(48, 13);
            this.tPHPStatus.TabIndex = 5;
            this.tPHPStatus.Text = "Disabled";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(40, 74);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(87, 13);
            this.label6.TabIndex = 3;
            this.label6.Text = "Database status:";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // picturePHP
            // 
            this.picturePHP.Image = global::WDS.Properties.Resources.grey;
            this.picturePHP.Location = new System.Drawing.Point(172, 52);
            this.picturePHP.Name = "picturePHP";
            this.picturePHP.Size = new System.Drawing.Size(10, 10);
            this.picturePHP.TabIndex = 4;
            this.picturePHP.TabStop = false;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(40, 51);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(63, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "PHP status:";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // tWebServerStatus
            // 
            this.tWebServerStatus.AutoSize = true;
            this.tWebServerStatus.Location = new System.Drawing.Point(185, 28);
            this.tWebServerStatus.Name = "tWebServerStatus";
            this.tWebServerStatus.Size = new System.Drawing.Size(48, 13);
            this.tWebServerStatus.TabIndex = 2;
            this.tWebServerStatus.Text = "Disabled";
            // 
            // pictureWebServer
            // 
            this.pictureWebServer.Image = global::WDS.Properties.Resources.grey;
            this.pictureWebServer.Location = new System.Drawing.Point(172, 29);
            this.pictureWebServer.Name = "pictureWebServer";
            this.pictureWebServer.Size = new System.Drawing.Size(10, 10);
            this.pictureWebServer.TabIndex = 1;
            this.pictureWebServer.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(40, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(96, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Web server status:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // tabWebServer
            // 
            this.tabWebServer.Controls.Add(this.WebServerConfigBox);
            this.tabWebServer.ImageKey = "grey";
            this.tabWebServer.Location = new System.Drawing.Point(4, 27);
            this.tabWebServer.Name = "tabWebServer";
            this.tabWebServer.Padding = new System.Windows.Forms.Padding(3);
            this.tabWebServer.Size = new System.Drawing.Size(451, 335);
            this.tabWebServer.TabIndex = 1;
            this.tabWebServer.Text = "Web Server";
            this.tabWebServer.UseVisualStyleBackColor = true;
            // 
            // WebServerConfigBox
            // 
            this.WebServerConfigBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.WebServerConfigBox.Controls.Add(this.buttonWebServerVersion);
            this.WebServerConfigBox.Controls.Add(this.buttonInstallWebServer);
            this.WebServerConfigBox.Controls.Add(this.tWebServerVersion);
            this.WebServerConfigBox.Controls.Add(this.comboWebServerVersion);
            this.WebServerConfigBox.Controls.Add(this.checkWebServer);
            this.WebServerConfigBox.Location = new System.Drawing.Point(6, 7);
            this.WebServerConfigBox.Name = "WebServerConfigBox";
            this.WebServerConfigBox.Size = new System.Drawing.Size(438, 161);
            this.WebServerConfigBox.TabIndex = 0;
            this.WebServerConfigBox.TabStop = false;
            this.WebServerConfigBox.Text = "Configuration";
            // 
            // buttonWebServerVersion
            // 
            this.buttonWebServerVersion.Location = new System.Drawing.Point(133, 92);
            this.buttonWebServerVersion.Name = "buttonWebServerVersion";
            this.buttonWebServerVersion.Size = new System.Drawing.Size(75, 23);
            this.buttonWebServerVersion.TabIndex = 4;
            this.buttonWebServerVersion.Text = "Configure";
            this.buttonWebServerVersion.UseVisualStyleBackColor = true;
            this.buttonWebServerVersion.Click += new System.EventHandler(this.buttonWebServerVersion_Click);
            // 
            // buttonInstallWebServer
            // 
            this.buttonInstallWebServer.Image = global::WDS.Properties.Resources.plusB12;
            this.buttonInstallWebServer.Location = new System.Drawing.Point(340, 63);
            this.buttonInstallWebServer.Name = "buttonInstallWebServer";
            this.buttonInstallWebServer.Size = new System.Drawing.Size(23, 23);
            this.buttonInstallWebServer.TabIndex = 3;
            this.buttonInstallWebServer.UseVisualStyleBackColor = true;
            this.buttonInstallWebServer.Click += new System.EventHandler(this.buttonInstallWebServer_Click);
            // 
            // tWebServerVersion
            // 
            this.tWebServerVersion.AutoSize = true;
            this.tWebServerVersion.Location = new System.Drawing.Point(16, 67);
            this.tWebServerVersion.Name = "tWebServerVersion";
            this.tWebServerVersion.Size = new System.Drawing.Size(102, 13);
            this.tWebServerVersion.TabIndex = 2;
            this.tWebServerVersion.Text = "Web server version:";
            // 
            // comboWebServerVersion
            // 
            this.comboWebServerVersion.FormattingEnabled = true;
            this.comboWebServerVersion.Location = new System.Drawing.Point(133, 64);
            this.comboWebServerVersion.Name = "comboWebServerVersion";
            this.comboWebServerVersion.Size = new System.Drawing.Size(205, 21);
            this.comboWebServerVersion.TabIndex = 1;
            this.comboWebServerVersion.SelectedIndexChanged += new System.EventHandler(this.comboWebServerVersion_SelectedIndexChanged);
            this.comboWebServerVersion.TextUpdate += new System.EventHandler(this.comboWebServerVersion_TextUpdate);
            // 
            // checkWebServer
            // 
            this.checkWebServer.AutoSize = true;
            this.checkWebServer.Location = new System.Drawing.Point(165, 28);
            this.checkWebServer.Name = "checkWebServer";
            this.checkWebServer.Size = new System.Drawing.Size(117, 17);
            this.checkWebServer.TabIndex = 0;
            this.checkWebServer.Text = "Enable Web server";
            this.checkWebServer.UseVisualStyleBackColor = true;
            this.checkWebServer.CheckedChanged += new System.EventHandler(this.checkWebServer_CheckedChanged);
            // 
            // tabPHP
            // 
            this.tabPHP.Controls.Add(this.groupBox1);
            this.tabPHP.Controls.Add(this.PHPConfigBox);
            this.tabPHP.ImageKey = "grey";
            this.tabPHP.Location = new System.Drawing.Point(4, 27);
            this.tabPHP.Name = "tabPHP";
            this.tabPHP.Size = new System.Drawing.Size(451, 335);
            this.tabPHP.TabIndex = 2;
            this.tabPHP.Text = "PHP";
            this.tabPHP.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.bInfoPHPFiles);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.InfoPHPVer);
            this.groupBox1.Controls.Add(this.tInfoPHPVer);
            this.groupBox1.Location = new System.Drawing.Point(7, 171);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(437, 158);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "groupBox1";
            // 
            // bInfoPHPFiles
            // 
            this.bInfoPHPFiles.Image = ((System.Drawing.Image)(resources.GetObject("bInfoPHPFiles.Image")));
            this.bInfoPHPFiles.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bInfoPHPFiles.Location = new System.Drawing.Point(10, 98);
            this.bInfoPHPFiles.Name = "bInfoPHPFiles";
            this.bInfoPHPFiles.Size = new System.Drawing.Size(124, 23);
            this.bInfoPHPFiles.TabIndex = 6;
            this.bInfoPHPFiles.Text = "Access files";
            this.bInfoPHPFiles.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.bInfoPHPFiles.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            this.bInfoPHPFiles.UseVisualStyleBackColor = true;
            this.bInfoPHPFiles.Click += new System.EventHandler(this.bInfoPHPFiles_Click);
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(90, 76);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(44, 18);
            this.label5.TabIndex = 5;
            this.label5.Text = "VC11";
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(7, 76);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(77, 18);
            this.label7.TabIndex = 4;
            this.label7.Text = "Compiler:";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(90, 53);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(44, 18);
            this.label2.TabIndex = 3;
            this.label2.Text = "64 bits";
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(7, 53);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(77, 18);
            this.label3.TabIndex = 2;
            this.label3.Text = "Architecture:";
            // 
            // InfoPHPVer
            // 
            this.InfoPHPVer.Location = new System.Drawing.Point(90, 30);
            this.InfoPHPVer.Name = "InfoPHPVer";
            this.InfoPHPVer.Size = new System.Drawing.Size(44, 18);
            this.InfoPHPVer.TabIndex = 1;
            this.InfoPHPVer.Text = "5.5.1";
            // 
            // tInfoPHPVer
            // 
            this.tInfoPHPVer.Location = new System.Drawing.Point(7, 30);
            this.tInfoPHPVer.Name = "tInfoPHPVer";
            this.tInfoPHPVer.Size = new System.Drawing.Size(77, 18);
            this.tInfoPHPVer.TabIndex = 0;
            this.tInfoPHPVer.Text = "PHP Version:";
            // 
            // PHPConfigBox
            // 
            this.PHPConfigBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PHPConfigBox.Controls.Add(this.buttonPHPVersion);
            this.PHPConfigBox.Controls.Add(this.buttonInstallPHP);
            this.PHPConfigBox.Controls.Add(this.tPHPVersion);
            this.PHPConfigBox.Controls.Add(this.comboPHPVersion);
            this.PHPConfigBox.Controls.Add(this.checkPHP);
            this.PHPConfigBox.Location = new System.Drawing.Point(6, 7);
            this.PHPConfigBox.Name = "PHPConfigBox";
            this.PHPConfigBox.Size = new System.Drawing.Size(438, 161);
            this.PHPConfigBox.TabIndex = 1;
            this.PHPConfigBox.TabStop = false;
            this.PHPConfigBox.Text = "Configuration";
            // 
            // buttonPHPVersion
            // 
            this.buttonPHPVersion.Location = new System.Drawing.Point(133, 92);
            this.buttonPHPVersion.Name = "buttonPHPVersion";
            this.buttonPHPVersion.Size = new System.Drawing.Size(75, 23);
            this.buttonPHPVersion.TabIndex = 4;
            this.buttonPHPVersion.Text = "Configure";
            this.buttonPHPVersion.UseVisualStyleBackColor = true;
            this.buttonPHPVersion.Click += new System.EventHandler(this.buttonPHPVersion_Click);
            // 
            // buttonInstallPHP
            // 
            this.buttonInstallPHP.Image = global::WDS.Properties.Resources.plusB12;
            this.buttonInstallPHP.Location = new System.Drawing.Point(340, 63);
            this.buttonInstallPHP.Name = "buttonInstallPHP";
            this.buttonInstallPHP.Size = new System.Drawing.Size(23, 23);
            this.buttonInstallPHP.TabIndex = 3;
            this.buttonInstallPHP.UseVisualStyleBackColor = true;
            this.buttonInstallPHP.Click += new System.EventHandler(this.buttonInstallPHP_Click);
            // 
            // tPHPVersion
            // 
            this.tPHPVersion.AutoSize = true;
            this.tPHPVersion.Location = new System.Drawing.Point(16, 67);
            this.tPHPVersion.Name = "tPHPVersion";
            this.tPHPVersion.Size = new System.Drawing.Size(69, 13);
            this.tPHPVersion.TabIndex = 2;
            this.tPHPVersion.Text = "PHP version:";
            // 
            // comboPHPVersion
            // 
            this.comboPHPVersion.FormattingEnabled = true;
            this.comboPHPVersion.Location = new System.Drawing.Point(133, 64);
            this.comboPHPVersion.Name = "comboPHPVersion";
            this.comboPHPVersion.Size = new System.Drawing.Size(205, 21);
            this.comboPHPVersion.TabIndex = 1;
            this.comboPHPVersion.SelectedIndexChanged += new System.EventHandler(this.comboPHPVersion_SelectedIndexChanged);
            this.comboPHPVersion.TextUpdate += new System.EventHandler(this.comboPHPVersion_TextUpdate);
            // 
            // checkPHP
            // 
            this.checkPHP.AutoSize = true;
            this.checkPHP.Location = new System.Drawing.Point(165, 28);
            this.checkPHP.Name = "checkPHP";
            this.checkPHP.Size = new System.Drawing.Size(140, 17);
            this.checkPHP.TabIndex = 0;
            this.checkPHP.Text = "Enable PHP component";
            this.checkPHP.UseVisualStyleBackColor = true;
            this.checkPHP.CheckedChanged += new System.EventHandler(this.checkPHP_CheckedChanged);
            // 
            // tabDatabase
            // 
            this.tabDatabase.Controls.Add(this.DataBaseConfigBox);
            this.tabDatabase.ImageKey = "grey";
            this.tabDatabase.Location = new System.Drawing.Point(4, 27);
            this.tabDatabase.Name = "tabDatabase";
            this.tabDatabase.Size = new System.Drawing.Size(451, 335);
            this.tabDatabase.TabIndex = 3;
            this.tabDatabase.Text = "Database";
            this.tabDatabase.UseVisualStyleBackColor = true;
            // 
            // DataBaseConfigBox
            // 
            this.DataBaseConfigBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.DataBaseConfigBox.Controls.Add(this.buttonMySQLVersion);
            this.DataBaseConfigBox.Controls.Add(this.buttonInstallMySQL);
            this.DataBaseConfigBox.Controls.Add(this.tMySQLVersion);
            this.DataBaseConfigBox.Controls.Add(this.comboMySQLVersion);
            this.DataBaseConfigBox.Controls.Add(this.checkMySQL);
            this.DataBaseConfigBox.Location = new System.Drawing.Point(6, 7);
            this.DataBaseConfigBox.Name = "DataBaseConfigBox";
            this.DataBaseConfigBox.Size = new System.Drawing.Size(438, 161);
            this.DataBaseConfigBox.TabIndex = 1;
            this.DataBaseConfigBox.TabStop = false;
            this.DataBaseConfigBox.Text = "Configuration";
            // 
            // buttonMySQLVersion
            // 
            this.buttonMySQLVersion.Location = new System.Drawing.Point(133, 92);
            this.buttonMySQLVersion.Name = "buttonMySQLVersion";
            this.buttonMySQLVersion.Size = new System.Drawing.Size(75, 23);
            this.buttonMySQLVersion.TabIndex = 4;
            this.buttonMySQLVersion.Text = "Configure";
            this.buttonMySQLVersion.UseVisualStyleBackColor = true;
            this.buttonMySQLVersion.Click += new System.EventHandler(this.buttonMySQLVersion_Click);
            // 
            // buttonInstallMySQL
            // 
            this.buttonInstallMySQL.Image = global::WDS.Properties.Resources.plusB12;
            this.buttonInstallMySQL.Location = new System.Drawing.Point(340, 63);
            this.buttonInstallMySQL.Name = "buttonInstallMySQL";
            this.buttonInstallMySQL.Size = new System.Drawing.Size(23, 23);
            this.buttonInstallMySQL.TabIndex = 3;
            this.buttonInstallMySQL.UseVisualStyleBackColor = true;
            this.buttonInstallMySQL.Click += new System.EventHandler(this.buttonInstallMySQL_Click);
            // 
            // tMySQLVersion
            // 
            this.tMySQLVersion.AutoSize = true;
            this.tMySQLVersion.Location = new System.Drawing.Point(16, 67);
            this.tMySQLVersion.Name = "tMySQLVersion";
            this.tMySQLVersion.Size = new System.Drawing.Size(93, 13);
            this.tMySQLVersion.TabIndex = 2;
            this.tMySQLVersion.Text = "Database version:";
            // 
            // comboMySQLVersion
            // 
            this.comboMySQLVersion.FormattingEnabled = true;
            this.comboMySQLVersion.Location = new System.Drawing.Point(133, 64);
            this.comboMySQLVersion.Name = "comboMySQLVersion";
            this.comboMySQLVersion.Size = new System.Drawing.Size(205, 21);
            this.comboMySQLVersion.TabIndex = 1;
            this.comboMySQLVersion.SelectedIndexChanged += new System.EventHandler(this.comboMySQLVersion_SelectedIndexChanged);
            this.comboMySQLVersion.TextUpdate += new System.EventHandler(this.comboMySQLVersion_TextUpdate);
            // 
            // checkMySQL
            // 
            this.checkMySQL.AutoSize = true;
            this.checkMySQL.Location = new System.Drawing.Point(165, 28);
            this.checkMySQL.Name = "checkMySQL";
            this.checkMySQL.Size = new System.Drawing.Size(140, 17);
            this.checkMySQL.TabIndex = 0;
            this.checkMySQL.Text = "Enable Database server";
            this.checkMySQL.UseVisualStyleBackColor = true;
            this.checkMySQL.CheckedChanged += new System.EventHandler(this.checkMySQL_CheckedChanged);
            // 
            // tabAbout
            // 
            this.tabAbout.Controls.Add(this.tGithub);
            this.tabAbout.Controls.Add(this.label14);
            this.tabAbout.Location = new System.Drawing.Point(4, 27);
            this.tabAbout.Name = "tabAbout";
            this.tabAbout.Size = new System.Drawing.Size(451, 335);
            this.tabAbout.TabIndex = 4;
            this.tabAbout.Text = "About";
            this.tabAbout.UseVisualStyleBackColor = true;
            // 
            // tGithub
            // 
            this.tGithub.AutoSize = true;
            this.tGithub.Location = new System.Drawing.Point(145, 70);
            this.tGithub.Name = "tGithub";
            this.tGithub.Size = new System.Drawing.Size(134, 13);
            this.tGithub.TabIndex = 1;
            this.tGithub.TabStop = true;
            this.tGithub.Text = "https://github.com/mongui";
            this.tGithub.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.tGithub_LinkClicked);
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(51, 26);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(342, 91);
            this.label14.TabIndex = 0;
            this.label14.Text = "This is an open-source software licensed under Apache 2.0 License.\r\nIf you want t" +
    "o contribute, feel free to fork this project from my repository:\r\n\r\n\r\n\r\n\r\nWeb De" +
    "velopment Server © 2013 - 2014";
            // 
            // tabIcons
            // 
            this.tabIcons.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("tabIcons.ImageStream")));
            this.tabIcons.TransparentColor = System.Drawing.Color.Transparent;
            this.tabIcons.Images.SetKeyName(0, "blue");
            this.tabIcons.Images.SetKeyName(1, "green");
            this.tabIcons.Images.SetKeyName(2, "grey");
            this.tabIcons.Images.SetKeyName(3, "orange");
            this.tabIcons.Images.SetKeyName(4, "red");
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(474, 70);
            this.pictureBox1.TabIndex = 9;
            this.pictureBox1.TabStop = false;
            // 
            // NotifyIcon
            // 
            this.NotifyIcon.Text = "Web Development Server";
            this.NotifyIcon.Visible = true;
            this.NotifyIcon.DoubleClick += new System.EventHandler(this.NotifyIcon_DoubleClick);
            // 
            // bWizard
            // 
            this.bWizard.Image = ((System.Drawing.Image)(resources.GetObject("bWizard.Image")));
            this.bWizard.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.bWizard.Location = new System.Drawing.Point(6, 446);
            this.bWizard.Name = "bWizard";
            this.bWizard.Size = new System.Drawing.Size(102, 23);
            this.bWizard.TabIndex = 12;
            this.bWizard.Text = "&Wizard";
            this.bWizard.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.bWizard.UseVisualStyleBackColor = true;
            this.bWizard.MouseClick += new System.Windows.Forms.MouseEventHandler(this.bWizard_Click);
            // 
            // contextMenuFileOpener
            // 
            this.contextMenuFileOpener.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.asdfasToolStripMenuItem,
            this.toolStripMenuItem1,
            this.phpiniToolStripMenuItem});
            this.contextMenuFileOpener.Name = "contextMenuFileOpener";
            this.contextMenuFileOpener.Size = new System.Drawing.Size(153, 76);
            // 
            // asdfasToolStripMenuItem
            // 
            this.asdfasToolStripMenuItem.Name = "asdfasToolStripMenuItem";
            this.asdfasToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.asdfasToolStripMenuItem.Text = "error_log";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(149, 6);
            // 
            // phpiniToolStripMenuItem
            // 
            this.phpiniToolStripMenuItem.Name = "phpiniToolStripMenuItem";
            this.phpiniToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.phpiniToolStripMenuItem.Text = "php.ini";
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(463, 473);
            this.Controls.Add(this.bWizard);
            this.Controls.Add(this.bMinimize);
            this.Controls.Add(this.bPreferences);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.TabList);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = global::WDS.Properties.Resources.Server;
            this.MaximizeBox = false;
            this.Name = "Main";
            this.Text = "Web Development Server";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Main_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Main_FormClosed);
            this.Load += new System.EventHandler(this.Main_Load);
            this.Resize += new System.EventHandler(this.MinimizeButton_Click);
            this.TabList.ResumeLayout(false);
            this.tabStart.ResumeLayout(false);
            this.StartNetworkBox.ResumeLayout(false);
            this.StartNetworkBox.PerformLayout();
            this.StartServersBox.ResumeLayout(false);
            this.StartServersBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureMySQL)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picturePHP)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureWebServer)).EndInit();
            this.tabWebServer.ResumeLayout(false);
            this.WebServerConfigBox.ResumeLayout(false);
            this.WebServerConfigBox.PerformLayout();
            this.tabPHP.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.PHPConfigBox.ResumeLayout(false);
            this.PHPConfigBox.PerformLayout();
            this.tabDatabase.ResumeLayout(false);
            this.DataBaseConfigBox.ResumeLayout(false);
            this.DataBaseConfigBox.PerformLayout();
            this.tabAbout.ResumeLayout(false);
            this.tabAbout.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.contextMenuFileOpener.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button bMinimize;
        private System.Windows.Forms.Button bPreferences;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.TabControl TabList;
        private System.Windows.Forms.TabPage tabStart;
        private System.Windows.Forms.GroupBox StartNetworkBox;
        private System.Windows.Forms.Label LocalAddress;
        private System.Windows.Forms.Label tLocalAddress;
        private System.Windows.Forms.Label PublicAddress;
        private System.Windows.Forms.Label tPublicAddress;
        private System.Windows.Forms.Label EnabledNetwork;
        private System.Windows.Forms.Label tEnabledNetwork;
        private System.Windows.Forms.GroupBox StartServersBox;
        private System.Windows.Forms.Button bstop;
        private System.Windows.Forms.Button bstart;
        private System.Windows.Forms.Label tDatabaseStatus;
        private System.Windows.Forms.PictureBox pictureMySQL;
        private System.Windows.Forms.Label tPHPStatus;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.PictureBox picturePHP;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label tWebServerStatus;
        private System.Windows.Forms.PictureBox pictureWebServer;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TabPage tabWebServer;
        private System.Windows.Forms.GroupBox WebServerConfigBox;
        private System.Windows.Forms.Button buttonWebServerVersion;
        private System.Windows.Forms.Button buttonInstallWebServer;
        private System.Windows.Forms.Label tWebServerVersion;
        private System.Windows.Forms.ComboBox comboWebServerVersion;
        private System.Windows.Forms.CheckBox checkWebServer;
        private System.Windows.Forms.TabPage tabPHP;
        private System.Windows.Forms.TabPage tabDatabase;
        private System.Windows.Forms.TabPage tabAbout;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.GroupBox PHPConfigBox;
        private System.Windows.Forms.Button buttonPHPVersion;
        private System.Windows.Forms.Button buttonInstallPHP;
        private System.Windows.Forms.Label tPHPVersion;
        private System.Windows.Forms.ComboBox comboPHPVersion;
        private System.Windows.Forms.CheckBox checkPHP;
        private System.Windows.Forms.GroupBox DataBaseConfigBox;
        private System.Windows.Forms.Button buttonMySQLVersion;
        private System.Windows.Forms.Button buttonInstallMySQL;
        private System.Windows.Forms.Label tMySQLVersion;
        private System.Windows.Forms.ComboBox comboMySQLVersion;
        private System.Windows.Forms.CheckBox checkMySQL;
        private System.Windows.Forms.ImageList tabIcons;
        private System.Windows.Forms.NotifyIcon NotifyIcon;
        private System.Windows.Forms.LinkLabel tGithub;
        private System.Windows.Forms.ListBox LogList;
        private System.Windows.Forms.Label Uptime;
        private System.Windows.Forms.Label tUptime;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button bWizard;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label InfoPHPVer;
        private System.Windows.Forms.Label tInfoPHPVer;
        private System.Windows.Forms.ContextMenuStrip contextMenuFileOpener;
        private System.Windows.Forms.ToolStripMenuItem asdfasToolStripMenuItem;
        private System.Windows.Forms.Button bInfoPHPFiles;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem phpiniToolStripMenuItem;

    }
}

