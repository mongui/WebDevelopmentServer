using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.IO;

namespace WDS
{
    class Decompress
    {
        public static ZipFile zip = null;

        public static void ExtractZip(String Target, String FullFile, bool Overwrite = false)
        {
            String FileName = FullFile.Replace(".zip", "");
            if (Directory.Exists(Target + FileName))
            {
                Directory.Delete(Target + FileName, true);
            }

            String TempDir = Target + @"temp\";
            if (Directory.Exists(TempDir))
            {
                Directory.Delete(TempDir, true);
            }
            Directory.CreateDirectory(TempDir);

            using (zip = ZipFile.Read(Target + FullFile))
            {
                if (Overwrite)
                {
                    zip.ExtractExistingFile = ExtractExistingFileAction.OverwriteSilently;
                }

                try
                {
                    zip.ExtractAll(TempDir);
                }
                catch (Exception ex)
                {
                    Globals.Error.Show(ex.Message);
                }
                
                String[] DirContent1 = Directory.GetFiles(TempDir);
                String[] DirContent2 = Directory.GetDirectories(TempDir);

                if (DirContent2.Length == 1 && DirContent1.Length == 0)
                {
                    Directory.Move(DirContent2[0], Target + FileName);
                }
                else
                {
                    Directory.Move(TempDir, Target + FileName + @"\");
                }

                if (Directory.Exists(TempDir))
                {
                    Directory.Delete(TempDir, true);
                }
            }
        }

        public static void ExtractMsi(String Target, String FileName)
        {
            String TempDir = Target + @"temp\";
            if (Directory.Exists(TempDir))
            {
                Directory.Delete(TempDir, true);
            }
            Directory.CreateDirectory(TempDir);

            try
            {
                System.Diagnostics.Process process = new System.Diagnostics.Process();
                process.StartInfo.FileName = "msiexec";
                process.StartInfo.Arguments = "/a \"" + Target + FileName + "\" /qb- TARGETDIR=\"" + TempDir + "\"";

                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.Start();
                process.WaitForExit();
            }
            catch (Exception ex)
            {
                Globals.Error.Show(ex.Message);
            }
            
            WebServerPaths.Add(new DirectoryInfo(TempDir));
            FindWebServerTempPath();
            
            try
            {
                if (Directory.Exists(FoundWebServerPath))
                {
                    String newTarget = Target + FileName.Replace(".msi", "");
                    Directory.Move(FoundWebServerPath, newTarget);
                    Directory.Delete(TempDir, true);
                }
            }
            catch (Exception ex)
            {
                Globals.Error.Show(ex.Message);
            }
        }

        private static List<DirectoryInfo> WebServerPaths = new List<DirectoryInfo>();
        private static String FoundWebServerPath;
        public static void FindWebServerTempPath()
        {
            if (WebServerPaths.Count <= 0)
            {
                return;
            }

            DirectoryInfo Find;
            using (var iter = WebServerPaths.GetEnumerator())
            {
                iter.MoveNext();
                Find = iter.Current;
            }

            IList<String> availableFiles = Globals.Servers.WebServer.AvailableFiles;
            bool foundFile = false;

            foreach (FileInfo fileName in Find.GetFiles())
            {
                if (availableFiles.Contains(fileName.ToString()))
                {
                    FoundWebServerPath = Find.FullName.ToString();
                    WebServerPaths.Clear();
                    foundFile = true;
                }
            }

            if (!foundFile)
            {
                foreach (DirectoryInfo subDirectory in Find.GetDirectories())
                {
                    if (subDirectory.ToString() == "bin")
                    {
                        FoundWebServerPath = Find.FullName.ToString();
                        WebServerPaths.Clear();
                        return;
                    }
                    WebServerPaths.Add(subDirectory);
                }
            }
            WebServerPaths.Remove(Find);
            FindWebServerTempPath();
        }
    }
}
