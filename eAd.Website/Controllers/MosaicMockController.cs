//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Data.Entity;
//using System.Linq;
//using System.Web;
//using System.Web.Mvc;
//using eAd.DataAccess;

//namespace eAd.Website.Controllers
//{
//    public class MosaicMockController : Controller
//    {
//        private eAdEntities db = new eAdEntities();

//        //
//        // GET: /MosaicMock/

//        public ViewResult Index()
//        {
//            return View(db.Mosaics.ToList());
//        }

//        //
//        // GET: /MosaicMock/Details/5

//        public ViewResult Details(long id)
//        {
//            Mosaic mosaic = db.Mosaics.Single(m => m.MosaicID == id);
//            return View(mosaic);
//        }

//        //
//        // GET: /MosaicMock/Create

//        public ActionResult Create()
//        {
//            return View();
//        }

//        //
//        // POST: /MosaicMock/Create

//        [HttpPost]
//        public ActionResult Create(Mosaic mosaic)
//        {
//            if (ModelState.IsValid)
//            {
//                db.Mosaics.AddObject(mosaic);
//                db.SaveChanges();
//                return RedirectToAction("Index");
//            }

//            return View(mosaic);
//        }

//        //
//        // GET: /MosaicMock/Edit/5

//        public ActionResult Edit(long id)
//        {
//            Mosaic mosaic = db.Mosaics.Single(m => m.MosaicID == id);
//            return View(mosaic);
//        }

//        //
//        // POST: /MosaicMock/Edit/5

//        [HttpPost]
//        public ActionResult Edit(Mosaic mosaic)
//        {
//            if (ModelState.IsValid)
//            {
//                db.Mosaics.Attach(mosaic);
//                db.ObjectStateManager.ChangeObjectState(mosaic, EntityState.Modified);
//                db.SaveChanges();
//                return RedirectToAction("Index");
//            }
//            return View(mosaic);
//        }

//        //
//        // GET: /MosaicMock/Delete/5

//        public ActionResult Delete(long id)
//        {
//            Mosaic mosaic = db.Mosaics.Single(m => m.MosaicID == id);
//            return View(mosaic);
//        }

//        //
//        // POST: /MosaicMock/Delete/5

//        [HttpPost, ActionName("Delete")]
//        public ActionResult DeleteConfirmed(long id)
//        {
//            Mosaic mosaic = db.Mosaics.Single(m => m.MosaicID == id);
//            db.Mosaics.DeleteObject(mosaic);
//            db.SaveChanges();
//            return RedirectToAction("Index");
//        }

//        protected override void Dispose(bool disposing)
//        {
//            db.Dispose();
//            base.Dispose(disposing);
//        }
//    }
//}