using Microsoft.EntityFrameworkCore;

namespace UcakWebProje.Models
{
    public class TravelContext : DbContext
    {
            
        public DbSet<Bilet> Travels { get; set; }
      

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb; Database=UcakProjesi;Trusted_Connection=True;");
        }
    }

}

