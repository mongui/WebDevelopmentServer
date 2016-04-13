using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Net;
using System.IO;

namespace WDS
{
    public partial class DownloaderWindow : Form
    {
        public WebClient client = null;

        private String[] FileList;
        private String Target = AppDomain.CurrentDomain.BaseDirectory;
        private String FileName = "downloaded.zip";
        private int Count = 0;

        public DownloaderWindow(String[] fList, String Trgt)
        {
            this.FileList = fList;
            this.Target = Trgt;

            this.client = new WebClient();
            this.client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(DownloadProgressChanged);
            this.client.DownloadFileCompleted += new AsyncCompletedEventHandler(DownloadFileCompleted);
            
            InitializeComponent();
        }

        private void Downloader_Load(object sender, EventArgs e)
        {
            Type valueType = this.FileList.GetType();

            if (this.FileList != null && valueType.IsArray && this.Count < this.FileList.Length)
            {
                StartDownload();
            }
        }

        public void StartDownload()
        {
            tDownloader.Text = "Downloading " + (this.Count + 1) + " of " + this.FileList.Length + "...";

            String[] TmpFlNm = FileList[this.Count].Trim().Split('/');
            this.FileName = TmpFlNm[TmpFlNm.Length - 1];
            this.DeleteFile(this.Target + this.FileName);

            try
            {
                this.client.DownloadFileAsync(new Uri(FileList[this.Count]), this.Target + this.FileName);
            }
            catch (Exception e)
            {
                Globals.Error.Show(e.Message);
                this.DeleteFile(this.Target + this.FileName);
                this.NextFile();
            }
        }

        void DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            this.progressBar.Value = e.ProgressPercentage;
        }

        void DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {

            if (e.Cancelled)
            {
                this.client = null;
                this.DeleteFile(this.Target + this.FileName);
                this.DeleteFile(this.Target + @"temp\");
                return;
            }
            else if (e.Error != null)
            {
                Globals.Error.Show(e.Error.Message);
            }
            else
            {
                tDownloader.Text = "Decompressing " + (this.Count + 1) + " of " + this.FileList.Length + "...";
                this.bCancel.Enabled = false;
                if (this.FileName.Contains(".zip"))
                {
                    Decompress.ExtractZip(this.Target, this.FileName, true);
                }
                else if (this.FileName.Contains(".msi"))
                {
                    Decompress.ExtractMsi(this.Target, this.FileName);
                }
                this.bCancel.Enabled = true;
            }

            this.DeleteFile(this.Target + this.FileName);

            this.NextFile();
        }

        private void bCancel_Click(object sender, EventArgs e)
        {
            if (this.client != null)
            {
                this.client.CancelAsync();
            }
        }

        private void DeleteFile(String Fl)
        {
            if (File.Exists(Fl))
            {
                try
                {
                    File.Delete(Fl);
                }
                catch (Exception e)
                {
                    Globals.Error.Show(e.Message);
                }
            }
            else if (Directory.Exists(Fl))
            {
                try
                {
                    Directory.Delete(Fl);
                }
                catch (Exception e)
                {
                    Globals.Error.Show(e.Message);
                }
            }
        }

        private void NextFile()
        {
            this.Count++;
            if (this.Count < this.FileList.Length)
            {
                this.StartDownload();
            }
            else
            {
                Close();
            }
        }
    }
}
