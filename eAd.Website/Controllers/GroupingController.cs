using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using eAd.DataAccess;
using eAd.DataViewModels;
using eAd.Website.eAdDataService;
//using MessageViewModel = eAd.Website.eAdDataService.MessageViewModel;

namespace eAd.Website.Controllers
{ 
    public class GroupingController : Controller
    {
        private eAdDataContainer db = new eAdDataContainer();

        private ServiceClient _service;
        private ServiceClient Service
        {
            get
            {

                if (_service == null)
                {
                    _service = new ServiceClient();
                    //   if (HttpContext.Request.UserHostAddress != "1.9.13.61")
                    //{
                    //    _service.ClientCredentials.Windows.ClientCredential.UserName = "admin";
                    //    _service.ClientCredentials.Windows.ClientCredential.Password = "Green2o11";
                    //}
                }
                return _service;
            }
        }

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
                grouping.MosaicID = 1;
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
            var mystations = grouping.Stations.ToList();
            var otherStations = db.Stations.ToList().Except(mystations);

            var mythemes = grouping.Themes.ToList();
            var otherThemes = db.Themes.ToList().Except(mythemes);
            ViewBag.NonAddedStations = new SelectList(otherStations, "StationID", "Name");
            ViewBag.AddedStations = new SelectList(mystations, "StationID", "Name");

            ViewBag.NonAddedThemes = new SelectList(otherThemes, "ThemeID", "Name");
            ViewBag.AddedThemes = new SelectList(mythemes, "ThemeID", "Name");


            ViewBag.MosaicID = new SelectList(db.Mosaics, "MosaicID", "Name", grouping.MosaicID);

            return View(grouping);
        }


       public JsonResult AddStation(long stationID, long groupID)
       {
           Grouping grouping = db.Groupings.Single(g => g.GroupingID == groupID);
           var station = db.Stations.Where(s => s.StationID == stationID).FirstOrDefault();
           if (station != null)
           {
               grouping.Stations.Add(station);
               db.SaveChanges();

               var message = new MessageViewModel();
               message.Command = "Joined Group";
               message.Type = "Group";
               //   message.UserID = 
               message.Text = groupID.ToString();
               Service.SendMessageToStation(stationID, message);

              return Json(new {message = "Station Added"},JsonRequestBehavior.AllowGet);
           }
           else
           {
               return Json(new { message=  "Station Does Not Exist"},JsonRequestBehavior.AllowGet);
           }

       }
       public JsonResult RemoveStation(long stationID, long groupID)
       {
           Grouping grouping = db.Groupings.Single(g => g.GroupingID == groupID);
           var station = grouping.Stations.Where(s => s.StationID == stationID).FirstOrDefault();
           if (station != null)
           {
               grouping.Stations.Remove(station);
               db.SaveChanges();

               var message = new MessageViewModel();
               message.Command = "Left Group";
               message.Type = "Group";
               //   message.UserID = 
               message.Text = groupID.ToString();
               Service.SendMessageToStation(stationID, message);
               return Json(new { message = "Station Removed" }, JsonRequestBehavior.AllowGet);
           }
           else
           {
               return Json(new { message = "Station Does Not Exist" }, JsonRequestBehavior.AllowGet);
           }

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

        
                var message = new MessageViewModel();
                message.Command = "Updated Mosaic";
                message.Type = "Media";
        
                message.Text = grouping.Name;
                Service.SendMessageToGroup(grouping.GroupingID, message);
             
                return RedirectToAction("Index");
            }
            ViewBag.MosaicID = new SelectList(db.Mosaics, "MosaicID", "Name", grouping.MosaicID);
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

        public ActionResult AddTheme(long stationID, long groupID)
        {
            Grouping grouping = db.Groupings.Single(g => g.GroupingID == groupID);
            var theme = db.Themes.Where(s => s.ThemeID == stationID).FirstOrDefault();
            if (theme != null)
            {
                grouping.Themes.Add(theme);
                db.SaveChanges();
                var message = new MessageViewModel();
                message.Command = "Added Theme";
                message.Type = "Media";
             //   message.UserID = 
                message.Text = theme.ThemeID.ToString();
                Service.SendMessageToGroup(groupID, message);
                return Json(new { message = "Station Added" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { message = "Station Does Not Exist" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult RemoveTheme(long stationID, long groupID)
        {
            Grouping grouping = db.Groupings.Single(g => g.GroupingID == groupID);
            var theme = grouping.Themes.Where(s => s.ThemeID == stationID).FirstOrDefault();
            if (theme != null)
            {
                grouping.Themes.Remove(theme);
                db.SaveChanges();
                var message = new MessageViewModel();
                message.Command = "Removed Theme";
                message.Type = "Media";
                //   message.UserID = 
                message.Text = theme.ThemeID.ToString();
                Service.SendMessageToGroup(groupID, message);
                return Json(new { message = "Station Removed" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { message = "Station Does Not Exist" }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}