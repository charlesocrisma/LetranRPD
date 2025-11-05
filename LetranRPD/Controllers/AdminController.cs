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

                // Update the values from the model
                Progress1 = model.Progress1,
                Progress2 = model.Progress2,
                Progress3 = model.Progress3,
                Progress4 = model.Progress4,
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

        // ===== Article Management (From AdminController1) =====
        [HttpPost]
        public IActionResult AddArticle([FromBody] ArticleModel article)
        {
            string filePath = Path.Combine(_env.WebRootPath, "js", "articles.json");
            if (!System.IO.File.Exists(filePath))
                System.IO.File.WriteAllText(filePath, "[]");

            var json = System.IO.File.ReadAllText(filePath);
            var articles = JsonSerializer.Deserialize<List<ArticleModel>>(json) ?? new();

            article.Date = DateTime.Now.ToString("MMMM dd, yyyy");
            articles.Add(article);

            var updatedJson = JsonSerializer.Serialize(articles, new JsonSerializerOptions { WriteIndented = true });
            System.IO.File.WriteAllText(filePath, updatedJson);

            return Json(new { success = true });
        }

        // ===== News Management (From AdminController1) =====
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

                string imageFileName = null;
                if (ImageFile != null)
                {
                    string uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "news");
                    Directory.CreateDirectory(uploadsFolder);

                    imageFileName = Guid.NewGuid() + Path.GetExtension(ImageFile.FileName);
                    string imagePath = Path.Combine(uploadsFolder, imageFileName);
                    using var stream = new FileStream(imagePath, FileMode.Create);
                    ImageFile.CopyTo(stream);
                }

                newsList.Add(new NewsModel
                {
                    Id = Guid.NewGuid().ToString(),
                    Title = Title,
                    Content = Content,
                    Category = Category,
                    ImagePath = imageFileName != null ? $"/uploads/news/{imageFileName}" : null,
                    Date = DateTime.Now.ToString("yyyy-MM-dd")
                });

                System.IO.File.WriteAllText(filePath, JsonSerializer.Serialize(newsList, new JsonSerializerOptions { WriteIndented = true }));
                return Json(new { success = true, message = "News added successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error adding news: {ex.Message}" });
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
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

                // Map status to numeric value
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
                        var submission = await _context.ServiceInformations
                            .Include(si => si.ServiceProgress)
                            .FirstOrDefaultAsync(si => si.ServiceId == serviceId);

                        if (submission != null && submission.ServiceProgress != null)
                        {
                            submission.ServiceProgress.Progress1 = statusValue;
                            _context.ServiceInformations.Update(submission);
                            updatedCount++;
                        }
                    }
                }

                if (updatedCount > 0)
                {
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