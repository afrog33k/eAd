using System;
using System.Linq;
using eAd.DataViewModels;

namespace eAd.DataAccess
{
    public static  class ClassViewModelExtensions
    {
        public static CustomerViewModel CreateModel(this Customer customer)
        {
            CustomerViewModel model = new CustomerViewModel();
            model .RFID = customer.RFID;
            if (customer.Cars.Count()>0)
            {
                var car = customer.Cars.FirstOrDefault();
                model.CarLicense = car.License;
                model.CarMake = car.Make;
                model.CarModel = car.Model;
                model.ChargeRemaining = car.BatteryCharge;
                model.LastRechargeDate = car.LastRechargeDate;
            }
         
            
            model.AccountBalance = customer.Balance;
            model.LastBillAmount = customer.LastBillAmount;
            model.Name = customer.Name;
            model.RFID = customer.RFID;
            model.CustomerID = customer.CustomerID;
            model.Email = customer.Email;
            model.Picture = customer.Picture;
            model.Phone = customer.Phone;
            model.Address = customer.Address;
            return model;
        }
        public static StationViewModel CreateModel(this Station station)
        {
            StationViewModel model = new StationViewModel();
            model.StationID = station.StationID;
            model.Name = station.Name;
            if (station.Available != null) 
                model.Available = (bool) station.Available;
            else
            {
                model.Available = false;
            }
            model.Location = station.Location;
            model.LastCheckIn = station.LastCheckIn;
            model.IsOnline = station.LastCheckIn >= DateTime.Now.AddSeconds(-10);
            return model;
        }
        public static PositionViewModel CreateModel(this Position message)
        {
            PositionViewModel model = new PositionViewModel();
            model.PositionID = (long)message.PositionID;
            model.MosaicID = message.MosaicID;
            model.Name = message.Name;
            model.ContentURL =  message.ContentURL;
            model.Height = message.Height;
            model.Width = message.Width;
            model.X = message.X;
            model.Y = message.Y;  
            return model;
        }

        public static MessageViewModel CreateModel(this Message message)
        {
            MessageViewModel model = new MessageViewModel();
            model.Command = message.Command;
            model.ID = message.MessageID;
            model.Sent = message.Sent;
            model.StationID = message.StationID;
            model.Text = message.Text;
            model.Type = message.Type;
            model.UserID = message.UserID;
            return model;
        }
      

    }
}