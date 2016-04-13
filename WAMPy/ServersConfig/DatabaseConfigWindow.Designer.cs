namespace WDS
{
    partial class DatabaseConfigWindow
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
            this.groupDBConfig = new System.Windows.Forms.GroupBox();
            this.Password2 = new System.Windows.Forms.MaskedTextBox();
            this.Password1 = new System.Windows.Forms.MaskedTextBox();
            this.tPass2 = new System.Windows.Forms.Label();
            this.tPass1 = new System.Windows.Forms.Label();
            this.ConfigOK = new System.Windows.Forms.Button();
            this.groupDBConfig.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupDBConfig
            // 
            this.groupDBConfig.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupDBConfig.Controls.Add(this.Password2);
            this.groupDBConfig.Controls.Add(this.Password1);
            this.groupDBConfig.Controls.Add(this.tPass2);
            this.groupDBConfig.Controls.Add(this.tPass1);
            this.groupDBConfig.Location = new System.Drawing.Point(7, 4);
            this.groupDBConfig.Name = "groupDBConfig";
            this.groupDBConfig.Size = new System.Drawing.Size(316, 104);
            this.groupDBConfig.TabIndex = 0;
            this.groupDBConfig.TabStop = false;
            this.groupDBConfig.Text = "Database Config";
            // 
            // Password2
            // 
            this.Password2.Location = new System.Drawing.Point(120, 62);
            this.Password2.Name = "Password2";
            this.Password2.PasswordChar = '*';
            this.Password2.Size = new System.Drawing.Size(170, 20);
            this.Password2.TabIndex = 5;
            // 
            // Password1
            // 
            this.Password1.Location = new System.Drawing.Point(120, 30);
            this.Password1.Name = "Password1";
            this.Password1.PasswordChar = '*';
            this.Password1.Size = new System.Drawing.Size(170, 20);
            this.Password1.TabIndex = 4;
            // 
            // tPass2
            // 
            this.tPass2.AutoSize = true;
            this.tPass2.Location = new System.Drawing.Point(22, 65);
            this.tPass2.Name = "tPass2";
            this.tPass2.Size = new System.Drawing.Size(93, 13);
            this.tPass2.TabIndex = 2;
            this.tPass2.Text = "Repeat password:";
            // 
            // tPass1
            // 
            this.tPass1.AutoSize = true;
            this.tPass1.Location = new System.Drawing.Point(22, 33);
            this.tPass1.Name = "tPass1";
            this.tPass1.Size = new System.Drawing.Size(92, 13);
            this.tPass1.TabIndex = 0;
            this.tPass1.Text = "Password for root:";
            // 
            // ConfigOK
            // 
            this.ConfigOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ConfigOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.ConfigOK.Location = new System.Drawing.Point(167, 114);
            this.ConfigOK.Name = "ConfigOK";
            this.ConfigOK.Size = new System.Drawing.Size(156, 23);
            this.ConfigOK.TabIndex = 3;
            this.ConfigOK.Text = "&Close";
            this.ConfigOK.UseVisualStyleBackColor = true;
            // 
            // DatabaseConfigWindow
            // 
            this.AcceptButton = this.ConfigOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.ConfigOK;
            this.ClientSize = new System.Drawing.Size(331, 144);
            this.Controls.Add(this.ConfigOK);
            this.Controls.Add(this.groupDBConfig);
            this.Name = "DatabaseConfigWindow";
            this.ShowIcon = false;
            this.Text = "DatabaseConfigWindow";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.DatabaseConfigWindow_FormClosing);
            this.Load += new System.EventHandler(this.DatabaseConfigWindow_Load);
            this.groupDBConfig.ResumeLayout(false);
            this.groupDBConfig.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupDBConfig;
        private System.Windows.Forms.Label tPass1;
        private System.Windows.Forms.Label tPass2;
        private System.Windows.Forms.MaskedTextBox Password1;
        private System.Windows.Forms.MaskedTextBox Password2;
        private System.Windows.Forms.Button ConfigOK;
    }
}