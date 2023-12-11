using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UcakWebProje.Models;
using UcakWebProje.Services;

namespace UcakWebProje.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private LanguageService _localization;

        private TravelContext tc = new TravelContext();

        public HomeController(ILogger<HomeController> logger, LanguageService localization)
        {
            _logger = logger;
            _localization = localization;
        }

        public IActionResult Index()
        {
            if (HttpContext.Request.Cookies["travel"] is not null)
            {
                HttpContext.Response.Cookies.Delete("travel");
            }

            ViewData["current"] = HttpContext.Request.Path + HttpContext.Request.QueryString;
            ViewBag.Places = new List<SelectListItem> { 
                new SelectListItem{ Value = "Istanbul", Text = "Istanbul"},
                new SelectListItem{ Value = "Ankara", Text = "Ankara"},
                new SelectListItem{ Value = "Sakarya", Text = "Sakarya"},
                new SelectListItem{ Value = "Tahran", Text = "Tahran"}
            };
            return View();
        }

        public IActionResult TicketResults()
        {
            if (HttpContext.Request.Cookies["travel"] is not null)
            {
                HttpContext.Response.Cookies.Delete("travel");
            }

            ViewData["current"] = HttpContext.Request.Path + HttpContext.Request.QueryString;

            if (ModelState.IsValid)
            {
                string dep = HttpContext.Request.Query["departure"];
                string des = HttpContext.Request.Query["destination"];
                if (dep != des)
                {
                    try
                    {
                        DateTime date = DateTime.Parse(HttpContext.Request.Query["date"]);
                        int numPssngr = int.Parse(HttpContext.Request.Query["numberOfPassengers"]);

                        var t = tc.Ucaklar.ToList();
                        var t1 = from travel in tc.Ucaklar
                                 where travel.departure == dep && travel.destination == des && travel.date > DateTime.Now &&
                                 travel.date.Year == date.Year && travel.date.Month == date.Month && travel.date.Day == date.Day &&
                                 travel.seatCount >= numPssngr
                                 select travel;

                        ViewData["numberOfPassengers"] = numPssngr;
                        return View(t1);
                    }
                    catch { }
                }
            }
            TempData["Error"] = 1;
            return RedirectToAction("Index");
        }

        public IActionResult BuyTicket ()
        {
            try
            {
                Bilet ticket = JsonConvert.DeserializeObject<Bilet>(HttpContext.Request.Cookies["travel"]);
                if (ticket.departure is not null && ticket.destination is not null && ticket.AirLine is not null)
                {
                    ticket.date = DateTime.Parse(ticket.date.ToString(CultureInfo.GetCultureInfo("en-US")));
                    string passengerUN = HttpContext.Session.GetString("userSession");
                    if (passengerUN is null)
                    {
                        TempData["loginAlert"] = 1;
                        return RedirectToAction("Login");
                    }
                    else
                    {
                        var t = tc.Ucaklar.ToList();
                        var tquery = from travel in tc.Ucaklar
                                     where travel.departure == ticket.departure && travel.destination == ticket.destination &&
                                     travel.date == ticket.date && travel.date > DateTime.Now &&
                                     travel.AirLine == ticket.AirLine && travel.seatCount >= ticket.numberOfPassengers
                                     select travel;
                        if (tquery.Count() == 1)
                        {
                            var t1 = tquery.First();
                            var k1 = tc.Kullanicilar.ToList().First(user => user.UserName == passengerUN);

                            tc.Add(new Bilet
                            {
                                departure = t1.departure,
                                destination = t1.destination,
                                date = t1.date,
                                AirLine = t1.AirLine,
                                numberOfPassengers = ticket.numberOfPassengers,
                                passengerUN = k1.UserName,
                                orderTime = DateTime.Now
                            });
                            t1.seatCount -= ticket.numberOfPassengers;
                            tc.Update(t1);
                            tc.SaveChanges();
                            HttpContext.Response.Cookies.Delete("travel");
                            ////////////////////////
                            return RedirectToAction("MyTickets");
                        }
                    }
                }
            }
            catch { }
            TempData["Error"] = 1;
            return RedirectToAction("Index");
        }

        public IActionResult Login()
        {
            if (HttpContext.Session.GetString("userSession") is not null)
            {
                return RedirectToAction("Index");
            }
            ViewData["current"] = HttpContext.Request.Path + HttpContext.Request.QueryString;
            return View();
        }
        [HttpPost]
        public IActionResult LoginCheck ()
        {
            if (HttpContext.Session.GetString("userSession") is not null)
            {
                return RedirectToAction("Index");
            }
            if (ModelState.IsValid)
            {
                try
                {
                    StringBuilder sb = new StringBuilder();
                    using (SHA256 sha256Hash = SHA256.Create())
                    {
                        byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(HttpContext.Request.Form["Password"].ToString()));

                        for (int i = 0; i < bytes.Length; i++)
                        {
                            sb.Append(bytes[i].ToString("x2"));
                        }
                    }
                    var k = tc.Kullanicilar.ToList();
                    var u = from user in tc.Kullanicilar
                            where user.UserName == HttpContext.Request.Form["UserName"].ToString() &&
                            user.Password == sb.ToString()
                            select user;
                    HttpContext.Session.SetString("userSession", u.First().UserName);
                    return RedirectToAction("MyTickets");
                }
                catch {
                    TempData["loginFailed"] = 1;
                    return RedirectToAction("Login");
                }
            }
            TempData["Error"] = 1;
            return RedirectToAction("Index");
        }

        public IActionResult SignUp()
        {
            if (HttpContext.Session.GetString("userSession") is not null)
            {
                return RedirectToAction("Index");
            }
            ViewData["current"] = HttpContext.Request.Path + HttpContext.Request.QueryString;
            return View();
        }
        [HttpPost]
        public IActionResult SignUpCheck ()
        {
            if (HttpContext.Session.GetString("userSession") is not null)
            {
                return RedirectToAction("Index");
            }
            if (ModelState.IsValid)
            {
                try
                {
                    StringBuilder sb = new StringBuilder();
                    using (SHA256 sha256Hash = SHA256.Create())
                    {
                        byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(HttpContext.Request.Form["Password"].ToString()));

                        for (int i = 0; i < bytes.Length; i++)
                        {
                            sb.Append(bytes[i].ToString("x2"));
                        }
                    }
                    tc.Add(new User()
                    {
                        UserName = HttpContext.Request.Form["UserName"].ToString(),
                        Password = sb.ToString(),
                        FirstName = HttpContext.Request.Form["FirstName"].ToString(),
                        LastName = HttpContext.Request.Form["LastName"].ToString(),
                        Mail = HttpContext.Request.Form["Mail"].ToString(),
                        phoneNum = HttpContext.Request.Form["phoneNum"].ToString()
                    });
                    tc.SaveChanges();
                    HttpContext.Session.SetString("userSession", HttpContext.Request.Form["UserName"].ToString());
                    return RedirectToAction("MyTickets");
                }
                catch
                {
                    TempData["signupFailed"] = 1;
                    return RedirectToAction("SignUp");
                }
            }
            TempData["Error"] = 1;
            return RedirectToAction("Index");
        }

        public IActionResult MyTickets()
        {
            if (HttpContext.Request.Cookies["travel"] is not null)
            {
                return RedirectToAction("BuyTicket");
            }
            if (HttpContext.Session.GetString("userSession") is null)
            {
                return RedirectToAction("Login");
            }
            ViewData["current"] = HttpContext.Request.Path + HttpContext.Request.QueryString;
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}