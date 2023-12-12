using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UcakWebProje.Areas.Identity.Data;
using UcakWebProje.Models;

namespace UcakWebProje.Areas.Identity.Data;

public class TravelContext : IdentityDbContext<User>
{
    public TravelContext(DbContextOptions<TravelContext> options)
        : base(options)
    {
    }

    public DbSet<Bilet> Biletler { get; set; }
    public DbSet<Ucak> Ucaklar { get; set; }
    public DbSet<User> Kullanicilar { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);
        builder.Entity<Bilet>().HasKey(table => new {
            table.departure,
            table.destination,
            table.date,
            table.AirLine,
            table.passengerUN,
            table.orderTime
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
