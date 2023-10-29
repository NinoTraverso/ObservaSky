using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Observasky.Models;

namespace Observasky.Controllers
{
    public class GlossaryController : Controller
    {
        private ModelDbContext db = new ModelDbContext();

        [HttpGet]
        public ActionResult Index()
        {
            return View(db.Glossary.ToList());
        }

        [HttpGet]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Glossary glossary = db.Glossary.Find(id);
            if (glossary == null)
            {
                return HttpNotFound();
            }
            return View(glossary);
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IdGlossary,Name,Description")] Glossary glossary)
        {
            if (ModelState.IsValid)
            {
                db.Glossary.Add(glossary);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(glossary);
        }

        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Glossary glossary = db.Glossary.Find(id);
            if (glossary == null)
            {
                return HttpNotFound();
            }
            return View(glossary);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IdGlossary,Name,Description")] Glossary glossary)
        {
            if (ModelState.IsValid)
            {
                db.Entry(glossary).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(glossary);
        }

        [HttpGet]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Glossary glossary = db.Glossary.Find(id);
            if (glossary == null)
            {
                return HttpNotFound();
            }
            return View(glossary);
        }

        
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Glossary glossary = db.Glossary.Find(id);
            db.Glossary.Remove(glossary);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

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
