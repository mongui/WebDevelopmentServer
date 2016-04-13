namespace WDS
{
    partial class PreferencesWindow
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
            this.bOK = new System.Windows.Forms.Button();
            this.bCancel = new System.Windows.Forms.Button();
            this.gPreferences = new System.Windows.Forms.GroupBox();
            this.checkRewriteWhenStart = new System.Windows.Forms.CheckBox();
            this.checkStartInSystray = new System.Windows.Forms.CheckBox();
            this.checkStartOnLaunch = new System.Windows.Forms.CheckBox();
            this.gPreferences.SuspendLayout();
            this.SuspendLayout();
            // 
            // bOK
            // 
            this.bOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.bOK.Location = new System.Drawing.Point(150, 143);
            this.bOK.Name = "bOK";
            this.bOK.Size = new System.Drawing.Size(75, 23);
            this.bOK.TabIndex = 0;
            this.bOK.Text = "&Ok";
            this.bOK.UseVisualStyleBackColor = true;
            // 
            // bCancel
            // 
            this.bCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bCancel.Location = new System.Drawing.Point(232, 143);
            this.bCancel.Name = "bCancel";
            this.bCancel.Size = new System.Drawing.Size(75, 23);
            this.bCancel.TabIndex = 1;
            this.bCancel.Text = "&Cancel";
            this.bCancel.UseVisualStyleBackColor = true;
            // 
            // gPreferences
            // 
            this.gPreferences.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gPreferences.Controls.Add(this.checkRewriteWhenStart);
            this.gPreferences.Controls.Add(this.checkStartInSystray);
            this.gPreferences.Controls.Add(this.checkStartOnLaunch);
            this.gPreferences.Location = new System.Drawing.Point(7, 5);
            this.gPreferences.Name = "gPreferences";
            this.gPreferences.Size = new System.Drawing.Size(300, 130);
            this.gPreferences.TabIndex = 2;
            this.gPreferences.TabStop = false;
            this.gPreferences.Text = "Program preferenes";
            // 
            // checkRewriteWhenStart
            // 
            this.checkRewriteWhenStart.AutoSize = true;
            this.checkRewriteWhenStart.Location = new System.Drawing.Point(18, 93);
            this.checkRewriteWhenStart.Name = "checkRewriteWhenStart";
            this.checkRewriteWhenStart.Size = new System.Drawing.Size(222, 17);
            this.checkRewriteWhenStart.TabIndex = 2;
            this.checkRewriteWhenStart.Text = "Rewrite the config files when servers start";
            this.checkRewriteWhenStart.UseVisualStyleBackColor = true;
            this.checkRewriteWhenStart.CheckedChanged += new System.EventHandler(this.checkRewriteWhenStart_CheckedChanged);
            // 
            // checkStartInSystray
            // 
            this.checkStartInSystray.AutoSize = true;
            this.checkStartInSystray.Location = new System.Drawing.Point(18, 59);
            this.checkStartInSystray.Name = "checkStartInSystray";
            this.checkStartInSystray.Size = new System.Drawing.Size(112, 17);
            this.checkStartInSystray.TabIndex = 1;
            this.checkStartInSystray.Text = "Start in the systray";
            this.checkStartInSystray.UseVisualStyleBackColor = true;
            this.checkStartInSystray.CheckedChanged += new System.EventHandler(this.checkStartInSystray_CheckedChanged);
            // 
            // checkStartOnLaunch
            // 
            this.checkStartOnLaunch.AutoSize = true;
            this.checkStartOnLaunch.Location = new System.Drawing.Point(18, 26);
            this.checkStartOnLaunch.Name = "checkStartOnLaunch";
            this.checkStartOnLaunch.Size = new System.Drawing.Size(208, 17);
            this.checkStartOnLaunch.TabIndex = 0;
            this.checkStartOnLaunch.Text = "Start servers on launch this application";
            this.checkStartOnLaunch.UseVisualStyleBackColor = true;
            this.checkStartOnLaunch.CheckedChanged += new System.EventHandler(this.checkStartOnLaunch_CheckedChanged);
            // 
            // PreferencesWindow
            // 
            this.AcceptButton = this.bOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.bCancel;
            this.ClientSize = new System.Drawing.Size(314, 174);
            this.Controls.Add(this.gPreferences);
            this.Controls.Add(this.bCancel);
            this.Controls.Add(this.bOK);
            this.Name = "PreferencesWindow";
            this.ShowIcon = false;
            this.Text = "Preferences";
            this.Load += new System.EventHandler(this.PreferencesWindow_Load);
            this.gPreferences.ResumeLayout(false);
            this.gPreferences.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button bOK;
        private System.Windows.Forms.Button bCancel;
        private System.Windows.Forms.GroupBox gPreferences;
        private System.Windows.Forms.CheckBox checkStartInSystray;
        private System.Windows.Forms.CheckBox checkStartOnLaunch;
        private System.Windows.Forms.CheckBox checkRewriteWhenStart;
    }
}