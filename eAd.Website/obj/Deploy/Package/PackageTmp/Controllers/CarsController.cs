using System.Data;
using System.Linq;
using System.Web.Mvc;
using eAd.DataAccess;

namespace eAd.Website.Controllers
{ 
    public class CarsController : Controller
    {
        private eAdDataContainer db = new eAdDataContainer();

        //
        // GET: /Car/

        public ViewResult Index()
        {
            var cars = db.Cars.Include("Customer");
            return View(cars.ToList());
        }

        //
        // GET: /Car/Details/5

        public ViewResult Details(long id)
        {
            Car car = db.Cars.Single(c => c.CarID == id);
            return View(car);
        }

        //
        // GET: /Car/Create

        public ActionResult Create()
        {
            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "Name");
            return View();
        } 

        //
        // POST: /Car/Create

        [HttpPost]
        public ActionResult Create(Car car)
        {
            if (ModelState.IsValid)
            {
                db.Cars.AddObject(car);
                db.SaveChanges();
                return RedirectToAction("Index");  
            }

            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "Name", car.CustomerID);
            return View(car);
        }
        
        //
        // GET: /Car/Edit/5
 
        public ActionResult Edit(long id)
        {
            Car car = db.Cars.Single(c => c.CarID == id);
            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "Name", car.CustomerID);
            return View(car);
        }

        //
        // POST: /Car/Edit/5

        [HttpPost]
        public ActionResult Edit(Car car)
        {
            if (ModelState.IsValid)
            {
                db.Cars.Attach(car);
                db.ObjectStateManager.ChangeObjectState(car, EntityState.Modified);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "Name", car.CustomerID);
            return View(car);
        }

        //
        // GET: /Car/Delete/5
 
        public ActionResult Delete(long id)
        {
            Car car = db.Cars.Single(c => c.CarID == id);
            return View(car);
        }

        //
        // POST: /Car/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(long id)
        {            
            Car car = db.Cars.Single(c => c.CarID == id);
            db.Cars.DeleteObject(car);
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