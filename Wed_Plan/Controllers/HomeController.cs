using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Wedding_Planner.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;



namespace Wedding_Planner.Controllers
{
    public class HomeController : Controller
    {
        private int? UserSession
        {
            get { return HttpContext.Session.GetInt32("UserId"); }
            set { HttpContext.Session.SetInt32("UserId", (int)value); }
        }

        private MyContext dbContext;

        // here we can "inject" our context service into the constructor
        public HomeController(MyContext context)

        {
            dbContext = context;
        }

        [HttpPost("register")]
        public IActionResult Register(User newUser)
        {
            {
                if (ModelState.IsValid)
                {
                    // Check if email already exists in db
                    var existingUser = dbContext.Users.FirstOrDefault(u => u.Email == newUser.Email);
                    if (existingUser != null)
                    {
                        ModelState.AddModelError("Email", "Email already exists");
                        return View("Index");
                    }
                    // Hash new user's password and save new user to db
                    PasswordHasher<User> Hasher = new PasswordHasher<User>();

                    newUser.Password = Hasher.HashPassword(newUser, newUser.Password);

                    dbContext.Users.Add(newUser);

                    dbContext.SaveChanges();

                    UserSession = newUser.UserId;

                    // return RedirectToAction("Dashboard", "Wedding");

                    return RedirectToAction("Dashboard");
                }
                return View("Index");
            }
        }

        [HttpPost("login")]
        public IActionResult Login(User userSubmission)
        {

            // User RetrievedUser = dbContext.Users.FirstOrDefault(user => user.Email == userSubmission.Email);
            // if (ModelState.IsValid)
            // {
            var userInDb = dbContext.Users.FirstOrDefault(u => u.Email == userSubmission.Email);
            if (userInDb == null)
            {
                // Add an error to ModelState and return to View!
                ModelState.AddModelError("Email", "Invalid Email/Password");
                return View("Index");
            }

            // Initialize hasher object
            var hasher = new PasswordHasher<User>();
            var result = hasher.VerifyHashedPassword(userSubmission, userInDb.Password, userSubmission.Password);

            // verify provided password against hash stored in db
            Console.WriteLine(userInDb.Password + "     Break em Up     " + userSubmission.Password);


            Console.WriteLine("********************************************************************");
            Console.WriteLine(result);
            Console.WriteLine("********************************************************************");                // result can be compared to 0 for failure
            if (result != 0)
            {
                // HttpContext.Session.SetString("email", userSubmission.Email);
                UserSession = userInDb.UserId;
                Console.WriteLine(userInDb.UserId);

                return RedirectToAction("Dashboard");
                // handle failure (this should be similar to how "existing email" is handled)
                // }


            }

            Console.WriteLine("********************************************************************");
            Console.WriteLine(userInDb.UserId);

            // Console.WriteLine(result);
            Console.WriteLine("********************************************************************");
            ModelState.AddModelError("Password", "Git ta know ur hashword dummy!");
            return View("Index");
        }



        public IActionResult Index()
        {
            return View();
        }


        // [HttpGet("dashboard")]
        // public IActionResult Dashboard()
        // {
        //     return View("dashboard");
        // }



        // HttpContext.Session.GetString("email", newUser.Email);





        [HttpGet("dashboard")]
        public IActionResult Dashboard()
        {
            // Redirect to register/login page if no user in session
            if (UserSession == null)
                return RedirectToAction("Index");

            // Get all weddings with included responses ordered by date
            var AllWeddings = dbContext.Weddings
                .Include(w => w.RSVPs)
                .OrderByDescending(w => w.Date)
                .ToList();


            // var WDate = dbContext.Weddings.All(u => u.Date == WDate.CreatedAt);

            // DateTime date1 = Wedding.CreatedAt;
            // DateTime date2 = new DateTime(2009, 8, 1, 12, 0, 0);
            // int result = DateTime.Compare(date1, date2);

            ViewBag.UserId = UserSession;
            return View("dashboard", AllWeddings);
        }

        [HttpGet("{weddingId}")]
        public IActionResult Show(int weddingId)
        {
            var thisWedding = dbContext.Weddings
            .Include(w => w.RSVPs)
            .ThenInclude(r => r.Guest)
            .FirstOrDefault(w => w.WeddingId == weddingId);
            return View("Show", thisWedding);
        }

        [HttpGet("newwedding")]
        public IActionResult NewWedding()
        {
            return View();
        }

        [HttpPost("create")]
        public IActionResult Create(Wedding newWedding)
        {
            if (UserSession == null)
                return RedirectToAction("Index");
            if (ModelState.IsValid)
            {
                // Crete new wedding with UserId set to current session user's id
                newWedding.UserId = (int)UserSession;
                dbContext.Weddings.Add(newWedding);
                dbContext.SaveChanges();
                return RedirectToAction("Dashboard");
            }
            return View("newwedding");
        }

        [HttpGet("delete")]
        public IActionResult Delete(int weddingId)
        {
            if (UserSession == null)
                return RedirectToAction("Index");

            Wedding toDelete = dbContext.Weddings.FirstOrDefault(w => w.WeddingId == weddingId);

            if (toDelete == null)
                return RedirectToAction("Dashboard");
            // Redirect to dashboard if user trying to delete isn't the wedding creator
            if (toDelete.UserId != UserSession)
                return RedirectToAction("Dashboard");

            dbContext.Weddings.Remove(toDelete);
            dbContext.SaveChanges();
            return RedirectToAction("Dashboard");
        }

        [HttpGet("rsvp/{weddingId}")]
        public IActionResult RSVP(int weddingId)
        {
            if (UserSession == null)
                return RedirectToAction("Index");

            // Create a new response with the given weddingId and current userId
            RSVP newRSVP = new RSVP()
            {
                WeddingId = weddingId,
                UserId = (int)UserSession
            };
            dbContext.RSVPs.Add(newRSVP);
            dbContext.SaveChanges();
            return RedirectToAction("Dashboard");
        }

        [HttpGet("unrsvp/{weddingId}")]
        public IActionResult UnRSVP(int weddingId)
        {
            if (UserSession == null)
                return RedirectToAction("Index");

            // Query to grab the appropriate response to remove
            RSVP toDelete = dbContext.RSVPs.FirstOrDefault(r => r.WeddingId == weddingId && r.UserId == UserSession);

            // Redirect to dashboard if no match for RSVP in db
            if (toDelete == null)
                return RedirectToAction("Dashboard");

            dbContext.RSVPs.Remove(toDelete);
            dbContext.SaveChanges();
            return RedirectToAction("Dashboard");
        }


        [HttpGet("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }



        [HttpGet("details")]
        public IActionResult Details()
        {
            return View("details");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }




















    }
}






