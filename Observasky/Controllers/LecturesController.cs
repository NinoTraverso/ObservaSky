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
    public class LecturesController : Controller
    {
        private ModelDbContext db = new ModelDbContext();

        // -------------------------------------------------------------------------------  INDEX  --------------------------------------  //

        [HttpGet]
        public ActionResult Index()
        {
            return View(db.Lectures.ToList());
        }

        // -------------------------------------------------------------------------------  DETAILS  --------------------------------------  //

        [HttpGet]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Lectures lectures = db.Lectures.Find(id);
            if (lectures == null)
            {
                return HttpNotFound();
            }
            return View(lectures);
        }

        // -------------------------------------------------------------------------------  CREATE  --------------------------------------  //

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Lectures lectures)
        {
            lectures.Photo = "";
            if (ModelState.IsValid)
            {
                if (lectures.Image != null && lectures.Image.ContentLength > 0)
                {
                    var image = Path.GetFileName(lectures.Image.FileName);
                    var path = Path.Combine(Server.MapPath("~/Content/Images/"), image);
                    lectures.Image.SaveAs(path);

                    lectures.Photo = image;
                }
                db.Lectures.Add(lectures);
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
            Lectures lectures = db.Lectures.Find(id);
            if (lectures == null)
            {
                return HttpNotFound();
            }
            return View(lectures);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Lectures lectures)
        {
            if (ModelState.IsValid)
            {
                if (lectures.Image != null && lectures.Image.ContentLength > 0)
                {
                    var image = Path.GetFileName(lectures.Image.FileName);
                    var path = Path.Combine(Server.MapPath("~/Content/Images/"), image);
                    lectures.Image.SaveAs(path);
                    lectures.Photo = image;
                }

                db.Entry(lectures).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(lectures);
        }

        // -------------------------------------------------------------------------------  DELETE  --------------------------------------  //

        [HttpGet]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Lectures lectures = db.Lectures.Find(id);
            if (lectures == null)
            {
                return HttpNotFound();
            }
            return View(lectures);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Lectures lectures = db.Lectures.Find(id);
            db.Lectures.Remove(lectures);
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
