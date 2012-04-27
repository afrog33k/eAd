using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Objects.DataClasses;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Web;
using System.Xml.Serialization;
using eAd.DataViewModels;
using eAd.Utilities;
using irio.utilities;

namespace eAd.DataAccess
{
    public partial class Service : IService
    {

        public static string ServerPath = ConfigurationSettings.AppSettings.Get("ServerPath");
		#region Methods (11) 

		// Public Methods (11) 

        public static string AppPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath ;

        public string GetData(int value)
        {
            return string.Format("You entered: {0}", value);
        }

        public CompositeType GetDataUsingDataContract(CompositeType composite)
        {
            if (composite == null)
            {
                throw new ArgumentNullException("composite");
            }

            if (composite.BoolValue)
            {
                composite.StringValue = composite.StringValue + "Suffix";
            }

            return composite;
        }

        #endregion Methods 



        #region IService Members

        public bool CaptureScreenShot(long stationID)
        {
            try
            {
                var container = new eAdDataContainer();

                var station = (from s in container.Stations
                               where s.StationID == stationID
                               select s).FirstOrDefault<Station>();

                if (station != null)
                {
                    station.Available = true;

                    var entity = new Message
                                     {
                                         StationID = stationID,
                                         Text = "",
                                         Command = "Screenshot",
                                         Type = "Status",
                                         UserID = 1L,
                                         DateAdded = DateTime.Now
                                     };

                    container.Messages.AddObject(entity);

                    container.SaveChanges();
                }
            }

            catch (Exception)
            {
                return false;
            }

            return true;
        }


        public bool DoIHaveUpdates(long stationID)
        {
            var container = new eAdDataContainer();

            return ((from s in container.Messages
                     where (s.StationID == stationID) && !s.Sent
                     select s).Count<Message>() > 0);
        }


        public FileDownloadReturnMessage DownloadFile(FileDownloadMessage request)
        {
            FileDownloadReturnMessage message;

            string localFileName = request.MetaData.LocalFileName;

            try
            {
                string str2 = ConfigurationManager.AppSettings["FileTransferPath"];

                string path = Path.Combine(str2, request.MetaData.RemoteFileName);

                Stream stream = new FileStream(path, FileMode.Open);

                message = new FileDownloadReturnMessage(new FileMetaData(localFileName, path, "1"), stream);
            }

            catch (IOException exception)
            {
                throw new FaultException<IOException>(exception);
            }

            return message;
        }


        public List<CustomerViewModel> GetAllCustomers()
        {
            var container = new eAdDataContainer();

            return (from c in container.Customers.ToList() select c.CreateModel()).ToList<CustomerViewModel>();
        }


        public List<MessageViewModel> GetAllMyMessages(long clientID)
        {
            var container = new eAdDataContainer();

            return (from c in
                        (from s in container.Messages
                         where (s.StationID == clientID) && !s.Sent
                         select s).ToList<Message>()
                    select c.CreateModel()).ToList<MessageViewModel>();
        }


        public List<StationViewModel> GetAllStations()
        {
            var container = new eAdDataContainer();

            return (from c in container.Stations.ToList() select c.CreateModel()).ToList<StationViewModel>();
        }


        public CustomerViewModel GetCustomerByRFID(string tag)
        {
            var container = new eAdDataContainer();

            var car = (from e in container.Cars
                       where e.RFID == tag
                       select e).FirstOrDefault<Car>();

            if (car != null)
            {
                Customer customer = car.Customer;

                if (customer != null)
                {
                    return customer.CreateModel();
                }
            }

            return CustomerViewModel.Empty;
        }


        public string GetHi()
        {
            return "Hi";
        }


        public TimeSpan GetMediaDuration(long mediaID)
        {
            var container = new eAdDataContainer();

            return (from s in container.Media
                    where s.MediaID == mediaID
                    select s).FirstOrDefault<Medium>().Duration.Value;
        }


        public string GetMediaLocation(long mediaID)
        {
            var container = new eAdDataContainer();

            return (from s in container.Media
                    where s.MediaID == mediaID
                    select s).FirstOrDefault<Medium>().Location;
        }


        public Mosaic GetMosaicForStation(long stationID)
        {
            var container = new eAdDataContainer();

            if ((from s in container.Stations
                 where s.StationID == stationID
                 select s).FirstOrDefault<Station>() != null)
            {
                var grouping = (from m in container.Groupings
                                where m.Mosaic != null
                                select m).FirstOrDefault<Grouping>();

                if (grouping != null)
                {
                    return grouping.Mosaic;
                }
            }

            return (from m in container.Mosaics
                    where m.Name.Contains("as")
                    select m).FirstOrDefault<Mosaic>();
        }


        public long GetMosaicIDForStation(long stationID)
        {
            var container = new eAdDataContainer();

            if ((from s in container.Stations
                 where s.StationID == stationID
                 select s).FirstOrDefault<Station>() != null)
            {
                var grouping = (from m in container.Groupings
                                where m.Mosaic != null
                                select m).FirstOrDefault<Grouping>();

                if (grouping != null)
                {
                    return grouping.MosaicID;
                }
            }

            return (from m in container.Mosaics
                    where m.Name.Contains("as")
                    select m).FirstOrDefault<Mosaic>().MosaicID;
        }


        public List<MediaListModel> GetMyMedia(long stationID)
        {
            var container = new eAdDataContainer();

            var source = new List<MediaListModel>();

            var station = (from s in container.Stations
                           where s.StationID == stationID
                           select s).FirstOrDefault<Station>();

            if (station != null)
            {
                EntityCollection<Grouping> groupings = station.Groupings;

                foreach (Grouping grouping in groupings)
                {
                    foreach (Theme theme in grouping.Themes)
                    {
                        using (IEnumerator<Medium> enumerator3 = theme.Media.GetEnumerator())
                        {
                            Func<MediaListModel, bool> predicate = null;

                            Medium media;

                            while (enumerator3.MoveNext())
                            {
                                media = enumerator3.Current;

                                if (predicate == null)
                                {
                                    predicate = l => l.MediaID == media.MediaID;
                                }

                                if (source.Where(predicate).Count<MediaListModel>() <= 0)
                                {
                                    var item = new MediaListModel
                                                   {
                                                       MediaID = media.MediaID,
                                                       DisplayLocation = media.Location,
                                                       Duration = media.Duration.Value
                                                   };

                                    source.Add(item);
                                }
                            }
                        }
                    }
                }
            }

            return source;
        }


        public List<StationViewModel> GetOnlineStations()
        {
            return GetAllStations().Where(delegate(StationViewModel s)
                                              {
                                                  DateTime? lastCheckIn = s.LastCheckIn;

                                                  DateTime time = DateTime.Now.AddSeconds(-10.0);

                                                  return (lastCheckIn.HasValue && (lastCheckIn.GetValueOrDefault() >= time));
                                              }).ToList<StationViewModel>();
        }


        public List<PositionViewModel> GetPositionsForMosaic(long mosaicID)
        {
            var container = new eAdDataContainer();

            List<PositionViewModel> list = new List<PositionViewModel>();
            var firstOrDefault = (container.Mosaics.Where(m => m.MosaicID == mosaicID)).FirstOrDefault();
            if (firstOrDefault != null)
                foreach (Position p in firstOrDefault.Positions)
                    list.Add(p.CreateModel());
            return list;
        }


        public bool MakeStationAvailable(long stationID)
        {
            try
            {
                var container = new eAdDataContainer();

                var station = (from s in container.Stations
                               where s.StationID == stationID
                               select s).FirstOrDefault<Station>();

                if (station != null)
                {
                    station.Available = true;

                    var entity = new Message
                                     {
                                         StationID = stationID,
                                         Text = "",
                                         Command = "Make Available",
                                         Type = "Status",
                                         UserID = 1L,
                                         DateAdded = DateTime.Now
                                     };

                    container.Messages.AddObject(entity);

                    container.SaveChanges();
                }
            }

            catch (Exception)
            {
                return false;
            }

            return true;
        }


        public bool MakeStationUnAvailable(long stationID, string rfidCode = "")
        {
            try
            {
                var container = new eAdDataContainer();

                var station = (from s in container.Stations
                               where s.StationID == stationID
                               select s).FirstOrDefault<Station>();

                if (station != null)
                {
                    station.Available = false;

                    var entity = new Message
                                     {
                                         StationID = stationID,
                                         Text = rfidCode,
                                         Command = "Make UnAvailable",
                                         Type = "Status",
                                         UserID = 1L,
                                         DateAdded = DateTime.Now
                                     };

                    container.Messages.AddObject(entity);

                    container.SaveChanges();
                }
            }

            catch (Exception)
            {
                return false;
            }

            return true;
        }


        public bool MessageRead(long messageID)
        {
            var container = new eAdDataContainer();

            IQueryable<Message> queryable = from s in container.Messages
                                            where (s.MessageID == messageID) && !s.Sent
                                            select s;

            foreach (Message message in queryable)
            {
                message.Sent = true;

                message.DateReceived = DateTime.Now;
            }

            container.SaveChanges();

            return true;
        }


        public string SayHi(long clientID)
        {
            var container = new eAdDataContainer();

            var station = (from s in container.Stations
                           where s.StationID == clientID
                           select s).FirstOrDefault<Station>();

            if (station != null)
            {
                station.Available = false;

                station.LastCheckIn = DateTime.Now;

                container.SaveChanges();

                return "Hi there";
            }

            return "Invalid";
        }


        public bool SendMessageToGroup(long groupID, MessageViewModel message)
        {
            try
            {
                var container = new eAdDataContainer();

                var grouping = (from s in container.Groupings
                                where s.GroupingID == groupID
                                select s).FirstOrDefault<Grouping>();

                if (grouping != null)
                {
                    foreach (Station station in grouping.Stations)
                    {
                        station.Available = false;

                        var entity = new Message
                                         {
                                             DateAdded = DateTime.Now,
                                             StationID = station.StationID,
                                             Text = message.Text,
                                             Command = message.Command,
                                             Type = message.Type,
                                             UserID = message.UserID
                                         };

                        container.Messages.AddObject(entity);
                    }
                }

                container.SaveChanges();
            }

            catch (Exception)
            {
                return false;
            }

            return true;
        }


        public bool SendMessageToStation(long stationID, MessageViewModel message)
        {
            try
            {
                var container = new eAdDataContainer();

                var station = (from s in container.Stations
                               where s.StationID == stationID
                               select s).FirstOrDefault<Station>();

                if (station != null)
                {
                    station.Available = false;

                    var entity = new Message
                                     {
                                         StationID = station.StationID,
                                         DateAdded = DateTime.Now,
                                         Text = message.Text,
                                         Command = message.Command,
                                         Type = message.Type,
                                         UserID = message.UserID
                                     };

                    container.Messages.AddObject(entity);
                }

                container.SaveChanges();
            }

            catch (Exception)
            {
                return false;
            }

            return true;
        }


        public void SetStationStatus(long stationID, string status)
        {
            var container = new eAdDataContainer();

            foreach (Station station in from s in container.Stations
                                        where s.StationID == stationID
                                        select s)
            {
                station.Status = status;
            }

            container.SaveChanges();
        }


        public void UploadFile(FileMetaData MetaData, FileStream stream)
        {
            try
            {
                string str = ConfigurationManager.AppSettings["FileTransferPath"];

                using (var stream2 = new FileStream(Path.Combine(str, MetaData.RemoteFileName), FileMode.Create))
                {
                    var buffer = new byte[0x10000];

                    for (int i = stream.Read(buffer, 0, 0x10000); i > 0; i = stream.Read(buffer, 0, 0x10000))
                    {
                        stream2.Write(buffer, 0, i);
                    }
                }
            }

            catch (IOException exception)
            {
                throw new FaultException<IOException>(exception);
            }
        }

        #endregion
    }
}