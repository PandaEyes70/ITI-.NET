using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Lab15_StudentPortalWeb.Models;
using Lab15_StudentPortalWeb.Services;

namespace Lab15_StudentPortalWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly StudentPortalContext _context;
        private readonly ISaifStampService _stampServiceA;
        private readonly ISaifStampService _stampServiceB;

        public HomeController(
            StudentPortalContext context,
            ISaifStampService stampServiceA,
            ISaifStampService stampServiceB)
        {
            _context = context;
            _stampServiceA = stampServiceA;
            _stampServiceB = stampServiceB;
        }

        public async Task<IActionResult> Index()
        {
            var students = await _context.Students
                .OrderBy(s => s.FullName)
                .ToListAsync();

            ViewBag.Owner = _stampServiceA.Owner;
            ViewBag.StampA = _stampServiceA.Stamp;
            ViewBag.StampB = _stampServiceB.Stamp;

            return View(students);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
