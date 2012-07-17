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
public class ThemesController : Controller
{
    private eAdEntities db = new eAdEntities();

    //
    // GET: /Themes/

    public ViewResult Index()
    {
        return View(db.Themes.ToList());
    }

    //
    // GET: /Themes/Details/5

    public ViewResult Details(long id)
    {
        Theme theme = db.Themes.Single(t => t.ThemeID == id);
        return View(theme);
    }

    //
    // GET: /Themes/Create

    public ActionResult Create()
    {
        return View();
    }

    //
    // POST: /Themes/Create

    [HttpPost]
    public ActionResult Create(Theme theme)
    {
        if (ModelState.IsValid)
        {
            db.Themes.AddObject(theme);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        return View(theme);
    }

    //
    // GET: /Themes/Edit/5

    public ActionResult Edit(long id)
    {
        Theme theme = db.Themes.Single(t => t.ThemeID == id);

        var mystations = theme.Media.ToList();
        var otherStations = db.Media.ToList().Except(mystations);
        ViewBag.NonAddedMedia = new SelectList(otherStations, "MediaID", "Name");
        ViewBag.AddedMedia = new SelectList(mystations, "MediaID", "Name");
        return View(theme);
    }

    //
    // POST: /Themes/Edit/5

    [HttpPost]
    public ActionResult Edit(Theme theme)
    {
        if (ModelState.IsValid)
        {
            db.Themes.Attach(theme);
            db.ObjectStateManager.ChangeObjectState(theme, EntityState.Modified);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        return View(theme);
    }

    //
    // GET: /Themes/Delete/5

    public ActionResult Delete(long id)
    {
        Theme theme = db.Themes.Single(t => t.ThemeID == id);
        return View(theme);
    }

    //
    // POST: /Themes/Delete/5

    [HttpPost, ActionName("Delete")]
    public ActionResult DeleteConfirmed(long id)
    {
        Theme theme = db.Themes.Single(t => t.ThemeID == id);
        db.Themes.DeleteObject(theme);
        db.SaveChanges();
        return RedirectToAction("Index");
    }

    protected override void Dispose(bool disposing)
    {
        db.Dispose();
        base.Dispose(disposing);
    }

    public ActionResult RemoveMedia(long stationID, long groupID)
    {
        var grouping = db.Themes.Single(g => g.ThemeID == groupID);
        var station = grouping.Media.Where(s => s.MediaID == stationID).FirstOrDefault();
        if (station != null)
        {
            grouping.Media.Remove(station);
            db.SaveChanges();
            return Json(new { message = "Media Removed" }, JsonRequestBehavior.AllowGet);
        }
        else
        {
            return Json(new { message = "Media Does Not Exist" }, JsonRequestBehavior.AllowGet);
        }
    }

    public ActionResult AddMedia(long stationID, long groupID)
    {
        Theme theme = db.Themes.Single(g => g.ThemeID == groupID);
        var station = db.Media.Where(s => s.MediaID == stationID).FirstOrDefault();
        if (station != null)
        {
            theme.Media.Add(station);
            db.SaveChanges();
            return Json(new { message = "Media Added" }, JsonRequestBehavior.AllowGet);
        }
        else
        {
            return Json(new { message = "Media Does Not Exist" }, JsonRequestBehavior.AllowGet);
        }
    }
}
}