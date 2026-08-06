// LAB 19 — Lab ID: 31 | MIN_GRADE_LAB = 2.5 | COURSE_COUNT = 3
// Part A Answer: CoursesController.Index only needs to count the number of enrollments for each course (which is a property directly on the loaded Enrollment records), so it only needs to load the first hop. CoursesController.Details, however, must display the name of the actual student associated with each enrollment, which is a second-hop property (Enrollment.Student.FullName) that requires ThenInclude(e => e.Student) to avoid being null.

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentPortalWeb.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace StudentPortalWeb.Controllers
{
    public class EnrollmentsController : Controller
    {
        private readonly StudentPortalContext _context;

        public EnrollmentsController(StudentPortalContext context)
        {
            _context = context;
        }

        // Part B Question: why does this action need to query the database at all, when Session 17's Create() GET half for Students needed zero queries?
        // Answer: Students have no external dependencies for their creation, so the GET action just serves a blank input form. An Enrollment, however, connects existing records, so the GET action must query the database to fetch the list of available students and courses to populate the dropdown selections for the user.
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var students = await _context.Students.OrderBy(s => s.FullName).ToListAsync();
            var courses = await _context.Courses.OrderBy(c => c.CourseName).ToListAsync();

            ViewData["Students"] = students;
            ViewData["Courses"] = courses;

            return View();
        }

        // Part C Question: why is EnrollmentDate set in the controller instead of bound from the form, even though the form technically could include a hidden field for it?
        // Answer: Setting EnrollmentDate server-side in the controller prevents malicious clients from tampering with the request payload (e.g. via browser developer tools or HTTP posting utilities) to backdate or fake the enrollment date.
        // Part E Question: what real HTTP/database behaviour did you observe when the duplicate insert was attempted, and does it match what Block 5's console demo showed?
        // Answer: When the duplicate insert was attempted, the database rejected it by throwing a DbUpdateException wrapping a SqlException due to the composite unique index constraint violation (error number 2601), causing the application to return an HTTP 500 server error page, which matches the database unique index enforcement demonstrated in Block 5's console demo.
        [HttpPost]
        public async Task<IActionResult> Create(Enrollment enrollment)
        {
            ModelState.Remove(nameof(enrollment.Student));
            ModelState.Remove(nameof(enrollment.Course));
            ModelState.Remove(nameof(enrollment.EnrollmentDate));

            if (!ModelState.IsValid)
            {
                ViewData["Students"] = await _context.Students.OrderBy(s => s.FullName).ToListAsync();
                ViewData["Courses"] = await _context.Courses.OrderBy(c => c.CourseName).ToListAsync();
                return View(enrollment);
            }

            enrollment.EnrollmentDate = DateTime.Now;

            _context.Enrollments.Add(enrollment);
            await _context.SaveChangesAsync();

            var student = await _context.Students.FindAsync(enrollment.StudentId);
            var course = await _context.Courses.FindAsync(enrollment.CourseId);
            string studentName = student?.FullName ?? $"Student #{enrollment.StudentId}";
            string courseName = course?.CourseName ?? $"Course #{enrollment.CourseId}";

            TempData["Message"] = $"{studentName} was successfully enrolled in {courseName}.";

            return RedirectToAction("Details", "Students", new { id = enrollment.StudentId });
        }
    }
}
