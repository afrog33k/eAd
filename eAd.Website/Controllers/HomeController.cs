using System;
using System.Web.Mvc;
using eAd.Website.eAdDataService;
using System.Linq;
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
    }
}
