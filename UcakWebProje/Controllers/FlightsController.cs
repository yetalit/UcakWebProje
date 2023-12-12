using Microsoft.AspNetCore.Mvc;
using System.Runtime.Intrinsics.Arm;
using UcakWebProje.Areas.Identity.Data;
using UcakWebProje.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace UcakWebProje.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlightsController : ControllerBase
    {
        private TravelContext tc = new TravelContext(new Microsoft.EntityFrameworkCore.DbContextOptions<TravelContext>());
        // GET: api/<FlightsController>
        [HttpGet]
        public IEnumerable<Ucak> Get(string dep, string des, DateTime date, int numPssngr)
        {
            var t = tc.Ucaklar.ToList();
            var t1 = from travel in tc.Ucaklar
                     where travel.departure == dep && travel.destination == des && travel.date > DateTime.Now &&
                     travel.date.Year == date.Year && travel.date.Month == date.Month && travel.date.Day == date.Day &&
                     travel.seatCount >= numPssngr
                     select travel;
            return t1;
        }
    }
}
