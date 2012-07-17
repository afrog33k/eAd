using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Serialization;
using eAd.DataAccess;
using eAd.DataViewModels;
using eAd.Utilities;
using eAd.Website.Repositories;
using eAd.Website.eAdDataService;

namespace eAd.Website.Controllers
{
    public class MosaicController : Controller
    {
        private eAdEntities db = new eAdEntities();
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

        public ActionResult Index()
        {
            return RedirectToAction("IndexGeneral");
        }

        //
        // GET: /Mosaic/

        public ActionResult IndexGeneral()
        {
            ViewBag.Media = db.Media.ToList();
            ViewBag.MosaicID = new SelectList(db.Mosaics.Where(m => m.Type == "General"), "MosaicID", "Name");

            return View();
        }

        public ActionResult IndexProfile()
        {
            ViewBag.Media = db.Media.ToList();
            ViewBag.MosaicID = new SelectList(db.Mosaics.Where(m => m.Type == "General"), "MosaicID", "Name");

            return View();
        }

        public ActionResult IndexMobile()
        {
            ViewBag.Media = db.Media.ToList();
            ViewBag.MosaicID = new SelectList(db.Mosaics.Where(m => m.Type == "General"), "MosaicID", "Name");

            return View();
        }


        public ActionResult GetMosaicList()
        {
            return Json(new SelectList(db.Mosaics.Where(m => m.Type == "General"), "MosaicID", "Name"), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetMosaicListProfile()
        {
            return Json(new SelectList(db.Mosaics.Where(m => m.Type == "Profile"), "MosaicID", "Name"), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetMosaicListMobile()
        {
            return Json(new SelectList(db.Mosaics.Where(m => m.Type == "Mobile"), "MosaicID", "Name"), JsonRequestBehavior.AllowGet);
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
                if (!String.IsNullOrEmpty(mosaic.Background))
                {
                    var name = Path.GetFileName(mosaic.Background).Replace("/eAd.Website", "").Replace("Thumb", "");
                    var media = db.Media.Where(m => m.Location.Contains(name)).Single();
                    mosaic.Background = media.Location;

                }
                mosaic.Updated = DateTime.Now;

                var mosaicToSave = db.Mosaics.Where(m => m.MosaicID == mosaic.MosaicID).SingleOrDefault();
                if (mosaicToSave != null)
                {
                    mosaicToSave.Name = mosaic.Name;
                    mosaicToSave.Background = mosaic.Background;
                    mosaicToSave.Updated = mosaic.Updated;
                    mosaicToSave.Width = mosaic.Width;
                    mosaicToSave.Height = mosaic.Height;
                }


                //   db.Mosaics.Attach(mosaic);
             //   db.ObjectStateManager.ChangeObjectState(mosaic, EntityState.Modified);
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
                ViewBag.Positions = mosaic.Positions.Select(d => d.CreateModel()).ToList();

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
            mosaic.Updated = DateTime.Now;

            mosaic.Width = 768;
            mosaic.Height = 1366;
            mosaic.Type = "General";
            db.Mosaics.AddObject(mosaic);
            db.SaveChanges();

            return Json(new { message = "Mosaic Created" }, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public ActionResult CreateMobile(string name)
        {

            Mosaic mosaic = new Mosaic();
            mosaic.Name = name;
            mosaic.Created = DateTime.Now;
            mosaic.Updated = DateTime.Now;

            mosaic.Width = 240;
            mosaic.Height = 320;
            mosaic.Type = "Mobile";
            db.Mosaics.AddObject(mosaic);
            db.SaveChanges();

            return Json(new { message = "Mosaic Created" }, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public ActionResult CreateProfile(string name)
        {

            Mosaic mosaic = new Mosaic();
            mosaic.Name = name;
            mosaic.Created = DateTime.Now;
            mosaic.Updated = DateTime.Now;

            mosaic.Width = 768;
            mosaic.Height = 1366;
            mosaic.Type = "Profile";
            db.Mosaics.AddObject(mosaic);
            db.SaveChanges();

            return Json(new { message = "Mosaic Created" }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult DeletePosition(long mosaic, string position)
        {
            try
            {

                Position pos = db.Positions.Single(m => m.Name == position && m.MosaicID == mosaic);

                var mediaList = new List<PositionMedium>();

                foreach (var posmedium in pos.PositionMediums)
                {
                    mediaList.Add(posmedium);
                }

                foreach (var posmedium in mediaList)
                {
                //    pos.PositionMediums.Remove(posmedium);
                    db.PositionMediums.DeleteObject(posmedium);
                }
                
                db.Positions.DeleteObject(pos);
                db.SaveChanges();
                return Json("Sucessfully Deleted Position", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
              //  return Json("Sucessfully Deleted Position", JsonRequestBehavior.AllowGet);
                return Json("Failed To Delete Position" + ex.Message, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult LoadMosaic(long id)
        {
            if (db.Mosaics.Where(m => m.MosaicID == id).Count() > 0)
            {
                var mosaic = db.Mosaics.Where(m => m.MosaicID == id).FirstOrDefault();

                var extraItems = new List<PositionViewModel>();

                if (mosaic.Type == "Profile")
                {
                    if (mosaic.ExtraData == null)
                    {
                        extraItems.Add(new PositionViewModel()
                                           {
                                               Name = "PersonalInfo",
                                               Height = 100,
                                               Width = 100,
                                               X = 0,
                                               Y = 0,
                                               Media = null,
                                               ContentURL = "../Content/Mosaics/profile-photo.jpg"
                                           });
                        extraItems.Add(new PositionViewModel()
                                           {
                                               Name = "LocationInfo",
                                               Height = 100,
                                               Width = 100,
                                               X = 100,
                                               Y = 0,
                                               Media = null,
                                               ContentURL = "../Content/Mosaics/dana-location-map.png"
                                           });
                        extraItems.Add(new PositionViewModel()
                                           {
                                               Name = "BatteryInfo",
                                               Height = 100,
                                               Width = 100,
                                               X = 0,
                                               Y = 100,
                                               Media = null,
                                               ContentURL = "../Content/Mosaics/battery-capacity.jpg"
                                           });
                        extraItems.Add(new PositionViewModel()
                                           {
                                               Name = "CarInfo",
                                               Height = 100,
                                               Width = 100,
                                               X = 100,
                                               Y = 100,
                                               Media = null,
                                               ContentURL = "../Content/Mosaics/proton-saga-1-big.jpg"
                                           });
                    }
                    else
                    {
                        //Load Data
                        XmlSerializer serializer = new XmlSerializer(typeof (List<PositionViewModel>));
                        extraItems =
                            serializer.Deserialize(new StringReader(mosaic.ExtraData)) as List<PositionViewModel>;
                        if (extraItems.Where(e => e.Name == "PersonalInfo").Count() <= 0)
                        {
                            extraItems.Add(new PositionViewModel()
                                               {
                                                   Name = "PersonalInfo",
                                                   Height = 100,
                                                   Width = 100,
                                                   X = 0,
                                                   Y = 0,
                                                   Media = null,
                                                   ContentURL = "../Content/Mosaics/profile-photo.jpg"
                                               });
                        }
                        else
                        {
                            extraItems.Where(e => e.Name == "PersonalInfo").FirstOrDefault().ContentURL =
                                "../Content/Mosaics/profile-photo.jpg";
                        }
                        if (extraItems.Where(e => e.Name == "LocationInfo").Count() <= 0)
                        {
                            extraItems.Add(new PositionViewModel()
                                               {
                                                   Name = "LocationInfo",
                                                   Height = 100,
                                                   Width = 100,
                                                   X = 100,
                                                   Y = 0,
                                                   Media = null,
                                                   ContentURL = "../Content/Mosaics/dana-location-map.png"
                                               });
                        }
                        else
                        {
                            extraItems.Where(e => e.Name == "LocationInfo").FirstOrDefault().ContentURL =
                                "../Content/Mosaics/dana-location-map.png";
                        }
                        if (extraItems.Where(e => e.Name == "BatteryInfo").Count() <= 0)
                        {
                            extraItems.Add(new PositionViewModel()
                                               {
                                                   Name = "BatteryInfo",
                                                   Height = 100,
                                                   Width = 100,
                                                   X = 0,
                                                   Y = 100,
                                                   Media = null,
                                                   ContentURL = "../Content/Mosaics/battery-capacity.jpg"
                                               });
                        }
                        else
                        {
                            extraItems.Where(e => e.Name == "BatteryInfo").FirstOrDefault().ContentURL =
                                "../Content/Mosaics/battery-capacity.jpg";
                        }
                        if (extraItems.Where(e => e.Name == "CarInfo").Count() <= 0)
                        {
                            extraItems.Add(new PositionViewModel()
                                               {
                                                   Name = "CarInfo",
                                                   Height = 100,
                                                   Width = 100,
                                                   X = 100,
                                                   Y = 100,
                                                   Media = null,
                                                   ContentURL = "../Content/Mosaics/proton-saga-1-big.jpg"
                                               });
                        }
                        else
                        {
                            extraItems.Where(e => e.Name == "CarInfo").FirstOrDefault().ContentURL =
                                "../Content/Mosaics/proton-saga-1-big.jpg";
                        }
                    }
                }

                var posns = (mosaic.Positions.Select(d => d.CreateModel()).Union(extraItems)).ToArray();

                if (mosaic != null)
                    return Json(new
                {
                    background =
                    !String.IsNullOrEmpty(mosaic.Background) ? mosaic.Background.Replace("\\", "/") : "",
                    positions =posns
                }, JsonRequestBehavior.AllowGet);
            }
            return null;
        }




        //
        // GET: /Default1/Delete/5

        public ActionResult Delete(long id)
        {
            try
            {
                Mosaic mosaic = db.Mosaics.Single(m => m.MosaicID == id);
                return View(mosaic);
            }
            catch (Exception ex) //Doesnt Exist
            {
                return Json("Mosaic Doesn't Exist", JsonRequestBehavior.AllowGet);

            }

        }

        //
        // POST: /Default1/Delete/5

        //     [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(long id)
        {
            try
            {

                Mosaic mosaic = db.Mosaics.Single(m => m.MosaicID == id);

                var positions = new List<Position>();
                foreach (var position in db.Positions.Where(p => p.MosaicID == id))
                {
                    positions.Add(position);
                }

                foreach (var position in positions)
                {
                    mosaic.Positions.Remove(position);
                }



                db.Mosaics.DeleteObject(mosaic);
                db.SaveChanges();
                return Json("Sucessfully Deleted Mosaic", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                return Json("Failed To Delete Mosaic", JsonRequestBehavior.AllowGet);
            }
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

        public ActionResult SavePosition(long id, string name, float x = 0, float y = 0, float width = 0, float height = 0, List<string> items = null)
        {
            if (db.Mosaics.Where(m => m.MosaicID == id).Count() > 0)
            {
                var mosaic = db.Mosaics.Where(m => m.MosaicID == id).FirstOrDefault();
                mosaic.Updated = DateTime.Now;
                Position position = null;
                if (mosaic.Positions.Where(p => p.Name == name).Count() > 0)
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
                var list = position.PositionMediums.ToList();

                foreach (var medium in list)
                {
                    position.PositionMediums.Remove(medium);

                }
                db.SaveChanges();

                foreach (var item in items)
                {
                    if (!String.IsNullOrEmpty(item))
                        if (position.Media.Where(i => i.Name == item).Count() <= 0)
                        {
                            var nname = Path.GetFileNameWithoutExtension(item).Replace("Thumb", "");
                            //item.Remove("Uploads/Temp/Media/Thumb".Length);
                            // name =
                            position.PositionMediums.Add(new PositionMedium()
                                                             {
                                                                 Medium = db.Media.Where(m => m.Location.Contains(nname)).FirstOrDefault()
                                                                 ,Position = position
                                                             });
                                
                        }
                }

                db.SaveChanges();
                return Json("Sucessfully Saved Position", JsonRequestBehavior.AllowGet);


            }
            else
            {
                return Json("Invalid Mosiac, Please Choose One Before Saving", JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult SaveMosaic(PositionViewModel[] positionList)
        {
            if (positionList != null)
            {
                try
                {

                    var MosaicID = positionList[0].MosaicID;
                    var extra = positionList.Where(p => p.Name == "PersonalInfo" || p.Name == "CarInfo"
                                                        || p.Name == "LocationInfo" || p.Name == "BatteryInfo"
                        );

                    
                    var posn = positionList.Except(extra);
                    var mosaic = db.Mosaics.Where(m => m.MosaicID == MosaicID).FirstOrDefault();
                   
                    if (mosaic != null)
                    {
                        mosaic.Updated = DateTime.Now;
                        if (positionList != null)
                            foreach (var positionItem in posn)
                            {
                                
                                Position position =
                                    mosaic.Positions.Where(p => p.Name == positionItem.Name).FirstOrDefault();
                                if (position == null)
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
                                var list = position.PositionMediums.ToList();

                                for (int i = 0; i < list.Count; i++)
                                {
                                    var medium = list[i];
                                     db.PositionMediums.DeleteObject(medium);
                                    list[i] = null;
                                }
                                db.SaveChanges();
                                var order = 0;
                                if (positionItem.MediaUri != null)
                                    foreach (var path in positionItem.MediaUri)
                                    {
                                        if (!String.IsNullOrEmpty(path))
                                       //     if (!position.Media.Any(i => Path.GetFileNameWithoutExtension(i.Name).ToLower().Replace("thumb", "") ==  Path.GetFileNameWithoutExtension(path).ToLower().Replace("thumb", "")))  // Duplicates now allowed
                                            {
                                          // if(order%2==0)  //Deal with duplication bug from js ... Fixed already
                                            {
                                                
                                            
                                                var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(path);
                                                if (fileNameWithoutExtension != null)
                                                {
                                                    var nname = fileNameWithoutExtension.ToLower().Replace("thumb", "");

                                                    var newpath = "";
                                                    if(nname.Contains("-crop-"))
                                                    {
                                                        nname = nname.Substring(0,nname.IndexOf("-crop-"));
                                                        var serverpath = HttpContext.Request.Url.GetLeftPart(UriPartial.Authority) +
                   HttpRuntime.AppDomainAppVirtualPath;
                                                        newpath = path.Replace(serverpath, "/");
                                                    }
                                                    //item.Remove("Uploads/Temp/Media/Thumb".Length);
                                                    // name =
                                                    position.PositionMediums.Add(new PositionMedium()
                                                                                     {
                                                                                         Medium =
                                                                                             db.Media.Where(
                                                                                                 m =>
                                                                                                 m.Location.Contains(
                                                                                                     nname)).
                                                                                             FirstOrDefault(),
                                                                                             Options = nname,
                                                                                         Position = position,
                                                                                         Location =!(String.IsNullOrEmpty(newpath)) ?newpath:"",
                                                                                         PlayOrder = (short?) order
                                                                                     });
                                                }
                                                order++;
                                                }
                                            }
                                    }
                            }

                        //Deal With Extras
                        XmlSerializer serializer = new XmlSerializer(typeof(List<PositionViewModel>));
                        var writer = new StringWriter();
                        serializer.Serialize(writer,extra.ToList());
                        mosaic.ExtraData = writer.ToString();
                    }
                    else
                    {
                        return Json("Invalid Mosiac, Please Choose One Before Saving", JsonRequestBehavior.AllowGet);
                    }
                    if (positionList != null && positionList.Count() > 0)
                    {
                        db.SaveChanges();
                        return Json("Sucessfully Saved Mosaic", JsonRequestBehavior.AllowGet);
                    }
                }
                catch (Exception ex)
                {
                    return Json("Error Occured During Save " + ex.Message + " " + ex.StackTrace +(ex.InnerException!=null? ex.InnerException.Message + ex.InnerException.StackTrace:""), JsonRequestBehavior.AllowGet);

                }
               
            }
            return Json("Invalid Mosiac, Please Choose One Before Saving", JsonRequestBehavior.AllowGet);
        }
    }
}
