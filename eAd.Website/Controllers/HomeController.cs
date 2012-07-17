using System;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Xml.Serialization;
using eAd.DataAccess;
using eAd.Website.eAdDataService;
using System.Linq;
using System.IO;
namespace eAd.Website.Controllers
{
public class HomeController : Controller
{
    public static Controller Instance = null;
    private eAdEntities db = new eAdEntities();
    public void DownloadInfo(HttpContextBase httpContext,string xmlUrl, string stationID, bool start=true)
    {
        try
        {

            Station station;
            Customer customer;
            Car car;

            StreamReader reader = new StreamReader(WebRequest.Create(xmlUrl).GetResponse().GetResponseStream());
            XmlSerializer xSerializer = new XmlSerializer(typeof(proton));
            proton greenlotsInfo = (proton)xSerializer.Deserialize(reader);

            if(db.Stations.Where(s=>s.UniqueID==stationID).Count() <=0)
            {
                station = new Station {UniqueID = stationID};
                db.Stations.AddObject(station);
                db.SaveChanges();
            }
            else
            {
                station = db.Stations.Where(s => s.UniqueID.Trim() == stationID.Trim()).FirstOrDefault();

            }



            string path = httpContext.Server.MapPath("~/Logs/GreenLots/" + "log.txt");



            if (station != null)
            {
                station.Name = greenlotsInfo.Location.information[0].name;
                station.Address = greenlotsInfo.Location.information[0].address + "\n" + greenlotsInfo.Location.information[0].address1 + "\n" + greenlotsInfo.Location.information[0].address2;
                station.PostalCode = greenlotsInfo.Location.information[0].postal;
                try
                {
                    station.Rate = Convert.ToDouble( greenlotsInfo.Location.information[0].rate);
                }
                catch(Exception ex)
                {

                }
            }

            var customerInfo = greenlotsInfo.User.information[0];
            if (db.Customers.Where(s => s.Email == customerInfo.email).Count() <= 0)
            {
                customer = new Customer();
                customer.Email = greenlotsInfo.User.information[0].email;
                db.Customers.AddObject(customer);
                db.SaveChanges();
            }
            else
            {
                customer = db.Customers.Where(s => s.Email == customerInfo.email).FirstOrDefault();
            }

            if (customer != null)
            {
                customer.Picture = customerInfo.image;
                customer.Name = customerInfo.firstname + " " + customerInfo.lastname;
                customer.Address = customerInfo.address + " " + customerInfo.address1 + " " + customerInfo.address2;
                customer.Language = customerInfo.language;
                customer.SMSAlert =customerInfo.sms_alert=="1";
                customer.EmailAlert =customerInfo.email_alert=="1";
                customer.Phone = customerInfo.contact;
                customer.Balance = customerInfo.credit_balance;
            }


            var carInfo = greenlotsInfo.Vehicle.information[0];
            var history = greenlotsInfo.History.session[0];
            if (db.Cars.Where(s => s.RFID == carInfo.rfid).Count() <= 0)
            {
                car = new Car();
                car.RFID = carInfo.rfid;
                if (customer != null)
                    car.CustomerID = customer.CustomerID;
                db.Cars.AddObject(car);
                db.SaveChanges();
            }
            else
            {
                car = db.Cars.Where(s => s.RFID == carInfo.rfid).FirstOrDefault();
            }

            if (car != null)
            {
                car.License = carInfo.license_plate;
                car.Make = carInfo.make;
                car.Model = carInfo.model;
                try
                {
                    car.TotalUsage = Convert.ToDouble(carInfo.total_usage);
                    car.BatteryCycle = Convert.ToDouble(carInfo.battery_cycle);
                }
                catch(Exception ex)
                {

                }
                //Fix up date
                var datetxt = history.end;
                try
                {
                    datetxt = datetxt.Replace("Today", DateTime.Now.Date.ToShortDateString());
                    datetxt = datetxt.Replace("Yesterday", DateTime.Now.Date.ToShortDateString());
                    car.LastRechargeDate = DateTime.Parse(datetxt);
                }
                catch (Exception ex)
                {
                    try
                    {
                        DateTime.ParseExact(datetxt, "dd/MM/yyyy HH:mm", null);
                    }
                    catch(Exception ex1)
                    {

                        Logger.WriteLine(path, "\nInvalid Date Format Detected");
                    }

                }


            }



            Logger.WriteLine(path, "\n--------------- Data Received  ---------------");
            Logger.WriteLine(path, (string) greenlotsInfo.User.information[0].address);
            //   Logger.WriteLine(path, greenlotsInfo.email);
            Logger.WriteLine(path, "\n--------------- Data Received End---------------");

            db.SaveChanges();

            if (start)
            {
                if (station != null && car!=null)
                    Service.MakeStationUnAvailable(station.StationID, car.RFID);
            }
            else
            {
                if (station != null)
                    Service.MakeStationAvailable(station.StationID);
            }
        }
        catch (Exception)
        {

            throw;
        }
    }

    public ActionResult GreenLotsLog()
    {
        string path =  this.HttpContext.Server.MapPath("~/Logs/GreenLots/" + "log.txt");
        using (
            FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)
        )
        {
            return Content(new StreamReader(stream).ReadToEnd().Replace("\n", "<br/>"));
        }
    }

    public ActionResult DialogBoxTest()
    {
        return View();
    }

    public ActionResult ServerLog()
    {
        string path = this.HttpContext.Server.MapPath("~/Logs/" + "serverlog.txt");
        using (
            FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)
        )
        {
            return Content(new StreamReader(stream).ReadToEnd().Replace("\n", "<br/>"));
        }
    }
    public ActionResult Index()
    {
        Instance = this;
        ViewBag.Message = "Welcome to GreenCore!";
        Response.AddHeader("Refresh", "10");
        try
        {

            ViewBag.NumberOfRegisteredCustomers = Service.GetAllCustomers().Count();
            ViewBag.NumberOfOnlineStations = Service.GetOnlineStations().Count();
            Service.Close();
        }
        catch (Exception exception)
        {
            ViewBag.Status = ("Opps " + exception.GetType() + "\n" + exception.Message + "\n" + exception.StackTrace + "\n" + ((exception.InnerException!=null)?exception.InnerException.Message:"").Replace("\n", "</br>"));
            Service.Abort();
        }

        return View();
    }

    private  ServiceClient _service;
    private ServiceClient Service
    {
        get
        {

            if (_service == null)
            {
                _service = new ServiceClient();
                //   if (HttpContext.Request.UserHostAddress != "1.9.13.61")
                //{
                //    _service.ClientCredentials.Windows.ClientCredential.UserName = "admin";
                //    _service.ClientCredentials.Windows.ClientCredential.Password = "Green2o11";
                //}
            }
            return _service;
        }
    }
    public ActionResult About()
    {
        Instance = this;
        return View();
    }



    public ActionResult Init(string id, string xmlUrl)
    {
        HttpContextBase httpContext = this.HttpContext;
        if (httpContext != null)
        {
            string path = httpContext.Server.MapPath("~/Logs/GreenLots/" + "log.txt");
            Logger.WriteLine(path,DateTime.Now +  " Init ID: " + id + " xmlUrl: " + xmlUrl);
            DownloadInfo(httpContext, xmlUrl,id);
            Logger.WriteLine(path, DateTime.Now + " Init ID: " + id + " xmlUrl: " + xmlUrl + "Updated Database");
        }

        return Content("1"); //Success

    }

    public ActionResult Start(string id, string xmlUrl)
    {
        HttpContextBase httpContext = this.HttpContext;
        if (httpContext != null)
        {
            string path = httpContext.Server.MapPath("~/Logs/GreenLots/" + "log.txt");
            Logger.WriteLine(path, DateTime.Now + "Start  ID: " + id + " xmlUrl: " + xmlUrl);
            DownloadInfo(httpContext, xmlUrl,id);
            Logger.WriteLine(path, DateTime.Now + " Start ID: " + id + " xmlUrl: " + xmlUrl + "Updated Database");
        }

        return Content("1"); //Success

    }

    public ActionResult Stop(string id, string xmlUrl)
    {

        HttpContextBase httpContext = this.HttpContext;
        if (httpContext != null)
        {

            string path = httpContext.Server.MapPath("~/Logs/GreenLots/" + "log.txt");
            Logger.WriteLine(path, DateTime.Now + " Stop   ID: " + id + " xmlUrl: " + xmlUrl);
            DownloadInfo(httpContext, xmlUrl,id,false);
            Logger.WriteLine(path, DateTime.Now + " Stop ID: " + id + " xmlUrl: " + xmlUrl + "Updated Database");
        }
        return Content("1"); //Success
        //return Content("Stop  ID: " + id + " xmlUrl: " + xmlUrl);
    }

}
}
