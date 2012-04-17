using System.IO;
using System.ServiceModel;

namespace eAd.DataAccess
{
    [MessageContract]
    public class FileDownloadReturnMessage
    {
        public FileDownloadReturnMessage(FileMetaData metaData, Stream stream)
        {
            this.DownloadedFileMetadata = metaData;
            this.FileByteStream = stream;
        }
 
        [MessageHeader(MustUnderstand = true)]
        public FileMetaData DownloadedFileMetadata;
        [MessageBodyMember(Order = 1)]
        public Stream FileByteStream;
    }
}