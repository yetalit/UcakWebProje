using Microsoft.EntityFrameworkCore;

namespace UcakWebProje.Models
{
    public class TravelContext : DbContext
    {
            
        public DbSet<Bilet> Biletler { get; set; }
        public DbSet<Ucak> Ucaklar { get; set; }
        public DbSet<User> Kullanicilar { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Bilet>().HasKey(table => new {
                table.departure,
                table.destination,
                table.date,
                table.AirLine,
                table.passengerUN
            });
            builder.Entity<Ucak>().HasKey(table => new {
                table.departure,
                table.destination,
                table.date,
                table.AirLine
            });
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb; Database=UcakProjesi;Trusted_Connection=True;");
        }
    }

}

