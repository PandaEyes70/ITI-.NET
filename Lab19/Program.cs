// =====================================================================
// StudentPortalWeb — SESSION PROJECT (Style Guide Rule 20/34/35/39/40)
// ITI Summer Training | Web Development Using .NET | Morning Group
// Session 19 — Models: the Many-to-Many Relationship
//
// ⚠️ NOTHING IN THIS FILE CHANGES TODAY. Not one line. Today's five TODOs
//    live in Models/StudentPortalContext.cs, Controllers/StudentsController.cs,
//    Controllers/CoursesController.cs (new file) and Views/Students/Details.cshtml.
//
// THIS PROJECT IS DAY-READY (Rule 39). Press F5 right now, before a
// single TODO is done, and the whole site from Sessions 15-18 works.
// `/Courses` will 404 until TODO 4 exists — that is expected, not a bug.
//
// ⚠️ TODAY'S SCHEMA CHANGE HAPPENS ELSEWHERE. This project does not own
//    migrations (Rule 38) — the Session 14 console project does. Before
//    any of today's Enrollment queries here can return real data, the
//    Enrollment entity must already be migrated for real from THAT
//    project. See the Instructor Guide, Block 2.
// =====================================================================

using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StudentPortalWeb.Constraints;
using StudentPortalWeb.Models;
using System;

namespace StudentPortalWeb
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllersWithViews();

            builder.Services.AddRouting(options =>
            {
                options.ConstraintMap.Add("honourBand", typeof(HonourBandConstraint));
            });

            builder.Services.AddDbContext<StudentPortalContext>(options =>
            {
                options.UseSqlServer("Server=DESKTOP-67SE8UE\\SQLEXPRESS;Database=ITI_StudentPortal_EF;Trusted_Connection=True;TrustServerCertificate=True")
                .LogTo(Console.WriteLine , LogLevel.Information)
                .EnableSensitiveDataLogging();
            });

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<StudentPortalContext>();
                context.Database.Migrate();
            }

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.Use(async (context, next) =>
            {
                Console.WriteLine($"[START] Request path : {context.Request.Path}");
                await next.Invoke();
                Console.WriteLine($"[END] Request path : {context.Request.Path}");
            });

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            //app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "studentsList",
                pattern: "students",
                defaults: new { controller = "Students", action = "Index" });

            app.MapControllerRoute(
                name: "studentsDetails",
                pattern: "students/{id:int}",
                defaults: new { controller = "Students", action = "Details" });

            app.MapControllerRoute(
                name: "studentsByYear",
                pattern: "students/year/{year:int:range(1,4)}",
                defaults: new { controller = "Students", action = "ByYear" });

            app.MapControllerRoute(
                name: "studentsHonours",
                pattern: "students/honours/{band:honourBand}",
                defaults: new { controller = "Students", action = "Honours" });

            // No new route for Courses today. It reaches CoursesController
            // through the same default route every other unadorned
            // controller in this project already uses — proof that not
            // every new controller needs its own custom route.
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}

#region 📋 Full TODO Checklist
// ---------------------------------------------------------------------
// Nothing in Program.cs today.
//
// Models/StudentPortalContext.cs
//   TODO 1: The Enrollment entity — four parts, four locations   [Block 1]
//   TODO 2: Fluent API — two relationships + a unique index      [Block 2]
//
// Controllers/StudentsController.cs
//   TODO 3: Include/ThenInclude on Details                       [Block 3]
//
// Controllers/CoursesController.cs (new file)
//   TODO 4: Field+constructor, Index, Details — three parts      [Block 4]
//
// Views/Students/Details.cshtml
//   TODO 5: Show the enrolled-courses table, reusing <gpa-badge>  [Block 4]
// ---------------------------------------------------------------------
#endregion
