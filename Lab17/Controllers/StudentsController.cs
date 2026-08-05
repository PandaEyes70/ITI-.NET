// LAB 17 — Lab ID: 31 | MIN_GPA_EDIT = 2.3 | MAX_YEAR_EDIT = 3
// Answer to Q: The Create() GET action is not async because it does not perform any asynchronous I/O operations
// (such as querying a database or reading a file) and simply returns the default empty form View(),
// whereas the POST version performs asynchronous database modifications via AddAsync and SaveChangesAsync which must be awaited.


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
        // the field, the constructor, the
        // injection. Nobody writes `new StudentPortalContext()` here.
        private readonly StudentPortalContext _context;

        public StudentsController(StudentPortalContext context)
        {
            _context = context;
        }

        // ============================================================
        // BLOCK 1 — ONE ACTION, SEVERAL DIFFERENT ANSWERS
        // ============================================================

        // TODO 1: Write one brand-new action called Demo that takes a
        //         single whole-number parameter named id, and answers
        //         with a DIFFERENT kind of result depending on what it
        //         is handed. Its return type is the same interface every
        //         action in this file already returns — the one that
        //         means "some sort of web answer, I am not saying which
        //         yet". Do not mark it async: it touches no database, so
        //         there is nothing to await, and marking it async anyway
        //         would be a promise the method does not keep.
        //         Inside, in this exact order:
        //           1. If id is zero, answer with the helper that means
        //              "there is nothing here" — the same one Details
        //              already uses further down this file.
        //           2. If id is one, answer with the helper that hands
        //              back a piece of plain text rather than a page.
        //              Pass it the sentence: I am plain text, not a page.
        //           3. If id is two, answer with the helper that turns an
        //              object into JSON. Pass it a small anonymous object
        //              with two properties, Message and Id, holding the
        //              text This one is JSON and the value of id.
        //           4. If id is three, answer with the helper that sends
        //              the browser somewhere else entirely, naming the
        //              listing action of this same controller as a string.
        //           5. For anything else, answer with the helper that
        //              renders a page, passing it no model at all.
        //         Every one of those five helpers is a method you inherit
        //         from the Controller base class. You have not added a
        //         single using directive to reach them, and Block 1
        //         explains why that is the whole point of inheriting.

        public IActionResult Demo(int id)
        {
            if (id == 0) return NotFound();
            if (id == 1) return Content("I'm a plain text , not a page.");
            if (id == 2) return Json(new { Message = "This is a JSON" , Id = id });
            if (id == 3) return RedirectToAction("Index");
            return View();
        }

        // ============================================================
        // BLOCK 2 — WHERE DOES A PARAMETER COME FROM?
        // ============================================================

        // TODO 2: Write a second new action called Echo whose entire job
        //         is to report where its own arguments came from. Give it
        //         three parameters, each marked with the attribute that
        //         names its source explicitly:
        //           - a whole number called id, marked as coming from the
        //             route,
        //           - a piece of text called note, marked as coming from
        //             the query string,
        //           - a piece of text called agent, marked as coming from
        //             a request header, and told which header by name —
        //             the standard one that identifies the browser,
        //             spelled with a hyphen in the middle.
        //         Return the plain-text helper from TODO 1, handing back
        //         one string that prints all three values with a label
        //         each, separated by pipe characters, so the whole answer
        //         fits on one line of the browser.
        //         ⚠️ Do NOT make the header parameter's name match the
        //         header's name. It cannot: the header has a hyphen in it
        //         and a C# identifier may not. That mismatch is exactly
        //         why the attribute takes a name argument, and Block 2
        //         shows you what happens when you leave it out.

        public IActionResult Echo(
            [FromRoute] int id,
            [FromQuery] string note,
            [FromHeader(Name = "User-Agent")] string agent
            )
        {
            return Content($"id (route) = {id} | note (query) = {note} | agent (header) = {agent}");
        }

        // ============================================================
        // BLOCK 3 — THE FORM (carried-forward actions resume below)
        // ============================================================

        // TODO 3: Write the FIRST half of adding a student: an action
        //         called Create that takes no parameters at all and does
        //         nothing except render its page. Nothing is saved here.
        //         Nothing is read here. Its only job is to put an empty
        //         form on screen, which is why it is three lines long and
        //         why the view file for it already exists, pre-written.
        //         Mark it with the attribute naming the HTTP verb that
        //         means "I am fetching something, I am changing nothing".
        //         The verb attribute is not decoration — TODO 4 adds a
        //         SECOND method with the SAME name, and without the verbs
        //         the framework cannot tell which of the two you meant.
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // TODO 4: Write the SECOND half: another action, also called
        //         Create, but taking ONE parameter — a whole Student
        //         object. Mark this one with the attribute naming the
        //         verb that means "I am sending you something to change".
        //         For now the body does only one thing: hand the student
        //         it received straight back to the same view, so you can
        //         see on screen that the object really did arrive fully
        //         filled in. Saving comes in TODO 7; today's order is
        //         deliberately prove-it-arrived first, save second.
        //         ⚠️ Two methods, same name, same class, different
        //         parameter lists — that is method overloading from
        //         Session 9, and it is doing real work here.
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
            var student = await _context.Students
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

        // Note again: the URL says "search", the
        // METHOD is called Searching, and the VIEW is therefore
        // Views/Students/Searching.cshtml. Attribute routing frees the URL
        // from the method name. It does not free the view from it.
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

        // ============================================================
        // BLOCKS 4 AND 5 — the two lines that turn a form that always
        // saves into a form that decides.
        //
        // ============================================================

        // TODO 6: (Block 4 — do TODO 5 first, in the Models file.) Inside
        //         the POST version of Create, before anything else, add a
        //         guard clause. Ask the built-in property that holds the
        //         verdict on the incoming data whether it is valid, and if
        //         it is NOT, hand the student straight back to the view
        //         and return immediately.
        //         Note carefully what you are NOT writing: not one `if`
        //         per rule, no length checks, no null checks. You are
        //         reading a verdict that was already reached, before your
        //         method ran, by the framework, using the attributes from
        //         TODO 5. Block 4 proves that by putting a breakpoint on
        //         this line and showing the verdict already sitting there.
        




        // ============================================================
        // LAB 17 - PARTS B, C, AND E
        // Trainee: Saif Elden Khaled Nazmy Lotfy | Lab ID: 31
        // MIN_GPA_EDIT = 2.3 | MAX_YEAR_EDIT = 3
        // ============================================================

        // Part B - The GET half: show the real row
        // Question: why does this action load the student from the database at all, instead of just rendering an empty form the way Create()'s GET half does?
        // Answer: It loads the existing student from the database to pre-populate the form inputs with their current values,
        // allowing the user to see what they are editing, whereas Create is for a new record and starts with an empty form.
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var student = await _context.Students
                .FirstOrDefaultAsync(s => s.Id == id);

            if (student is null)
            {
                return NotFound();
            }

            return View(student);
        }

        // Part C - The POST half: save the change, properly
        // Question: what would happen to this action's signature if you renamed its id parameter to studentId and left the route pattern saying {id}?
        // Answer: The model binder matches parameters by name. If the parameter is renamed to studentId but the route specifies {id},
        // the binder will search route values for "studentId", find nothing, and silently default studentId to 0,
        // which would fail the guard clause (studentId != student.Id).
        
        [HttpPost]
        public async Task<IActionResult> Edit(int id, Student student)
        {
            // Guard clause: validate id match and ModelState
            if (id != student.Id || !ModelState.IsValid)
            {
                return View(student);
            }

            var existingStudent = await _context.Students
                .FirstOrDefaultAsync(s => s.Id == id);

            if (existingStudent is null)
            {
                return NotFound();
            }

            // Update properties
            existingStudent.FullName = student.FullName;
            existingStudent.YearOfStudy = student.YearOfStudy;
            existingStudent.Gpa = student.Gpa;

            _context.Students.Update(existingStudent);
            await _context.SaveChangesAsync();

            TempData["Message"] = $"{student.FullName} was updated successfully.";

            // Part E: Redirect to SaifConfirmed confirmation action
            return RedirectToAction("SaifConfirmed", new { id = student.Id });
        }

        // Part E - One action that carries your own name
        // Question: why must this action reload the student from the database rather than simply reusing the student object the POST action already had in memory?
        // Answer: Reloading from the database guarantees that we display the actual committed state of the data in the database,
        // verifying that the transaction was successfully persisted.
        [HttpGet]
        public async Task<IActionResult> SaifConfirmed(int id)
        {
            var student = await _context.Students
                .FirstOrDefaultAsync(s => s.Id == id);

            if (student is null)
            {
                return NotFound();
            }

            return View("Details", student);
        }
    }
}
