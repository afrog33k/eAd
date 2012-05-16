using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace eAd.Website.Controllers
{
    public class MobileJSON
    {
        public List<MobileAdvert> Adverts { get; set; }
    }

    public class MobileAdvert
    {
        
        public int AdvertisementId { get; set; }
        public string MediaUrl { get; set; }
        public string ContentUrl { get; set; }
        public string MediaType { get; set; }
        public string Type { get; set; }
        public string Hash { get; set; }
    }

    public class MobileClientController : Controller
    {

       

        //
        // GET: /MobileClient/

        public ActionResult GetAds()
        {
            MobileJSON test = new MobileJSON();
            test.Adverts = new List<MobileAdvert>();
            test.Adverts.Add(new MobileAdvert(){AdvertisementId = 1, ContentUrl = "www", MediaUrl = "ppp", Hash = "dasdasdas", MediaType = "image", Type = "fullscreen"});
           test.Adverts.Add(new MobileAdvert(){AdvertisementId = 1, ContentUrl = "www", MediaUrl = "ppp", Hash = "dasdasdas", MediaType = "image", Type = "fullscreen"});
           test.Adverts.Add(new MobileAdvert(){AdvertisementId = 1, ContentUrl = "www", MediaUrl = "ppp", Hash = "dasdasdas", MediaType = "image", Type = "fullscreen"});
           test.Adverts.Add(new MobileAdvert(){AdvertisementId = 1, ContentUrl = "www", MediaUrl = "ppp", Hash = "dasdasdas", MediaType = "image", Type = "fullscreen"});
           test.Adverts.Add(new MobileAdvert(){AdvertisementId = 1, ContentUrl = "www", MediaUrl = "ppp", Hash = "dasdasdas", MediaType = "image", Type = "fullscreen"});
     
            return Json(test,JsonRequestBehavior.AllowGet);
        }

        //
        // GET: /MobileClient/

        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /MobileClient/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /MobileClient/Create

        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /MobileClient/Create

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
        // GET: /MobileClient/Edit/5
 
        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /MobileClient/Edit/5

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
        // GET: /MobileClient/Delete/5
 
        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /MobileClient/Delete/5

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
    }
}
