using System;
using System.Collections.Generic;
using System.IO;
using System.ServiceModel;
using eAd.DataViewModels;

namespace eAd.DataAccess
{
// NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
[ServiceContract]
public interface IService
{
    [OperationContract]
    string GetHi();

    [OperationContract]
    void UploadFile(FileMetaData MetaData, FileStream stream);

    // void UploadFile(FileUploadMessage request);

    [OperationContract]
    FileDownloadReturnMessage DownloadFile(FileDownloadMessage request);

    [OperationContract]
    string SayHi(long clientID);

    [OperationContract]
    string SayHiKey(string hardwareKey);

    [OperationContract]
    List<MediaListModel> GetMyMedia(long stationID);

    [OperationContract]
    TimeSpan GetMediaDuration(long mediaID);

    [OperationContract]
    void SetStationStatus(long stationID, string status);

    [OperationContract]
    bool DoIHaveUpdates(long clientID);

    [OperationContract]
    bool DoIHaveUpdatesKey(string hardwareKey);

    [OperationContract]
    Mosaic GetMosaicForStation(long clientID);

    [OperationContract]
    Mosaic GetMosaicForStationKey(string hardwareKey);

    [OperationContract]
    bool MakeStationUnAvailable(long stationID, string rfidCode = "");

    [OperationContract]
    string GetMediaLocation(long mediaID);

    [OperationContract]
    long GetMosaicIDForStation(long stationID);

    [OperationContract]
    long GetMosaicIDForStationKey(string hardwareKey);

    [OperationContract]
    List<PositionViewModel> GetPositionsForMosaic(long mosaicID);

    [OperationContract]
    bool MakeStationAvailable(long stationID);


    [OperationContract]
    bool MessageRead(long messageID);


    [OperationContract]
    bool CaptureScreenShot(long stationID);


    [OperationContract]
    CustomerViewModel GetCustomerByRFID(string tag);


    [OperationContract]
    bool SendMessageToStation(long stationID, MessageViewModel message);

    [OperationContract]
    bool SendMessageToGroup(long groupID, MessageViewModel message);

    [OperationContract]
    List<MessageViewModel> GetAllMyMessages(long clientID);

    [OperationContract]
    List<MessageViewModel> GetAllMyMessagesKey(string hardwareKey);


    [OperationContract]
    List<CustomerViewModel> GetAllCustomers();


    [OperationContract]
    List<StationViewModel> GetAllStations();


    // TODO: Add your service operations here

    [OperationContract]
    List<StationViewModel> GetOnlineStations();

    [OperationContract]
    string RegisterDisplay(string serverKey, string hardwareKey, string displayName, string version);

    [OperationContract]
    FilesModel RequiredFiles(string serverKey, string hardwareKey, string version);

    [OperationContract]
    byte[] GetFile(string serverKey, string hardwareKey, string filePath, string fileType, long chunkOffset,
                   long chuckSize, string version);
    [OperationContract]
    ScheduleModel Schedule(string serverKey, string hardwareKey, string version);
    [OperationContract]
    bool RecieveXmlLog(string serverKey, string hardwareKey, string xml, string version);
    [OperationContract]
    void BlackList(string serverKey, string hardwareKey, int mediaId, string type, string reason, string version);
    [OperationContract]
    bool SubmitLog(string version, string serverKey, string hardwareKey, string logXml);
    [OperationContract]
    bool SubmitStats(string version, string serverKey, string hardwareKey, string statXml);

    [OperationContract]
    bool MediaInventory(string version, string serverKey, string hardwareKey,
                        [System.Xml.Serialization.SoapElementAttribute("mediaInventory")] string mediaInventory1);

    [OperationContract]
    string GetResource(string serverKey, string hardwareKey, int layoutId, string regionId, string mediaId,
                       string version);
}


// Use a data contract as illustrated in the sample below to add composite types to service operations.
}