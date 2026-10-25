using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Data;

namespace WDS.Resources
{
    public partial class ComponentDownloader : Form
    {
        public ComponentType SelTypes = 0;
        public DataTable SvrList = new DataTable();
        public DataTable SvrURLs = new DataTable();
        
        public enum ComponentType
        {
            WebServer,
            PHP,
            DataBase
        }

        private String Target = Globals.AppFolder;
        private List<String> CheckedItems = new List<String>();
        Dictionary<String, String> Added = new Dictionary<String, String>();
        List<String> Removed = new List<String>();

        private Label CompatibilityInfo;

        public ComponentDownloader()
        {
            InitializeComponent();

            // Under the list: can the highlighted version work with the selected ones?
            this.checkList.Height -= 70;
            this.CompatibilityInfo = new Label();
            this.CompatibilityInfo.Location = new System.Drawing.Point(this.checkList.Left, this.checkList.Bottom + 4);
            this.CompatibilityInfo.Size = new System.Drawing.Size(this.checkList.Width, 66);
            this.CompatibilityInfo.Text = "Select a version to see if it works with the versions you use.";
            this.CompatibilityInfo.ForeColor = System.Drawing.SystemColors.GrayText;
            this.groupComponents.Controls.Add(this.CompatibilityInfo);
            this.checkList.SelectedIndexChanged += (s, e) => this.ShowCompatibility();
        }

        private void ShowCompatibility()
        {
            if (this.checkList.SelectedItem == null || this.comboComponent.SelectedIndex < 0 ||
                this.comboComponent.SelectedIndex >= this.SvrList.Rows.Count)
            {
                return;
            }

            Product product = Compatibility.ProductFromListName(this.SvrList.Rows[this.comboComponent.SelectedIndex]["Name"].ToString());
            String item = this.checkList.SelectedItem.ToString();
            SoftwareComponent c = Directory.Exists(this.Target + item)
                ? Compatibility.FromFolder(Compatibility.KindOf(product), this.Target + item)
                : Compatibility.FromPackage(product, item);

            List<String> lines = new List<String>();
            bool bad = false, limited = false;

            foreach (Issue i in Compatibility.CheckAlone(c))
            {
                if (i.Level == Severity.Info) continue;
                lines.Add((i.Level == Severity.Error ? "✖ " : "⚠ ") + i.Text);
                bad |= (i.Level == Severity.Error);
                limited |= (i.Level == Severity.Warning);
            }

            SoftwareComponent web = Compatibility.Active(ComponentKind.WebServer);
            SoftwareComponent php = Compatibility.Active(ComponentKind.PHP);
            SoftwareComponent db = Compatibility.Active(ComponentKind.DataBase);
            List<PairResult> pairs = new List<PairResult>();
            switch (c.Kind)
            {
                case ComponentKind.WebServer:
                    if (php != null) pairs.Add(Compatibility.WebPhp(c, php));
                    break;
                case ComponentKind.PHP:
                    if (web != null) pairs.Add(Compatibility.WebPhp(web, c));
                    if (db != null) pairs.Add(Compatibility.PhpDb(c, db));
                    break;
                case ComponentKind.DataBase:
                    if (php != null) pairs.Add(Compatibility.PhpDb(php, c));
                    break;
            }

            foreach (PairResult p in pairs)
            {
                SoftwareComponent other = (p.A == c) ? p.B : p.A;
                if (!p.Compatible)
                {
                    bad = true;
                    lines.Add("✖ Not with " + other.Title + ": " + p.Summary);
                }
                else
                {
                    bool lim = (p.Link == PhpLink.Cgi || p.Notes.Count > 0);
                    limited |= lim;
                    lines.Add((lim ? "⚠ " : "✔ ") + "With " + other.Title + (p.Link != PhpLink.None ? ": " + p.Method : "") + ".");
                }
            }

            if (lines.Count == 0)
            {
                lines.Add("✔ No problems found. Select versions in the main window to compare them.");
            }

            this.CompatibilityInfo.Text = c.Details + "\r\n" + String.Join("\r\n", lines.ToArray());
            this.CompatibilityInfo.ForeColor = bad ? System.Drawing.Color.Firebrick
                : limited ? System.Drawing.Color.DarkOrange : System.Drawing.Color.ForestGreen;
        }

        private void ComponentDownloader_Load(object sender, EventArgs e)
        {
            switch (this.SelTypes)
            {
                case ComponentType.WebServer:
                    this.Target = Globals.AppFolder + @"bin\webserver\";
                    break;
                case ComponentType.PHP:
                    this.Target = Globals.AppFolder + @"bin\php\";
                    break;
                case ComponentType.DataBase:
                    this.Target = Globals.AppFolder + @"bin\mysql\";
                    break;
            }

            if (!Directory.Exists(this.Target))
            {
                try
                {
                    Directory.CreateDirectory(this.Target);
                }
                catch (Exception ex)
                {
                    Globals.Error.Show("Unexpected error", ex);
                }
            }

            LoadServerList();

            if (this.SvrList.Rows.Count > 0)
            {
                foreach (DataRow row in this.SvrList.Rows)
                {
                    try
                    {
                        this.comboComponent.Items.Add(row["FullName"].ToString());
                    }
                    catch (Exception)
                    {
                    }
                }
            }

            if (this.comboComponent.Items.Count > 0)
            {
                this.comboComponent.SelectedIndex = 0;
            }
            else
            {
                this.comboComponent.Enabled = false;
                this.checkList.Enabled = false;
                Globals.Error.Show("The list of servers is not available yet. Check your connection and try again later.");
            }
        }
        /*
        public Boolean LoadServersDatabase()
        {
            // Check if the database is available.
            if (!File.Exists(Globals.AppFolder + @"Servers.db"))
            {
                MessageBox.Show("Error: Unable to find the database file at:\r\n\r\n" + Globals.AppFolder);
                return false;
            }

            try
            {
                Globals.dbServ = new SQLiteDatabase(Globals.AppFolder + @"Servers.db");
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }*/

        public Boolean LoadServerList()
        {
            if (Globals.dbServ == null)
            {
                this.SvrList = new DataTable();
                return false;
            }

            try
            {
                this.SvrList = Globals.dbServ.GetDataTable("SELECT * FROM ServerNames WHERE Active = 1 AND Type = @p0", Convert.ToInt32(this.SelTypes) + 1);
                return true;
            }
            catch (Exception)
            {
                Globals.Error.Show("'Servers.db' is not a valid database.");
                return false;
            }
        }

        private void comboComponent_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.checkList.Items.Clear();
            if (comboComponent.SelectedIndex < 0 || comboComponent.SelectedIndex >= this.SvrList.Rows.Count)
            {
                return;
            }
            DataRow row = this.SvrList.Rows[comboComponent.SelectedIndex];

            LoadURLs(Convert.ToInt32(row["Id"]));
        }

        private void LoadURLs(int ServerId)
        {
            var items = this.checkList.Items;
            List<String> Fldrs = GetPossibleServers();

            LoadServerFileList(ServerId);

            if (this.SvrURLs.Rows.Count > 0)
            {
                String Itm = "";

                foreach (DataRow row in this.SvrURLs.Rows)
                {
                    Itm = CleanURL(row["URL"].ToString());
                    if (String.IsNullOrEmpty(Itm))
                    {
                        continue;
                    }

                    items.Add(Itm, Fldrs.Contains(Itm));
                }
            }
        }

        public String CleanURL(String URL)
        {
            try
            {
                String[] fl = URL.Split('/');
                int size = fl.Length;

                if (fl[size - 1] != "")
                {
                    return fl[size - 1].Replace(".zip", "").Replace(".msi", "").TrimEnd('\r', '\n');
                }
                return null;
            }
            catch (Exception Ex)
            {
                Globals.Error.Show("Unexpected error", Ex);
                return null;
            }
        }

        public Boolean LoadServerFileList(int ServerId)
        {
            // Get server URLs from DataBase.
            if (Globals.dbServ == null)
            {
                this.SvrURLs = new DataTable();
                return false;
            }

            try
            {
                this.SvrURLs = Globals.dbServ.GetDataTable("SELECT * FROM ServerURLs WHERE ServerId = @p0", ServerId);
                return true;
            }
            catch (Exception)
            {
                Globals.Error.Show("'Servers.db' is not a valid database.");
                return false;
            }
        }

        public List<string> GetPossibleServers()
        {
            return GetPossibleServers(this.Target);
        }

        public List<string> GetPossibleServers(String trgt)
        {
            // Get installed servers.
            List<String> Fldrs = new List<String>();

            if (!Directory.Exists(trgt))
            {
                return Fldrs;
            }

            foreach (String Dir in Directory.GetDirectories(trgt))
            {
                Fldrs.Add(Path.GetFileName(Dir));
            }
            return Fldrs;
        }


        private void buttonOK_Click(object sender, EventArgs e)
        {
            // Delete unwanted packages.
            foreach (String Item in this.Removed)
            {
                if (Directory.Exists(this.Target + Item))
                {
                    if (Globals.Processes.RunningCount > 0 && IsActiveVersion(Item))
                    {
                        Globals.Error.Show(Item + " is running. Stop the servers before removing it.");
                        continue;
                    }
                    this.Empty(new DirectoryInfo(this.Target + Item));
                }
            }

            String[] URLlist = (new List<String>(this.Added.Values)).ToArray();
            
            // Download new packages.
            if (URLlist.Length > 0)
            {
                DownloaderWindow downloader = new DownloaderWindow(URLlist, this.Target);

                try
                {
                    downloader.ShowDialog();
                    downloader.Dispose();
                }
                catch (Exception ex)
                {
                    Globals.Error.Show("Unexpected error", ex);
                }
            }
        }

        private void checkList_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            String Item = checkList.Items[e.Index].ToString();

            // Has the item been checked?
            if (e.NewValue == CheckState.Checked)
            {
                // If the item is in the file list...
                String SelectedURL = null;
                foreach (DataRow row in this.SvrURLs.Rows)
                {
                    if (CleanURL(row["URL"].ToString()) == Item)
                    {
                        SelectedURL = row["URL"].ToString();
                        break;
                    }
                }

                // If the list doesn't contain the item and it hasn't its own folder...
                if (!String.IsNullOrEmpty(SelectedURL) && !this.Added.ContainsKey(Item) && !Directory.Exists(this.Target + Item))
                {
                    // Is it in the removed list?
                    if (this.Removed.Contains(Item))
                    {
                        this.Removed.Remove(Item);
                    }
                    this.Added.Add(Item, SelectedURL);
                }
            }
            else if (!this.Removed.Contains(Item))
            {
                if (this.Added.ContainsKey(Item))
                {
                    this.Added.Remove(Item);
                }
                if (Directory.Exists(this.Target + Item))
                {
                    this.Removed.Add(Item);
                }
            }
        }

        private bool IsActiveVersion(String Item)
        {
            switch (this.SelTypes)
            {
                case ComponentType.WebServer: return Item == Globals.ActiveWebServerVersion;
                case ComponentType.PHP: return Item == Globals.ActivePHPVersion;
                default: return Item == Globals.ActiveMySQLVersion;
            }
        }

        private void Empty(DirectoryInfo directory)
        {
            try
            {
                directory.Delete(true);
            }
            catch(Exception e)
            {
                Globals.Error.Show("Unexpected error", e);
            }
        }
    }
}
