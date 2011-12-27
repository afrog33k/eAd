using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using eAd.DataAccess;
using eAd.Website.Repositories;
namespace eAd.Website.Controllers
{
    public class MosaicController : Controller
    {
        private eAdEntities db = new eAdEntities();
        //
        // GET: /Mosaic/

        public ActionResult Index()
        {
            return View(db.Media.ToList());
        }

    }
}
