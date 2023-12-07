using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
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
                string dep = HttpContext.Request.Query["departure"];
                string des = HttpContext.Request.Query["destination"];
                string airLine = HttpContext.Request.Query["AirLine"];
                if (dep is not null && des is not null && airLine is not null)
                {
                    DateTime date = DateTime.Parse(HttpContext.Request.Query["date"]);
                    int numPssngr = int.Parse(HttpContext.Request.Query["numberOfPassengers"]);
                    //
                    HttpContext.Session.SetString("userSession", "mmm");
                    //
                    string passengerUN = HttpContext.Session.GetString("userSession");
                    if (passengerUN is null)
                    {
                        // Must Login
                    }
                    else
                    {
                        var t = tc.Ucaklar.ToList();
                        var tquery = from travel in tc.Ucaklar
                                 where travel.departure == dep && travel.destination == des && travel.date > DateTime.Now && travel.date == date &&
                                 travel.AirLine == airLine && travel.seatCount >= numPssngr
                                 select travel;
                        if (tquery.Count() == 1)
                        {
                            var t1 = tquery.First();
                            var k1 = tc.Kullanicilar.ToList().First(user => user.UserName == passengerUN);

                            tc.Add(new Bilet { 
                                departure = t1.departure,
                                destination = t1.destination,
                                date = t1.date,
                                AirLine = t1.AirLine,
                                numberOfPassengers = numPssngr,
                                passengerUN = k1.UserName
                            });
                            t1.seatCount -= numPssngr;
                            tc.Update(t1);
                            tc.SaveChanges();
                            ////////////////////////
                            return RedirectToAction("Index");
                        }
                    }
                }
            }
            catch { }
            TempData["Error"] = 1;
            return RedirectToAction("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}