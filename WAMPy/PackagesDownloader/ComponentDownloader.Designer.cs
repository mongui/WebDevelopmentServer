namespace WDS.Resources
{
    partial class ComponentDownloader
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
            this.groupComponents = new System.Windows.Forms.GroupBox();
            this.comboComponent = new System.Windows.Forms.ComboBox();
            this.tCheckUncheck = new System.Windows.Forms.Label();
            this.checkList = new System.Windows.Forms.CheckedListBox();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.groupComponents.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupComponents
            // 
            this.groupComponents.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupComponents.Controls.Add(this.comboComponent);
            this.groupComponents.Controls.Add(this.tCheckUncheck);
            this.groupComponents.Controls.Add(this.checkList);
            this.groupComponents.Location = new System.Drawing.Point(12, 12);
            this.groupComponents.Name = "groupComponents";
            this.groupComponents.Size = new System.Drawing.Size(346, 406);
            this.groupComponents.TabIndex = 0;
            this.groupComponents.TabStop = false;
            this.groupComponents.Text = "Add/Remove";
            // 
            // comboComponent
            // 
            this.comboComponent.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboComponent.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboComponent.Location = new System.Drawing.Point(6, 57);
            this.comboComponent.Name = "comboComponent";
            this.comboComponent.Size = new System.Drawing.Size(334, 21);
            this.comboComponent.TabIndex = 1;
            this.comboComponent.SelectedIndexChanged += new System.EventHandler(this.comboComponent_SelectedIndexChanged);
            // 
            // tCheckUncheck
            // 
            this.tCheckUncheck.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tCheckUncheck.Location = new System.Drawing.Point(6, 21);
            this.tCheckUncheck.Name = "tCheckUncheck";
            this.tCheckUncheck.Size = new System.Drawing.Size(334, 32);
            this.tCheckUncheck.TabIndex = 1;
            this.tCheckUncheck.Text = "Check / Uncheck the different versions of the component you want to have installe" +
    "d in your WAMPy server:";
            // 
            // checkList
            // 
            this.checkList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.checkList.FormattingEnabled = true;
            this.checkList.Location = new System.Drawing.Point(6, 88);
            this.checkList.Name = "checkList";
            this.checkList.Size = new System.Drawing.Size(334, 304);
            this.checkList.TabIndex = 2;
            this.checkList.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.checkList_ItemCheck);
            // 
            // buttonOK
            // 
            this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOK.Location = new System.Drawing.Point(202, 424);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 3;
            this.buttonOK.Text = "&Ok";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(283, 424);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 4;
            this.buttonCancel.Text = "&Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // ComponentDownloader
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(370, 459);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.groupComponents);
            this.Name = "ComponentDownloader";
            this.Text = "Download server components";
            this.Load += new System.EventHandler(this.ComponentDownloader_Load);
            this.groupComponents.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupComponents;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Label tCheckUncheck;
        private System.Windows.Forms.CheckedListBox checkList;
        private System.Windows.Forms.ComboBox comboComponent;
    }
}