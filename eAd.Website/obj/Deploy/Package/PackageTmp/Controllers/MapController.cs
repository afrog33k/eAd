using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using eAd.Website.Repositories;
using eAd.Website.eAdDataService;

namespace eAd.Website.Controllers
{

    

    public class MapController : Controller
    {
        //
        // GET: /Map/

        public ActionResult Index(int stationID)
        {
            var mapRepository = new MapRepository();
            ServiceClient myService = new ServiceClient();
            myService.ClientCredentials.Windows.ClientCredential.UserName = "admin";
            myService.ClientCredentials.Windows.ClientCredential.Password = "Green2o11";
  
            var stations = myService.GetAllStations().Where(s=>s.StationID==stationID);
         //   var map = mapRepository.GetById(stationID);
            GoogleMaps.Locations = stations.ToArray();
           ViewBag.Map =  (GoogleMaps.Webpage);
            return View();
        }

        public ActionResult Map()
        {

            var mapRepository = new MapRepository();

            var map = mapRepository.GetById(1);

            return Json(map,JsonRequestBehavior.AllowGet);

        }

    }
}
