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
using irio.utilities;

namespace eAd.Website.Controllers
{ 
    public class StationsController : Controller
    {
        private eAdDataContainer db = new eAdDataContainer();

        //
        // GET: /Stations/

        public ViewResult Index()
        {
           try
           {
               return View(db.Stations.ToList());
           }
           catch (Exception exception)
            {
                ViewBag.Status = ("Opps " + exception.GetType() + "\n" + exception.Message + "\n" + exception.StackTrace + "\n" + ((exception.InnerException != null) ? exception.InnerException.Message : "").Replace("\n", "</br>"));
  
            }
           return View();
        }

        //
        // GET: /Stations/Details/5

        public ViewResult Details(long id)
        {
            Station station = db.Stations.Single(s => s.StationID == id);
            return View(station);
        }

        //
        // GET: /Stations/Create

        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /Stations/Create

        [HttpPost]
        public ActionResult Create(Station station)
        {
            if (ModelState.IsValid)
            {
                db.Stations.AddObject(station);
                db.SaveChanges();
                return RedirectToAction("Index");  
            }

            return View(station);
        }

        // 
        //
        // GET: /Stations/UploadScreenshot/5
         public ActionResult UploadScreenshot(long id)
         {
             ViewBag.StationID = id;
             return View();
         }
        // 
        //
        // GET: /Stations/UploadScreenshot/5
        [HttpPost]
         public ActionResult UploadScreenshot(long stationID,HttpPostedFileBase file)
        {
            if (file.ContentLength > 0)
            {
           //     var fileName = Path.GetFileName(file.FileName);
                var path = Path.Combine(Server.MapPath("~/ScreenShots/"+stationID+"/"), "shot.jpg");
                if (FileUtilities.FileExists(path))
                    new FileInfo(path).Delete();
                FileUtilities.SaveStream(file.InputStream, path, false);
            
            }

         //   UploadedContent upload;

            
            //var location = UploadRepository.GetFileUrl(this.HttpContext, medium.MediaID.ToString(),
            //                                           medium.MediaID.ToString(), UploadType.Media, out upload);

            //if (upload != null)
            //{
            //    if (medium.Duration == TimeSpan.Zero)
            //    {
            //        medium.Duration = new TimeSpan(upload.Duration.Ticks);
            //    }
            //    else
            //    {

            //    }
            //}

            //if (location != null)
            //    medium.Location = location;
            return RedirectToAction("Index");
        }

        // 
        //
        // GET: /Stations/Edit/5
 
        public ActionResult Edit(long id)
        {
            Station station = db.Stations.Single(s => s.StationID == id);
         
            
            return View(station);
        }

        //
        // POST: /Stations/Edit/5

        [HttpPost]
        public ActionResult Edit(Station station)
        {
            if (ModelState.IsValid)
            {
                db.Stations.Attach(station);
                db.ObjectStateManager.ChangeObjectState(station, EntityState.Modified);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(station);
        }

        //
        // GET: /Stations/Delete/5
 
        public ActionResult Delete(long id)
        {
            Station station = db.Stations.Single(s => s.StationID == id);
            return View(station);
        }

        //
        // POST: /Stations/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(long id)
        {            
            Station station = db.Stations.Single(s => s.StationID == id);
            db.Stations.DeleteObject(station);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}