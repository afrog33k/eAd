using System.ServiceModel;

namespace eAd.DataAccess
{
    [ServiceContract(Namespace = "http://schemas.acme.it/2009/04")]
    public interface IFileTransferService
    {
        [OperationContract(IsOneWay = true)]
        void UploadFile(FileUploadMessage request);

        [OperationContract(IsOneWay = false)]
        FileDownloadReturnMessage DownloadFile(FileDownloadMessage request);
    }
}