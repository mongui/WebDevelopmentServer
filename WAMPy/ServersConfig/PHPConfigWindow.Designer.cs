namespace WDS
{
    partial class PHPConfigWindow
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
            this.tabPHP = new System.Windows.Forms.TabControl();
            this.tabExtensions = new System.Windows.Forms.TabPage();
            this.groupExtensionInfo = new System.Windows.Forms.GroupBox();
            this.ModulesInfo = new System.Windows.Forms.TextBox();
            this.groupExtensionConfig = new System.Windows.Forms.GroupBox();
            this.ModuleTag = new System.Windows.Forms.Label();
            this.ModulesConfig = new System.Windows.Forms.TextBox();
            this.checkedListModules = new System.Windows.Forms.CheckedListBox();
            this.tabSettings = new System.Windows.Forms.TabPage();
            this.RemoveSetting = new System.Windows.Forms.Button();
            this.AddSetting = new System.Windows.Forms.Button();
            this.groupDescription = new System.Windows.Forms.GroupBox();
            this.PHPSettingValue = new System.Windows.Forms.TextBox();
            this.tValue = new System.Windows.Forms.Label();
            this.checkedListSettings = new System.Windows.Forms.CheckedListBox();
            this.ConfigOK = new System.Windows.Forms.Button();
            this.ModulesLink = new System.Windows.Forms.LinkLabel();
            this.tabPHP.SuspendLayout();
            this.tabExtensions.SuspendLayout();
            this.groupExtensionInfo.SuspendLayout();
            this.groupExtensionConfig.SuspendLayout();
            this.tabSettings.SuspendLayout();
            this.groupDescription.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabPHP
            // 
            this.tabPHP.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabPHP.Controls.Add(this.tabExtensions);
            this.tabPHP.Controls.Add(this.tabSettings);
            this.tabPHP.Location = new System.Drawing.Point(12, 6);
            this.tabPHP.Name = "tabPHP";
            this.tabPHP.SelectedIndex = 0;
            this.tabPHP.Size = new System.Drawing.Size(500, 475);
            this.tabPHP.TabIndex = 0;
            // 
            // tabExtensions
            // 
            this.tabExtensions.Controls.Add(this.groupExtensionInfo);
            this.tabExtensions.Controls.Add(this.groupExtensionConfig);
            this.tabExtensions.Controls.Add(this.checkedListModules);
            this.tabExtensions.Location = new System.Drawing.Point(4, 22);
            this.tabExtensions.Name = "tabExtensions";
            this.tabExtensions.Padding = new System.Windows.Forms.Padding(3);
            this.tabExtensions.Size = new System.Drawing.Size(492, 449);
            this.tabExtensions.TabIndex = 0;
            this.tabExtensions.Text = "PHP Extensions";
            this.tabExtensions.UseVisualStyleBackColor = true;
            // 
            // groupExtensionInfo
            // 
            this.groupExtensionInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupExtensionInfo.Controls.Add(this.ModulesLink);
            this.groupExtensionInfo.Controls.Add(this.ModulesInfo);
            this.groupExtensionInfo.Location = new System.Drawing.Point(195, 207);
            this.groupExtensionInfo.Name = "groupExtensionInfo";
            this.groupExtensionInfo.Size = new System.Drawing.Size(291, 237);
            this.groupExtensionInfo.TabIndex = 3;
            this.groupExtensionInfo.TabStop = false;
            this.groupExtensionInfo.Text = "Extension Info";
            // 
            // ModulesInfo
            // 
            this.ModulesInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ModulesInfo.BackColor = System.Drawing.SystemColors.Window;
            this.ModulesInfo.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.ModulesInfo.Location = new System.Drawing.Point(9, 19);
            this.ModulesInfo.Multiline = true;
            this.ModulesInfo.Name = "ModulesInfo";
            this.ModulesInfo.ReadOnly = true;
            this.ModulesInfo.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.ModulesInfo.Size = new System.Drawing.Size(276, 190);
            this.ModulesInfo.TabIndex = 3;
            // 
            // groupExtensionConfig
            // 
            this.groupExtensionConfig.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupExtensionConfig.Controls.Add(this.ModuleTag);
            this.groupExtensionConfig.Controls.Add(this.ModulesConfig);
            this.groupExtensionConfig.Location = new System.Drawing.Point(195, 6);
            this.groupExtensionConfig.Name = "groupExtensionConfig";
            this.groupExtensionConfig.Size = new System.Drawing.Size(291, 195);
            this.groupExtensionConfig.TabIndex = 1;
            this.groupExtensionConfig.TabStop = false;
            this.groupExtensionConfig.Text = "Extension Config";
            // 
            // ModuleTag
            // 
            this.ModuleTag.AutoSize = true;
            this.ModuleTag.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ModuleTag.ForeColor = System.Drawing.SystemColors.ControlText;
            this.ModuleTag.Location = new System.Drawing.Point(8, 18);
            this.ModuleTag.Name = "ModuleTag";
            this.ModuleTag.Size = new System.Drawing.Size(70, 15);
            this.ModuleTag.TabIndex = 1;
            this.ModuleTag.Text = "Extension";
            // 
            // ModulesConfig
            // 
            this.ModulesConfig.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ModulesConfig.Enabled = false;
            this.ModulesConfig.Location = new System.Drawing.Point(9, 41);
            this.ModulesConfig.Multiline = true;
            this.ModulesConfig.Name = "ModulesConfig";
            this.ModulesConfig.Size = new System.Drawing.Size(273, 141);
            this.ModulesConfig.TabIndex = 0;
            // 
            // checkedListModules
            // 
            this.checkedListModules.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.checkedListModules.FormattingEnabled = true;
            this.checkedListModules.IntegralHeight = false;
            this.checkedListModules.Location = new System.Drawing.Point(6, 6);
            this.checkedListModules.Name = "checkedListModules";
            this.checkedListModules.Size = new System.Drawing.Size(182, 438);
            this.checkedListModules.TabIndex = 0;
            this.checkedListModules.SelectedIndexChanged += new System.EventHandler(this.checkedListModules_SelectedIndexChanged);
            // 
            // tabSettings
            // 
            this.tabSettings.Controls.Add(this.RemoveSetting);
            this.tabSettings.Controls.Add(this.AddSetting);
            this.tabSettings.Controls.Add(this.groupDescription);
            this.tabSettings.Controls.Add(this.checkedListSettings);
            this.tabSettings.Location = new System.Drawing.Point(4, 22);
            this.tabSettings.Name = "tabSettings";
            this.tabSettings.Padding = new System.Windows.Forms.Padding(3);
            this.tabSettings.Size = new System.Drawing.Size(492, 449);
            this.tabSettings.TabIndex = 1;
            this.tabSettings.Text = "PHP Settings";
            this.tabSettings.UseVisualStyleBackColor = true;
            // 
            // RemoveSetting
            // 
            this.RemoveSetting.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.RemoveSetting.Enabled = false;
            this.RemoveSetting.Location = new System.Drawing.Point(113, 417);
            this.RemoveSetting.Name = "RemoveSetting";
            this.RemoveSetting.Size = new System.Drawing.Size(75, 23);
            this.RemoveSetting.TabIndex = 5;
            this.RemoveSetting.Text = "-";
            this.RemoveSetting.UseVisualStyleBackColor = true;
            this.RemoveSetting.Click += new System.EventHandler(this.RemoveSetting_Click);
            // 
            // AddSetting
            // 
            this.AddSetting.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.AddSetting.Location = new System.Drawing.Point(6, 417);
            this.AddSetting.Name = "AddSetting";
            this.AddSetting.Size = new System.Drawing.Size(75, 23);
            this.AddSetting.TabIndex = 4;
            this.AddSetting.Text = "+";
            this.AddSetting.UseVisualStyleBackColor = true;
            this.AddSetting.Click += new System.EventHandler(this.AddSetting_Click);
            // 
            // groupDescription
            // 
            this.groupDescription.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupDescription.Controls.Add(this.PHPSettingValue);
            this.groupDescription.Controls.Add(this.tValue);
            this.groupDescription.Location = new System.Drawing.Point(194, 7);
            this.groupDescription.Name = "groupDescription";
            this.groupDescription.Size = new System.Drawing.Size(292, 437);
            this.groupDescription.TabIndex = 2;
            this.groupDescription.TabStop = false;
            this.groupDescription.Text = "Description";
            // 
            // PHPSettingValue
            // 
            this.PHPSettingValue.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PHPSettingValue.Enabled = false;
            this.PHPSettingValue.Location = new System.Drawing.Point(64, 26);
            this.PHPSettingValue.Name = "PHPSettingValue";
            this.PHPSettingValue.Size = new System.Drawing.Size(199, 20);
            this.PHPSettingValue.TabIndex = 1;
            // 
            // tValue
            // 
            this.tValue.AutoSize = true;
            this.tValue.Location = new System.Drawing.Point(21, 29);
            this.tValue.Name = "tValue";
            this.tValue.Size = new System.Drawing.Size(37, 13);
            this.tValue.TabIndex = 0;
            this.tValue.Text = "Value:";
            // 
            // checkedListSettings
            // 
            this.checkedListSettings.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.checkedListSettings.FormattingEnabled = true;
            this.checkedListSettings.IntegralHeight = false;
            this.checkedListSettings.Location = new System.Drawing.Point(6, 6);
            this.checkedListSettings.Name = "checkedListSettings";
            this.checkedListSettings.Size = new System.Drawing.Size(182, 403);
            this.checkedListSettings.TabIndex = 1;
            this.checkedListSettings.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.checkedListSettings_ItemCheck);
            this.checkedListSettings.SelectedIndexChanged += new System.EventHandler(this.checkedListSettings_SelectedIndexChanged);
            // 
            // ConfigOK
            // 
            this.ConfigOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ConfigOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.ConfigOK.Location = new System.Drawing.Point(356, 487);
            this.ConfigOK.Name = "ConfigOK";
            this.ConfigOK.Size = new System.Drawing.Size(156, 23);
            this.ConfigOK.TabIndex = 3;
            this.ConfigOK.Text = "&Close";
            this.ConfigOK.UseVisualStyleBackColor = true;
            // 
            // ModulesLink
            // 
            this.ModulesLink.AutoSize = true;
            this.ModulesLink.Location = new System.Drawing.Point(8, 214);
            this.ModulesLink.Name = "ModulesLink";
            this.ModulesLink.Size = new System.Drawing.Size(0, 13);
            this.ModulesLink.TabIndex = 4;
            this.ModulesLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.ModulesLink_LinkClicked);
            // 
            // PHPConfigWindow
            // 
            this.AcceptButton = this.ConfigOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.ConfigOK;
            this.ClientSize = new System.Drawing.Size(524, 518);
            this.Controls.Add(this.ConfigOK);
            this.Controls.Add(this.tabPHP);
            this.Name = "PHPConfigWindow";
            this.ShowIcon = false;
            this.Text = "Configure PHP Components";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.PHPConfigWindow_FormClosing);
            this.Load += new System.EventHandler(this.PHPConfigWindow_Load);
            this.tabPHP.ResumeLayout(false);
            this.tabExtensions.ResumeLayout(false);
            this.groupExtensionInfo.ResumeLayout(false);
            this.groupExtensionInfo.PerformLayout();
            this.groupExtensionConfig.ResumeLayout(false);
            this.groupExtensionConfig.PerformLayout();
            this.tabSettings.ResumeLayout(false);
            this.groupDescription.ResumeLayout(false);
            this.groupDescription.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabPHP;
        private System.Windows.Forms.TabPage tabExtensions;
        private System.Windows.Forms.TabPage tabSettings;
        private System.Windows.Forms.Button ConfigOK;
        private System.Windows.Forms.CheckedListBox checkedListModules;
        private System.Windows.Forms.GroupBox groupExtensionConfig;
        private System.Windows.Forms.CheckedListBox checkedListSettings;
        private System.Windows.Forms.GroupBox groupDescription;
        private System.Windows.Forms.TextBox PHPSettingValue;
        private System.Windows.Forms.Label tValue;
        private System.Windows.Forms.Button RemoveSetting;
        private System.Windows.Forms.Button AddSetting;
        private System.Windows.Forms.GroupBox groupExtensionInfo;
        private System.Windows.Forms.TextBox ModulesConfig;
        private System.Windows.Forms.Label ModuleTag;
        private System.Windows.Forms.TextBox ModulesInfo;
        private System.Windows.Forms.LinkLabel ModulesLink;
    }
}