using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SSD_Lab1.Data;
using SSD_Lab1.Models;
using SSD_Lab1.Middleware;

namespace SSD_Lab1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            // Register exception handler
            builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders()
                .AddDefaultUI();

            builder.Services.AddControllersWithViews();

            // Add memory caching
            builder.Services.AddMemoryCache();

            // Configure antiforgery
            builder.Services.AddAntiforgery(options =>
            {
                options.HeaderName = "X-CSRF-TOKEN";
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            // Use the global exception handler
            app.UseExceptionHandler(options => { });

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            app.MapRazorPages();

            // Database seeding
            var configuration = app.Services.GetService<IConfiguration>();
            if (configuration != null)
            {
                var secrets = configuration.GetSection("Secrets").Get<Secrets>();
                if (secrets != null)
                {
                    DbInitializer.appSecrets = new AppSecrets
                    {
                        SupervisorPassword = secrets.SupervisorPassword,
                        EmployeePassword = secrets.EmployeePassword
                    };
                }
            }

            using (var scope = app.Services.CreateScope())
            {
                var serviceProvider = scope.ServiceProvider;
                DbInitializer.SeedUsersAndRoles(serviceProvider).Wait();
            }

            app.Run();
        }
    }
}