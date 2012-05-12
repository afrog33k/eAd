using System;

namespace DesktopClient
{
internal class FileTransferProxyException : Exception
{
    public FileTransferProxyException(string unableToOpenTheFileToUpload)
    {
        Console.WriteLine(unableToOpenTheFileToUpload);
    }
}
}