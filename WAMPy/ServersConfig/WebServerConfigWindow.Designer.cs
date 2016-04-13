namespace WDS
{
    partial class WebServerConfigWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tabsWebServer = new System.Windows.Forms.TabControl();
            this.tabVServer = new System.Windows.Forms.TabPage();
            this.RemoveServer = new System.Windows.Forms.Button();
            this.AddServer = new System.Windows.Forms.Button();
            this.VirtualServers = new System.Windows.Forms.GroupBox();
            this.SelectFolder = new System.Windows.Forms.Button();
            this.Port = new System.Windows.Forms.NumericUpDown();
            this.Others = new System.Windows.Forms.TextBox();
            this.tOthers = new System.Windows.Forms.Label();
            this.RemoveFolder = new System.Windows.Forms.Button();
            this.AddFolder = new System.Windows.Forms.Button();
            this.Folders = new System.Windows.Forms.ListBox();
            this.tFolders = new System.Windows.Forms.Label();
            this.RemoveAlias = new System.Windows.Forms.Button();
            this.AddAlias = new System.Windows.Forms.Button();
            this.Alias = new System.Windows.Forms.ListBox();
            this.tAlias = new System.Windows.Forms.Label();
            this.DocumentRoot = new System.Windows.Forms.TextBox();
            this.tDocumentRoot = new System.Windows.Forms.Label();
            this.ServerAlias = new System.Windows.Forms.TextBox();
            this.tServerAlias = new System.Windows.Forms.Label();
            this.ServerName = new System.Windows.Forms.TextBox();
            this.tServerName = new System.Windows.Forms.Label();
            this.tPort = new System.Windows.Forms.Label();
            this.HostIP = new System.Windows.Forms.TextBox();
            this.tHostIP = new System.Windows.Forms.Label();
            this.ServerList = new System.Windows.Forms.ListBox();
            this.tabModules = new System.Windows.Forms.TabPage();
            this.groupModuleConfig = new System.Windows.Forms.GroupBox();
            this.tExtraConfig = new System.Windows.Forms.Label();
            this.ExtraConfig = new System.Windows.Forms.TextBox();
            this.ModuleConfIfNotActive = new System.Windows.Forms.RadioButton();
            this.ModuleConfIfActive = new System.Windows.Forms.RadioButton();
            this.ModuleConfDisabled = new System.Windows.Forms.RadioButton();
            this.groupModuleInfo = new System.Windows.Forms.GroupBox();
            this.checkedListModules = new System.Windows.Forms.CheckedListBox();
            this.tabCustom = new System.Windows.Forms.TabPage();
            this.CustomSetting = new System.Windows.Forms.GroupBox();
            this.tAddEdit = new System.Windows.Forms.Label();
            this.CustomEdit = new System.Windows.Forms.TextBox();
            this.RemoveCustom = new System.Windows.Forms.Button();
            this.AddCustom = new System.Windows.Forms.Button();
            this.CustomList = new System.Windows.Forms.ListBox();
            this.ConfigOK = new System.Windows.Forms.Button();
            this.ModulesInfo = new System.Windows.Forms.TextBox();
            this.ModulesLink = new System.Windows.Forms.LinkLabel();
            this.tabsWebServer.SuspendLayout();
            this.tabVServer.SuspendLayout();
            this.VirtualServers.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Port)).BeginInit();
            this.tabModules.SuspendLayout();
            this.groupModuleConfig.SuspendLayout();
            this.groupModuleInfo.SuspendLayout();
            this.tabCustom.SuspendLayout();
            this.CustomSetting.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabsWebServer
            // 
            this.tabsWebServer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabsWebServer.Controls.Add(this.tabVServer);
            this.tabsWebServer.Controls.Add(this.tabModules);
            this.tabsWebServer.Controls.Add(this.tabCustom);
            this.tabsWebServer.Location = new System.Drawing.Point(12, 6);
            this.tabsWebServer.Name = "tabsWebServer";
            this.tabsWebServer.SelectedIndex = 0;
            this.tabsWebServer.Size = new System.Drawing.Size(523, 491);
            this.tabsWebServer.TabIndex = 0;
            // 
            // tabVServer
            // 
            this.tabVServer.Controls.Add(this.RemoveServer);
            this.tabVServer.Controls.Add(this.AddServer);
            this.tabVServer.Controls.Add(this.VirtualServers);
            this.tabVServer.Controls.Add(this.ServerList);
            this.tabVServer.Location = new System.Drawing.Point(4, 22);
            this.tabVServer.Name = "tabVServer";
            this.tabVServer.Padding = new System.Windows.Forms.Padding(3);
            this.tabVServer.Size = new System.Drawing.Size(515, 465);
            this.tabVServer.TabIndex = 0;
            this.tabVServer.Text = "Virtual Server";
            this.tabVServer.UseVisualStyleBackColor = true;
            // 
            // RemoveServer
            // 
            this.RemoveServer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.RemoveServer.Enabled = false;
            this.RemoveServer.Location = new System.Drawing.Point(137, 434);
            this.RemoveServer.Name = "RemoveServer";
            this.RemoveServer.Size = new System.Drawing.Size(75, 23);
            this.RemoveServer.TabIndex = 3;
            this.RemoveServer.Text = "-";
            this.RemoveServer.UseVisualStyleBackColor = true;
            this.RemoveServer.Click += new System.EventHandler(this.RemoveServer_Click);
            // 
            // AddServer
            // 
            this.AddServer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.AddServer.Location = new System.Drawing.Point(6, 434);
            this.AddServer.Name = "AddServer";
            this.AddServer.Size = new System.Drawing.Size(75, 23);
            this.AddServer.TabIndex = 2;
            this.AddServer.Text = "+";
            this.AddServer.UseVisualStyleBackColor = true;
            this.AddServer.Click += new System.EventHandler(this.AddServer_Click);
            // 
            // VirtualServers
            // 
            this.VirtualServers.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.VirtualServers.Controls.Add(this.SelectFolder);
            this.VirtualServers.Controls.Add(this.Port);
            this.VirtualServers.Controls.Add(this.Others);
            this.VirtualServers.Controls.Add(this.tOthers);
            this.VirtualServers.Controls.Add(this.RemoveFolder);
            this.VirtualServers.Controls.Add(this.AddFolder);
            this.VirtualServers.Controls.Add(this.Folders);
            this.VirtualServers.Controls.Add(this.tFolders);
            this.VirtualServers.Controls.Add(this.RemoveAlias);
            this.VirtualServers.Controls.Add(this.AddAlias);
            this.VirtualServers.Controls.Add(this.Alias);
            this.VirtualServers.Controls.Add(this.tAlias);
            this.VirtualServers.Controls.Add(this.DocumentRoot);
            this.VirtualServers.Controls.Add(this.tDocumentRoot);
            this.VirtualServers.Controls.Add(this.ServerAlias);
            this.VirtualServers.Controls.Add(this.tServerAlias);
            this.VirtualServers.Controls.Add(this.ServerName);
            this.VirtualServers.Controls.Add(this.tServerName);
            this.VirtualServers.Controls.Add(this.tPort);
            this.VirtualServers.Controls.Add(this.HostIP);
            this.VirtualServers.Controls.Add(this.tHostIP);
            this.VirtualServers.Location = new System.Drawing.Point(221, 7);
            this.VirtualServers.Name = "VirtualServers";
            this.VirtualServers.Size = new System.Drawing.Size(284, 452);
            this.VirtualServers.TabIndex = 1;
            this.VirtualServers.TabStop = false;
            this.VirtualServers.Text = "Virtual Server";
            // 
            // SelectFolder
            // 
            this.SelectFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.SelectFolder.Image = global::WDS.Properties.Resources.folder;
            this.SelectFolder.Location = new System.Drawing.Point(246, 119);
            this.SelectFolder.Name = "SelectFolder";
            this.SelectFolder.Size = new System.Drawing.Size(24, 22);
            this.SelectFolder.TabIndex = 24;
            this.SelectFolder.UseVisualStyleBackColor = true;
            this.SelectFolder.Click += new System.EventHandler(this.SelectFolder_Click);
            // 
            // Port
            // 
            this.Port.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Port.Location = new System.Drawing.Point(220, 26);
            this.Port.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.Port.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.Port.Name = "Port";
            this.Port.Size = new System.Drawing.Size(49, 20);
            this.Port.TabIndex = 23;
            this.Port.Value = new decimal(new int[] {
            80,
            0,
            0,
            0});
            // 
            // Others
            // 
            this.Others.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Others.Location = new System.Drawing.Point(101, 345);
            this.Others.Multiline = true;
            this.Others.Name = "Others";
            this.Others.Size = new System.Drawing.Size(168, 91);
            this.Others.TabIndex = 22;
            // 
            // tOthers
            // 
            this.tOthers.AutoSize = true;
            this.tOthers.Location = new System.Drawing.Point(10, 345);
            this.tOthers.Name = "tOthers";
            this.tOthers.Size = new System.Drawing.Size(41, 13);
            this.tOthers.TabIndex = 18;
            this.tOthers.Text = "Others:";
            // 
            // RemoveFolder
            // 
            this.RemoveFolder.Enabled = false;
            this.RemoveFolder.Location = new System.Drawing.Point(66, 306);
            this.RemoveFolder.Name = "RemoveFolder";
            this.RemoveFolder.Size = new System.Drawing.Size(29, 24);
            this.RemoveFolder.TabIndex = 17;
            this.RemoveFolder.Text = "-";
            this.RemoveFolder.UseVisualStyleBackColor = true;
            this.RemoveFolder.Click += new System.EventHandler(this.RemoveFolder_Click);
            // 
            // AddFolder
            // 
            this.AddFolder.Enabled = false;
            this.AddFolder.Location = new System.Drawing.Point(13, 306);
            this.AddFolder.Name = "AddFolder";
            this.AddFolder.Size = new System.Drawing.Size(29, 23);
            this.AddFolder.TabIndex = 16;
            this.AddFolder.Text = "+";
            this.AddFolder.UseVisualStyleBackColor = true;
            this.AddFolder.Click += new System.EventHandler(this.AddFolder_Click);
            // 
            // Folders
            // 
            this.Folders.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Folders.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Folders.FormattingEnabled = true;
            this.Folders.Location = new System.Drawing.Point(101, 248);
            this.Folders.Name = "Folders";
            this.Folders.Size = new System.Drawing.Size(168, 80);
            this.Folders.TabIndex = 15;
            this.Folders.SelectedIndexChanged += new System.EventHandler(this.Folders_SelectedIndexChanged);
            this.Folders.DoubleClick += new System.EventHandler(this.Folders_DoubleClick);
            // 
            // tFolders
            // 
            this.tFolders.AutoSize = true;
            this.tFolders.Location = new System.Drawing.Point(10, 248);
            this.tFolders.Name = "tFolders";
            this.tFolders.Size = new System.Drawing.Size(44, 13);
            this.tFolders.TabIndex = 14;
            this.tFolders.Text = "Folders:";
            // 
            // RemoveAlias
            // 
            this.RemoveAlias.Enabled = false;
            this.RemoveAlias.Location = new System.Drawing.Point(66, 210);
            this.RemoveAlias.Name = "RemoveAlias";
            this.RemoveAlias.Size = new System.Drawing.Size(29, 24);
            this.RemoveAlias.TabIndex = 13;
            this.RemoveAlias.Text = "-";
            this.RemoveAlias.UseVisualStyleBackColor = true;
            this.RemoveAlias.Click += new System.EventHandler(this.RemoveAlias_Click);
            // 
            // AddAlias
            // 
            this.AddAlias.Enabled = false;
            this.AddAlias.Location = new System.Drawing.Point(13, 210);
            this.AddAlias.Name = "AddAlias";
            this.AddAlias.Size = new System.Drawing.Size(29, 23);
            this.AddAlias.TabIndex = 12;
            this.AddAlias.Text = "+";
            this.AddAlias.UseVisualStyleBackColor = true;
            this.AddAlias.Click += new System.EventHandler(this.AddAlias_Click);
            // 
            // Alias
            // 
            this.Alias.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Alias.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Alias.FormattingEnabled = true;
            this.Alias.Location = new System.Drawing.Point(101, 152);
            this.Alias.Name = "Alias";
            this.Alias.Size = new System.Drawing.Size(168, 80);
            this.Alias.TabIndex = 11;
            this.Alias.SelectedIndexChanged += new System.EventHandler(this.Alias_SelectedIndexChanged);
            this.Alias.DoubleClick += new System.EventHandler(this.Alias_DoubleClick);
            // 
            // tAlias
            // 
            this.tAlias.AutoSize = true;
            this.tAlias.Location = new System.Drawing.Point(10, 152);
            this.tAlias.Name = "tAlias";
            this.tAlias.Size = new System.Drawing.Size(32, 13);
            this.tAlias.TabIndex = 10;
            this.tAlias.Text = "Alias:";
            // 
            // DocumentRoot
            // 
            this.DocumentRoot.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.DocumentRoot.Location = new System.Drawing.Point(101, 120);
            this.DocumentRoot.Name = "DocumentRoot";
            this.DocumentRoot.Size = new System.Drawing.Size(144, 20);
            this.DocumentRoot.TabIndex = 9;
            // 
            // tDocumentRoot
            // 
            this.tDocumentRoot.AutoSize = true;
            this.tDocumentRoot.Location = new System.Drawing.Point(10, 123);
            this.tDocumentRoot.Name = "tDocumentRoot";
            this.tDocumentRoot.Size = new System.Drawing.Size(85, 13);
            this.tDocumentRoot.TabIndex = 8;
            this.tDocumentRoot.Text = "Document Root:";
            // 
            // ServerAlias
            // 
            this.ServerAlias.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ServerAlias.Location = new System.Drawing.Point(101, 88);
            this.ServerAlias.Name = "ServerAlias";
            this.ServerAlias.Size = new System.Drawing.Size(168, 20);
            this.ServerAlias.TabIndex = 7;
            // 
            // tServerAlias
            // 
            this.tServerAlias.AutoSize = true;
            this.tServerAlias.Location = new System.Drawing.Point(10, 91);
            this.tServerAlias.Name = "tServerAlias";
            this.tServerAlias.Size = new System.Drawing.Size(66, 13);
            this.tServerAlias.TabIndex = 6;
            this.tServerAlias.Text = "Server Alias:";
            // 
            // ServerName
            // 
            this.ServerName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ServerName.Location = new System.Drawing.Point(101, 56);
            this.ServerName.Name = "ServerName";
            this.ServerName.Size = new System.Drawing.Size(168, 20);
            this.ServerName.TabIndex = 5;
            // 
            // tServerName
            // 
            this.tServerName.AutoSize = true;
            this.tServerName.Location = new System.Drawing.Point(10, 59);
            this.tServerName.Name = "tServerName";
            this.tServerName.Size = new System.Drawing.Size(72, 13);
            this.tServerName.TabIndex = 4;
            this.tServerName.Text = "Server Name:";
            // 
            // tPort
            // 
            this.tPort.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.tPort.AutoSize = true;
            this.tPort.Location = new System.Drawing.Point(184, 28);
            this.tPort.Name = "tPort";
            this.tPort.Size = new System.Drawing.Size(29, 13);
            this.tPort.TabIndex = 2;
            this.tPort.Text = "Port:";
            // 
            // HostIP
            // 
            this.HostIP.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.HostIP.Location = new System.Drawing.Point(61, 25);
            this.HostIP.Name = "HostIP";
            this.HostIP.Size = new System.Drawing.Size(117, 20);
            this.HostIP.TabIndex = 1;
            // 
            // tHostIP
            // 
            this.tHostIP.AutoSize = true;
            this.tHostIP.Location = new System.Drawing.Point(10, 28);
            this.tHostIP.Name = "tHostIP";
            this.tHostIP.Size = new System.Drawing.Size(45, 13);
            this.tHostIP.TabIndex = 0;
            this.tHostIP.Text = "Host IP:";
            // 
            // ServerList
            // 
            this.ServerList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.ServerList.FormattingEnabled = true;
            this.ServerList.IntegralHeight = false;
            this.ServerList.Location = new System.Drawing.Point(6, 7);
            this.ServerList.Name = "ServerList";
            this.ServerList.Size = new System.Drawing.Size(208, 417);
            this.ServerList.TabIndex = 0;
            this.ServerList.SelectedIndexChanged += new System.EventHandler(this.ServerList_SelectedIndexChanged);
            // 
            // tabModules
            // 
            this.tabModules.Controls.Add(this.groupModuleConfig);
            this.tabModules.Controls.Add(this.groupModuleInfo);
            this.tabModules.Controls.Add(this.checkedListModules);
            this.tabModules.Location = new System.Drawing.Point(4, 22);
            this.tabModules.Name = "tabModules";
            this.tabModules.Padding = new System.Windows.Forms.Padding(3);
            this.tabModules.Size = new System.Drawing.Size(515, 465);
            this.tabModules.TabIndex = 1;
            this.tabModules.Text = "Modules";
            this.tabModules.UseVisualStyleBackColor = true;
            // 
            // groupModuleConfig
            // 
            this.groupModuleConfig.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupModuleConfig.Controls.Add(this.tExtraConfig);
            this.groupModuleConfig.Controls.Add(this.ExtraConfig);
            this.groupModuleConfig.Controls.Add(this.ModuleConfIfNotActive);
            this.groupModuleConfig.Controls.Add(this.ModuleConfIfActive);
            this.groupModuleConfig.Controls.Add(this.ModuleConfDisabled);
            this.groupModuleConfig.Location = new System.Drawing.Point(220, 7);
            this.groupModuleConfig.Name = "groupModuleConfig";
            this.groupModuleConfig.Size = new System.Drawing.Size(289, 206);
            this.groupModuleConfig.TabIndex = 3;
            this.groupModuleConfig.TabStop = false;
            this.groupModuleConfig.Text = "Extra config";
            // 
            // tExtraConfig
            // 
            this.tExtraConfig.AutoSize = true;
            this.tExtraConfig.Location = new System.Drawing.Point(6, 49);
            this.tExtraConfig.Name = "tExtraConfig";
            this.tExtraConfig.Size = new System.Drawing.Size(205, 13);
            this.tExtraConfig.TabIndex = 4;
            this.tExtraConfig.Text = "Add an extra configuration for this module:";
            // 
            // ExtraConfig
            // 
            this.ExtraConfig.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ExtraConfig.Location = new System.Drawing.Point(9, 69);
            this.ExtraConfig.Multiline = true;
            this.ExtraConfig.Name = "ExtraConfig";
            this.ExtraConfig.ReadOnly = true;
            this.ExtraConfig.Size = new System.Drawing.Size(270, 126);
            this.ExtraConfig.TabIndex = 3;
            // 
            // ModuleConfIfNotActive
            // 
            this.ModuleConfIfNotActive.AutoSize = true;
            this.ModuleConfIfNotActive.Location = new System.Drawing.Point(176, 20);
            this.ModuleConfIfNotActive.Name = "ModuleConfIfNotActive";
            this.ModuleConfIfNotActive.Size = new System.Drawing.Size(84, 17);
            this.ModuleConfIfNotActive.TabIndex = 2;
            this.ModuleConfIfNotActive.Text = "If Not Active";
            this.ModuleConfIfNotActive.UseVisualStyleBackColor = true;
            this.ModuleConfIfNotActive.CheckedChanged += new System.EventHandler(this.ModuleConfIfNotActive_CheckedChanged);
            // 
            // ModuleConfIfActive
            // 
            this.ModuleConfIfActive.AutoSize = true;
            this.ModuleConfIfActive.Location = new System.Drawing.Point(93, 20);
            this.ModuleConfIfActive.Name = "ModuleConfIfActive";
            this.ModuleConfIfActive.Size = new System.Drawing.Size(64, 17);
            this.ModuleConfIfActive.TabIndex = 1;
            this.ModuleConfIfActive.Text = "If Active";
            this.ModuleConfIfActive.UseVisualStyleBackColor = true;
            this.ModuleConfIfActive.CheckedChanged += new System.EventHandler(this.ModuleConfIfActive_CheckedChanged);
            // 
            // ModuleConfDisabled
            // 
            this.ModuleConfDisabled.AutoSize = true;
            this.ModuleConfDisabled.Checked = true;
            this.ModuleConfDisabled.Location = new System.Drawing.Point(9, 20);
            this.ModuleConfDisabled.Name = "ModuleConfDisabled";
            this.ModuleConfDisabled.Size = new System.Drawing.Size(66, 17);
            this.ModuleConfDisabled.TabIndex = 0;
            this.ModuleConfDisabled.TabStop = true;
            this.ModuleConfDisabled.Text = "Disabled";
            this.ModuleConfDisabled.UseVisualStyleBackColor = true;
            this.ModuleConfDisabled.CheckedChanged += new System.EventHandler(this.ModuleConfDisabled_CheckedChanged);
            // 
            // groupModuleInfo
            // 
            this.groupModuleInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupModuleInfo.Controls.Add(this.ModulesLink);
            this.groupModuleInfo.Controls.Add(this.ModulesInfo);
            this.groupModuleInfo.Location = new System.Drawing.Point(220, 219);
            this.groupModuleInfo.Name = "groupModuleInfo";
            this.groupModuleInfo.Size = new System.Drawing.Size(289, 240);
            this.groupModuleInfo.TabIndex = 2;
            this.groupModuleInfo.TabStop = false;
            this.groupModuleInfo.Text = "Module Info";
            // 
            // checkedListModules
            // 
            this.checkedListModules.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.checkedListModules.FormattingEnabled = true;
            this.checkedListModules.IntegralHeight = false;
            this.checkedListModules.Location = new System.Drawing.Point(6, 8);
            this.checkedListModules.Name = "checkedListModules";
            this.checkedListModules.Size = new System.Drawing.Size(208, 451);
            this.checkedListModules.TabIndex = 0;
            this.checkedListModules.SelectedIndexChanged += new System.EventHandler(this.checkedListModules_SelectedIndexChanged);
            // 
            // tabCustom
            // 
            this.tabCustom.Controls.Add(this.CustomSetting);
            this.tabCustom.Controls.Add(this.RemoveCustom);
            this.tabCustom.Controls.Add(this.AddCustom);
            this.tabCustom.Controls.Add(this.CustomList);
            this.tabCustom.Location = new System.Drawing.Point(4, 22);
            this.tabCustom.Name = "tabCustom";
            this.tabCustom.Padding = new System.Windows.Forms.Padding(3);
            this.tabCustom.Size = new System.Drawing.Size(515, 465);
            this.tabCustom.TabIndex = 2;
            this.tabCustom.Text = "Custom Settings";
            this.tabCustom.UseVisualStyleBackColor = true;
            // 
            // CustomSetting
            // 
            this.CustomSetting.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.CustomSetting.Controls.Add(this.tAddEdit);
            this.CustomSetting.Controls.Add(this.CustomEdit);
            this.CustomSetting.Location = new System.Drawing.Point(221, 7);
            this.CustomSetting.Name = "CustomSetting";
            this.CustomSetting.Size = new System.Drawing.Size(288, 450);
            this.CustomSetting.TabIndex = 7;
            this.CustomSetting.TabStop = false;
            this.CustomSetting.Text = "Custom Setting";
            // 
            // tAddEdit
            // 
            this.tAddEdit.AutoSize = true;
            this.tAddEdit.Location = new System.Drawing.Point(6, 22);
            this.tAddEdit.Name = "tAddEdit";
            this.tAddEdit.Size = new System.Drawing.Size(86, 13);
            this.tAddEdit.TabIndex = 1;
            this.tAddEdit.Text = "Add/Edit setting:";
            // 
            // CustomEdit
            // 
            this.CustomEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.CustomEdit.Enabled = false;
            this.CustomEdit.Location = new System.Drawing.Point(11, 42);
            this.CustomEdit.Multiline = true;
            this.CustomEdit.Name = "CustomEdit";
            this.CustomEdit.Size = new System.Drawing.Size(267, 393);
            this.CustomEdit.TabIndex = 0;
            // 
            // RemoveCustom
            // 
            this.RemoveCustom.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.RemoveCustom.Enabled = false;
            this.RemoveCustom.Location = new System.Drawing.Point(137, 434);
            this.RemoveCustom.Name = "RemoveCustom";
            this.RemoveCustom.Size = new System.Drawing.Size(75, 23);
            this.RemoveCustom.TabIndex = 6;
            this.RemoveCustom.Text = "-";
            this.RemoveCustom.UseVisualStyleBackColor = true;
            this.RemoveCustom.Click += new System.EventHandler(this.RemoveCustom_Click);
            // 
            // AddCustom
            // 
            this.AddCustom.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.AddCustom.Location = new System.Drawing.Point(6, 434);
            this.AddCustom.Name = "AddCustom";
            this.AddCustom.Size = new System.Drawing.Size(75, 23);
            this.AddCustom.TabIndex = 5;
            this.AddCustom.Text = "+";
            this.AddCustom.UseVisualStyleBackColor = true;
            this.AddCustom.Click += new System.EventHandler(this.AddCustom_Click);
            // 
            // CustomList
            // 
            this.CustomList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.CustomList.FormattingEnabled = true;
            this.CustomList.IntegralHeight = false;
            this.CustomList.Location = new System.Drawing.Point(6, 7);
            this.CustomList.Name = "CustomList";
            this.CustomList.Size = new System.Drawing.Size(208, 417);
            this.CustomList.TabIndex = 4;
            this.CustomList.SelectedIndexChanged += new System.EventHandler(this.CustomList_SelectedIndexChanged);
            // 
            // ConfigOK
            // 
            this.ConfigOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ConfigOK.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.ConfigOK.Location = new System.Drawing.Point(379, 506);
            this.ConfigOK.Name = "ConfigOK";
            this.ConfigOK.Size = new System.Drawing.Size(156, 23);
            this.ConfigOK.TabIndex = 2;
            this.ConfigOK.Text = "&Close";
            this.ConfigOK.UseVisualStyleBackColor = true;
            // 
            // ModulesInfo
            // 
            this.ModulesInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ModulesInfo.BackColor = System.Drawing.SystemColors.Window;
            this.ModulesInfo.Location = new System.Drawing.Point(9, 16);
            this.ModulesInfo.Multiline = true;
            this.ModulesInfo.Name = "ModulesInfo";
            this.ModulesInfo.ReadOnly = true;
            this.ModulesInfo.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.ModulesInfo.Size = new System.Drawing.Size(270, 193);
            this.ModulesInfo.TabIndex = 0;
            // 
            // ModulesLink
            // 
            this.ModulesLink.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ModulesLink.AutoSize = true;
            this.ModulesLink.Location = new System.Drawing.Point(7, 217);
            this.ModulesLink.Name = "ModulesLink";
            this.ModulesLink.Size = new System.Drawing.Size(0, 13);
            this.ModulesLink.TabIndex = 1;
            // 
            // WebServerConfigWindow
            // 
            this.AcceptButton = this.ConfigOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.ConfigOK;
            this.ClientSize = new System.Drawing.Size(547, 538);
            this.Controls.Add(this.ConfigOK);
            this.Controls.Add(this.tabsWebServer);
            this.MinimumSize = new System.Drawing.Size(500, 510);
            this.Name = "WebServerConfigWindow";
            this.ShowIcon = false;
            this.Text = "Configure Web Server";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.WebServerConfigWindow_FormClosing);
            this.Load += new System.EventHandler(this.WebServerConfig_Load);
            this.tabsWebServer.ResumeLayout(false);
            this.tabVServer.ResumeLayout(false);
            this.VirtualServers.ResumeLayout(false);
            this.VirtualServers.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Port)).EndInit();
            this.tabModules.ResumeLayout(false);
            this.groupModuleConfig.ResumeLayout(false);
            this.groupModuleConfig.PerformLayout();
            this.groupModuleInfo.ResumeLayout(false);
            this.groupModuleInfo.PerformLayout();
            this.tabCustom.ResumeLayout(false);
            this.CustomSetting.ResumeLayout(false);
            this.CustomSetting.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabsWebServer;
        private System.Windows.Forms.TabPage tabVServer;
        private System.Windows.Forms.TabPage tabModules;
        private System.Windows.Forms.CheckedListBox checkedListModules;
        private System.Windows.Forms.Button ConfigOK;
        private System.Windows.Forms.GroupBox groupModuleInfo;
        private System.Windows.Forms.GroupBox VirtualServers;
        private System.Windows.Forms.TextBox DocumentRoot;
        private System.Windows.Forms.Label tDocumentRoot;
        private System.Windows.Forms.TextBox ServerAlias;
        private System.Windows.Forms.Label tServerAlias;
        private System.Windows.Forms.TextBox ServerName;
        private System.Windows.Forms.Label tServerName;
        private System.Windows.Forms.Label tPort;
        private System.Windows.Forms.TextBox HostIP;
        private System.Windows.Forms.Label tHostIP;
        private System.Windows.Forms.ListBox ServerList;
        private System.Windows.Forms.Button RemoveServer;
        private System.Windows.Forms.Button AddServer;
        private System.Windows.Forms.Button RemoveAlias;
        private System.Windows.Forms.Button AddAlias;
        private System.Windows.Forms.ListBox Alias;
        private System.Windows.Forms.Label tAlias;
        private System.Windows.Forms.Label tOthers;
        private System.Windows.Forms.Button RemoveFolder;
        private System.Windows.Forms.Button AddFolder;
        private System.Windows.Forms.ListBox Folders;
        private System.Windows.Forms.Label tFolders;
        private System.Windows.Forms.TextBox Others;
        private System.Windows.Forms.NumericUpDown Port;
        private System.Windows.Forms.TabPage tabCustom;
        private System.Windows.Forms.Button RemoveCustom;
        private System.Windows.Forms.Button AddCustom;
        private System.Windows.Forms.ListBox CustomList;
        private System.Windows.Forms.GroupBox groupModuleConfig;
        private System.Windows.Forms.Label tExtraConfig;
        private System.Windows.Forms.TextBox ExtraConfig;
        private System.Windows.Forms.RadioButton ModuleConfIfNotActive;
        private System.Windows.Forms.RadioButton ModuleConfIfActive;
        private System.Windows.Forms.RadioButton ModuleConfDisabled;
        private System.Windows.Forms.GroupBox CustomSetting;
        private System.Windows.Forms.Label tAddEdit;
        private System.Windows.Forms.TextBox CustomEdit;
        private System.Windows.Forms.Button SelectFolder;
        private System.Windows.Forms.LinkLabel ModulesLink;
        private System.Windows.Forms.TextBox ModulesInfo;
    }
}