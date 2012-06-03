namespace ClientApp.Update
{
    using Ionic.Zip;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;

    public class Update
    {
        public static List<string> GetUpdateInfo(string downloadsURL, string versionFile, string resourceDownloadFolder, int startLine)
        {
            if (!Directory.Exists(resourceDownloadFolder))
            {
                Directory.CreateDirectory(resourceDownloadFolder);
            }
            if (!Webdata.DownloadFromWeb(downloadsURL, versionFile, resourceDownloadFolder))
            {
                return null;
            }
            return PopulateInfoFromWeb(versionFile, resourceDownloadFolder, startLine);
        }

        public static void InstallUpdateNow(string downloadsURL, string filename, string downloadTo, bool unzip)
        {
            Webdata.DownloadFromWeb(downloadsURL, filename, downloadTo);
            if (unzip)
            {
                UnZip(downloadTo + filename, downloadTo);
            }
        }

        public static void InstallUpdateRestart(string downloadsURL, string filename, string destinationFolder, string processToEnd, string postProcess, string startupCommand, string updater)
        {
            string str = "";
            str = ((((str + "|downloadFile|" + filename) + "|URL|" + downloadsURL) + "|destinationFolder|" + destinationFolder) + "|processToEnd|" + processToEnd) + "|postProcess|" + postProcess;
            try
            {
                ProcessStartInfo info2 = new ProcessStartInfo {
                    FileName = updater,
                    Arguments = str
                };
                ProcessStartInfo startInfo = info2;
                Process.Start(startInfo);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private static List<string> PopulateInfoFromWeb(string versionFile, string resourceDownloadFolder, int line)
        {
            List<string> list = new List<string>();
            int num = 0;
            foreach (string str in File.ReadAllLines(resourceDownloadFolder + versionFile))
            {
                if (num == line)
                {
                    foreach (string str2 in str.Split(new char[] { '|' }))
                    {
                        list.Add(str2);
                    }
                    return list;
                }
                num++;
            }
            return null;
        }

        private static bool UnZip(string file, string unZipTo)
        {
            try
            {
                using (ZipFile file2 = ZipFile.Read(file))
                {
                    file2.ExtractAll(unZipTo);
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public static void UpdateMe(string updaterPrefix, string containingFolder)
        {
            DirectoryInfo info = new DirectoryInfo(containingFolder);
            foreach (FileInfo info2 in info.GetFiles(updaterPrefix + "*"))
            {
                string sourceFileName = containingFolder + info2.Name;
                string path = containingFolder + @"\" + info2.Name.Substring(updaterPrefix.Length, info2.Name.Length - updaterPrefix.Length);
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
                File.Move(sourceFileName, path);
            }
        }
    }
}

