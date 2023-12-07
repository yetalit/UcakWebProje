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

        [HttpPost]
        public IActionResult TicketResults()
        {
            ViewData["current"] = "javascript:rePost('/Home/TicketResults')";
            var jsonObject = new JObject();
            foreach (string name in Request.Form.Keys)
            {
                string value = Request.Form[name];
                jsonObject.Add(name, value);
            }
            ViewData["postData"] = "'" + JsonConvert.SerializeObject(jsonObject).ToString() + "'";
            if (ModelState.IsValid)
            {
                string dep = HttpContext.Request.Form["departure"];
                string des = HttpContext.Request.Form["destination"];
                if (dep != des)
                {
                    try
                    {
                        DateTime date = DateTime.Parse(HttpContext.Request.Form["date"]);
                        int numPssngr = int.Parse(HttpContext.Request.Form["numberOfPassengers"]);

                        var t = tc.Ucaklar.ToList();
                        var t1 = from travel in tc.Ucaklar
                                 where travel.departure == dep && travel.destination == des && travel.date > DateTime.Now &&
                                 travel.date.Year == date.Year && travel.date.Month == date.Month && travel.date.Day == date.Day &&
                                 travel.seatCount >= numPssngr
                                 select travel;

                        return View(t1);
                    }
                    catch { }
                }
            }
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