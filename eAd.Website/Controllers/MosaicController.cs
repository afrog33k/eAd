using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using eAd.DataAccess;
using eAd.DataViewModels;
using eAd.Utilities;
using eAd.Website.Repositories;
using eAd.Website.eAdDataService;

namespace eAd.Website.Controllers
{
    public class MosaicController : Controller
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
        // GET: /Mosaic/

        public ActionResult Index()
        {
            ViewBag.Media = db.Media.ToList();
            ViewBag.MosaicID = new SelectList(db.Mosaics, "MosaicID", "Name");
    
            return View();
        }

        public ActionResult GetMosaicList()
        {
          return   Json(new SelectList(db.Mosaics, "MosaicID", "Name"),JsonRequestBehavior.AllowGet);
        }

        public ActionResult EditPartial(int id)
        {
            var mosaic = db.Mosaics.Where(m => m.MosaicID == id).FirstOrDefault();

            return View(mosaic);
        }

        [HttpPost]
        public ActionResult EditPartial(Mosaic mosaic)
        {
            if (ModelState.IsValid)
            {    
           
                mosaic.Updated = DateTime.Now;
                db.Mosaics.Attach(mosaic);
                db.ObjectStateManager.ChangeObjectState(mosaic, EntityState.Modified);
                db.SaveChanges();
                return Json("Successfully Saved Mosaic");
            }
            return Json("Please Check your inputs and save again");
        }

        public ActionResult Preview(int id)
        {
          
            ViewBag.Media = db.Media.ToList();
           
            var mosaic = db.Mosaics.Where(m => m.MosaicID == id).FirstOrDefault();

            if (mosaic != null)
            {
                ViewBag.Positions= mosaic.Positions.Select(d => d.CreateModel()).ToList();

                return View(mosaic);
            }
            return Content("Invalid Mosaic Chosen - Hit Back And Choose Again");
        }

        [HttpPost]
        public ActionResult Create(string name)
        {

            Mosaic mosaic = new Mosaic();
            mosaic.Name = name;
            mosaic.Created = DateTime.Now;
                db.Mosaics.AddObject(mosaic);
                db.SaveChanges();
              
            return Json(new { message = "Mosaic Created" }, JsonRequestBehavior.AllowGet);
       
        }

        public ActionResult LoadMosaic(long id)
        {
            if (db.Mosaics.Where(m => m.MosaicID == id).Count() > 0)
            {
                var mosaic = db.Mosaics.Where(m => m.MosaicID == id).FirstOrDefault();

                if (mosaic != null)
                    return Json(mosaic.Positions.Select(d=>d.CreateModel()).ToArray(), JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        public ActionResult MosaicUpdated(long id)
        {
            var mosaic = db.Mosaics.Single(g => g.MosaicID == id);
            var groupings = mosaic.Groupings;
            if (groupings != null)
            {

                var message = new MessageViewModel();
                message.Command = "Updated Mosaic";
                message.Type = "Media";
                //   message.UserID = 
                message.Text = mosaic.Name;

                foreach (var grouping in groupings)
                {
                    Service.SendMessageToGroup(grouping.GroupingID, message);
                }



                return Json(new { message = "Mosaic Updated" }, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        public ActionResult SavePosition(long id, string name, float x = 0, float y = 0, float width = 0, float height = 0, List<string> items=null)
        {
            if(db.Mosaics.Where(m=>m.MosaicID==id).Count()>0)
           {
               var mosaic = db.Mosaics.Where(m => m.MosaicID == id).FirstOrDefault();
               mosaic.Updated = DateTime.Now;
               Position position = null;
               if(mosaic.Positions.Where(p=>p.Name==name).Count()>0)
               {
                   position = mosaic.Positions.Where(p => p.Name == name).FirstOrDefault();
               }
               else
               {
                   position = new Position();
                   position.MosaicID = id;
                   db.Positions.AddObject(position);
                   db.SaveChanges();
               }

               if (position != null)
               {
                   position.Name = name;
                   position.X = x;
                   position.Y = y;
                   position.Width = width;
                   position.Height = height;
               }
               var list = position.Media.ToList();

               foreach (var medium in list)
               {
                   position.Media.Remove(medium);

               }
               db.SaveChanges();

               foreach (var item in items)
               {
                    if(!String.IsNullOrEmpty(item))
                   if(position.Media.Where(i=>i.Name==item).Count()<=0)
                   {
                       var nname = Path.GetFileNameWithoutExtension(item).Replace("Thumb","");
                          //item.Remove("Uploads/Temp/Media/Thumb".Length);
                      // name = 
                       position.Media.Add(db.Media.Where(m => m.Location.Contains(nname)).FirstOrDefault());
                   }
               }

               db.SaveChanges();
               return Json("Sucessfully Saved Position", JsonRequestBehavior.AllowGet);
          

           }
           else
           {
               return Json("Invalid Mosiac, Please Choose One Before Saving",JsonRequestBehavior.AllowGet);
           }
        }

        public ActionResult SaveMosaic(PositionViewModel[] positionList )
        {
            try
            {

          
            if(positionList!=null)
            foreach (var positionItem in positionList)
            {
                var mosaic = db.Mosaics.Where(m => m.MosaicID == positionItem.MosaicID).FirstOrDefault();
                if (mosaic!=null)
                {
                    mosaic.Updated = DateTime.Now;
                    Position position = mosaic.Positions.Where(p => p.Name == positionItem.Name).FirstOrDefault();
                    if (position==null)
                    {
                   
                        position = new Position();
                        position.MosaicID = positionItem.MosaicID;
                        db.Positions.AddObject(position);
                        db.SaveChanges();
                    }

                    position.Name = positionItem.Name;
                    position.X = (double) positionItem.X;
                    position.Y = (double) positionItem.Y;
                    position.Width = (double) positionItem.Width;
                    position.Height = (double) positionItem.Height;
                    var list = position.Media.ToList();

                    for (int i = 0; i < list.Count;i++ )
                    {
                        var medium =list[i];
                       position.Media.Remove(medium);
                        list[i] = null;
                    }
                    db.SaveChanges();

                    if (positionItem.MediaUri != null)
                        foreach (var path in positionItem.MediaUri)
                        {
                            if (!String.IsNullOrEmpty(path))
                                if (position.Media.Where(i => i.Name == path).Count() <= 0)
                                {
                                    var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(path);
                                    if (fileNameWithoutExtension != null)
                                    {
                                        var nname = fileNameWithoutExtension.Replace("Thumb", "");
                                        //item.Remove("Uploads/Temp/Media/Thumb".Length);
                                        // name = 
                                        position.Media.Add(db.Media.Where(m => m.Location.Contains(nname)).FirstOrDefault());
                                    }
                                }
                        }
                }
                else
                {
                    return Json("Invalid Mosiac, Please Choose One Before Saving", JsonRequestBehavior.AllowGet);
                }
            }
            if (positionList != null && positionList.Count()>0)
            {
                  db.SaveChanges();
            return Json("Sucessfully Saved Mosaic", JsonRequestBehavior.AllowGet);
            }
            }
            catch (Exception)
            {

                
            }
            return Json("Invalid Mosiac, Please Choose One Before Saving", JsonRequestBehavior.AllowGet);
         
                     
        }
    }
}
