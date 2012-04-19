using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using eAd.DataAccess;

namespace eAd.Website.Controllers
{ 
    public class Default1Controller : Controller
    {
        private eAdDataContainer db = new eAdDataContainer();

        //
        // GET: /Default1/

        public ViewResult Index()
        {
            return View(db.Mosaics.ToList());
        }

        //
        // GET: /Default1/Details/5

        public ViewResult Details(long id)
        {
            Mosaic mosaic = db.Mosaics.Single(m => m.MosaicID == id);
            return View(mosaic);
        }

        //
        // GET: /Default1/Create

        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /Default1/Create

        [HttpPost]
        public ActionResult Create(Mosaic mosaic)
        {
            if (ModelState.IsValid)
            {
                db.Mosaics.AddObject(mosaic);
                db.SaveChanges();
                return RedirectToAction("Index");  
            }

            return View(mosaic);
        }
        
        //
        // GET: /Default1/Edit/5
 
        public ActionResult Edit(long id)
        {
            Mosaic mosaic = db.Mosaics.Single(m => m.MosaicID == id);
            return View(mosaic);
        }

        //
        // POST: /Default1/Edit/5

        [HttpPost]
        public ActionResult Edit(Mosaic mosaic)
        {
            if (ModelState.IsValid)
            {
                db.Mosaics.Attach(mosaic);
                db.ObjectStateManager.ChangeObjectState(mosaic, EntityState.Modified);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(mosaic);
        }

       

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}