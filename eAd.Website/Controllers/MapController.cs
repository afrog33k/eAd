using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using eAd.Website.Repositories;

namespace eAd.Website.Controllers
{

    

    public class MapController : Controller
    {
        //
        // GET: /Map/

        public ActionResult Index(int stationID)
        {
            var mapRepository = new MapRepository();

            var map = mapRepository.GetById(stationID);
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
