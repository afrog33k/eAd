using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using eAd.DataViewModels;

namespace eAd.DataAccess
{
   


        public partial class Position
        {
            public IEnumerable<Medium> Media
            {
                get { return this.PositionMediums.Select(p => p.Medium); }
            }
        }

        public partial class Medium
        {
            public IEnumerable<Position> Positions
            {
                get { return this.PositionMediums.Select(p => p.Position); }
            }
        }

public static class ClassViewModelExtensions
{

 

    public static readonly string[] Displayableurls = new string[] { ".jpg", ".png", ".gif" };
   public static readonly string[] PowerpointExtensions= new string[] { ".ppt", ".pptx", ".pps" };
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

        try
        {
        var location = medium.Location;

        var extension = Path.GetExtension(location);
        if (!String.IsNullOrEmpty(extension) && !String.IsNullOrEmpty(location) && !String.IsNullOrEmpty(extension))
        {
            var path = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) +
                       HttpRuntime.AppDomainAppVirtualPath;
            // var path = VirtualPathUtility.ToAbsolute("~/");

            if (path.Contains("eAd.Website"))
            {
                path = "http://localhost/eAd.Website/";
            }

            model.ThumbnailUrl = medium.ThumbnailUrl;

            if (Displayableurls.Contains(Path.GetExtension(location)))
            {
                model.DisplayLocation = !(String.IsNullOrEmpty(location))
                                            ? "/Uploads/Temp/Media" + "/" + "Thumb" +
                                              Path.GetFileNameWithoutExtension(location) + extension
                                            : "/Content/Images/no_image.gif";
            }
            if (!String.IsNullOrEmpty(medium.ThumbnailUrl) &&
                     Displayableurls.Contains(Path.GetExtension(medium.ThumbnailUrl)))
            {
                model.DisplayLocation = medium.ThumbnailUrl;
            }
            else
            {
                model.DisplayLocation = "/Uploads/Temp/Media" + "/" + "Thumb" +
                                        Path.GetFileNameWithoutExtension(location) + ".jpg";
            }
            model.Location = path + model.Location.Replace("\\", "/");
            model.DisplayLocation = path + model.DisplayLocation.Replace("\\", "/");
            model.ThumbnailUrl = path + model.ThumbnailUrl.Replace("\\", "/");

        }
        }
        catch (Exception)
        {
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