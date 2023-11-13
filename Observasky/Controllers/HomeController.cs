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
                
                if (db.Users.Any(u => u.Username == users.Username || u.Email == users.Email))
                {
                    ModelState.AddModelError(string.Empty, "Username or email already registered.");
                    return View(users); 
                }

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
                return RedirectToAction("Index");
            }

            return View(users);
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
                if (db.Users.Any(u => u.Username == users.Username || u.Email == users.Email))
                {
                    ModelState.AddModelError(string.Empty, "Username or email already registered.");
                    return View(users);
                }

                if (users.Image != null && users.Image.ContentLength > 0)
                {
                    var image = Path.GetFileName(users.Image.FileName);
                    var path = Path.Combine(Server.MapPath("~/Content/Images/"), image);
                    users.Image.SaveAs(path);
                    users.Photo = image;
                }

                users.Role = "Astronomer";
                db.Users.Add(users);
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(users);
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


                    if (db.Users.Any(u => (u.IdUser != user.IdUser) && (u.Username == modifiedUser.Username || u.Email == modifiedUser.Email)))
                    {
                        ModelState.AddModelError(string.Empty, "Username or email already registered.");
                        return View("ModifyProfile", modifiedUser); 
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

        public PartialViewResult ShowLectures()
        {
            using (ModelDbContext context = new ModelDbContext())
            {
                DateTime now = DateTime.Now;

                var upcomingLectures = context.Lectures
                    .Where(lecture => lecture.DateTime.HasValue && lecture.DateTime > now)
                    .OrderBy(lecture => lecture.DateTime)
                    .Take(5)
                    .ToList();

                return PartialView("_ShowLectures", upcomingLectures);
            }
        }

        // -------------------------------------------------------------------------------  LECTURES BOOKING  --------------------------------------  //

        

        public ActionResult AllLectures()
        {

            var allLectures = db.Lectures.ToList();

            var upcomingLectures = allLectures
                .Where(l => l.DateTime > DateTime.Now)
                .ToList();

            var pastLectures = allLectures
                .Where(l => l.DateTime <= DateTime.Now)
                .ToList();

            ViewBag.UpcomingLectures = upcomingLectures;
            ViewBag.PastLectures = pastLectures;

            return View();
        }

        public ActionResult BookLecture()
        {
            if (User.Identity.IsAuthenticated)
            {
                var currentDate = DateTime.Now;
                using (var dbContext = new ModelDbContext())
                {
                    var upcomingLectures = dbContext.Lectures.Where(lecture => lecture.DateTime > currentDate).ToList();
                    var guest = new Guests();

                    ViewBag.UpcomingLectures = new SelectList(upcomingLectures, "IdLecture", "Name");

                    return View(guest);
                }
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        /*
         OLD SubmitBooking

        [HttpPost]
           public ActionResult SubmitBooking(Guests guest)
           {
               if (ModelState.IsValid)
               {
                   using (var dbContext = new ModelDbContext())
                   {
           
                       var lecture = dbContext.Lectures.Find(guest.LectureID);
           
                       if (lecture != null)
                       {
           
                           lecture.Seats -= guest.NumberOfGuests;
           
           
                           if (lecture.Seats < 0)
                           {
                               lecture.Seats = 0; 
                           }
           
           
                           dbContext.Guests.Add(guest);
                           dbContext.SaveChanges();
           
           
                           return RedirectToAction("BookingSuccess", new { id = guest.IdBooking });
                       }
                   }
               }
           
               return View("BookLecture", guest);
           }
         
         
         */

        [HttpPost]
        public ActionResult SubmitBooking(Guests guest)
        {
            if (ModelState.IsValid)
            {
                using (var dbContext = new ModelDbContext())
                {
                    var lecture = dbContext.Lectures.Find(guest.LectureID);

                    if (lecture != null)
                    {

                        int totalSeatsBookedByUserForCurrentLecture = dbContext.Guests
                            .Where(g => g.Email == guest.Email && g.LectureID == guest.LectureID)
                            .Sum(g => g.NumberOfGuests) ?? 0; 

                        if (totalSeatsBookedByUserForCurrentLecture + guest.NumberOfGuests > 8)
                        {
                            ModelState.AddModelError("NumberOfGuests", "Cannot book more than a total of 8 seats for this lecture.");
                            ViewBag.UpcomingLectures = new SelectList(dbContext.Lectures.Where(l => l.DateTime > DateTime.Now).ToList(), "IdLecture", "Name");
                            return View("BookLecture", guest);
                        }

                        if (guest.NumberOfGuests > (lecture.Seats ?? 0)) 
                        {
                            ModelState.AddModelError("NumberOfGuests", "Cannot book more guests than available seats for this lecture.");
                            ViewBag.UpcomingLectures = new SelectList(dbContext.Lectures.Where(l => l.DateTime > DateTime.Now).ToList(), "IdLecture", "Name");
                            return View("BookLecture", guest);
                        }

                        lecture.Seats -= guest.NumberOfGuests;

                        if (lecture.Seats < 0)
                        {
                            lecture.Seats = 0;
                        }

                        dbContext.Guests.Add(guest);
                        dbContext.SaveChanges();

                        return RedirectToAction("BookingSuccess", new { id = guest.IdBooking });
                    }
                }
            }

            using (var dbContext = new ModelDbContext())
            {
                ViewBag.UpcomingLectures = new SelectList(dbContext.Lectures.Where(l => l.DateTime > DateTime.Now).ToList(), "IdLecture", "Name");
            }

            return View("BookLecture", guest);
        }




        public ActionResult BookingSuccess(int id)
        {
            using (var dbContext = new ModelDbContext())
            {
                var booking = dbContext.Guests.Find(id);

                if (booking != null)
                {
                    ViewBag.LectureName = dbContext.Lectures.Find(booking.LectureID)?.Name;
                    return View(booking);
                }
            }

            return RedirectToAction("BookingNotFound");
        }

        // -------------------------------------------------------------------------------  SEE MY BOOKINGS  --------------------------------------  //

        public ActionResult MyBookings()
        {
            if (User.Identity.IsAuthenticated)
            {
                using (var dbContext = new ModelDbContext())
                {
                    string userEmail = dbContext.Users
                        .Where(u => u.Username == User.Identity.Name)
                        .Select(u => u.Email)
                        .FirstOrDefault();

                    if (!string.IsNullOrEmpty(userEmail))
                    {
                        var userBookings = dbContext.Guests
                            .Where(g => g.Email == userEmail)
                            .Include(g => g.Lectures)
                            .ToList();

                        return View(userBookings);
                    }
                }
            }

            return RedirectToAction("Login", "Home");
        }

        // -------------------------------------------------------------------------------  CANCEL MY BOOKINGS  --------------------------------------  //
        public ActionResult ConfirmCancellation(int id)
        {
            using (var dbContext = new ModelDbContext())
            {
                var booking = dbContext.Guests
                    .Include(b => b.Lectures) 
                    .SingleOrDefault(b => b.IdBooking == id);

                if (booking != null)
                {
                    return View(booking);
                }
            }

            return RedirectToAction("MyBookings");
        }


        [HttpPost]
        public ActionResult ConfirmCancellationConfirmed(int id)
        {
            using (var dbContext = new ModelDbContext())
            {
                var booking = dbContext.Guests.Find(id);

                if (booking != null)
                {
                    var lecture = dbContext.Lectures.Find(booking.LectureID);

                    if (lecture != null)
                    {
                        lecture.Seats += booking.NumberOfGuests;

                        dbContext.Guests.Remove(booking);

                        dbContext.SaveChanges();

                        return RedirectToAction("MyBookings");
                    }
                }
            }

            return RedirectToAction("MyBookings");
        }

        // -------------------------------------------------------------------------------  ALL ARTICLES  --------------------------------------  //

        public ActionResult ArticleList()
        {
            var articles = db.Articles.ToList();
            return View(articles);
        }

        // -------------------------------------------------------------------------------  VIEW SINGLE ARTICLE  --------------------------------------  //
        public ActionResult FullArticle(int id)
        {
            var article = db.Articles.Find(id);
            if (article == null)
            {
                return HttpNotFound();
            }

            return View(article);
        }

        // -------------------------------------------------------------------------------  SEARCH GLOSSARY  --------------------------------------  //
        public ActionResult GlossaryList(string searchQuery, char? letter)
        {
            var glossary = db.Glossary.ToList();

            if (!string.IsNullOrEmpty(searchQuery))
            {
                searchQuery = searchQuery.ToLower();
                glossary = glossary
                    .Where(g => g.Name.ToLower().Contains(searchQuery))
                    .OrderBy(g => g.Name)
                    .ToList();
            }
            else if (letter.HasValue)
            {
                glossary = glossary
                    .Where(g => g.Name.ToLower().StartsWith(letter.Value.ToString().ToLower()))
                    .OrderBy(g => g.Name)
                    .ToList();
            }

            return View(glossary);
        }







    }
}

