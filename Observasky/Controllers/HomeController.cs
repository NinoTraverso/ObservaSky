using Observasky.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Observasky.Controllers
{
    public class HomeController : Controller
    {
        private ModelDbContext db = new ModelDbContext();

        // -------------------------------------------------------------------------------  INDEX  --------------------------------------  //
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            return View();
        }
        // -------------------------------------------------------------------------------  REGISTER  --------------------------------------  //
        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(Users users)
        {
            users.Photo = "";
            if (ModelState.IsValid)
            {
                if (users.Image != null && users.Image.ContentLength > 0)
                {
                    var image = Path.GetFileName(users.Image.FileName);
                    var path = Path.Combine(Server.MapPath("~/Content/Images/"), image);
                    users.Image.SaveAs(path);

                    users.Photo = image;
                }
                db.Users.Add(users);

                users.Role = "Stargazer";

                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }
        // -------------------------------------------------------------------------------  LOGIN  --------------------------------------  //
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public ActionResult Login([Bind(Include = "Username, Password, Role")] Users users)
        {

            var user = db.Users.FirstOrDefault(u => u.Username == users.Username && u.Password == users.Password);

            if (user != null)
            {

                FormsAuthentication.SetAuthCookie(users.Username, false);
                return RedirectToAction("Index", "Home");
            }

            return View();
        }
        // -------------------------------------------------------------------------------  LOGOUT  --------------------------------------  //
        public ActionResult Logout()
        {

            FormsAuthentication.SignOut();

            return RedirectToAction("Login", "Home");
        }

        // -------------------------------------------------------------------------------  INFO  --------------------------------------  //
        [HttpGet]
        public ActionResult Info()
        {
            return View();
        }

    }
}
