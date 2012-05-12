using System;
using System.IO;

namespace eAd.Website.Controllers
{
public static class Logger
{
    public static void WriteLine(string path, string text)
    {
        //   string path = httpContext.Server.MapPath("~/Logs/GreenLots/" + "log.txt");

        irio.utilities.FileUtilities.FolderCreate(Path.GetDirectoryName(path));
        // This text is added only once to the file.
        if (!System.IO.File.Exists(path))
        {
            // Create a file to write to.
            using (StreamWriter log = File.CreateText(path))
            {
                log.WriteLine("Date: " + DateTime.Now.ToShortTimeString() + " " + text);
            }
        }


        using (
            FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Write, FileShare.ReadWrite)
        )
        {
            // This text is always added, making the file longer over time
            // if it is not deleted.
            using (StreamWriter log = new StreamWriter(stream))
            {
                log.WriteLine("Date: " + DateTime.Now.ToShortTimeString() + " " + text);


            }
        }
    }
}
}