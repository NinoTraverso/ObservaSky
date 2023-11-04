using Observasky.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using static System.Net.Mime.MediaTypeNames;

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
        // ------------------------------------------------------------------------------- STARGAZER REGISTER  --------------------------------------  //
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

        // -------------------------------------------------------------------------------  ADD ASTRONOMER  --------------------------------------  //
        [HttpGet]
        public ActionResult AddAstronomer()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddAstronomer(Users users)
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

                users.Role = "Astronomer";

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
                if (user.Role == "Banned")
                {
                    return RedirectToAction("Index", "Home");
                }

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

        // -------------------------------------------------------------------------------  MEMBERS COUNT --------------------------------------  //

        [HttpGet]
        public ActionResult Members()
        {
            int astronomerCount = db.Users.Count(u => u.Role == "Astronomer");
            int stargazerCount = db.Users.Count(u => u.Role == "Stargazer");
            int bannedCount = db.Users.Count(u => u.Role == "Banned");

            ViewBag.AstronomerCount = astronomerCount;
            ViewBag.StargazerCount = stargazerCount;
            ViewBag.BannedCount = bannedCount;

            List<Users> astronomers = db.Users.Where(u => u.Role == "Astronomer").ToList();
            List<Users> stargazers = db.Users.Where(u => u.Role == "Stargazer").ToList();
            List<Users> bannedMembers = db.Users.Where(u => u.Role == "Banned").ToList();
            

            ViewBag.Astronomers = astronomers;
            ViewBag.Stargazers = stargazers;
            ViewBag.BannedMembers = bannedMembers;

            return View();
        }
        // -------------------------------------------------------------------------------  MEMBER SEARCH --------------------------------------  //
        public ActionResult SearchUsers(string searchQuery)
        {
            List<Users> searchResults = db.Users
                .Where(u => u.Username.Contains(searchQuery) && (u.Role == "Astronomer" || u.Role == "Stargazer" || u.Role == "Banned"))
                .ToList();

            ViewBag.SearchResults = searchResults;

            return View("Members");
        }

        // -------------------------------------------------------------------------------  BAN MEMBER --------------------------------------  //
        [HttpPost]
        public ActionResult BanUser(int userId, string userRole)
        {
            var user = db.Users.Find(userId);

            if (user != null && (userRole == "Astronomer" || userRole == "Stargazer"))
            {
                user.Role = "Banned";
                db.SaveChanges(); 
            }

            return Json(new { success = true }); 
        }

        // -------------------------------------------------------------------------------  UNBAN MEMBER --------------------------------------  //

        [HttpPost]
        public ActionResult UnbanUser(int userId, string userRole)
        {
            var user = db.Users.Find(userId);

            if (user != null && user.Role == "Banned")
            {
                user.Role = userRole;
                db.SaveChanges();
            }

            return Json(new { success = true });
        }

        // -------------------------------------------------------------------------------  INFO  --------------------------------------  //
        [HttpGet]
        public ActionResult Info()
        {
            return View();
        }

        // -------------------------------------------------------------------------------  PROFILE CRUD  --------------------------------------  //
        
        public Users GetUserProfile()
        {
            using (var db = new ModelDbContext())
            {

                string username = User.Identity.Name; 

                var user = db.Users.FirstOrDefault(u => u.Username == username);

                return user;
            }
        }

        public ActionResult Profile()
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = GetUserProfile(); 

                if (user == null)
                {
                    return HttpNotFound();
                }

                return View(user); 
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        public ActionResult ModifyProfile()
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = GetUserProfile();

                if (user == null)
                {
                    return HttpNotFound();
                }

                return View(user);
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveProfileChanges(Users modifiedUser, HttpPostedFileBase Image)
        {
            if (ModelState.IsValid)
            {
                using (var db = new ModelDbContext())
                {
                    var user = db.Users.Find(modifiedUser.IdUser);

                    if (user == null)
                    {
                        return HttpNotFound();
                    }

                    user.Username = modifiedUser.Username;
                    user.Email = modifiedUser.Email;
                    user.Password = modifiedUser.Password;

                    if (Image != null)
                    {
                        var fileName = Path.GetFileName(Image.FileName);
                        var path = Path.Combine(Server.MapPath("~/Content/Images"), fileName);
                        Image.SaveAs(path);

                        user.Photo = fileName;
                    }


                    db.SaveChanges(); 

                    return RedirectToAction("Profile"); 
                }
            }

            return View("ModifyProfile", modifiedUser);
        }

        [HttpGet]
        public ActionResult DeleteProfile()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteProfileConfirmed()
        {
            FormsAuthentication.SignOut();

            using (var db = new ModelDbContext())
            {
                string username = User.Identity.Name;
                var user = db.Users.FirstOrDefault(u => u.Username == username);

                if (user != null)
                {
                    db.Users.Remove(user);
                    db.SaveChanges();

                    return RedirectToAction("Login", "Home");
                }
            }

            return View("DeletionFailed");
        }


        // -------------------------------------------------------------------------------  TOP 3 EVENTS  --------------------------------------  //
        public PartialViewResult CarouselEvents()
        {
            using (ModelDbContext context = new ModelDbContext())
            {
                DateTime now = DateTime.Now;

                var upcomingEvents = context.Events
                    .Where(e => e.Date.HasValue && e.Date > now)
                    .OrderBy(e => e.Date)
                    .Take(3)
                    .ToList();

                return PartialView("_CarouselEvents", upcomingEvents);
            }
        }


        // -------------------------------------------------------------------------------  TOP 3 LECTURES  --------------------------------------  //

        public PartialViewResult CarouselLectures()
        {
            using (ModelDbContext context = new ModelDbContext())
            {
                DateTime now = DateTime.Now;

                var upcomingLectures = context.Events
                    .Where(e => e.Date.HasValue && e.Date > now)
                    .OrderBy(e => e.Date)
                    .Take(3)
                    .ToList();

                return PartialView("_CarouselLectures", upcomingLectures);
            }
        }

    }
}

