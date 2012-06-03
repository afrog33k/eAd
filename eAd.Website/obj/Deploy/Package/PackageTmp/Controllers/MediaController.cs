using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ImageUploadAndCrop.Models;
using eAd.DataAccess;
using eAd.DataViewModels;
using eAd.Utilities;
using eAd.Website.Extensions;
using eAd.Website.Repositories;

namespace eAd.Website.Controllers
{
public class MediaController : Controller
{
    private readonly eAdDataContainer _db = new eAdDataContainer();

    //
    // GET: /Default1/

    public ViewResult Index()
    {
        return View(_db.Media.ToList());
    }

    //
    // GET: /Default1/Details/5

    public ViewResult Details(long id)
    {
        Medium medium = _db.Media.Single(m => m.MediaID == id);
        return View(medium);
    }

    //
    // GET: /Default1/Create

    public ActionResult Create()
    {
        UploadRepository.CreateUploadGUID(HttpContext);
        Medium medium = new Medium();
        medium.Duration = TimeSpan.Zero;

        return View(medium);
    }


    //public ActionResult Picker(string name, string type)
    //{
    //   var media =  _db.Media
    //    return View();
    //}

    public ActionResult Picker(string componentId    )
    {

        ViewBag.Types = new SelectList(_db.Media.Select(m=>m.Type).Distinct().ToList(), "", "");
        ViewBag.Themes = new SelectList(_db.Themes.Select(m => m.Name).Distinct().ToList(), "", "");
        ViewBag.ComponentId = componentId;

        var media = _db.Media.ToList();


        return View(media.Select(m=> m.CreateModel()));
    }

    public ActionResult PickerList(string name, string type, string theme="")
    {
        var types = _db.Media.Distinct().ToList();
        ViewBag.Types = new SelectList(types.Select(t=>t.Type), "", "");
        var media = _db.Media.ToList();

        if (!String.IsNullOrEmpty(name))
        {
            media = new List<Medium>(media.Where(n => n.Name.ToLower().Contains(name.ToLower())));
        }

        if (!String.IsNullOrEmpty(type))
        {
            media = new List<Medium>(media.Where(n => n.Type.ToLower().Contains(type.ToLower())));
        }

        if (!String.IsNullOrEmpty(theme))
        {
            media = new List<Medium>(media.Where(n => 
                n.Themes.Where(t=>t.Name.ToLower().Contains(type.ToLower())).Count()>0)
                );
        }
        var mediaList = media.Select(m => m.CreateModel());

        return View(mediaList);
    }

    //
    // POST: /Default1/Create

    [HttpPost]
    public ActionResult Create(Medium medium)
    {
        if (ModelState.IsValid)
        {
            medium.Created = DateTime.Now;
            _db.Media.AddObject(medium);
            _db.SaveChanges();

            UploadedContent upload;



            var   location = UploadRepository.GetFileUrl(this.HttpContext, medium.MediaID.ToString(),
                             medium.MediaID.ToString(), UploadType.Media, out upload);


            if (upload != null)
            {
                if (medium.Duration == TimeSpan.Zero)
                {
                    medium.Duration = new TimeSpan(upload.Duration.Ticks);
                }
                else
                {

                }
            }

            if (location != null && location[0] != null)
            {
                medium.Location = location[0];
                using(var fs = new FileStream(Server.MapPath("~/"+location[0]),FileMode.Open,FileAccess.Read,FileShare.ReadWrite))
                {
                    medium.Hash = Hashes.MD5(fs);
                }
                if(location.Length>1)
                {
                    medium.ThumbnailUrl = location[1];
                }

            }


            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        return View(medium);
    }

    //
    // GET: /Default1/Edit/5

    public ActionResult Edit(long id)
    {
        UploadRepository.CreateUploadGUID(HttpContext);
        Medium medium = _db.Media.Single(m => m.MediaID == id);
        if(medium.Duration==null)
            medium.Duration = TimeSpan.Zero;
        medium.Updated = DateTime.Now;
        bool shouldCalculatenewHash =false;
        if (medium.Hash == null || medium.Size == 0)
        {
            shouldCalculatenewHash = true;
        }

        // Calculate new hash/size
        if (shouldCalculatenewHash)
        {

            using (var fs = new FileStream(Server.MapPath("~/" + medium.Location), FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                medium.Hash = Hashes.MD5(fs);
                medium.Size = new FileInfo(Server.MapPath("~/" + medium.Location)).Length;
            }
        }

        return View(medium);
    }

    //
    // POST: /Default1/Edit/5

    [HttpPost]
    public ActionResult Edit(Medium medium)
    {
        if (ModelState.IsValid)
        {
            var dbmedium = _db.Media.Where(m => m.MediaID == medium.MediaID).SingleOrDefault();
            dbmedium.Name = medium.Name;
            dbmedium.Url = medium.Url;
            dbmedium.Duration = medium.Duration;
            dbmedium.Tags = medium.Tags;
          UploadedContent upload;
          var location = UploadRepository.GetFileUrl(this.HttpContext, dbmedium.MediaID.ToString(),
                           dbmedium.MediaID.ToString(), UploadType.Media, out upload);
            if (upload!=null)
            {
                dbmedium.Duration = new TimeSpan(upload.Duration.Ticks);
            }

            var shouldCalculatenewHash = false;
            if (location != null )
            {
                if (location[0] != dbmedium.Location)
                {
                    dbmedium.Location = location[0];
                    shouldCalculatenewHash = true;
                }
                if (location[1] != dbmedium.ThumbnailUrl)
                {
                    dbmedium.ThumbnailUrl = location[1];
                  
                }
            }
            if(dbmedium.Hash==null||dbmedium.Size==0)
            {
                shouldCalculatenewHash = true;
            }

            // Calculate new hash/size
            if(shouldCalculatenewHash)
            {

                using (var fs = new FileStream(Server.MapPath("~/" + dbmedium.Location), FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    dbmedium.Hash = Hashes.MD5(fs);
                    dbmedium.Size = new FileInfo(Server.MapPath("~/" + dbmedium.Location)).Length;
                }
            }

            foreach (var mosaic in dbmedium.Positions.Select(p=>p.Mosaic))
            {
                mosaic.Updated = DateTime.Now;
            }

            dbmedium.Updated = DateTime.Now;
            _db.SaveChanges();
            return RedirectToAction("Index");
        }
        return View("Edit",  medium );
    }

    //
    // GET: /Default1/Delete/5

    public ActionResult Delete(long id)
    {
        Medium medium = _db.Media.Single(m => m.MediaID == id);
        return View(medium);
    }

    //
    // POST: /Default1/Delete/5

    [HttpPost, ActionName("Delete")]
    public ActionResult DeleteConfirmed(long id)
    {
        Medium medium = _db.Media.Single(m => m.MediaID == id);

        //Remove Media From Mosaic Positions
        var positions = medium.Positions.ToList();

        foreach (var position in positions)
        {
            var mediaPositions =  position.Media.Where(m => m.MediaID == medium.MediaID).ToList();

            foreach (var mediaPosition in mediaPositions)
            {
                position.Media.Remove(mediaPosition);
            }

        }

        //Remove Media From Themes
        var themes = medium.Themes.ToList();

        foreach (var theme in themes)
        {
            var mediaPositions = theme.Media.Where(m => m.MediaID == medium.MediaID).ToList();

            foreach (var mediaPosition in mediaPositions)
            {
                theme.Media.Remove(mediaPosition);
            }

        }
        //Delete Physical Medium
        try
        {
            new FileInfo(HttpContext.Server.MapPath("~" + medium.Location)).Delete();
        }
        catch (Exception ex)
        {

            Console.WriteLine(ex.StackTrace+"\n"+ex.Message);
        }


        //Should Update all related stations that media is gone

        _db.Media.DeleteObject(medium);
        _db.SaveChanges();
        return RedirectToAction("Index");
    }

    protected override void Dispose(bool disposing)
    {
        _db.Dispose();
        base.Dispose(disposing);
    }


      //
    // GET: /Default1/Delete/5

    public ActionResult ResizeAndSave()
    {
        return View();
    }


        #region static variables

        private static string ImageCacheKey = "ImageKey_";
        private static int W_FixedSize = 200;
        private static int H_FixedSize = 200;
        
        #endregion

        #region local properties

        private string WorkingImageCacheKey
        {
            get { return string.Format("{0}{1}", ImageCacheKey, WorkingImageId); }
        }
        private string ModifiedImageCacheKey
        {
            get { return string.Format("{0}{1}", ImageCacheKey, ModifiedImageId); }
        }
        
        #endregion

        #region session properties

        private Guid WorkingImageId
        {
            get
            {
                if (Session["WorkingImageId"] != null)
                    return (Guid)Session["WorkingImageId"];
                else
                    return new Guid();
            }
            set { Session["WorkingImageId"] = value; }
        }

        private Guid ModifiedImageId
        {
            get
            {
                if (Session["ModifiedImageId"] != null)
                    return (Guid)Session["ModifiedImageId"];
                else
                    return new Guid();
            }
            set { Session["ModifiedImageId"] = value; }
        }

        private string WorkingImageExtension
        {
            get
            {
                if (Session["WorkingImageExtension"] != null)
                    return Session["WorkingImageExtension"].ToString();
                else
                    return string.Empty;
            }
            set { Session["WorkingImageExtension"] = value; }
        }
        
        #endregion

        #region cached properties

        private byte[] WorkingImage
        {
            get
            {
                byte[] img = null;

                if (HttpContext.Cache[WorkingImageCacheKey] != null)
                    img = (byte[])HttpContext.Cache[WorkingImageCacheKey];

                return img;
            }
            set
            {
                HttpContext.Cache.Add(WorkingImageCacheKey,
                  value,
                  null,
                  System.Web.Caching.Cache.NoAbsoluteExpiration,
                  new TimeSpan(0, 40, 0),
                  System.Web.Caching.CacheItemPriority.Low,
                  null);
            }
        }

        private byte[] ModifiedImage
        {
            get
            {
                byte[] img = null;
                if (HttpContext.Cache[ModifiedImageCacheKey] != null)
                    img = (byte[])HttpContext.Cache[ModifiedImageCacheKey];
                return img;
            }
            set
            {
                HttpContext.Cache.Add(ModifiedImageCacheKey,
                    value,
                    null,
                    System.Web.Caching.Cache.NoAbsoluteExpiration,
                    new TimeSpan(0, 40, 0),
                    System.Web.Caching.CacheItemPriority.Low,
                    null);
            }
        }

        #endregion

        private enum ImageModificationType
        {
            Crop,
            Resize
        };        



        /// <summary>
        /// Files the upload.
        /// </summary>
        /// <param name="uploadedFileMeta">The uploaded file meta.</param>
        /// <returns></returns>
        [Authorize]
        public ActionResult FileUpload(MediaAssetUploadModel uploadedFileMeta)
        {
            Guid newImageId = new Guid();
            try
            {
                newImageId = ProcessUploadedImage(uploadedFileMeta);
            }
            catch (Exception ex)
            {
                string errorMsg = string.Format("Error processing image: {0}", ex.Message);
                Response.StatusCode = 500;
                Response.Write(errorMsg);
                return Json(string.Empty);
            }

            return Json(new { Id = newImageId, Status = "OK" });
        }

        /// <summary>
        /// Processes the uploaded image.
        /// </summary>
        /// <param name="uploadedFileMeta">The uploaded file meta.</param>
        /// <returns>Image Id</returns>
        private Guid ProcessUploadedImage(MediaAssetUploadModel uploadedFileMeta)
        {
            // Get the file extension
            WorkingImageExtension = Path.GetExtension(uploadedFileMeta.Filename).ToLower();
            string[] allowedExtensions = { ".png", ".jpeg", ".jpg", ".gif" }; // Make sure it is an image that can be processed
            if (allowedExtensions.Contains(WorkingImageExtension))
            {
                WorkingImageId = Guid.NewGuid();
                Image workingImage = new Bitmap(uploadedFileMeta.fileData.InputStream);
                WorkingImage = ImageHelper.ImageToByteArray(workingImage);
            }
            else
            {
                throw new Exception("Cannot process files of this type.");
            }

            return WorkingImageId;
        }

        /// <summary>
        /// Crops the image.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="w">The w.</param>
        /// <param name="h">The h.</param>
        /// <returns>Image Id</returns>
        public JsonResult CropImage(int x, int y, int w, int h)
        {
            try
            {
                if (w == 0 && h == 0) // Make sure the user selected a crop area
                    throw new Exception("A crop selection was not made.");

                string imageId = ModifyImage(x, y, w, h, ImageModificationType.Crop);
                return Json(imageId);
            }
            catch (Exception ex)
            {
                string errorMsg = string.Format("Error cropping image: {0}", ex.Message);
                Response.StatusCode = 500;
                Response.Write(errorMsg);
                return Json(string.Empty);
            }
        }

        /// <summary>
        /// Resizes the image.
        /// </summary>
        /// <returns>Image Id</returns>
        public JsonResult ResizeImage()
        {
            try
            {
                string imageId = ModifyImage(0, 0, W_FixedSize, H_FixedSize, ImageModificationType.Resize);
                return Json(imageId);
            }
            catch (Exception ex)
            {
                string errorMsg = string.Format("Error resizing image: {0}", ex.Message);
                Response.StatusCode = 500;
                Response.Write(errorMsg);
                return Json(string.Empty);
            }
        }

        /// <summary>
        /// Modifies an image image.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="w">The w.</param>
        /// <param name="h">The h.</param>
        /// <param name="modType">Type of the mod. Crop or Resize</param>
        /// <returns>New Image Id</returns>
        private string ModifyImage(int x, int y, int w, int h, ImageModificationType modType)
        {
            ModifiedImageId = Guid.NewGuid();
            Image img = ImageHelper.ByteArrayToImage(WorkingImage);

            using (System.Drawing.Bitmap _bitmap = new System.Drawing.Bitmap(w, h))
            {
                _bitmap.SetResolution(img.HorizontalResolution, img.VerticalResolution);
                using (Graphics _graphic = Graphics.FromImage(_bitmap))
                {
                    _graphic.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    _graphic.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                    _graphic.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                    _graphic.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;

                    if (modType == ImageModificationType.Crop)
                    {
                        _graphic.DrawImage(img, 0, 0, w, h);
                        _graphic.DrawImage(img, new Rectangle(0, 0, w, h), x, y, w, h, GraphicsUnit.Pixel);
                    }
                    else if (modType == ImageModificationType.Resize)
                    {
                        _graphic.DrawImage(img, 0, 0, img.Width, img.Height);
                        _graphic.DrawImage(img, new Rectangle(0, 0, W_FixedSize, H_FixedSize), 0, 0, img.Width, img.Height, GraphicsUnit.Pixel);
                    }

                    string extension = WorkingImageExtension;

                    // If the image is a gif file, change it into png
                    if (extension.EndsWith("gif", StringComparison.OrdinalIgnoreCase))
                    {
                        extension = ".png";
                    }

                    using (EncoderParameters encoderParameters = new EncoderParameters(1))
                    {
                        encoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, 100L);
                        ModifiedImage = ImageHelper.ImageToByteArray(_bitmap, extension, encoderParameters);
                    }
                }
            }

            return ModifiedImageId.ToString();
        }
    }

}