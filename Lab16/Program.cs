// LAB 16 - Lab ID: 31 | MAX_YEAR = 4 | MIN_GPA = 3.0 | INTAKE_CODE = itiB
// Answer to Q: The default route sits at the bottom of the table because it is a very general catch-all that matches almost any URL pattern with 0 to 3 segments (due to its defaults and optional id parameter). Since route matching is evaluated top-to-bottom and stops at the first matching pattern, placing the default route at the top would cause it to capture and misroute requests intended for more specific custom routes (e.g. /students/3 or /students/top/3).

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
            // =========================================================
            // PHASE ONE - WHAT CAN THIS APP DO?
            // Everything above builder.Build() registers capabilities
            // into the DI container. Nothing here handles a request yet.
            // =========================================================
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllersWithViews();

            // Register custom constraint nicknames in the ConstraintMap (Part D, nick: intakecode)
            builder.Services.AddRouting(options =>
            {
                options.ConstraintMap.Add("honourBand", typeof(HonourBandConstraint));
                options.ConstraintMap.Add("intakecode", typeof(IntakeCodeConstraint));
            });

            // Register DbContext with connection string pointing to local SQLEXPRESS instance and the correct ITI_StudentPortal_EF database
            builder.Services.AddDbContext<StudentPortalContext>(options =>
            {
                options.UseSqlServer("Server=DESKTOP-67SE8UE\\SQLEXPRESS;Database=ITI_StudentPortal_EF;Trusted_Connection=True;TrustServerCertificate=True")
                .LogTo(Console.WriteLine, LogLevel.Information)
                .EnableSensitiveDataLogging();
            });

            var app = builder.Build();
            // ↑↑↑ THE DIVIDING LINE. Above: what exists. Below: what runs.

            // =========================================================
            // PHASE TWO - HOW IS A REQUEST HANDLED?
            // =========================================================
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            // Middleware for logging request paths (Session 15, unchanged)
            app.Use(async (context, next) =>
            {
                Console.WriteLine($"[START] Request path : {context.Request.Path}");
                await next.Invoke();
                Console.WriteLine($"[END] Request path : {context.Request.Path}");
            });

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthorization();

            // =========================================================
            // THE ROUTE TABLE - Specific routes on top, general last.
            // =========================================================

            // Part B - Custom roster alias route.
            // Answer: Yes, it is acceptable for two different URLs to reach the same action (e.g. /students and /roster) when they act as aliases or synonyms for convenience or SEO backward-compatibility, as long as a canonical link is specified to avoid duplicate indexing penalties.
            app.MapControllerRoute(
                name: "rosterAlias",
                pattern: "roster",
                defaults: new { controller = "Students", action = "Index" });

            // Conventional route for students list
            app.MapControllerRoute(
                name: "studentsList",
                pattern: "students",
                defaults: new { controller = "Students", action = "Index" });

            // Conventional route for student details with integer constraint
            app.MapControllerRoute(
                name: "studentsDetails",
                pattern: "students/{id:int}",
                defaults: new { controller = "Students", action = "Details" });

            // Part C - Top students listing with personal count constraint (MAX_YEAR = 4)
            // Answer: My MAX_YEAR (4) is accepted because the range(1,4) constraint is inclusive of both endpoints.
            app.MapControllerRoute(
                name: "studentsTop",
                pattern: "students/top/{count:int:range(1,4)}",
                defaults: new { controller = "Students", action = "Top" });

            // Conventional route for by-year listing (constrained range 1-4)
            app.MapControllerRoute(
                name: "studentsByYear",
                pattern: "students/year/{year:int:range(1,4)}",
                defaults: new { controller = "Students", action = "ByYear" });

            // Part D - Intake listing route using our custom IRouteConstraint (INTAKE_CODE = itiB)
            app.MapControllerRoute(
                name: "studentsIntake",
                pattern: "students/intake/{code:intakecode}",
                defaults: new { controller = "Students", action = "Intake" });

            // Conventional route for honours listing (using custom honourBand constraint)
            app.MapControllerRoute(
                name: "studentsHonours",
                pattern: "students/honours/{band:honourBand}",
                defaults: new { controller = "Students", action = "Honours" });

            // Default route (catch-all, last)
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}

/*
====================================================================================================
                              LAB 16 ANSWERS & REFLECTION COMMENTS
====================================================================================================
Name: Saif Elden Khaled Nazmy Lotfy
Lab ID: 31
MAX_YEAR: 4
MIN_GPA: 3.0
INTAKE_CODE: itiB

----------------------------------------------------------------------------------------------------
PART A - Verify and orient
* The default route sits at the bottom of the table because it is a very general catch-all that matches 
  almost any URL pattern with 0 to 3 segments (due to defaults and the optional id). If placed at the top, 
  it would capture specific URLs like /students/3 or /students/top/3, matching them incorrectly and 
  bypassing our custom routes.

----------------------------------------------------------------------------------------------------
PART B - A second address for a page that already exists
* Synonyms route pattern: "roster" -> mapped to Students.Index.
* Is it acceptable for two different URLs to reach the same action? Yes, it is acceptable if they represent 
  semantic aliases or synonyms for user convenience or backward compatibility, as long as a canonical link 
  is defined in HTML to avoid duplicate content search engine penalties.

----------------------------------------------------------------------------------------------------
PART C - A route with a personalised constraint
* Top students route pattern: "students/top/{count:int:range(1,4)}" -> mapped to Students.Top.
* Count is constrained to be an integer and between 1 and 4 inclusive.
* Is your MAX_YEAR itself accepted, or rejected? Accepted. The range(1,4) constraint is inclusive of both bounds, 
  so 4 (MAX_YEAR) is accepted, while 5 is rejected.

----------------------------------------------------------------------------------------------------
PART D - A constraint you write yourself
* Custom constraint class: IntakeCodeConstraint (verifies code equals "itiB" case-insensitively).
* Nickname registered: "intakecode"
* Intake route pattern: "students/intake/{code:intakecode}" -> mapped to Students.Intake.
* Why must the constraint NOT touch the database? A constraint is evaluated on every single incoming request during 
  route matching (including static file requests, icons, and bad URLs). Performing database queries inside Match() 
  would cause massive performance bottlenecks and redundant connection overhead. Constraints should only validate 
  the shape or value of route parameters, never check database state.

----------------------------------------------------------------------------------------------------
PART E - Your own address
* Personalised action: About (attribute routed to [Route("about/saif")])
* Query parameter: minGpa (marked with [FromQuery] and defaults to MIN_GPA = 3.0 if null).
* Why does /Students/About return a 404? Because once an action is decorated with an attribute route (like 
  [Route("about/saif")]), it is decoupled from conventional routing and no longer responds to conventional 
  patterns like {controller}/{action}.
* Why does minGpa belong in the query string rather than in the path? Because minGpa is a filtering constraint 
  (a view of the resources) rather than an identity of a resource. Changing minGpa filters the students, but we 
  are still looking at the same page (about students), not a different entity. Therefore, it is a query concern, 
  not a path concern.

====================================================================================================
*/
