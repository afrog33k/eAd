﻿using System;
using System.Collections.Generic;
using System.Linq;
using eAd.DataViewModels;

namespace eAd.DataAccess
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    public class Service : IService//(SessionMode = SessionMode.Required,CallbackContract = typeof(IServiceCallback))]
    {
        public string GetData(int value)
        {
            return string.Format("You entered: {0}", value);
        }
       //public string GetHi()
       //{
       //    return "Hello There From Server: " + DateTime.Now;
       //}

        public string GetHi()
        {

            return "Hi";

        }
        public CompositeType GetDataUsingDataContract(CompositeType composite)
        {
            if (composite == null)
            {
                throw new ArgumentNullException("composite");
            }
            if (composite.BoolValue)
            {
                composite.StringValue += "Suffix";
            }
            return composite;
        }

     





        public bool DoIHaveUpdates(long stationID)
        {

            eAdDataContainer entities = new eAdDataContainer();

            return entities.Messages.Where(s => s.StationID == stationID && s.Sent == false).Count() > 0;

        }

        public bool SendMessageToGroup(long groupID, MessageViewModel message)
        {
            try
            {

                eAdDataContainer entities = new eAdDataContainer();

                var grouping = entities.Groupings.Where(s => s.GroupingID == groupID).FirstOrDefault();

                if (grouping != null)
                {
                    foreach (var station in grouping.Stations)
                    {


                        station.Available = false;
                       
                        Message statusChange = new Message();
                        statusChange.DateAdded = DateTime.Now;
                        statusChange.StationID = station.StationID;

                        statusChange.Text = message.Text;

                        statusChange.Command = message.Command;

                        statusChange.Type = message.Type;

                        statusChange.UserID = message.UserID;

                        entities.Messages.AddObject(statusChange);

                       

                    }
                }
                entities.SaveChanges();
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

                eAdDataContainer entities = new eAdDataContainer();

                var station = entities.Stations.Where(s => s.StationID == stationID).FirstOrDefault();

                if (station != null)
                {
                   


                        station.Available = false;

                        Message statusChange = new Message();

                        statusChange.StationID = station.StationID;
                        statusChange.DateAdded = DateTime.Now;
                        statusChange.Text = message.Text;

                        statusChange.Command = message.Command;

                        statusChange.Type = message.Type;

                        statusChange.UserID = message.UserID;

                        entities.Messages.AddObject(statusChange);



                    
                }
                entities.SaveChanges();
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



                eAdDataContainer entities = new eAdDataContainer();

                var station = entities.Stations.Where(s => s.StationID == stationID).FirstOrDefault();

                if (station != null)
                {

                    station.Available = false;

                    Message statusChange = new Message();

                    statusChange.StationID = stationID;

                    statusChange.Text = rfidCode;

                    statusChange.Command = "Make UnAvailable";

                    statusChange.Type = "Status";

                    statusChange.UserID = 1;
                    statusChange.DateAdded = DateTime.Now;
                    entities.Messages.AddObject(statusChange);

                    entities.SaveChanges();

                }

            }

            catch (Exception)
            {



                return false;

            }

            return true;

        }



        public bool MakeStationAvailable(long stationID)
        {

            try
            {



                eAdDataContainer entities = new eAdDataContainer();

                var station = entities.Stations.Where(s => s.StationID == stationID).FirstOrDefault();

                if (station != null)
                {

                    station.Available = true;

                    Message statusChange = new Message();

                    statusChange.StationID = stationID;

                    statusChange.Text = "";

                    statusChange.Command = "Make Available";

                    statusChange.Type = "Status";

                    statusChange.UserID = 1;
                    statusChange.DateAdded = DateTime.Now;
                    entities.Messages.AddObject(statusChange);

                    entities.SaveChanges();

                }

            }

            catch (Exception)
            {



                return false;

            }

            return true;

        }

        public List<MediaListModel> GetMyMedia(long stationID)
        {
          
            var entities = new eAdDataContainer();

            var list = new List<MediaListModel>();
            var me = entities.Stations.Where(s => s.StationID == stationID).FirstOrDefault();

            if (me != null)
            {
                var myGroups = me.Groupings;

                foreach (var grouping in myGroups)
                {
                    foreach (var theme in grouping.Themes)
                    {

                        foreach (var media in theme.Media)
                        {
                         
                            if(list.Where(l=>l.MediaID==media.MediaID).Count()<=0)
                                list.Add(new MediaListModel(){MediaID = media.MediaID,Location = media.Location,Duration =  (TimeSpan) media.Duration});
                        }
                    }
                }
                
            }
            return list;
        }



        public bool MessageRead(long messageID)
        {

            var entities = new eAdDataContainer();

            var messages = entities.Messages.Where(s => s.MessageID == messageID && s.Sent == false);

            {

                foreach (var message in messages)
                {

                    message.Sent = true;
                    message.DateReceived = DateTime.Now;

                }

                entities.SaveChanges();



            }

            return true;

        }

        public List<MessageViewModel> GetAllMyMessages(long clientID)
        {

            eAdDataContainer entities = new eAdDataContainer();

            var messages = entities.Messages.Where(s => s.StationID == clientID && s.Sent == false);

        return messages.ToList().Select(c => c.CreateModel()).ToList();

        }



        public CustomerViewModel GetCustomerByRFID(string tag)
        {

            eAdDataContainer entities = new eAdDataContainer();
          var customer =  entities.Customers.Where(e => e.RFID == tag).FirstOrDefault();
          if (customer!=null)
          {
              return customer.CreateModel();
          }

            return CustomerViewModel.Empty;


        }





        public List<CustomerViewModel> GetAllCustomers()
        {

            eAdDataContainer entities = new eAdDataContainer();

            return entities.Customers.ToList().Select(c => c.CreateModel()).ToList();

        }



        public List<StationViewModel> GetOnlineStations()
        {



            return GetAllStations().Where(s => s.LastCheckIn >= DateTime.Now.AddSeconds(-10)).ToList();

        }

        public List<StationViewModel> GetAllStations()
        {

            eAdDataContainer entities = new eAdDataContainer();

            return entities.Stations.ToList().Select(c => c.CreateModel()).ToList();

        }





        public string SayHi(long clientID)
        {

            eAdDataContainer entities = new eAdDataContainer();

            var thisstation = entities.Stations.Where(s => s.StationID == clientID).FirstOrDefault();

            if (thisstation != null)
            {

                thisstation.Available = false;

                thisstation.LastCheckIn = DateTime.Now;

                entities.SaveChanges();

                return "Hi there";

            }

            return "Invalid";

        }
    }
}
