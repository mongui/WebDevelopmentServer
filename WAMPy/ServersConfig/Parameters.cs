using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace WDS
{
    public partial class Parameters : Form
    {
        public String Title;
        public String textBox;
        public String textParameter1;
        public String Parameter1;
        public String textParameter2;
        public String Parameter2;

        //  Cuando sea true...
        public bool SelectFolder = false;
        public int SelectFolderPosition = 1;


        /* FOLDER options */
        public bool Folders = false;

        public bool SymLinks = false;
        public bool Includes = false;
        public bool Indexes = false;
        public bool MultiViews = false;
        public bool AllowOverride = false;
        public String Others = "";

        public Parameters()
        {
            InitializeComponent();
        }

        private void Parameters_Load(object sender, EventArgs e)
        {
            if (Folders == true)
            {
                this.MinimumSize = new Size(300, 348);
                this.MaximumSize = new Size(1000, 348);
                this.Size = new Size(308, 348);

                this.Par2.Multiline = true;
                this.Par2.ScrollBars = ScrollBars.Vertical;
                this.Par2.Height = 65;

                this.checkSymLinks.Checked = this.SymLinks;
                this.checkIncludes.Checked = this.Includes;
                this.checkIndexes.Checked = this.Indexes;
                this.checkMultiViews.Checked = this.MultiViews;
                this.checkAllowOverride.Checked = this.AllowOverride;
            }
            else
            {
                this.MinimumSize = new Size(300, 194);
                this.MaximumSize = new Size(1000, 194);
                this.Size = new Size(450, 194);
            }

            this.Text = this.Title;
            this.parBox.Text = this.textBox;

            if (this.textParameter1 != null)
            {
                this.tPar1.Text = this.textParameter1;
                this.Par1.Text = this.Parameter1;
            }
            else
            {
                this.tPar1.Enabled = false;
                this.Par1.Enabled = false;
            }


            if (this.textParameter2 != null)
            {
                this.tPar2.Text = this.textParameter2;
                this.Par2.Text = this.Parameter2;
            }
            else
            {
                this.tPar2.Enabled = false;
                this.Par2.Enabled = false;
            }
        }

        private void parOK_Click(object sender, EventArgs e)
        {
            this.Parameter1 = this.Par1.Text;
            this.Parameter2 = this.Par2.Text;

            this.AllowOverride = this.checkAllowOverride.Checked;
            this.Includes = this.checkIncludes.Checked;
            this.Indexes = this.checkIndexes.Checked;
            this.MultiViews = this.checkMultiViews.Checked;
            this.SymLinks = this.checkSymLinks.Checked;
        }
    }
}
