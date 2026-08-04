using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Lab15_StudentPortalWeb.Models;
using Lab15_StudentPortalWeb.Services;
using System;

namespace Lab15_StudentPortalWeb
{
    public class Program
    {
        public static void Main(string[] args)
        {

            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            // Register StudentPortalContext with DI (Scoped lifetime by default)
            builder.Services.AddDbContext<StudentPortalContext>(options =>
                options.UseSqlServer("Server=DESKTOP-67SE8UE\\SQLEXPRESS;Database=ITI_StudentPortal_EF;Trusted_Connection=True;TrustServerCertificate=True;"));

            // Lab ID 31: 31 mod 3 = 1 -> Scoped lifetime is assigned.
            builder.Services.AddScoped<ISaifStampService, SaifStampService>();

            var app = builder.Build();

            // =========================================================
            // (Middleware Pipeline)
            // =========================================================

            // Custom inline middleware for logging and auditing (Part F)
            app.Use(async (context, next) =>
            {
                var path = context.Request.Path.Value ?? "";
                Console.WriteLine($"[START] Request path: {path}");

                if (path.Contains("/audit-31"))
                {
                    Console.WriteLine($"[AUDIT] Saif Elden Khaled Nazmy Lotfy saw a request for {path}");
                }

                await next.Invoke();

                Console.WriteLine($"[END] Request path: {path}");
            });

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapStaticAssets();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}

/*
====================================================================================================
                              LAB 15 ANSWERS & REFLECTION COMMENTS
====================================================================================================
Name: Saif Elden Khaled Nazmy Lotfy
Lab ID: 31
Build Number: 317

----------------------------------------------------------------------------------------------------
PART A — Scaffold the Web Project
* The app opens on: http://localhost:5114 (HTTP) / https://localhost:7037 (HTTPS)
* Port configuration found in: Properties/launchSettings.json

----------------------------------------------------------------------------------------------------
PART B — Predict Before You Run
B.1 Prediction vs Actual:
  * Prediction: Compiles fine, but runtime crash at startup.
  * Actual: Compiles, but throws System.InvalidOperationException: "The service collection cannot be modified because it is read-only."
B.2 Prediction vs Actual:
  * Prediction: Fails at request time when resolving the controller.
  * Actual: App starts. Throws System.InvalidOperationException when visiting the home page.
B.3 Prediction vs Actual:
  * Prediction: Custom middleware runs 1 time.
  * Actual: Runs 1 time. All static assets are served and short-circuited by UseStaticFiles().

----------------------------------------------------------------------------------------------------
PART C — Wire the Real Context Through DI
* Total students shown: 4 (Yara Adel, Omar Hesham, Nada Samir, Kareem Fouad)
* Roster count matches SELECT COUNT(*) FROM Students exactly.

----------------------------------------------------------------------------------------------------
PART D — Break It On Purpose
D.2: Throws System.InvalidOperationException: "Unable to resolve service for type 'Lab15_StudentPortalWeb.Models.StudentPortalContext' while attempting to activate 'Lab15_StudentPortalWeb.Controllers.HomeController'."
D.5: Serves the page normally, no crash on startup or requests.
D.6: This is bad news. DbContext is not thread-safe. Concurrent users sharing a Singleton instance will cause data corruption and throw concurrency exceptions.

----------------------------------------------------------------------------------------------------
PART E — The Lifetime Experiment
* Lifetime assigned (31 mod 3 = 1): Scoped
* DI Registration: AddScoped

Stamps Table:
+----------------+------------+------------+
| Load / Refresh |  Stamp A   |  Stamp B   |
+----------------+------------+------------+
| First Load     |  8b3f1e94  |  8b3f1e94  |
| Second Load    |  c47d2b8a  |  c47d2b8a  |
+----------------+------------+------------+

* Why did Stamp A and B match? Scoped lifetime resolves a single instance per request.
* Why did they change between refreshes? A new refresh is a new request, which creates a new scope and instance.
* Neighbor comparison:
  - Transient (mod 3 = 0): Stamps did not match within the same load.
  - Singleton (mod 3 = 2): Stamps matched and remained identical across refreshes.

----------------------------------------------------------------------------------------------------
PART F — The Pipeline, Observed
* Audit Path: /audit-31
* First Run Logs: Logs all 7 browser requests (/, bootstrap.css, site.css, jquery.js, bootstrap.js, site.js, favicon.ico)
* Audit Log: Logs [START], [AUDIT], [END] on /audit-31 with the audit statement.
* Second Run Logs: Only logs [START] and [END] on /. Disappeared paths are static files served and short-circuited by UseStaticFiles.

----------------------------------------------------------------------------------------------------
PART G — Reflection
1. Decoupling DbContext configuration from the context class itself (deleting OnConfiguring) allows us to configure different databases (test, dev, prod) from outside, making the class reusable.
2. Failing at startup (fail-fast) prevents broken builds from reaching users, while request-time failures are hidden landmines.
3. Multiple Enumeration (Session 13) and AsNoTracking modification drops (Session 14).
4. divergent migration history causing table schema conflicts and failure to update.
====================================================================================================
*/
