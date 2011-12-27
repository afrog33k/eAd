using System;
using System.Web.Mvc;
using eAd.Website.eAdDataService;
using System.Linq;
using System.IO;
namespace eAd.Website.Controllers
{
    public class HomeController : Controller
    {
        public static Controller Instance = null;
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


            string path = @"log.txt";
            // This text is added only once to the file.
            if (!System.IO.File.Exists(path))
            {
                // Create a file to write to.
                using (StreamWriter log = System.IO.File.CreateText(path))
                {
                    log.WriteLine("Init  ID: " + id + " xmlUrl: " + xmlUrl);
                }
            }

            // This text is always added, making the file longer over time
            // if it is not deleted.
            using (StreamWriter log = System.IO.File.AppendText(path))
            {
                log.WriteLine("Init  ID: " + id + " xmlUrl: " + xmlUrl);
            }	

            return Content("1"); //Success
            //return Content("Init ID: " + id + " xmlUrl: " + xmlUrl);
        }

        public ActionResult Start(string id, string xmlUrl)
        {
            
            string path = @"log.txt";
            // This text is added only once to the file.
            if (!System.IO.File.Exists(path))
            {
                // Create a file to write to.
                using (StreamWriter log = System.IO.File.CreateText(path))
                {
                    log.WriteLine("Start  ID: " + id + " xmlUrl: " + xmlUrl);
                }
            }

            // This text is always added, making the file longer over time
            // if it is not deleted.
            using (StreamWriter log = System.IO.File.AppendText(path))
            {
                log.WriteLine("Start  ID: " + id + " xmlUrl: " + xmlUrl);
            }	

            return Content("1"); //Success
     
           /// return Content("Start  ID: " + id + " xmlUrl: " + xmlUrl);
        }

        public ActionResult Stop(string id, string xmlUrl)
        {
       
            string path = @"log.txt";
            // This text is added only once to the file.
            if (!System.IO.File.Exists(path))
            {
                // Create a file to write to.
                using (StreamWriter log = System.IO.File.CreateText(path))
                {
                    log.WriteLine("Stop  ID: " + id + " xmlUrl: " + xmlUrl);
                }
            }

            // This text is always added, making the file longer over time
            // if it is not deleted.
            using (StreamWriter log = System.IO.File.AppendText(path))
            {
                log.WriteLine("Stop  ID: " + id + " xmlUrl: " + xmlUrl);
            }	

            return Content("1"); //Success
            //return Content("Stop  ID: " + id + " xmlUrl: " + xmlUrl);
        }

    }
}
