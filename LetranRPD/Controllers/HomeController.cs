using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LetranRPD.Models;

namespace LetranRPD.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDBContext _context; // ✅ Use a consistent name

        public HomeController(ILogger<HomeController> logger, ApplicationDBContext context)
        {
            _logger = logger;
            _context = context; // ✅ store in _context
        }

        public IActionResult Index()
        {
            return RedirectToAction("Home");
        }

        public IActionResult Home()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Downloads()
        {
            return View();
        }

        public IActionResult Journals()
        {
         
          
            return View(Journals);
        }

        public IActionResult Services()
        {
            return View();
        }

        public IActionResult Avail_Services()
        {
            return View();
        }

        public IActionResult Tracker()
        {
            // ✅ use _context instead of context
            var services = _context.ServiceInformations
                .Include(o => o.ServiceProgress)
                .OrderBy(i => i.ServiceId)
                .ToList();

            return View(services);
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
