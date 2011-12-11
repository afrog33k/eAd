using System;
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

            eAdDataEntities entities = new eAdDataEntities();

            return entities.Messages.Where(s => s.StationID == stationID && s.Sent == false).Count() > 0;

        }



        public bool MakeStationUnAvailable(long stationID, string rfidCode = "")
        {

            try
            {



                eAdDataEntities entities = new eAdDataEntities();

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



                eAdDataEntities entities = new eAdDataEntities();

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





        public bool MessageRead(long messageID)
        {

            eAdDataEntities entities = new eAdDataEntities();

            var messages = entities.Messages.Where(s => s.MessageID == messageID && s.Sent == false);

            {

                foreach (var message in messages)
                {

                    message.Sent = true;

                }

                entities.SaveChanges();



            }

            return true;

        }

        public List<MessageViewModel> GetAllMyMessages(long clientID)
        {

            eAdDataEntities entities = new eAdDataEntities();

            var messages = entities.Messages.Where(s => s.StationID == clientID && s.Sent == false);

            //foreach (var message in messages)

            //{

            //    message.Sent = true;

            //}

            // entities.SaveChanges();

            return messages.ToList().Select(c => c.CreateModel()).ToList();

        }



        public CustomerViewModel GetCustomerByRFID(string tag)
        {

            eAdDataEntities entities = new eAdDataEntities();
          var customer =  entities.Customers.Where(e => e.RFID == tag).FirstOrDefault();
          if (customer!=null)
          {
              return customer.CreateModel();
          }

            return CustomerViewModel.Empty;


        }





        public List<CustomerViewModel> GetAllCustomers()
        {

            eAdDataEntities entities = new eAdDataEntities();

            return entities.Customers.ToList().Select(c => c.CreateModel()).ToList();

        }



        public List<StationViewModel> GetOnlineStations()
        {



            return GetAllStations().Where(s => s.LastCheckIn >= DateTime.Now.AddSeconds(-10)).ToList();

        }

        public List<StationViewModel> GetAllStations()
        {

            eAdDataEntities entities = new eAdDataEntities();

            return entities.Stations.ToList().Select(c => c.CreateModel()).ToList();

        }





        public string SayHi(long clientID)
        {

            eAdDataEntities entities = new eAdDataEntities();

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
