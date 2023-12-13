using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Options;
using System.Globalization;
using System.Reflection;
using UcakWebProje.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UcakWebProje.Areas.Identity.Data;
using Microsoft.AspNetCore.Razor.Language.Intermediate;

public class Program
{
    static async Task Main(string[] args)   
    {
        var builder = WebApplication.CreateBuilder(args);
        var connectionString = builder.Configuration.GetConnectionString("TravelContextConnection") ?? throw new InvalidOperationException("Connection string 'TravelContextConnection' not found.");

        builder.Services.AddDbContext<TravelContext>(options =>
            options.UseSqlServer(connectionString));

        builder.Services.AddDefaultIdentity<User>(options => { options.SignIn.RequireConfirmedAccount = options.SignIn.RequireConfirmedEmail = false; })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<TravelContext>();

        // Add services to the container.
        builder.Services.AddControllersWithViews();

        builder.Services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromMinutes(45);
        });

        builder.Services.AddSingleton<LanguageService>();
        builder.Services.AddLocalization(ops => ops.ResourcesPath = "Resources");
        builder.Services.AddMvc().AddViewLocalization().AddDataAnnotationsLocalization(ops => ops.DataAnnotationLocalizerProvider = (type, factory) =>
        {
            var assemblyName = new AssemblyName(typeof(SharedResource).GetTypeInfo().Assembly.FullName);
            return factory.Create(nameof(SharedResource), assemblyName.Name);
        }
        );
        builder.Services.Configure<RequestLocalizationOptions>(ops =>
        {
            var supportedCultures = new List<CultureInfo>()
            {
        new CultureInfo("en-US"),
        new CultureInfo("tr-TR")
            };
            ops.DefaultRequestCulture = new RequestCulture(culture: "en-US", uiCulture: "en-US");
            ops.SupportedCultures = supportedCultures;
            ops.SupportedUICultures = supportedCultures;
            ops.RequestCultureProviders.Insert(0, new QueryStringRequestCultureProvider());
        });

        builder.Services.Configure<IdentityOptions>(options =>
        {
            // Default Password settings.
            options.Password.RequireDigit = false;
            options.Password.RequireLowercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequiredLength = 3;
            options.Password.RequiredUniqueChars = 0;
        });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRequestLocalization(app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value);

        app.UseRouting();

        app.UseSession();
        app.UseAuthentication(); ;

        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        app.MapRazorPages();

        using (var scope = app.Services.CreateScope())
        {
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var roles = new[]
            {
        "Admin",
        "Staff",
        "Customer"
            };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }

        using (var scope = app.Services.CreateScope())
        {
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            string admin = "b211210574@sakarya.edu.tr";
            string pass = "dfb3c9aac79bac448ca7c11952aba5a27ebb963f2070c5ea16e4074ceb1c0ad0";
            if (await userManager.FindByNameAsync(admin) == null)
            {
                var user = new User();
                user.UserName = admin;
                user.Email = admin;
                user.Password = pass;
                user.FirstName = "admin";
                user.LastName = "admin";
                user.Mail = admin;
                user.phoneNum = "05555555555";

                await userManager.CreateAsync(user, pass);
                await userManager.AddToRoleAsync(user, "Admin");
            }
        }

        app.Run();
    }
}
