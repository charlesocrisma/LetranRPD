using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using LetranRPD.Models;
using System.Text.Json;
using System.IO;
using System.Collections.Generic; // Added from old controller
using System.Linq; // Added from old controller
using System.Threading.Tasks; // Added from old controller

namespace LetranRPD.Controllers
{
    // ViewModels from both files
    public class ProgressUpdateModel
    {
        public int ServiceId { get; set; }
        public int Progress1 { get; set; }
        public int Progress2 { get; set; }
        public int Progress3 { get; set; }
        public int Progress4 { get; set; }
        public int RunCount { get; set; }
        public string? Remarks { get; set; }
    }

    public class UserViewModel
    {
        public int id { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string idno { get; set; }
        public string department { get; set; }
        public string role { get; set; }
    }

    public class DeleteViewModel
    {
        public int id { get; set; }
    }

    // Merged AdminController
    public class AdminController : Controller
    {
        private readonly ILogger<AdminController> _logger;
        private readonly ApplicationDBContext _context; // Using _context convention
        private readonly IWebHostEnvironment _env; // Added for file uploads

        // Merged constructor with all dependencies
        public AdminController(ILogger<AdminController> logger, ApplicationDBContext context, IWebHostEnvironment env)
        {
            _logger = logger;
            _context = context; // Using _context
            _env = env;
        }

        // ====== Admin Views (Combined from both files) ======
        public IActionResult Dashboard() 
        {
            var currentUser = HttpContext.Session.GetObject<Account>("account");

            if (currentUser == null || currentUser.isAdmin ==false)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        } 
        public IActionResult Research()
        {
            var currentUser = HttpContext.Session.GetObject<Account>("account");

            if (currentUser == null || currentUser.isAdmin == false)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }
        public IActionResult Create()
        {
            var currentUser = HttpContext.Session.GetObject<Account>("account");

            if (currentUser == null || currentUser.isAdmin == false)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }
        public IActionResult Submission()
        {
            var currentUser = HttpContext.Session.GetObject<Account>("account");

            if (currentUser == null || currentUser.isAdmin == false)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }
        public IActionResult Content()
        {
            var currentUser = HttpContext.Session.GetObject<Account>("account");

            if (currentUser == null || currentUser.isAdmin == false)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }  // From AdminController1
        public IActionResult Certificates()
        {
            var currentUser = HttpContext.Session.GetObject<Account>("account");

            if (currentUser == null || currentUser.isAdmin == false)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }
        public IActionResult Privacy()
        {
            var currentUser = HttpContext.Session.GetObject<Account>("account");

            if (currentUser == null || currentUser.isAdmin == false)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        public IActionResult User()
        {
            var currentUser = HttpContext.Session.GetObject<Account>("account");

            if (currentUser == null || currentUser.isAdmin == false)
            {
                return RedirectToAction("Index", "Home");
            }
            List<Account> allAccounts = _context.Accounts.ToList(); // Using _context
            return View(allAccounts);
        }

        public IActionResult Delete(string Name) 
        {
            var currentUser = HttpContext.Session.GetObject<Account>("account");

            if (currentUser == null || currentUser.isAdmin == false)
            {
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("User"); 
        }

        public IActionResult Tracking()
        {
            var currentUser = HttpContext.Session.GetObject<Account>("account");

            if (currentUser == null || currentUser.isAdmin == false)
            {
                return RedirectToAction("Index", "Home");
            }
            var serviceInfoList = _context.ServiceInformations // Using _context
                .Include(si => si.ServiceProgress)
                .OrderByDescending(si => si.ServiceProgress.AppliedDate)
                .ToList();

            return View(serviceInfoList);
        }

        // ===== User Management (Combined from both files) =====
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] UserViewModel model)
        {
            if (model == null)
                return BadRequest(new { success = false, message = "Invalid data." });

            if (await _context.Accounts.AnyAsync(a => a.Email == model.email || a.StudentNumber == model.idno)) // Using _context
                return Conflict(new { success = false, message = "Email or Student/Employee ID already exists." });

            var nameParts = model.name.Trim().Split(' ');
            string firstName = nameParts[0];
            string lastName = nameParts.Length > 1 ? string.Join(" ", nameParts.Skip(1)) : "";

            var newAccount = new Account
            {
                FirstName = firstName,
                LastName = lastName,
                Email = model.email,
                StudentNumber = model.idno,
                Level = model.department,
                isAdmin = model.role == "Admin"
            };

            _context.Accounts.Add(newAccount); // Using _context
            await _context.SaveChangesAsync(); // Using _context

            return Ok(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateUser([FromBody] UserViewModel model)
        {
            if (model == null)
                return BadRequest(new { success = false, message = "Invalid data." });

            var userToUpdate = await _context.Accounts.FindAsync(model.id); // Using _context
            if (userToUpdate == null)
                return NotFound(new { success = false, message = "User not found." });

            if (await _context.Accounts.AnyAsync(a => (a.Email == model.email || a.StudentNumber == model.idno) && a.Id != model.id)) // Using _context
                return Conflict(new { success = false, message = "Email or Student/Employee ID already exists." });

            var nameParts = model.name.Trim().Split(' ');
            string firstName = nameParts[0];
            string lastName = nameParts.Length > 1 ? string.Join(" ", nameParts.Skip(1)) : "";

            userToUpdate.FirstName = firstName;
            userToUpdate.LastName = lastName;
            userToUpdate.Email = model.email;
            userToUpdate.StudentNumber = model.idno;
            userToUpdate.Level = model.department;
            userToUpdate.isAdmin = model.role == "Admin";

            await _context.SaveChangesAsync(); // Using _context
            return Ok(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser([FromBody] DeleteViewModel model)
        {
            var userToDelete = await _context.Accounts.FindAsync(model.id); // Using _context
            if (userToDelete == null)
                return NotFound(new { success = false, message = "User not found." });

            _context.Accounts.Remove(userToDelete); // Using _context
            await _context.SaveChangesAsync(); // Using _context

            return Ok(new { success = true });
        }




        // ===== Progress Update (Using simpler logic from AdminController1) =====
        [File: AdminController.cs]

        [File: AdminController.cs]

        [File: AdminController.cs]

        [HttpPost]
        public async Task<IActionResult> UpdateProgress([FromBody] ProgressUpdateModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _logger.LogInformation($"Attempting update for ServiceId: {model.ServiceId}");

            var existingRecord = await _context.ServiceProgresses.AsNoTracking() // Using _context
                .FirstOrDefaultAsync(p => p.ServiceId == model.ServiceId);

            if (existingRecord == null)
                return NotFound("ServiceProgress record not found.");

            // Manually set properties on the original object
            // This is necessary because the original object was loaded AsNoTracking
            // We must re-attach it with its original Id
            var updatedProgress = new ServiceProgress
            {
                Id = existingRecord.Id, // <-- Preserve the original Primary Key
                ServiceId = existingRecord.ServiceId, // Preserve the Foreign Key
                AppliedDate = existingRecord.AppliedDate, // Preserve original date
                Progress1files = existingRecord.Progress1files, // Preserve files
                Progress2files = existingRecord.Progress2files, // Preserve files
                Progress3files = existingRecord.Progress3files, // Preserve files
                Progress4files = existingRecord.Progress4files, // Preserve files

                // Update the values from the model
                Progress1 = model.Progress1,
                Progress2 = model.Progress2,
                Progress3 = model.Progress3,
                Progress4 = model.Progress4,
                RunCount = model.RunCount,
                Remarks = model.Remarks
            };

            try
            {
                _context.ServiceProgresses.Update(updatedProgress); // Using _context
                await _context.SaveChangesAsync(); // Using _context
                return Json(new { success = true, message = "Progress updated successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during update for ServiceId: {ServiceId}", model.ServiceId);
                return StatusCode(500, "An error occurred while saving.");
            }
        }
        }

        // ====== Save Journal ======
        [HttpPost]
        public async Task<IActionResult> SaveJournal(string JournalName, string Volume, string SubVolume)
        {
            if (string.IsNullOrWhiteSpace(JournalName) || string.IsNullOrWhiteSpace(Volume) || string.IsNullOrWhiteSpace(SubVolume))
            {
                return Json(new { success = false, message = "⚠️ Please fill in all required fields." });
            }

            try
            {
                // ✅ Prevent duplicate Journal + Volume + SubVolume in the database
                bool isDuplicate = await _context.Journalss.AnyAsync(j =>
                    j.JournalName.ToLower() == JournalName.ToLower() &&
                    j.Volume.ToLower() == Volume.ToLower() &&
                    j.SubVolume.ToLower() == SubVolume.ToLower()
                );

                if (isDuplicate)
                {
                    return Json(new { success = false, message = "❌ Duplicate SubVolume detected for this Journal and Volume." });
                }

                // ✅ Create a new journal entry
                var newJournal = new JournalModel
                {
                    JournalName = JournalName,
                    Volume = Volume,
                    SubVolume = SubVolume
            
                };

                // ✅ Save to the database
                _context.Journalss.Add(newJournal);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "✅ Journal published successfully!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving journal");
                return Json(new { success = false, message = $"⚠️ Error saving journal: {ex.Message}" });
            }
        }

        // ===== Article Management (From AdminController1) =====
        [HttpPost]
        public async Task<IActionResult> AddArticle([FromBody] Article article)
        {
            if (article == null)
                return Json(new { success = false, message = "Invalid article data." });

            try
            {
                var newArticle = new Article
                {
                    JournalName = article.JournalName,
                    Volume = article.Volume,
                    SubVolume = article.SubVolume,
                    Title = article.Title,
                    Authors = article.Authors,
                    Abstract = article.Abstract,
                    Category = article.Category,
                    Date = string.IsNullOrWhiteSpace(article.Date)
                        ? DateTime.Now.ToString("MMMM dd, yyyy")
                        : article.Date
                };

                _context.Articless.Add(newArticle);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Article added successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding article");
                return Json(new { success = false, message = "Error adding article." });
            }
        }
        // ====== Content.cshtml fetch articles from Database ======
        [HttpGet]
        public async Task<IActionResult> GetArticlesAdmin()
        {
            var articles = await _context.Articless.ToListAsync();
            return Json(articles);
        }
        [HttpGet]
        public async Task<IActionResult> GetJournals()
        {
            try
            {
                // Fetch all journals from the database, ordered by latest
                var journals = await _context.Journalss
                    .OrderByDescending(j => j.Id)
                    .Select(j => new
                    {
                        j.Id,
                        j.JournalName,
                        j.Volume,
                        j.SubVolume
                    })
                    .ToListAsync();

                // ✅ Unified format for frontend script
                return Json(new { success = true, data = journals });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching journals");
                return Json(new { success = false, message = "Error retrieving journals." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetArticles()
        {
            try
            {
                var articles = await _context.Articless
                    .OrderByDescending(a => a.Id)
                    .Select(a => new
                    {
                        a.Id,
                        a.Title,
                        a.Authors,
                        a.Date,
                        a.Category,
                        a.Abstract,
                        a.JournalName,
                        a.Volume,
                        a.SubVolume
                    })
                    .ToListAsync();

                // ✅ Match GetJournals structure
                return Json(new { success = true, data = articles });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching articles");
                return Json(new { success = false, message = "Error retrieving articles." });
            }
        }


        // ====== Edit Article ======
        [HttpPost]
        public async Task<IActionResult> EditArticle([FromBody] Article updated)
        {
            if (updated == null)
                return Json(new { success = false, message = "Invalid article data." });

            var existing = await _context.Articless
                .FirstOrDefaultAsync(a => a.Id == updated.Id);

            if (existing == null)
                return Json(new { success = false, message = "Article not found." });

            existing.Title = updated.Title;
            existing.Authors = updated.Authors;
            existing.Abstract = updated.Abstract;
            existing.Category = updated.Category;
            existing.JournalName = updated.JournalName;
            existing.Volume = updated.Volume;
            existing.SubVolume = updated.SubVolume;

            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Article updated successfully." });
        }

        [HttpGet]
        public async Task<IActionResult> GetArticleById(int id)
        {
            var article = await _context.Articless
                .Where(a => a.Id == id)
                .Select(a => new {
                    a.Id,
                    a.Title,
                    a.Authors,
                    a.Abstract,
                    a.Category,
                    a.JournalName,
                    a.Volume,
                    a.SubVolume
                })
                .FirstOrDefaultAsync();

            if (article == null)
                return Json(new { success = false, message = "Article not found." });

            return Json(article);
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteArticle(int id)
        {
            try
            {
                var article = await _context.Articless.FindAsync(id);
                if (article == null)
                    return Json(new { success = false, message = "Article not found." });

                _context.Articless.Remove(article);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Article deleted successfully!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting article");
                return Json(new { success = false, message = "Error deleting article." });
            }
        }

        // ====== Display Journals with Articles ======








        [HttpPost]
        public IActionResult AddNews(IFormFile ImageFile, string Title, string Content, string Category)
        {
            try
            {
                string filePath = Path.Combine(_env.WebRootPath, "js", "news.json");
                if (!System.IO.File.Exists(filePath))
                    System.IO.File.WriteAllText(filePath, "[]");

                var json = System.IO.File.ReadAllText(filePath);
                var newsList = JsonSerializer.Deserialize<List<NewsModel>>(json) ?? new List<NewsModel>();

                // Handle image upload
                string imageFileName = null;
                if (ImageFile != null && ImageFile.Length > 0)
                {
                    string uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "news");
                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    imageFileName = Guid.NewGuid() + Path.GetExtension(ImageFile.FileName);
                    string imagePath = Path.Combine(uploadsFolder, imageFileName);
                    using (var stream = new FileStream(imagePath, FileMode.Create))
                    {
                        ImageFile.CopyTo(stream);
                    }
                }

                var nextId = newsList.Any() ? newsList.Max(n => n.Id) + 1 : 1;
                var newNews = new NewsModel
                {
                    Id = nextId,
                    Title = Title,
                    Content = Content,
                    Category = Category,
                    ImagePath = imageFileName != null ? $"/uploads/news/{imageFileName}" : null,
                    Date = DateTime.Now.ToString("yyyy-MM-dd")
                };

                newsList.Add(newNews);

                var updatedJson = JsonSerializer.Serialize(newsList, new JsonSerializerOptions { WriteIndented = true });
                System.IO.File.WriteAllText(filePath, updatedJson);

                return Json(new { success = true, message = "News added successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error adding news: {ex.Message}" });
            }
        }



        // ===== Edit News =====
        [HttpPost]
        public IActionResult EditNews(IFormFile ImageFile, string Id, string Title, string Content, string Category)
        {
            try
            {
                string filePath = Path.Combine(_env.WebRootPath, "js", "news.json");
                if (!System.IO.File.Exists(filePath))
                    return Json(new { success = false, message = "News file not found." });

                var json = System.IO.File.ReadAllText(filePath);
                var newsList = JsonSerializer.Deserialize<List<NewsModel>>(json) ?? new List<NewsModel>();
                int parsedId = int.Parse(Id);
                var news = newsList.FirstOrDefault(n => n.Id == parsedId);

                if (news == null)
                    return Json(new { success = false, message = "News not found." });

                // Update text fields
                news.Title = Title;
                news.Content = Content;
                news.Category = Category;

                // Handle optional image update
                if (ImageFile != null && ImageFile.Length > 0)
                {
                    string uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "news");
                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    string imageFileName = Guid.NewGuid() + Path.GetExtension(ImageFile.FileName);
                    string imagePath = Path.Combine(uploadsFolder, imageFileName);

                    using (var stream = new FileStream(imagePath, FileMode.Create))
                    {
                        ImageFile.CopyTo(stream);
                    }

                    news.ImagePath = $"/uploads/news/{imageFileName}";
                }

                // Save changes
                var updatedJson = JsonSerializer.Serialize(newsList, new JsonSerializerOptions { WriteIndented = true });
                System.IO.File.WriteAllText(filePath, updatedJson);

                return Json(new { success = true, message = "News updated successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error editing news: {ex.Message}" });
            }
        }

        // ===== Delete News =====
        [HttpPost]
        public IActionResult DeleteNews([FromBody] NewsModel news)
        {
            try
            {
                string filePath = Path.Combine(_env.WebRootPath, "js", "news.json");
                if (!System.IO.File.Exists(filePath))
                    return Json(new { success = false, message = "News file not found." });

                var json = System.IO.File.ReadAllText(filePath);
                var newsList = JsonSerializer.Deserialize<List<NewsModel>>(json) ?? new List<NewsModel>();

                var existing = newsList.FirstOrDefault(n => n.Id == news.Id);
                if (existing == null)
                    return Json(new { success = false, message = "News not found." });

                newsList.Remove(existing);
                System.IO.File.WriteAllText(filePath, JsonSerializer.Serialize(newsList, new JsonSerializerOptions { WriteIndented = true }));

                return Json(new { success = true, message = "News deleted successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error deleting news: {ex.Message}" });
            }
        }
        [HttpPost]
        public async Task<IActionResult> AddJournal([FromBody] JournalModel model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.JournalName))
                return Json(new { success = false, message = "Invalid journal data." });

            // Prevent duplicate Journal + Volume + SubVolume
            bool exists = await _context.Journalss.AnyAsync(j =>
                j.JournalName == model.JournalName &&
                j.Volume == model.Volume &&
                j.SubVolume == model.SubVolume);

            if (exists)
                return Json(new { success = false, message = "A journal with this name, volume, and subvolume already exists." });

            try
            {
                _context.Journalss.Add(model);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Journal added successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding journal");
                return Json(new { success = false, message = "Error adding journal." });
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}