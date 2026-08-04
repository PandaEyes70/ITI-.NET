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

        // Answers the students list route
        public async Task<IActionResult> Index()
        {
            var students = await _context.Students
                .OrderBy(s => s.FullName)
                .ToListAsync();

            return View(students);
        }

        // Answers the student detail route (constrained to int)
        public async Task<IActionResult> Details(int id)
        {
            // Guard clause: if id <= 0, return NotFound early without query
            if (id <= 0)
            {
                return NotFound();
            }

            var student = await _context.Students
                .FirstOrDefaultAsync(s => s.Id == id);

            if (student is null)
            {
                return NotFound();
            }

            return View(student);
        }

        // Answers the by-year route (constrained range 1-4)
        public async Task<IActionResult> ByYear(int year)
        {
            // Guard clause to protect behaviour
            if (year < 1 || year > 4)
            {
                return NotFound();
            }

            var students = await _context.Students
                .Where(s => s.YearOfStudy == year)
                .OrderBy(s => s.FullName)
                .ToListAsync();

            ViewData["Year"] = year;

            return View(students);
        }

        // Answers the honours route (constrained by custom honourBand constraint)
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
            else if (string.Equals(band, "pass", StringComparison.OrdinalIgnoreCase))
            {
                query = query.Where(s => s.Gpa < 3.0);
            }
            else
            {
                return NotFound();
            }

            var students = await query
                .OrderBy(s => s.FullName)
                .ToListAsync();

            ViewData["Band"] = band.ToLowerInvariant();

            return View(students);
        }

        // Query-string search action decorated with attribute route
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

        // Part C - Top students listing with personal count constraint (MAX_YEAR = 4)
        public async Task<IActionResult> Top(int count)
        {
            // Guard clause: count must be between 1 and 4 inclusive (MAX_YEAR = 4)
            if (count < 1 || count > 4)
            {
                return NotFound();
            }

            var students = await _context.Students
                .OrderByDescending(s => s.Gpa)
                .Take(count)
                .ToListAsync();

            return View("Index", students);
        }

        // Part D - Intake listing route using our custom IRouteConstraint (INTAKE_CODE = itiB)
        public async Task<IActionResult> Intake(string code)
        {
            // Guard clause: code must match "itiB" case-insensitively
            if (string.IsNullOrWhiteSpace(code) || !string.Equals(code, "itiB", StringComparison.OrdinalIgnoreCase))
            {
                return NotFound();
            }

            var students = await _context.Students
                .OrderBy(s => s.FullName)
                .ToListAsync();

            return View("Index", students);
        }

        // Part E - Your own address (MIN_GPA = 3.0)
        [Route("about/saif")]
        public async Task<IActionResult> About([FromQuery] double? minGpa)
        {
            double actualMinGpa = minGpa ?? 3.0; // Default to MIN_GPA = 3.0 for Lab ID 31

            var students = await _context.Students
                .Where(s => s.Gpa >= actualMinGpa)
                .OrderBy(s => s.FullName)
                .ToListAsync();

            return View("Index", students);
        }
    }
}
