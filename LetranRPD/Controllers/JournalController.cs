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
            var journals = _context.Journals
                .Include(j => j.Articles)
                .ToList();

            return View(journals);
        }

        [HttpGet]
        public IActionResult Compose()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Publish(Journal journal)
        {
            // Auto-fill metadata based on journal name
            switch (journal.JournalName.ToLower().Replace(" ", ""))
            {
                case "letranbusinessandeconomicsreviews":
                    journal.Description = "The Letran Business and Economic Review is an annual scholarly journal...";
                    journal.Publisher = "Colegio de San Juan de Letran";
                    journal.ISSN = "2704-4637";
                    journal.EISSN = "2704-4637";
                    journal.Category = "Business and Economics";
                    break;
                case "thehorizon":
                    journal.Description = "The Horizon is the official academic journal of Colegio de San Juan de Letran’s high school students...";
                    journal.Publisher = "Colegio de San Juan de Letran";
                    journal.ISSN = "2704-4645";
                    journal.EISSN = "2704-4645";
                    journal.Category = "Humanities and Social Sciences";
                    break;
                    // Add other journal templates here
            }

            journal.PublishedDate = DateTime.Now;
            _context.Journals.Add(journal);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}