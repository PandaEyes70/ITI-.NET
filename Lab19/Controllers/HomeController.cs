// =====================================================================
// HomeController — CARRIED FORWARD FROM SESSION 15 (Rule 39)
// ITI Summer Training | Web Development Using .NET | Morning Group
//
// Still unchanged. Still the control group.
// =====================================================================

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentPortalWeb.Models;
using System.Diagnostics;

namespace StudentPortalWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly StudentPortalContext _context;

        public HomeController(StudentPortalContext context) // Constructor Injection
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var students = await _context.Students
                .OrderBy(s=>s.FullName)
                .ToListAsync();

            return View(students);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}
