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

        public ActionResult LoadMosaic(long id)
        {
            if (db.Mosaics.Where(m => m.MosaicID == id).Count() > 0)
            {
                var mosaic = db.Mosaics.Where(m => m.MosaicID == id).FirstOrDefault();
            
                return Json(mosaic.Positions.Select(d=>d.CreateModel()).ToArray(), JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        public ActionResult SavePosition(long id, string name, float x = 0, float y = 0, float width = 0, float height = 0)
        {
            if(db.Mosaics.Where(m=>m.MosaicID==id).Count()>0)
           {
               var mosaic = db.Mosaics.Where(m => m.MosaicID == id).FirstOrDefault();
               Position position = null;
               if(mosaic.Positions.Where(p=>p.Name==name).Count()>0)
               {
                   position = mosaic.Positions.Where(p => p.Name == name).FirstOrDefault();
               }
               else
               {
                   position = new Position();
                   position.MosaicID = id;
                   db.Positions.AddObject(position);
                   db.SaveChanges();
               }

               if (position != null)
               {
                   position.Name = name;
                   position.X = x;
                   position.Y = y;
                   position.Width = width;
                   position.Height = height;
               }

               db.SaveChanges();
               return Json("Sucessfully Saved Position", JsonRequestBehavior.AllowGet);
          

           }
           else
           {
               return Json("Invalid Mosiac, Please Choose One Before Saving",JsonRequestBehavior.AllowGet);
           }
            return null;
        }
    }
}
