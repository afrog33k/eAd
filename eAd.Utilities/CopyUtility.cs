using System;
using System.IO;

namespace eAd.Utilities
{
public interface  IReporter
{
    void Report(string message);
}

public static class CopyUtility
{
    private static string _currentPath = "";
    // Copies all files from one directory to another.
    public static void CopyAllFiles(string sourceDirectory, string targetDirectory, bool recursive, IReporter reporter)
    {
        if (sourceDirectory == null)
            throw new ArgumentNullException("sourceDirectory");
        if (targetDirectory == null)
            throw new ArgumentNullException("targetDirectory");
        _currentPath = sourceDirectory;
        // Call the recursive method.
        CopyAllFiles(new DirectoryInfo(sourceDirectory), new DirectoryInfo(targetDirectory), recursive,reporter);
    }

    // Copies all files from one directory to another.
    public static void CopyAllFiles(DirectoryInfo source, DirectoryInfo target, bool recursive, IReporter reporter
                                   )
    {
      

        if (source == null)
            throw new ArgumentNullException("source");
        if (target == null)
            throw new ArgumentNullException("target");

        // If the source doesn't exist, we have to throw an exception.
        if (!source.Exists)
            throw new DirectoryNotFoundException("Source directory not found: " + source.FullName);
        // If the target doesn't exist, we create it.
        if (!target.Exists)
            target.Create();

        // Get all files and copy them over.
        foreach (FileInfo file in source.GetFiles())
        {
            reporter.Report("Copying " + file.Name + " to " + target.FullName);

            if (File.Exists(target.FullName))
            {
                File.Delete(target.FullName);
            }

            file.CopyTo(Path.Combine(target.FullName, file.Name), true);
        }

        // Return if no recursive call is required.
        if (!recursive)
            return;

        // Do the same for all sub directories.
        foreach (DirectoryInfo directory in source.GetDirectories())
        {
            if (directory.FullName != _currentPath)
                CopyAllFiles(directory, new DirectoryInfo(Path.Combine(target.FullName, directory.Name)),
                             recursive,reporter);
        }
    }
}

}