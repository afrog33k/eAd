using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
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
         List<MediaListModel> GetMyMedia(long stationID);

         [OperationContract]
         TimeSpan GetMediaDuration(long mediaID);
         [OperationContract]
        void SetStationStatus(long stationID, string status);
        [OperationContract]

        bool DoIHaveUpdates(long clientID);

        [OperationContract]

        Mosaic GetMosaicForStation(long clientID);

        [OperationContract]

        bool MakeStationUnAvailable(long stationID, string rfidCode = "");

        [OperationContract]
        string GetMediaLocation(long mediaID);

        [OperationContract]
         long GetMosaicIDForStation(long stationID);
   

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

        List<CustomerViewModel> GetAllCustomers();



        [OperationContract]

        List<StationViewModel> GetAllStations();



        // TODO: Add your service operations here

        [OperationContract]

        List<StationViewModel> GetOnlineStations();
    }


    // Use a data contract as illustrated in the sample below to add composite types to service operations.
    [DataContract]
    public class CompositeType
    {
        bool boolValue = true;
        string stringValue = "Hello ";

        [DataMember]
        public bool BoolValue
        {
            get { return boolValue; }
            set { boolValue = value; }
        }

        [DataMember]
        public string StringValue
        {
            get { return stringValue; }
            set { stringValue = value; }
        }
    }
}
