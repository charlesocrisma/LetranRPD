using LetranRPD.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace LetranRPD.Controllers {

 


    public class JournalController : Controller
    {
        private readonly ApplicationDBContext _context;

        public JournalController(ApplicationDBContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var journals = _context.Journalss
                .Include(j => j.Articless)
                .ToList();

            return View(journals);
        }

        [HttpGet]
        public IActionResult Compose()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Publish(JournalModel journal)
        {
            

            return RedirectToAction("Index");
        }
    }
}