using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Observasky.Models;

namespace Observasky.Controllers
{
    public class EventsController : Controller
    {
        private ModelDbContext db = new ModelDbContext();

        // -------------------------------------------------------------------------------  INDEX  --------------------------------------  //

        [HttpGet]
        public ActionResult Index()
        {
            return View(db.Events.ToList());
        }

        // -------------------------------------------------------------------------------  DETAILS  --------------------------------------  //

        [HttpGet]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Events events = db.Events.Find(id);
            if (events == null)
            {
                return HttpNotFound();
            }
            return View(events);
        }

        // -------------------------------------------------------------------------------  CREATE  --------------------------------------  //

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Events events)
        {
            events.Photo = "";
            if (ModelState.IsValid)
            {
                if (events.Image != null && events.Image.ContentLength > 0)
                {
                    var image = Path.GetFileName(events.Image.FileName);
                    var path = Path.Combine(Server.MapPath("~/Content/Images/"), image);
                    events.Image.SaveAs(path);

                    events.Photo = image;
                }
                db.Events.Add(events);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        // -------------------------------------------------------------------------------  EDIT  --------------------------------------  //

        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Events events = db.Events.Find(id);
            if (events == null)
            {
                return HttpNotFound();
            }
            return View(events);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Events events)
        {
            if (ModelState.IsValid)
            {
                if (events.Image != null && events.Image.ContentLength > 0)
                {
                    var image = Path.GetFileName(events.Image.FileName);
                    var path = Path.Combine(Server.MapPath("~/Content/Images/"), image);
                    events.Image.SaveAs(path);
                    events.Photo = image; 
                }

                db.Entry(events).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(events);
        }

        // -------------------------------------------------------------------------------  DELETE  --------------------------------------  //

        [HttpGet]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Events events = db.Events.Find(id);
            if (events == null)
            {
                return HttpNotFound();
            }
            return View(events);
        }

       
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Events events = db.Events.Find(id);
            db.Events.Remove(events);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // -------------------------------------------------------------------------------  DISPOSE  --------------------------------------  //

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
