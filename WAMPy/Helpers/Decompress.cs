using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace WDS
{
    class Decompress
    {
        // Executables that mark the root folder of a server inside an extracted package.
        private static readonly String[] KnownExecutables = {
            "httpd.exe", "apache.exe", "nginx.exe", "lighttpd.exe", "node.exe", "iisexpress.exe",
            "php-cgi.exe", "php.exe", "mysqld.exe", "mariadbd.exe"
        };

        /// <summary>
        /// Extracts Target\FullFile into Target\[file name without .zip].
        /// If the zip holds a single folder, that folder becomes the target folder.
        /// Returns false if something failed.
        /// </summary>
        public static bool ExtractZip(String Target, String FullFile, bool Overwrite = false)
        {
            String FileName = Path.GetFileNameWithoutExtension(FullFile);
            String Destination = Path.Combine(Target, FileName);
            String TempDir = Path.Combine(Target, "temp");

            try
            {
                if (Directory.Exists(Destination))
                {
                    if (!Overwrite)
                    {
                        Globals.Error.Show("The folder " + Destination + " already exists.");
                        return false;
                    }
                    Directory.Delete(Destination, true);
                }

                if (Directory.Exists(TempDir))
                {
                    Directory.Delete(TempDir, true);
                }
                Directory.CreateDirectory(TempDir);
                String TempRoot = Path.GetFullPath(TempDir) + Path.DirectorySeparatorChar;

                using (ZipArchive zip = ZipFile.OpenRead(Path.Combine(Target, FullFile)))
                {
                    foreach (ZipArchiveEntry entry in zip.Entries)
                    {
                        String outPath = Path.GetFullPath(Path.Combine(TempDir, entry.FullName));

                        // Refuse entries that would be written outside the temp folder ("zip slip").
                        if (!outPath.StartsWith(TempRoot, StringComparison.OrdinalIgnoreCase))
                        {
                            throw new InvalidDataException("The package contains an invalid path: " + entry.FullName);
                        }

                        if (String.IsNullOrEmpty(entry.Name))
                        {
                            Directory.CreateDirectory(outPath);
                            continue;
                        }

                        Directory.CreateDirectory(Path.GetDirectoryName(outPath));
                        entry.ExtractToFile(outPath, true);
                    }
                }

                String[] Files = Directory.GetFiles(TempDir);
                String[] Dirs = Directory.GetDirectories(TempDir);

                if (Dirs.Length == 1 && Files.Length == 0)
                {
                    Directory.Move(Dirs[0], Destination);
                    Directory.Delete(TempDir, true);
                }
                else
                {
                    Directory.Move(TempDir, Destination);
                }
                return true;
            }
            catch (Exception ex)
            {
                Globals.Error.Show("Unable to extract " + FullFile + "", ex);
                try
                {
                    if (Directory.Exists(TempDir)) Directory.Delete(TempDir, true);
                }
                catch (Exception) { }
                return false;
            }
        }

        /// <summary>
        /// Extracts an .msi package (administrative install) into Target\[file name without .msi].
        /// </summary>
        public static bool ExtractMsi(String Target, String FileName)
        {
            String TempDir = Path.Combine(Target, "temp");
            try
            {
                if (Directory.Exists(TempDir))
                {
                    Directory.Delete(TempDir, true);
                }
                Directory.CreateDirectory(TempDir);

                using (System.Diagnostics.Process process = new System.Diagnostics.Process())
                {
                    process.StartInfo.FileName = "msiexec";
                    process.StartInfo.Arguments = "/a \"" + Path.Combine(Target, FileName) + "\" /qb- TARGETDIR=\"" + TempDir + "\"";
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.CreateNoWindow = true;
                    process.Start();
                    process.WaitForExit();
                }

                String found = FindServerRoot(new DirectoryInfo(TempDir));
                if (found == null)
                {
                    Globals.Error.Show("No server executable was found inside " + FileName + ".");
                    Directory.Delete(TempDir, true);
                    return false;
                }

                String newTarget = Path.Combine(Target, Path.GetFileNameWithoutExtension(FileName));
                if (Directory.Exists(newTarget))
                {
                    Directory.Delete(newTarget, true);
                }
                Directory.Move(found, newTarget);
                if (Directory.Exists(TempDir))
                {
                    Directory.Delete(TempDir, true);
                }
                return true;
            }
            catch (Exception ex)
            {
                Globals.Error.Show("Unable to extract " + FileName + "", ex);
                return false;
            }
        }

        /// <summary>
        /// Breadth-first search for the folder that holds a server executable
        /// (or the parent of a "bin" folder).
        /// </summary>
        private static String FindServerRoot(DirectoryInfo start)
        {
            Queue<DirectoryInfo> pending = new Queue<DirectoryInfo>();
            pending.Enqueue(start);

            while (pending.Count > 0)
            {
                DirectoryInfo dir = pending.Dequeue();

                foreach (FileInfo file in dir.GetFiles())
                {
                    if (Array.IndexOf(KnownExecutables, file.Name.ToLowerInvariant()) >= 0)
                    {
                        return dir.FullName;
                    }
                }

                foreach (DirectoryInfo sub in dir.GetDirectories())
                {
                    if (String.Equals(sub.Name, "bin", StringComparison.OrdinalIgnoreCase))
                    {
                        return dir.FullName;
                    }
                    pending.Enqueue(sub);
                }
            }
            return null;
        }
    }
}
