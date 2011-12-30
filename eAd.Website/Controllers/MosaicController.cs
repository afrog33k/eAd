using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using eAd.DataAccess;
using eAd.Website.Repositories;
namespace eAd.Website.Controllers
{
    public class MosaicController : Controller
    {
        private eAdDataContainer db = new eAdDataContainer();
        //
        // GET: /Mosaic/

        public ActionResult Index()
        {
            ViewBag.Media = db.Media.ToList();
            ViewBag.MosaicID = new SelectList(db.Mosaics, "MosaicID", "Name");
    
            return View();
        }
        
        [HttpPost]
        public ActionResult Create(string name)
        {

            Mosaic mosaic = new Mosaic();
            mosaic.Name = name;
            
                db.Mosaics.AddObject(mosaic);
                db.SaveChanges();
              
            return Json(new { message = "Mosaic Created" }, JsonRequestBehavior.AllowGet);
       
        }



    }
}
