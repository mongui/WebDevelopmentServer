using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Windows.Forms;

namespace WDS
{
    public partial class DownloaderWindow : Form
    {
        public WebClient client = null;

        private String[] FileList;
        private String Target;
        private String FileName = "downloaded.zip";
        private int Count = 0;
        private bool isClosing = false;

        /// <summary>Number of packages that failed to download or extract.</summary>
        public int Failed { get; private set; }

        /// <summary>Raised when every package has been processed (or the download was cancelled).</summary>
        public event EventHandler AllCompleted;

        /// <summary>Raised with the progress (0-100) of the current file.</summary>
        public event EventHandler<int> Progress;

        public DownloaderWindow(String[] fList, String Trgt)
        {
            this.FileList = fList ?? new String[0];
            this.Target = Trgt;

            this.client = new WebClient();
            this.client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(DownloadProgressChanged);
            this.client.DownloadFileCompleted += new AsyncCompletedEventHandler(DownloadFileCompleted);

            InitializeComponent();
            this.FormClosing += DownloaderWindow_FormClosing;
        }

        private void Downloader_Load(object sender, EventArgs e)
        {
            if (this.Count < this.FileList.Length)
            {
                StartDownload();
            }
            else
            {
                Close();
            }
        }

        public void StartDownload()
        {
            if (this.Count >= this.FileList.Length)
            {
                this.Finish();
                return;
            }

            tDownloader.Text = "Downloading " + (this.Count + 1) + " of " + this.FileList.Length + "...";

            String url = (FileList[this.Count] ?? "").Trim();
            String[] TmpFlNm = url.Split('/');
            this.FileName = TmpFlNm[TmpFlNm.Length - 1];
            this.DeleteFile(Path.Combine(this.Target, this.FileName));

            try
            {
                this.client.DownloadFileAsync(new Uri(url), Path.Combine(this.Target, this.FileName));
            }
            catch (Exception e)
            {
                Globals.Error.Show("Unable to download " + url + "", e);
                this.Failed++;
                this.DeleteFile(Path.Combine(this.Target, this.FileName));
                this.NextFile();
            }
        }

        void DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            if (!this.IsDisposed)
            {
                this.progressBar.Value = Math.Max(0, Math.Min(100, e.ProgressPercentage));
            }
            if (Progress != null)
            {
                Progress(this, e.ProgressPercentage);
            }
        }

        void DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            String file = Path.Combine(this.Target, this.FileName);

            if (e.Cancelled)
            {
                this.Failed += this.FileList.Length - this.Count;
                this.DeleteFile(file);
                this.Finish();
                return;
            }

            if (e.Error != null)
            {
                Globals.Error.Show("Unable to download " + this.FileName + "", e.Error);
                this.Failed++;
            }
            else
            {
                if (!this.IsDisposed)
                {
                    tDownloader.Text = "Decompressing " + (this.Count + 1) + " of " + this.FileList.Length + "...";
                    this.bCancel.Enabled = false;
                }

                bool ok;
                if (this.FileName.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
                {
                    ok = Decompress.ExtractZip(this.Target, this.FileName, true);
                }
                else if (this.FileName.EndsWith(".msi", StringComparison.OrdinalIgnoreCase))
                {
                    ok = Decompress.ExtractMsi(this.Target, this.FileName);
                }
                else
                {
                    Globals.Error.Show("Unknown package type: " + this.FileName);
                    ok = false;
                }

                if (!ok)
                {
                    this.Failed++;
                }

                if (!this.IsDisposed)
                {
                    this.bCancel.Enabled = true;
                }
            }

            this.DeleteFile(file);
            this.NextFile();
        }

        private void bCancel_Click(object sender, EventArgs e)
        {
            this.CancelDownload();
        }

        private void DownloaderWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.isClosing = true;
            this.CancelDownload();
        }

        private void CancelDownload()
        {
            if (this.client != null && this.client.IsBusy)
            {
                this.client.CancelAsync();
            }
        }

        private void DeleteFile(String Fl)
        {
            try
            {
                if (File.Exists(Fl))
                {
                    File.Delete(Fl);
                }
            }
            catch (Exception e)
            {
                Globals.LogError("Unable to delete " + Fl + "", e);
            }
        }

        private void NextFile()
        {
            this.Count++;
            if (this.Count < this.FileList.Length && !this.isClosing)
            {
                this.StartDownload();
            }
            else
            {
                this.Finish();
            }
        }

        private void Finish()
        {
            if (this.client != null)
            {
                this.client.Dispose();
                this.client = null;
            }

            if (AllCompleted != null)
            {
                AllCompleted(this, EventArgs.Empty);
            }

            if (!this.IsDisposed && !this.isClosing)
            {
                Close();
            }
        }
    }
}
