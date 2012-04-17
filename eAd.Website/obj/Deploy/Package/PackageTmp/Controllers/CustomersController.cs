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
    public class CustomersController : Controller
    {
        private eAdDataContainer db = new eAdDataContainer();

        //
        // GET: /Customers/

        public ViewResult Index()
        {
            return View(db.Customers.ToList());
        }

        //
        // GET: /Customers/Details/5

        public ViewResult Details(long id)
        {
            Customer customer = db.Customers.Single(c => c.CustomerID == id);
            return View(customer);
        }

        //
        // GET: /Customers/Create

        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /Customers/Create

        [HttpPost]
        public ActionResult Create(Customer customer)
        {
            if (ModelState.IsValid)
            {
                db.Customers.AddObject(customer);
                db.SaveChanges();
                return RedirectToAction("Index");  
            }

            return View(customer);
        }
        
        //
        // GET: /Customers/Edit/5
 
        public ActionResult Edit(long id)
        {
            Customer customer = db.Customers.Single(c => c.CustomerID == id);
            return View(customer);
        }

        //
        // POST: /Customers/Edit/5

        [HttpPost]
        public ActionResult Edit(Customer customer)
        {
            if (ModelState.IsValid)
            {
                db.Customers.Attach(customer);
                db.ObjectStateManager.ChangeObjectState(customer, EntityState.Modified);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(customer);
        }

        //
        // GET: /Customers/Delete/5
 
        public ActionResult Delete(long id)
        {
            Customer customer = db.Customers.Single(c => c.CustomerID == id);
            return View(customer);
        }

        //
        // POST: /Customers/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(long id)
        {            
            Customer customer = db.Customers.Single(c => c.CustomerID == id);
            db.Customers.DeleteObject(customer);
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