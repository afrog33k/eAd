using System.IO;
using System.ServiceModel;

namespace eAd.DataAccess
{
    [MessageContract]
    public class FileUploadMessage
    {
        [MessageHeader(MustUnderstand = true)]
        public FileMetaData MetaData;
        [MessageBodyMember(Order = 1)]
        public Stream FileByteStream;
    }
}