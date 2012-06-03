namespace ClientApp.Core
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    public class Utilities
    {
        public static string ContentPath = (Environment.GetFolderPath(Environment.SpecialFolder.Startup) + "Content/Images/");

        public static void CreateFolder(string path)
        {
            DirectoryInfo info = new DirectoryInfo(path);
            if (info.Parent != null)
            {
                CreateFolder(info.Parent.FullName);
            }
            if (!info.Exists)
            {
                info.Create();
            }
        }

        public static void DeleteFolder(string folderPath)
        {
            DirectoryInfo info = new DirectoryInfo(folderPath);
            if (info.Exists)
            {
                info.Delete(true);
            }
        }

        public static bool FileExists(string fileFullPath)
        {
            FileInfo info = new FileInfo(fileFullPath);
            return info.Exists;
        }

        public static bool FolderExists(string folderPath)
        {
            DirectoryInfo info = new DirectoryInfo(folderPath);
            return info.Exists;
        }

        public static List<string> ListFiles(string path)
        {
            List<string> list = new List<string>();
            DirectoryInfo info = new DirectoryInfo(path);
            foreach (FileInfo info2 in info.GetFiles())
            {
                list.Add(info2.FullName);
            }
            return list;
        }
    }
}

