using System.IO;
using System.ServiceModel;

namespace eAd.DataAccess
{
    [MessageContract]
    public class FileUploadMessage
    {
        [MessageBodyMember(Order = 1)] public Stream FileByteStream;
        [MessageHeader(MustUnderstand = true)] public FileMetaData MetaData;
    }
}