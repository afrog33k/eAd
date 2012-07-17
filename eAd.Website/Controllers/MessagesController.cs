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
public class MessagesController : Controller
{
    private eAdEntities db = new eAdEntities();

    //
    // GET: /Messages/

    public ViewResult Index()
    {
        var messages = db.Messages.Include("Station");
        return View(messages.OrderBy(d=>d.DateAdded).ToList());
    }

    //
    // GET: /Messages/Details/5

    public ViewResult Details(long id)
    {
        Message message = db.Messages.Single(m => m.MessageID == id);
        return View(message);
    }

    //
    // GET: /Messages/Create

    public ActionResult Create()
    {
        ViewBag.StationID = new SelectList(db.Stations, "StationID", "Name");
        return View();
    }

    //
    // POST: /Messages/Create

    [HttpPost]
    public ActionResult Create(Message message)
    {
        if (ModelState.IsValid)
        {
            db.Messages.AddObject(message);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        ViewBag.StationID = new SelectList(db.Stations, "StationID", "Name", message.StationID);
        return View(message);
    }

    //
    // GET: /Messages/Edit/5

    public ActionResult Edit(long id)
    {
        Message message = db.Messages.Single(m => m.MessageID == id);

        ViewBag.StationID = new SelectList(db.Stations, "StationID", "Name", message.StationID);
        return View(message);
    }

    //
    // POST: /Messages/Edit/5

    [HttpPost]
    public ActionResult Edit(Message message)
    {
        if (ModelState.IsValid)
        {
            db.Messages.Attach(message);
            db.ObjectStateManager.ChangeObjectState(message, EntityState.Modified);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        ViewBag.StationID = new SelectList(db.Stations, "StationID", "Name", message.StationID);
        return View(message);
    }

    //
    // GET: /Messages/Delete/5

    public ActionResult Delete(long id)
    {
        Message message = db.Messages.Single(m => m.MessageID == id);
        return View(message);
    }

    //
    // POST: /Messages/Delete/5

    [HttpPost, ActionName("Delete")]
    public ActionResult DeleteConfirmed(long id)
    {
        Message message = db.Messages.Single(m => m.MessageID == id);
        db.Messages.DeleteObject(message);
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