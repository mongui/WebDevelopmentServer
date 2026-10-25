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
            // Nothing changed: just close.
            if (this.Password1.Text == Globals.DataBasePass && this.Password2.Text == Globals.DataBasePass)
            {
                return;
            }

            if (this.Password1.Text == this.Password2.Text && DBc.CheckRegex(this.Password1.Text))
            {
                DBc.SetDefaultConfig(this.Password1.Text);
                return;
            }

            DialogResult answer = MessageBox.Show(
                "The passwords don't match, have less than 8 characters, or don't contain at least one capital letter, one lowercase letter and one number.\r\n\r\nDiscard the changes and close?",
                "Database settings", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (answer != DialogResult.Yes)
            {
                e.Cancel = true;
            }
        }
    }
}
