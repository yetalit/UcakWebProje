using Microsoft.AspNetCore.Localization;
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
using UcakWebProje.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;

namespace UcakWebProje.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private LanguageService _localization;

        private TravelContext tc = new TravelContext(new Microsoft.EntityFrameworkCore.DbContextOptions<TravelContext>());

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

            ViewBag.Places = new List<SelectListItem> { 
                new SelectListItem{ Value = "Istanbul", Text = "Istanbul"},
                new SelectListItem{ Value = "Ankara", Text = "Ankara"},
                new SelectListItem{ Value = "Sakarya", Text = "Sakarya"},
                new SelectListItem{ Value = "Tahran", Text = "Tahran"}
            };
            return View();
        }

        public IActionResult ChangeLanguage(string culture)
        {
            Response.Cookies.Append(CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)), new CookieOptions()
                {
                    Expires = DateTimeOffset.UtcNow.AddYears(1)
                });
            string url = Request.Headers["Referer"].ToString();
            if (url == "")
            {
                url = "/Home/Index";
            }
            return Redirect(url);
        }

        public async Task<IActionResult> TicketResults()
        {
            if (HttpContext.Request.Cookies["travel"] is not null)
            {
                HttpContext.Response.Cookies.Delete("travel");
            }

            if (ModelState.IsValid)
            {
                string dep = HttpContext.Request.Query["departure"];
                string des = HttpContext.Request.Query["destination"];
                if (dep is not null && des is not null && dep != des)
                {
                    try
                    {
                        DateTime date = DateTime.Parse(HttpContext.Request.Query["date"]);
                        int numPssngr = int.Parse(HttpContext.Request.Query["numberOfPassengers"]);
                        
                        if (numPssngr > 0 && numPssngr <= 500)
                        {
                            //Api Call
                            HttpClient client = new HttpClient();
                            var response = await client.GetAsync("https://" + HttpContext.Request.Host + "/api/Flights?dep=" + dep +
                                "&&des=" + des +
                                "&&date=" + date +
                                "&&numPssngr=" + numPssngr);
                            var responseText = await response.Content.ReadAsStringAsync();
                            IEnumerable<Ucak> flights = JsonConvert.DeserializeObject<IEnumerable<Ucak>>(responseText);

                            ViewData["numberOfPassengers"] = numPssngr;
                            return View(flights);
                        }
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
                    if (HttpContext.User != null && HttpContext.User.Identity.IsAuthenticated)
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
                            var k1 = tc.Kullanicilar.ToList().First(user => user.UserName == HttpContext.User.Identity.Name);

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
                            TempData["purchase"] = t1.Price * ticket.numberOfPassengers;
                            return RedirectToAction("Purchased");
                        }
                    }
                    else
                    {
                        TempData["loginAlert"] = 1;
                        return RedirectToPage("/Account/Login", new { area = "Identity" });
                    }
                }
            }
            catch { }
            TempData["Error"] = 1;
            return RedirectToAction("Index");
        }

        public IActionResult Purchased ()
        {
            if (TempData["purchase"] is null)
            {
                return RedirectToAction("Index");
            }
            return View();
        } 

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}