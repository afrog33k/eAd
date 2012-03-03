using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
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

        public ActionResult Preview(int id)
        {
            ViewBag.Media = db.Media.ToList();
           
            var mosaic = db.Mosaics.Where(m => m.MosaicID == id).FirstOrDefault();

           ViewBag.Positions= mosaic.Positions.Select(d => d.CreateModel()).ToList();

            return View(mosaic);
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

        public ActionResult SavePosition(long id, string name, float x = 0, float y = 0, float width = 0, float height = 0, List<string> items=null)
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
               var list = position.Media.ToList();

               foreach (var medium in list)
               {
                   position.Media.Remove(medium);
               }
               db.SaveChanges();

               foreach (var item in items)
               {
                    if(!String.IsNullOrEmpty(item))
                   if(position.Media.Where(i=>i.Name==item).Count()<=0)
                   {
                       var nname = Path.GetFileNameWithoutExtension(item).Replace("Thumb","");
                          //item.Remove("Uploads/Temp/Media/Thumb".Length);
                      // name = 
                       position.Media.Add(db.Media.Where(m => m.Location.Contains(nname)).FirstOrDefault());
                   }
               }

               db.SaveChanges();
               return Json("Sucessfully Saved Position", JsonRequestBehavior.AllowGet);
          

           }
           else
           {
               return Json("Invalid Mosiac, Please Choose One Before Saving",JsonRequestBehavior.AllowGet);
           }
        }
    }
}
