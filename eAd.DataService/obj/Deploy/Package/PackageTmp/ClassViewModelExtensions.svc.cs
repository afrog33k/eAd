using System;
using System.IO;
using System.Linq;
using eAd.DataViewModels;

namespace eAd.DataAccess
{
public static class ClassViewModelExtensions
{
    public static CustomerViewModel CreateModel(this Customer customer)
    {
        var model = new CustomerViewModel();
        model.RFID = customer.RFID;
        if (customer.Cars.Count() > 0)
        {
            Car car = customer.Cars.FirstOrDefault();
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
        var model = new StationViewModel();
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
        model.Status = station.Status;
        model.IsOnline = station.LastCheckIn >= DateTime.Now.AddSeconds(-10);
        return model;
    }

    public static MediaListModel  CreateModel(this Medium medium)
    {
        var model = new MediaListModel();
        model.MediaID = medium.MediaID;
        model.Name = medium.Name;
        model.Location = medium.Location;

        var location = medium.Location;

        var displayableurls = new string[] {".jpg",".png",".gif"};

        if (displayableurls.Contains(Path.GetExtension(location)))
        {
            model.DisplayLocation = !(String.IsNullOrEmpty(location))
                                    ? "Uploads/Temp/Media" + "/" + "Thumb" +
                                    Path.GetFileNameWithoutExtension(location) + ".jpg"
                                    : "/Content/Images/no_image.gif";
        }
        else
        {
            model.DisplayLocation = "Uploads/Temp/Media" + "/" + "Thumb" +
                                    Path.GetFileNameWithoutExtension(location) + ".jpg";
        }

        return model;
    }


    public static PositionViewModel CreateModel(this Position position)
    {
        return new PositionViewModel
        {
            PositionID = position.PositionID,
            MosaicID = (long) position.MosaicID,
            Name = position.Name,
            ContentURL = position.ContentURL,
            Height = position.Height,
            Width = position.Width,
            X = position.X,
            Y = position.Y,
            Media = position.Media.Select(m=>m.CreateModel()).ToList()
            // (from m in position.Media select m.MediaID).ToList<long>()
        };
    }

    public static MessageViewModel CreateModel(this Message message)
    {
        return new MessageViewModel
        {
            Command = message.Command,
            ID = message.MessageID,
            Sent = message.Sent,
            StationID = message.StationID,
            Text = message.Text,
            Type = message.Type,
            UserID = message.UserID
        };
    }
}
}