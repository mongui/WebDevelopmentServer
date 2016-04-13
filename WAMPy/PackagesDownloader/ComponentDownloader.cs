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

        public ComponentDownloader()
        {
            InitializeComponent();
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
                    MessageBox.Show(ex.Message);
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

            this.comboComponent.SelectedIndex = 0;
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
            try
            {
                this.SvrList = Globals.dbServ.GetDataTable("SELECT * FROM ServerNames WHERE Active = 1 AND Type = " + (Convert.ToInt32(this.SelTypes)+1));
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

                    if (Fldrs.Contains(Itm))
                    {
                        try
                        {
                            items.Add(Itm, true);
                        }
                        catch (NullReferenceException)
                        {
                            //Globals.Error.Show("Error:\r\n" + ex);
                        }
                    }
                    else
                    {
                        items.Add(Itm);
                    }
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
                MessageBox.Show(Ex.Message);
                return null;
            }
        }

        public Boolean LoadServerFileList(int ServerId)
        {
            // Get server URLs from DataBase.
            try
            {
                this.SvrURLs = Globals.dbServ.GetDataTable("SELECT * FROM ServerURLs WHERE ServerId = " + ServerId);
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

            foreach (String Dir in Directory.GetDirectories(trgt))
            {
                Fldrs.Add(Dir.Replace(trgt, ""));
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
                    MessageBox.Show(ex.Message);
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
                    if (row["URL"].ToString().Contains(Item))
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

        private void Empty(DirectoryInfo directory)
        {
            try
            {
                foreach (FileInfo file in directory.GetFiles()) file.Delete();
                foreach (DirectoryInfo subDirectory in directory.GetDirectories()) subDirectory.Delete(true);
                directory.Delete(true);
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
    }
}
