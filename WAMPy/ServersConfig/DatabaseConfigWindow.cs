using System;
using System.Windows.Forms;

namespace WDS
{
    public partial class DatabaseConfigWindow : Form
    {
        private DataBaseConfig DBc = new DataBaseConfig();

        public DatabaseConfigWindow()
        {
            InitializeComponent();
        }

        private void DatabaseConfigWindow_Load(object sender, EventArgs e)
        {
            this.Password1.Text = Globals.DataBasePass;
            this.Password2.Text = Globals.DataBasePass;
        }

        private void DatabaseConfigWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.Password1.Text == this.Password2.Text && DBc.CheckRegex(this.Password1.Text))
            {
                Globals.DataBasePass = this.Password1.Text;
                Globals.DataBasePassChange = true;
                Config.SaveSetting("DataBasePass", this.Password1.Text);
            }
            else
            {
                Globals.Error.Show("The passwords don't match, have less than 8 characters, or doesn't contain at least one capital letter and one numeric character.");
                e.Cancel = true;
            }
        }
    }
}
