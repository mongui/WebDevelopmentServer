using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WDS
{
    public partial class PreferencesWindow : Form
    {
        public bool RewriteWhenStart = (Globals.RewriteConfigFiles == "0") ? false : true;
        public bool StartInSystray = (Globals.StartInSystray == "0") ? false : true;
        public bool StartOnLaunch = (Globals.StartOnLaunch == "0") ? false : true;

        public PreferencesWindow()
        {
            InitializeComponent();
        }

        private void PreferencesWindow_Load(object sender, EventArgs e)
        {
            this.checkRewriteWhenStart.Checked = this.RewriteWhenStart;
            this.checkStartInSystray.Checked = this.StartInSystray;
            this.checkStartOnLaunch.Checked = this.StartOnLaunch;
        }

        private void checkStartOnLaunch_CheckedChanged(object sender, EventArgs e)
        {
            this.StartOnLaunch = this.checkStartOnLaunch.Checked;
        }

        private void checkStartInSystray_CheckedChanged(object sender, EventArgs e)
        {
            this.StartInSystray = this.checkStartInSystray.Checked;
        }

        private void checkRewriteWhenStart_CheckedChanged(object sender, EventArgs e)
        {
            this.RewriteWhenStart = this.checkRewriteWhenStart.Checked;
        }
    }
}
