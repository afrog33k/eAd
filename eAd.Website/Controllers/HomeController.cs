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
        private eAdDataContainer db = new eAdDataContainer();
        public void DownloadInfo(HttpContextBase httpContext,string xmlUrl, string stationID)
        {
            try
            {
               
                Station station;

                    if(db.Stations.Where(s=>s.UniqueID==stationID).Count() <=0)
                    {
                     station  = new Station();
                        station.UniqueID = stationID;
                        db.Stations.AddObject(station);
                        db.SaveChanges();
                    }
                    else
                    {
                        station = db.Stations.Where(s => s.UniqueID == stationID).FirstOrDefault();
                    }

                string path = httpContext.Server.MapPath("~/Logs/GreenLots/" + "log.txt");
            
            StreamReader reader = new StreamReader(WebRequest.Create(xmlUrl).GetResponse().GetResponseStream());
            XmlSerializer xSerializer = new XmlSerializer(typeof(proton));
            proton greenlotsInfo = (proton)xSerializer.Deserialize(reader);

                station.Name = greenlotsInfo.Location.information[0].name;
                station.Address = greenlotsInfo.Location.information[0].address + "\n" + greenlotsInfo.Location.information[0].address1 + "\n" + greenlotsInfo.Location.information[0].address2;
                station.PostalCode = greenlotsInfo.Location.information[0].postal;
                station.Rate = Convert.ToDouble( greenlotsInfo.Location.information[0].rate);
                Logger.WriteLine(path, "\n--------------- Data Received ---------------");
                Logger.WriteLine(path, (string) greenlotsInfo.User.information[0].address);
             //   Logger.WriteLine(path, greenlotsInfo.email);
                Logger.WriteLine(path, "\n--------------- Data Received End---------------");

            db.SaveChanges();
            }
            catch (Exception)
            {

                throw;
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
                Logger.WriteLine(path, "Init ID: " + id + " xmlUrl: " + xmlUrl);
                DownloadInfo(httpContext, xmlUrl,id);
            }

            return Content("1"); //Success
         
        }

        public ActionResult Start(string id, string xmlUrl)
        {
             HttpContextBase httpContext = this.HttpContext;
             if (httpContext != null)
             {
                 string path = httpContext.Server.MapPath("~/Logs/GreenLots/" + "log.txt");
                Logger.WriteLine(path, "Start  ID: " + id + " xmlUrl: " + xmlUrl);
                DownloadInfo(httpContext, xmlUrl,id);
                 }
            
             return Content("1"); //Success
     
        }

        public ActionResult Stop(string id, string xmlUrl)
        {
       
          HttpContextBase httpContext = this.HttpContext;
          if (httpContext != null)
          {

              string path = httpContext.Server.MapPath("~/Logs/GreenLots/" + "log.txt");
              Logger.WriteLine(path, "Stop   ID: " + id + " xmlUrl: " + xmlUrl);
              DownloadInfo(httpContext, xmlUrl,id);
          }
            return Content("1"); //Success
            //return Content("Stop  ID: " + id + " xmlUrl: " + xmlUrl);
        }

    }
}
