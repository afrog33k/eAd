using System.ServiceModel;

namespace eAd.DataAccess
{
[MessageContract]
public class FileDownloadMessage
{
    [MessageHeader(MustUnderstand = true)] public FileMetaData MetaData;
}
}