using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
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
        ViewBag.ComponentId = componentId;

        var media = _db.Media;


        return View(media.Select(m=>new MediaListModel()
        {
            Duration =  (TimeSpan) m.Duration,
            MediaID = m.MediaID,
            Type = m.Type,
            Selected = false,
            Name = m.Name,
            DisplayLocation = m.Location
        }));
    }

    public ActionResult PickerList(string name, string type)
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

        return View(media.Select(m => new MediaListModel()
        {
            Duration = ((TimeSpan)m.Duration),
            MediaID = m.MediaID,
            Type = m.Type,
            Selected = false,
            Name = m.Name,
            DisplayLocation = m.Location
        }));
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

            if (location != null)
            {
                medium.Location = location;
                using(var fs = new FileStream(Server.MapPath("~/"+location),FileMode.Open,FileAccess.Read,FileShare.ReadWrite))
                {
                    medium.Hash = Hashes.MD5(fs);
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
            _db.Media.Attach(medium);
            _db.ObjectStateManager.ChangeObjectState(medium, EntityState.Modified);
            _db.SaveChanges();

            UploadedContent upload;
            var location = UploadRepository.GetFileUrl(this.HttpContext, medium.MediaID.ToString(),
                           medium.MediaID.ToString(), UploadType.Media, out upload);
            if (upload!=null)
            {
                medium.Duration = new TimeSpan(upload.Duration.Ticks);
            }

            var shouldCalculatenewHash = false;
            if (location != null )
            {
                if(location!=medium.Location)
                {
                    medium.Location = location;
                    shouldCalculatenewHash = true;
                }

            }
            if(medium.Hash==null||medium.Size==0)
            {
                shouldCalculatenewHash = true;
            }

            // Calculate new hash/size
            if(shouldCalculatenewHash)
            {

                using (var fs = new FileStream(Server.MapPath("~/" + medium.Location), FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    medium.Hash = Hashes.MD5(fs);
                    medium.Size = new FileInfo(Server.MapPath("~/" + medium.Location)).Length;
                }
            }

            foreach (var mosaic in medium.Positions.Select(p=>p.Mosaic))
            {
                mosaic.Updated = DateTime.Now;
            }

            medium.Updated = DateTime.Now;
            _db.SaveChanges();
            return RedirectToAction("Index");
        }
        return View(medium);
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
}
}