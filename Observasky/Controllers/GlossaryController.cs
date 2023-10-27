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

        // GET: Glossary
        public ActionResult Index()
        {
            return View(db.Glossary.ToList());
        }

        // GET: Glossary/Details/5
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

        // GET: Glossary/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Glossary/Create
        // Per la protezione da attacchi di overposting, abilitare le proprietà a cui eseguire il binding. 
        // Per altri dettagli, vedere https://go.microsoft.com/fwlink/?LinkId=317598.
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

        // GET: Glossary/Edit/5
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

        // POST: Glossary/Edit/5
        // Per la protezione da attacchi di overposting, abilitare le proprietà a cui eseguire il binding. 
        // Per altri dettagli, vedere https://go.microsoft.com/fwlink/?LinkId=317598.
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

        // GET: Glossary/Delete/5
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

        // POST: Glossary/Delete/5
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
