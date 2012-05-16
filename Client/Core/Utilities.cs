using System;
using System.Collections.Generic;
using System.IO;

namespace ClientApp.Core
{
public class Utilities
{
    public static String ContentPath = Environment.GetFolderPath(Environment.SpecialFolder.Startup) + "Content/Images/";

    /// <summary>
    /// Checks if file Exists.
    /// </summary>
    /// <returns>
    ///  true if file exists
    /// </returns>
    /// <param name='fileFullPath'>
    /// Full Path To File
    /// </param>
    public static bool FileExists(string fileFullPath)
    {
        FileInfo f = new FileInfo(fileFullPath);
        return f.Exists;
    }

    /// <summary>
    /// Checks if Folder Exists
    /// </summary>
    /// <returns>
    ///  true if folder exists
    /// </returns>
    /// <param name='folderPath'>
    ///
    /// </param>
    public static bool FolderExists(string folderPath)
    {
        DirectoryInfo f = new DirectoryInfo(folderPath);
        return f.Exists;
    }
    /// <summary>
    /// Deletes a folder.
    /// </summary>
    /// <param name='folderPath'>
    /// Folder path.
    /// </param>
    public static void DeleteFolder(string folderPath)
    {
        DirectoryInfo f = new DirectoryInfo(folderPath);

        if (f.Exists)
            f.Delete(true);
    }


    /// <summary>
    /// Recursively create folder
    /// </summary>
    /// <param name="path">Folder path to create.</param>
    public static void CreateFolder(string path)
    {
        DirectoryInfo directoryInfo = new DirectoryInfo(path);
        if (directoryInfo.Parent != null)
            CreateFolder(directoryInfo.Parent.FullName);
        if (!directoryInfo.Exists)
            directoryInfo.Create();
    }
    /// <summary>
    /// Lists the files in a folder.
    /// </summary>
    /// <returns>
    /// The files.
    /// </returns>
    /// <param name='path'>
    /// Folder Path
    /// </param>
    public static List<String> ListFiles(string path)
    {
        List<String> fileList = new List<String>();
        DirectoryInfo MyRoot = new DirectoryInfo(path);
        FileInfo[] MyFiles = MyRoot.GetFiles();
        foreach (FileInfo F in MyFiles)
        {
            fileList.Add(F.FullName);
        }
        return fileList;
    }

}


}
