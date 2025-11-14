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
        public int Progress5 { get; set; }
        public int Progress6 { get; set; }
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

    public class BulkUpdateRequest
    {
        public string Ids { get; set; }
        public string Status { get; set; }
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

            if (currentUser == null || currentUser.isAdmin == false)
            {
                return RedirectToAction("Index", "Home");
            }

            // ✅ 1. Count total students (non-admin users)
            int totalStudents = _context.Accounts.Count(a => a.isAdmin == false);

            // ✅ 2. Count total admins
            int totalAdmins = _context.Accounts.Count(a => a.isAdmin == true);

            // ✅ 3. Count pending / in-progress services
            int totalPending = _context.ServiceProgresses
                .Count(p => p.Progress4 == 1 || p.Progress4 == 0);

            // ✅ 4. Group service counts by ServiceType
            var serviceCounts = _context.ServiceInformations
                .GroupBy(s => s.ServiceType)
                .Select(g => new
                {
                    ServiceType = g.Key,
                    Count = g.Count()
                })
                .ToList();

            // ✅ 5. Pass data to the View
            ViewBag.ServiceCounts = serviceCounts.ToDictionary(s => s.ServiceType, s => s.Count);
            ViewBag.TotalStudents = totalStudents;
            ViewBag.TotalAdmins = totalAdmins;
            ViewBag.TotalPending = totalPending;

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
        }
        public IActionResult Reports()
        {
            var currentUser = HttpContext.Session.GetObject<Account>("account");

            if (currentUser == null || currentUser.isAdmin == false)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }
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

        // ===== Progress Update (With File Handling) =====
        [HttpPost]
        public async Task<IActionResult> UpdateProgress(ProgressUpdateModel model, IFormFileCollection files)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _logger.LogInformation($"Attempting update for ServiceId: {model.ServiceId} with {files?.Count ?? 0} file(s).");

            var existingRecord = await _context.ServiceProgresses.AsNoTracking()
                .FirstOrDefaultAsync(p => p.ServiceId == model.ServiceId);

            if (existingRecord == null)
                return NotFound("ServiceProgress record not found.");

            var savedFileNames = new List<string>();
            if (files != null && files.Count > 0)
            {
                // Define the upload path: e.g., wwwroot/uploads/servicefiles/{ServiceId}

                var directory = Path.Combine("wwwroot", "uploads", "Service_" + model.ServiceId,"AdminToStudent");
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), directory);
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);
                

                foreach (var file in files)
                {
                    // Create a unique file name (GUID) to prevent overwrites
                    var safeFileName = Path.GetFileName(file.FileName);
                    var uniqueFileName = $"{DateTime.Now.ToString("yyyyMMddHHmm")}_{safeFileName}";
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                    savedFileNames.Add(uniqueFileName);
                }
            }
            var updatedProgress = new ServiceProgress
            {
                Id = existingRecord.Id,
                ServiceId = existingRecord.ServiceId,
                AppliedDate = existingRecord.AppliedDate,
                Progress1files = existingRecord.Progress1files,
                Progress2files = existingRecord.Progress2files,
                Progress3files = existingRecord.Progress3files,
                Progress4files = existingRecord.Progress4files,
                Progress5files = existingRecord.Progress5files,
                Progress6files = existingRecord.Progress6files,

                // Update the values from the model
                Progress1 = model.Progress1,
                Progress2 = model.Progress2,
                Progress3 = model.Progress3,
                Progress4 = model.Progress4,
                Progress5 = model.Progress5,
                Progress6 = model.Progress6,
                RunCount = model.RunCount,
                Remarks = model.Remarks,
                AdminToStudentFiles = savedFileNames.Any() ? savedFileNames : existingRecord.AdminToStudentFiles

            };
            try
            {
                _context.ServiceProgresses.Update(updatedProgress);
                await _context.SaveChangesAsync();

                // Clear files from temp storage in the client-side array after successful save
                // (This is done in the JavaScript's success block, but mentioned here for completeness)

                return Json(new { success = true, message = "Progress and files processed successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during progress update for ServiceId: {ServiceId}", model.ServiceId);
                return StatusCode(500, "An error occurred while saving progress.");
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

        // AUDIT LOG 
        [HttpGet]
        public async Task<IActionResult> GetAuditLogs()
        {
            var data = await _context.ServiceInformations
                .Select(s => new
                {
                    service = s.ServiceType,
                    student = s.ContactPerson,
                    idNo = s.StudentNumber,

                    department = _context.Accounts
                        .Where(a => a.StudentNumber == s.StudentNumber)
                        .Select(a => a.Level)
                        .FirstOrDefault(),

                    // Most recent applied date
                    appliedDate = _context.ServiceProgresses
                        .Where(p => p.ServiceId == s.ServiceId)
                        .OrderByDescending(p => p.AppliedDate)
                        .Select(p => p.AppliedDate)
                        .FirstOrDefault(),

                    // Latest RunCount (attempts)
                    attempts = _context.ServiceProgresses
                        .Where(p => p.ServiceId == s.ServiceId)
                        .OrderByDescending(p => p.AppliedDate)
                        .Select(p => p.RunCount)
                        .FirstOrDefault(),

                    // ✅ Result based on attempts
                    result = _context.ServiceProgresses
                        .Where(p => p.ServiceId == s.ServiceId)
                        .OrderByDescending(p => p.AppliedDate)
                        .Select(p =>

                            p.RunCount == 4 ? "Failed" :
                            p.Progress4 == 2 ? "Success" :
                    (p.Progress4 == 1 || p.Progress4 == 0) ? "In Progress" :
                            "Success")
                        .FirstOrDefault()

                })
                .ToListAsync();

            return Json(data);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BulkUpdateStatus([FromBody] BulkUpdateRequest request)
        {
            var currentUser = HttpContext.Session.GetObject<Account>("account");
            if (currentUser == null || currentUser.isAdmin == false)
                return Json(new { success = false, message = "Unauthorized" });

            if (string.IsNullOrEmpty(request?.Ids))
                return Json(new { success = false, message = "No submissions selected" });

            try
            {
                var idList = request.Ids.Split(',')
                    .Where(id => !string.IsNullOrEmpty(id.Trim()))
                    .ToList();

                // This converts the string status (e.g., "2" or "complete") into a number
                int statusValue = request.Status switch
                {
                    "1" or "processing" => 1,
                    "2" or "complete" => 2,
                    "3" or "failed" => 3,
                    "4" or "archived" => 4,
                    _ => -1
                };

                if (statusValue == -1)
                    return Json(new { success = false, message = "Invalid status" });

                int updatedCount = 0;
                foreach (var idStr in idList)
                {
                    if (int.TryParse(idStr.Trim(), out int serviceId))
                    {
                        // Get the submission AND its progress.
                        // Entity Framework will track these objects for changes.
                        var submission = await _context.ServiceInformations
                            .Include(si => si.ServiceProgress)
                            .FirstOrDefaultAsync(si => si.ServiceId == serviceId);

                        if (submission != null && submission.ServiceProgress != null)
                        {
                            // === THIS IS THE CORRECT LOGIC ===
                            // We modify the properties based on the status value.
                            // EF will automatically detect these changes.
                            switch (statusValue)
                            {
                                case 1: // Processing
                                    submission.ServiceProgress.Progress4 = 1;
                                    break;

                                case 2: // Complete
                                        // Sets "Evaluation" (Progress4) to 'complete'
                                    submission.ServiceProgress.Progress4 = 2;
                                    break;

                                case 3: // Failed
                                    submission.ServiceProgress.RunCount = 4;
                                    submission.ServiceProgress.Progress4 = 3;
                                    break;

                                case 4: // Archived
                                    submission.ServiceProgress.Progress4 = 4;
                                    break;
                            }
                            updatedCount++;
                        }
                    }
                }

                // Only save to the database if we actually changed anything
                if (updatedCount > 0)
                {
                    // This one call saves all the changes EF has tracked.
                    await _context.SaveChangesAsync();
                    _logger.LogInformation($"Bulk update completed: {updatedCount} submissions updated to status {statusValue}");
                }

                return Json(new
                {
                    success = true,
                    message = $"Updated {updatedCount} submission(s) successfully"
                });
            }
            catch (Exception ex)
            {
                // Log the error and return a helpful message
                _logger.LogError(ex, "Error during bulk status update");
                return Json(new
                {
                    success = false,
                    message = "An error occurred while updating statuses: " + ex.Message
                });
            }
        }



    }
}