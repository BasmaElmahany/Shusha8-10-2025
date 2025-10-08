using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Hangfire;
using Shusha_project_BackUp.Data;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Authentication;
using OfficeOpenXml;

namespace Shusha_project_BackUp
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Connection String
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                                 ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            // Database Context
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            // Identity Setup
            builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            /*// Hangfire Setup
            builder.Services.AddHangfire(config => config.UseSqlServerStorage(connectionString));
            builder.Services.AddHangfireServer();*/

            // Register WardService for Hangfire
            builder.Services.AddScoped<WardService>();

            // MVC and Razor Pages
            builder.Services.AddControllersWithViews();
            builder.Services.AddRazorPages();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var app = builder.Build();

            // Role Seeding Logic
            using (var scope = app.Services.CreateScope())
            {
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var roleNames = new[] { "Admin", "Magary", "Lohman", "Bayad A", "Bayad B" };

                foreach (var roleName in roleNames)
                {
                    var roleExist = await roleManager.RoleExistsAsync(roleName);
                    if (!roleExist)
                    {
                        await roleManager.CreateAsync(new IdentityRole(roleName));
                    }
                }
            }

            

            // Middleware
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();


            // Hangfire Dashboard
            /* app.UseHangfireDashboard();

            // Recurring Job: Increment Ward Ages
           RecurringJob.AddOrUpdate<WardService>(
                "increment-ward-ages",
                service => service.IncrementWardAges(),
                Cron.Weekly);*/

            // Default Route
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=LandingPage}/{action=Index}/{id?}");

            app.MapRazorPages();

            // Redirect Root URL to Login
            app.MapGet("/", context =>
            {
                context.Response.Redirect("/LandingPage/Index");
                return Task.CompletedTask;
            });
            app.MapGet("/Logout", async context =>
            {
                await context.SignOutAsync();
                context.Response.Redirect("/LandingPage/Index");
            });

            app.Run();
        }
    }
}
