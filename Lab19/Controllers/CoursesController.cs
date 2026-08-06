// =====================================================================
// CoursesController — SESSION PROJECT (Style Guide Rule 20/35/39/40)
// ITI Summer Training | Web Development Using .NET | Morning Group
// Session 19 — Block 4: Displaying the Other Direction
//
// A brand-new file, but nothing in it is a brand-new IDEA. Every shape
// here — inject the context, write an async action, query, Include,
// return View(list-or-object) — is something you have written at least
// twice before, in StudentsController. Today proves those shapes were
// never really about Student; they were about "a controller with a
// database behind it," and any entity fits.
//
// Its two views, Views/Courses/Index.cshtml and Details.cshtml, are
// PRE-WRITTEN — Views were Session 18's taught topic, not today's, so
// today's TODOs stay on the C#/LINQ side, same as Rule 26 already
// applied to Session 18's own Instructors views.
// =====================================================================

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentPortalWeb.Models;
using System.Linq;
using System.Threading.Tasks;

namespace StudentPortalWeb.Controllers
{
    public class CoursesController : Controller
    {
        // TODO 4 (part one): Add the SAME field-plus-constructor shape
        //         every other controller in this project already has —
        //         a private readonly field holding the context, filled
        //         in by a constructor parameter of the same type. Copy
        //         nothing; type it, and notice it is now the fourth time
        //         you have written this exact shape this course.

        private readonly StudentPortalContext _context;

        public CoursesController(StudentPortalContext context)
        {
            _context = context;
        }

        // TODO 4 (part two): Write an action called Index, taking no
        //         parameters, async, returning the same interface every
        //         action in this project returns. Query all Courses,
        //         ordered by CourseName, and — this is today's new
        //         part — Include each course's Instructor (a course
        //         with no instructor name showing would be a strange
        //         page) AND Include each course's Enrollments, so the
        //         view can show how many students are taking it without
        //         a second query per row. Await the query into a list
        //         and return it as the view's model.

        public async Task<IActionResult> Index()
        {
            var courses = await _context.Courses
                .Include(c => c.Instructor)
                .Include(c => c.Enrollments)
                .ToListAsync();
            return View(courses);
        }

        // TODO 4 (part three): Write an action called Details, taking
        //         one whole-number parameter named id, async, same
        //         return interface. Query Courses for the one matching
        //         id, Including its Instructor, and Including its
        //         Enrollments — then, on that SAME Enrollments include,
        //         chain a ThenInclude reaching each enrollment's
        //         Student. This is the mirror image of TODO 3 in
        //         StudentsController: there you walked Student →
        //         Enrollments → Course; here you walk Course →
        //         Enrollments → Student. Same relationship, opposite
        //         direction, and neither direction had to be told the
        //         other one exists.
        //         Use FirstOrDefaultAsync, and if nothing matches,
        //         return the same "there is nothing here" helper every
        //         other Details action in this project already uses.
        //         Otherwise return the course as the view's model.
        public async Task<IActionResult> Details(int id)
        {
            var course = await _context.Courses
                .Include(c => c.Instructor)
                .Include(c => c.Enrollments)
                    .ThenInclude(e => e.Student)
                .FirstOrDefaultAsync(c => c.Id == id);
            if (course is null)
                return NotFound();

            return View(course);
        }
    }
}
