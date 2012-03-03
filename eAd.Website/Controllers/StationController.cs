using System.Linq;
using System.Web.Mvc;
using System.Web.Services.Description;
using eAd.DataViewModels;
using eAd.Website.eAdDataService;
//using MessageViewModel = eAd.Website.eAdDataService;


namespace eAd.Website.Controllers
{
    public class StationController : Controller
    {
        private ServiceClient _service;
        private ServiceClient Service
        {
            get
            {

                if (_service == null)
                {
                    _service = new ServiceClient();
                    if (HttpContext.Request.UserHostAddress != "1.9.13.61")
                    {
                        _service.ClientCredentials.Windows.ClientCredential.UserName = "admin";
                        _service.ClientCredentials.Windows.ClientCredential.Password = "Green2o11";
                    }
                }
                return _service;
            }
        }
        
        // GET: /Station/

        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /Station/

        public ActionResult List()
        {
            Response.AddHeader("Refresh", "10");
            ViewBag.StationsCount = Service.GetAllStations().Count();
            ViewBag.StationsOnlineCount = Service.GetOnlineStations().Count();
            ViewBag.StationSummary = Service.GetAllStations();
            MessageViewModel firstOrDefault = Service.GetAllMyMessages(1).OrderByDescending(m => m.ID).FirstOrDefault();
            if (firstOrDefault != null)
            {
                ViewBag.LastMessage = firstOrDefault.Text + "(1/" + Service.GetAllMyMessages(1).Count() + ")";
            }
            return View();
        }
        //
        // GET: /Station/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /Station/Create

        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /Station/Create

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
        
        //
        // GET: /Station/Edit/5
 
        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /Station/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here
 
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Station/Delete/5
 
        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /Station/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here
 
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        public ActionResult Engage(long stationID, string rfidCode)
        {
           //var station = Service.GetAllStations().Where(s=>s.StationID ==stationID).FirstOrDefault();
           // if(station!=null)
           // {
            //  station.Available = false;
            Service.MakeStationUnAvailable(stationID, rfidCode);
          //  }
                return RedirectToAction("List");
        }

        public ActionResult DisEngage(long stationID)
        {
            Service.MakeStationAvailable(stationID);
            return RedirectToAction("List");
        }

        public ActionResult Screenshot(long stationID)
        {
            Service.CaptureScreenShot(stationID);
            return RedirectToAction("ShowScreenShot");
        }

        public ActionResult ShowScreenShot(long stationID)
        {
            return View();
        }
    }
}
