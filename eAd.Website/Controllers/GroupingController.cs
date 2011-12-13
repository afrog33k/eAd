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
    public class GroupingController : Controller
    {
        private eAdEntities db = new eAdEntities();

        //
        // GET: /Grouping/

        public ViewResult Index()
        {
            return View(db.Groupings.ToList());
        }

        //
        // GET: /Grouping/Details/5

        public ViewResult Details(long id)
        {
            Grouping grouping = db.Groupings.Single(g => g.GroupingID == id);
            return View(grouping);
        }

        //
        // GET: /Grouping/Create

        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /Grouping/Create

        [HttpPost]
        public ActionResult Create(Grouping grouping)
        {
            if (ModelState.IsValid)
            {
                db.Groupings.AddObject(grouping);
                db.SaveChanges();
                return RedirectToAction("Index");  
            }

            return View(grouping);
        }
        
        //
        // GET: /Grouping/Edit/5
 
        public ActionResult Edit(long id)
        {
            Grouping grouping = db.Groupings.Single(g => g.GroupingID == id);
            return View(grouping);
        }

        //
        // POST: /Grouping/Edit/5

        [HttpPost]
        public ActionResult Edit(Grouping grouping)
        {
            if (ModelState.IsValid)
            {
                db.Groupings.Attach(grouping);
                db.ObjectStateManager.ChangeObjectState(grouping, EntityState.Modified);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(grouping);
        }

        //
        // GET: /Grouping/Delete/5
 
        public ActionResult Delete(long id)
        {
            Grouping grouping = db.Groupings.Single(g => g.GroupingID == id);
            return View(grouping);
        }

        //
        // POST: /Grouping/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(long id)
        {            
            Grouping grouping = db.Groupings.Single(g => g.GroupingID == id);
            db.Groupings.DeleteObject(grouping);
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