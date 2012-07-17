using System.Runtime.Serialization;

namespace eAd.DataAccess
{
[DataContract(Namespace = "http://schemas.acme.it/2009/04")]
public class FileMetaData
{
    [DataMember(Name = "FileType", Order = 0, IsRequired = true)] public FileTypeEnum FileType;
    [DataMember(Name = "LocalFilename", Order = 1, IsRequired = false)] public string LocalFileName;
    [DataMember(Name = "RemoteFilename", Order = 2, IsRequired = false)] public string RemoteFileName;
    [DataMember(Name = "StationID", Order = 2, IsRequired = true)] public string StationID;

    public FileMetaData(
        string localFileName,
        string remoteFileName, string stationID)
    {
        LocalFileName = localFileName;
        RemoteFileName = remoteFileName;
        FileType = FileTypeEnum.Generic;
        StationID = stationID;
    }

    public FileMetaData(
        string localFileName,
        string remoteFileName,
        FileTypeEnum fileType, string stationID)
    {
        LocalFileName = localFileName;
        RemoteFileName = remoteFileName;
        FileType = fileType;
        StationID = stationID;
    }
}
}