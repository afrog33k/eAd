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
    public class MediaController : Controller
    {
        private eAdEntities db = new eAdEntities();

        //
        // GET: /Default1/

        public ViewResult Index()
        {
            return View(db.Media.ToList());
        }

        //
        // GET: /Default1/Details/5

        public ViewResult Details(long id)
        {
            Medium medium = db.Media.Single(m => m.MediaID == id);
            return View(medium);
        }

        //
        // GET: /Default1/Create

        public ActionResult Create()
        {
            UploadRepository.CreateUploadGUID(HttpContext);
            return View();
        } 

        //
        // POST: /Default1/Create

        [HttpPost]
        public ActionResult Create(Medium medium)
        {
            if (ModelState.IsValid)
            {
                db.Media.AddObject(medium);
                db.SaveChanges();
                medium.Location = UploadRepository.GetFileUrl(this.HttpContext, medium.MediaID.ToString(),
                                                              medium.MediaID.ToString(), UploadType.Media);

                db.SaveChanges();
                return RedirectToAction("Index");  
            }

            return View(medium);
        }
        
        //
        // GET: /Default1/Edit/5
 
        public ActionResult Edit(long id)
        {
            UploadRepository.CreateUploadGUID(HttpContext);
            Medium medium = db.Media.Single(m => m.MediaID == id);
            return View(medium);
        }

        //
        // POST: /Default1/Edit/5

        [HttpPost]
        public ActionResult Edit(Medium medium)
        {
            if (ModelState.IsValid)
            {
                db.Media.Attach(medium);
                db.ObjectStateManager.ChangeObjectState(medium, EntityState.Modified);
                db.SaveChanges();
                medium.Location = UploadRepository.GetFileUrl(this.HttpContext, medium.MediaID.ToString(),
                                                                 medium.MediaID.ToString(), UploadType.Media);

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(medium);
        }

        //
        // GET: /Default1/Delete/5
 
        public ActionResult Delete(long id)
        {
            Medium medium = db.Media.Single(m => m.MediaID == id);
            return View(medium);
        }

        //
        // POST: /Default1/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(long id)
        {            
            Medium medium = db.Media.Single(m => m.MediaID == id);
            db.Media.DeleteObject(medium);
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