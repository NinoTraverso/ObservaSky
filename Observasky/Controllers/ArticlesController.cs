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
    public class ArticlesController : Controller
    {
        private ModelDbContext db = new ModelDbContext();


        public ActionResult Index()
        {
            return View(db.Articles.ToList());
        }


        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Articles articles = db.Articles.Find(id);
            if (articles == null)
            {
                return HttpNotFound();
            }
            return View(articles);
        }


        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Articles articles)
        {
            articles.Photo = "";
            if (ModelState.IsValid)
            {
                if (articles.Image != null && articles.Image.ContentLength > 0)
                {
                    var image = Path.GetFileName(articles.Image.FileName);
                    var path = Path.Combine(Server.MapPath("~/Content/Images/"), image);
                    articles.Image.SaveAs(path);

                    articles.Photo = image;
                }
                db.Articles.Add(articles);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Articles articles = db.Articles.Find(id);
            if (articles == null)
            {
                return HttpNotFound();
            }
            return View(articles);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Articles articles)
        {
            if (ModelState.IsValid)
            {
                if (articles.Image != null && articles.Image.ContentLength > 0)
                {
                    var image = Path.GetFileName(articles.Image.FileName);
                    var path = Path.Combine(Server.MapPath("~/Content/Images/"), image);
                    articles.Image.SaveAs(path);
                    articles.Photo = image;
                }

                db.Entry(articles).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(articles);
        }


        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Articles articles = db.Articles.Find(id);
            if (articles == null)
            {
                return HttpNotFound();
            }
            return View(articles);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Articles articles = db.Articles.Find(id);
            db.Articles.Remove(articles);
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
