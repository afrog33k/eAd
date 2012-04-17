using System.IO;
using System.ServiceModel;

namespace eAd.DataAccess
{
    [MessageContract]
    public class FileDownloadReturnMessage
    {
        [MessageHeader(MustUnderstand = true)] public FileMetaData DownloadedFileMetadata;
        [MessageBodyMember(Order = 1)] public Stream FileByteStream;

        public FileDownloadReturnMessage(FileMetaData metaData, Stream stream)
        {
            DownloadedFileMetadata = metaData;
            FileByteStream = stream;
        }
    }
}