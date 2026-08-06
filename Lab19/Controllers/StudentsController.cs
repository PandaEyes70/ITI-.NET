// =====================================================================
// StudentsController — CARRIED FORWARD FROM SESSIONS 15-18 (Rule 39)
// ITI Summer Training | Web Development Using .NET | Morning Group
// Session 19 — Models
//
// ⚠️ ALMOST NOTHING IN THIS FILE CHANGES TODAY. Six of its seven actions
//    are exactly as Session 17 left them. ONE method — Details — gets a
//    two-line extension: TODO 3, below, inside it.
// =====================================================================

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentPortalWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentPortalWeb.Controllers
{
    public class StudentsController : Controller
    {
        private readonly StudentPortalContext _context;

        public StudentsController(StudentPortalContext context)
        {
            _context = context;
        }

        public IActionResult Demo(int id)
        {
            if (id == 0) return NotFound();
            if (id == 1) return Content("I'm a plain text , not a page.");
            if (id == 2) return Json(new { Message = "This is a JSON" , Id = id });
            if (id == 3) return RedirectToAction("Index");
            return View();
        }

        public IActionResult Echo(
            [FromRoute] int id,
            [FromQuery] string note,
            [FromHeader(Name = "User-Agent")] string agent
            )
        {
            return Content($"id (route) = {id} | note (query) = {note} | agent (header) = {agent}");
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Student student)
        {
            if (!ModelState.IsValid)
            {
                return View(student);
            }
            await _context.Students.AddAsync(student);
            await _context.SaveChangesAsync();

            TempData["Message"] = $"{student.FullName} was Added";

            return RedirectToAction("Index");
        }

        // Answers the students list route.
        public async Task<IActionResult> Index()
        {
            var students = await _context.Students
                .OrderBy(s => s.FullName)
                .ToListAsync();

            return View(students);
        }

        // Answers the student detail route.
        public async Task<IActionResult> Details(int id)
        
        {
            // TODO 3: (Block 3.) Add TWO calls onto this query, between
            //         .Students and .FirstOrDefaultAsync — an Include,
            //         then a ThenInclude chained directly onto it.
            //
            //         The Include names the navigation property on
            //         Student that reaches its enrollments — the list
            //         property TODO 1 added in the Models file.
            //
            //         The ThenInclude goes one hop FURTHER, from each
            //         Enrollment already being loaded, naming the
            //         navigation property that reaches ITS course. Its
            //         parameter type is a single Enrollment, not a
            //         Student — you are now describing what to do with
            //         each item Include just loaded, not with the
            //         original Student any more.
            //
            //         ⚠️ Include alone would load the Enrollment rows —
            //         Id, StudentId, CourseId, EnrollmentDate, Grade —
            //         but leave each Enrollment's own Course property
            //         unpopulated (null). The page would compile, the
            //         enrollment count would be right, and
            //         enrollment.Course.CourseName would throw the
            //         moment anyone tried to print it. ThenInclude is
            //         the second hop, and skipping it is the single most
            //         common way today's relationship goes quietly
            //         wrong — Block 3 demonstrates this on purpose,
            //         once, before showing the fix.
            var student = await _context.Students
                .Include(s => s.Enrollments)
                    .ThenInclude(e => e.Course)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (student is null)
            {
                return NotFound();
            }

            return View(student);
        }

        // Answers the by-year route.
        public async Task<IActionResult> ByYear(int year)
        {
            var students = await _context.Students
                .Where(s => s.YearOfStudy == year)
                .OrderBy(s => s.FullName)
                .ToListAsync();

            ViewData["Year"] = year;

            return View(students);
        }

        // Answers the honours route, guarded by the Session 16 constraint.
        public async Task<IActionResult> Honours(string band)
        {
            if (string.IsNullOrWhiteSpace(band))
            {
                return NotFound();
            }

            IQueryable<Student> query = _context.Students;

            if (string.Equals(band, "first", StringComparison.OrdinalIgnoreCase))
            {
                query = query.Where(s => s.Gpa >= 3.5);
            }
            else if (string.Equals(band, "second", StringComparison.OrdinalIgnoreCase))
            {
                query = query.Where(s => s.Gpa >= 3.0 && s.Gpa < 3.5);
            }
            else
            {
                query = query.Where(s => s.Gpa < 3.0);
            }

            var students = await query
                .OrderBy(s => s.FullName)
                .ToListAsync();

            ViewData["Band"] = band.ToLowerInvariant();

            return View(students);
        }

        [Route("students/search")]
        public async Task<IActionResult> Searching([FromQuery] string name)
        {
            IQueryable<Student> query = _context.Students;

            if (!string.IsNullOrWhiteSpace(name))
            {
                query = query.Where(s => s.FullName.Contains(name));
            }

            var students = await query
                .OrderBy(s => s.FullName)
                .ToListAsync();

            ViewData["Name"] = name;

            return View(students);
        }
    }
}
