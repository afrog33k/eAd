using System;
using System.Collections.Generic;
using System.Data.Objects.DataClasses;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using eAd.DataAccess;
using eAd.Utilities;
using irio.utilities;

namespace eAd.Website.Controllers
{
    public class MobileClientController : Controller
    {

        private readonly eAdDataContainer _db = new eAdDataContainer();

        //
        // GET: /MobileClient/

        public ActionResult GetAds()
        {
            var mobileMosaic = _db.Mosaics.Where(m=>m.Type=="Mobile");
            var adsNotFull = mobileMosaic.SelectMany(m=>m.Positions).Where(p => p.Height < 100);
            var adsFull = mobileMosaic.SelectMany(m=>m.Positions).Where(p => p.Height > 100);
          
            var Adverts = new List<MobileAdvert>();

            //Create files + hash
            //foreach (var position in adsFull)
            {
             //   var media = position.Media; // until fix these are hard_coded
                var media = _db.Media.Where(m => m.Name.Contains("mobile_fs_"));
                int height = 480;
                CreateMobileAds(Adverts, height, media, 0);
            }

            //Create files + hash
       //     foreach (var position in adsNotFull)
            {
       //         var media = position.Media;
                var media = _db.Media.Where(m => m.Name.Contains("mobile_bnr_"));

                int height = 50;
                CreateMobileAds(Adverts, height, media, 120000);
            }

         
      
            return Json(Adverts,JsonRequestBehavior.AllowGet);
        }

        private void CreateMobileAds(List<MobileAdvert> adverts, int height, IEnumerable<Medium> media, int prefix)
        {
            string hash;
            foreach (var medium in media)
            {
                var loc = Server.MapPath("~/"+medium.Location);
                var newName = Path.GetDirectoryName(loc) +"/"+ Path.GetFileNameWithoutExtension(loc) + height +
                              Path.GetExtension(loc);
                var hashName = Path.GetDirectoryName(loc) + "/" + Path.GetFileNameWithoutExtension(loc) + height +
                               ".hash";
                var clientName = (Request.Url.GetLeftPart(System.UriPartial.Authority) +
                                  Path.GetDirectoryName(medium.Location) + "/" +
                                  Path.GetFileNameWithoutExtension(loc) + height +
                                  Path.GetExtension(loc)).Replace("////","//").Replace("\\","/");
                    ;
                try
                {

               
                if (System.IO.File.Exists(loc))
                {
                    if (!System.IO.File.Exists(newName))
                    {
                        //Create Image
                        Bitmap originalFile = new Bitmap(loc);
                        Bitmap smallImage = new Bitmap(ImageUtilities.Resize(originalFile, 320, height,RotateFlipType.RotateNoneFlipNone));
                        smallImage.Save(newName, ImageFormat.Jpeg);
                    }
                    if (!System.IO.File.Exists(hashName))
                    {
                        //Calculate Hash
                        hash = Hashes.MD5(newName);
                        System.IO.File.WriteAllText(hashName, hash);
                    }
                    else
                    {
                        hash = System.IO.File.ReadAllText(hashName);
                    }

                    adverts.Add(new MobileAdvert()
                                         {
                                             AdvertisementId = (int) medium.MediaID + prefix,
                                             ContentUrl = medium.Url.Contains("http://")?medium.Url:"http://" +medium.Url,
                                             MediaUrl = clientName,
                                             MediaType = medium.Type.ToLower().Contains("image") ? "image" : "video",
                                             Type = height == 480 ? "fullscreen" : "banner",
                                             Hash = hash,
                                         });
                }
                }
                catch (Exception)
                {

                   
                }
            }
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
