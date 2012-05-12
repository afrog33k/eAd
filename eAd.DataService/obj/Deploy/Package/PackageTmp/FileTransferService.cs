namespace eAd.DataAccess
{
public class FileTransferService : IFileTransferService
{
    #region IFileTransferService Members

    public void UploadFile(FileUploadMessage request)
    {
        // IMPLEMENTATION HERE
    }

    public FileDownloadReturnMessage DownloadFile(FileDownloadMessage request)
    {
        // IMPLEMENTATION HERE
        return null;
    }

    #endregion
}
}