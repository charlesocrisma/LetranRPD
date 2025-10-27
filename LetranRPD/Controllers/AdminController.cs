using LetranRPD.Models; // Assume your models are here
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Diagnostics;
using System.IO;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting; // Needed for IWebHostEnvironment
using System; // Needed for Guid, DateTime, StringComparison

namespace LetranRPD.Controllers;

// NOTE: It is best practice to move these models to a separate 'Models' folder.

// Placeholder Models (You MUST ensure these are defined in your project's Models folder)
// If they are not in LetranRPD.Models, you will need to define them properly.

// Assuming these models exist in LetranRPD.Models, we'll keep placeholders for local JSON usage
public class NewsModel
{
    public string Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public string Category { get; set; }
    public string ImagePath { get; set; }
    public string Date { get; set; }
}

public class ArticleModel
{
    public string JournalName { get; set; }
    public string Volume { get; set; }
    public string SubVolume { get; set; }
    public string Title { get; set; }
    public string Authors { get; set; }
    public string Abstract { get; set; }
    public string Category { get; set; }
    public string Date { get; set; }
}

public class JournalModel
{
    public string JournalName { get; set; }
    public string Volume { get; set; }
    public string SubVolume { get; set; }
    public List<ArticleModel>? Articles { get; set; } // Added for Journals()
}

// Data Model for Updating Service Progress
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


// Primary Controller Definition
public class AdminController : Controller
{
    private readonly ILogger<AdminController> _logger;
    private readonly ApplicationDBContext context;
    private readonly IWebHostEnvironment _env;

    // Consolidated Constructor with all dependencies
    public AdminController(ILogger<AdminController> logger, ApplicationDBContext context, IWebHostEnvironment env)
    {
        _logger = logger;
        this.context = context;
        _env = env;
    }

    //
    // ====== Admin Views ======
    //
    public IActionResult Dashboard() => View();
    public IActionResult Research() => View();
    public IActionResult Create() => View();
    public IActionResult Submission() => View();
    public IActionResult Privacy() => View();
    public IActionResult Content() => View();

    // Redefining User() and Delete(string Name) for clarity and to remove 'new'
    public IActionResult User()
    {
        List<Account> allAccounts = context.Accounts.ToList();
        return View(allAccounts);
    }

    // This looks like a legacy/incomplete method, redirecting as per original logic.
    public IActionResult Delete(string Name) => RedirectToAction("User");

    public IActionResult Tracking()
    {
        // ServiceInformations and ServiceProgress need to be defined in your EF Core models
        var serviceInfoList = context.ServiceInformations
                                     .Include(si => si.ServiceProgress)
                                     .OrderByDescending(si => si.ServiceProgress.AppliedDate)
                                     .ToList();
        return View(serviceInfoList);
    }

    public IActionResult Certificates() => View();

    //
    // ====== Journal Management (JSON File) ======
    //

    [HttpPost]
    public IActionResult SaveJournal(string JournalName, string Volume, string SubVolume)
    {
        string jsonPath = Path.Combine(_env.WebRootPath, "js", "journal-data.json");
        List<JournalModel> journals = new();

        if (System.IO.File.Exists(jsonPath))
        {
            var existingJson = System.IO.File.ReadAllText(jsonPath);
            if (!string.IsNullOrWhiteSpace(existingJson))
            {
                journals = JsonSerializer.Deserialize<List<JournalModel>>(existingJson) ?? new List<JournalModel>();
            }
        }

        // Prevent duplicate subVolume for the same journal and volume
        bool isDuplicate = journals.Any(j =>
            j.JournalName.Equals(JournalName, StringComparison.OrdinalIgnoreCase) &&
            j.Volume.Equals(Volume, StringComparison.OrdinalIgnoreCase) &&
            j.SubVolume.Equals(SubVolume, StringComparison.OrdinalIgnoreCase));

        if (isDuplicate)
        {
            return Json(new { success = false, message = "❌ Duplicate SubVolume detected for this Journal and Volume." });
        }

        journals.Add(new JournalModel
        {
            JournalName = JournalName,
            Volume = Volume,
            SubVolume = SubVolume
        });

        var newJson = JsonSerializer.Serialize(journals, new JsonSerializerOptions { WriteIndented = true });
        System.IO.File.WriteAllText(jsonPath, newJson);

        TempData["Message"] = "✅ Journal published successfully!";
        return Json(new { success = true });
    }

    public IActionResult Journals()
    {
        var journalsPath = Path.Combine(_env.WebRootPath, "js", "journal-data.json");
        var articlesPath = Path.Combine(_env.WebRootPath, "js", "articles.json");

        var journalsJson = System.IO.File.Exists(journalsPath) ? System.IO.File.ReadAllText(journalsPath) : "[]";
        var articlesJson = System.IO.File.Exists(articlesPath) ? System.IO.File.ReadAllText(articlesPath) : "[]";

        var journals = JsonSerializer.Deserialize<List<JournalModel>>(journalsJson,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<JournalModel>();

        var articles = JsonSerializer.Deserialize<List<ArticleModel>>(articlesJson,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<ArticleModel>();

        foreach (var journal in journals.Where(j => j != null))
        {
            journal.Articles = articles
                .Where(a =>
                    string.Equals(a.JournalName?.Trim(), journal.JournalName?.Trim(), StringComparison.OrdinalIgnoreCase) &&
                    string.Equals(a.Volume?.Trim(), journal.Volume?.Trim(), StringComparison.OrdinalIgnoreCase) &&
                    string.Equals(a.SubVolume?.Trim(), journal.SubVolume?.Trim(), StringComparison.OrdinalIgnoreCase)
                )
                .ToList();
        }

        return View("~/Views/Admin/Content.cshtml", journals);
    }

    //
    // ====== Article Management (JSON File) ======
    //

    [HttpPost]
    public IActionResult AddArticle([FromBody] ArticleModel article)
    {
        try
        {
            string filePath = Path.Combine(_env.WebRootPath, "js", "articles.json");

            if (!System.IO.File.Exists(filePath))
                System.IO.File.WriteAllText(filePath, "[]");

            var json = System.IO.File.ReadAllText(filePath);
            var articles = JsonSerializer.Deserialize<List<ArticleModel>>(json) ?? new List<ArticleModel>();

            if (article == null)
                return Json(new { success = false, message = "Article data is null." });

            // Force the date to a readable format
            if (string.IsNullOrWhiteSpace(article.Date) || !DateTime.TryParse(article.Date, out var parsedDate))
            {
                article.Date = DateTime.Now.ToString("MMMM dd, yyyy");
            }
            else
            {
                article.Date = parsedDate.ToString("MMMM dd, yyyy");
            }

            articles.Add(article);

            var updatedJson = JsonSerializer.Serialize(articles, new JsonSerializerOptions { WriteIndented = true });
            System.IO.File.WriteAllText(filePath, updatedJson);

            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to add article");
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpPost]
    public IActionResult EditArticle([FromBody] ArticleModel updated)
    {
        string path = Path.Combine(_env.WebRootPath, "js", "articles.json");
        var articles = System.IO.File.Exists(path)
            ? JsonSerializer.Deserialize<List<ArticleModel>>(System.IO.File.ReadAllText(path)) ?? new()
            : new();

        // NOTE: Comparing by all four fields (Journal, Volume, SubVolume, Title) to find the article
        var article = articles.FirstOrDefault(a =>
            a.JournalName == updated.JournalName &&
            a.Volume == updated.Volume &&
            a.SubVolume == updated.SubVolume &&
            a.Title == updated.Title);

        if (article == null)
            return Json(new { success = false, message = "Article not found." });

        // Update fields
        article.Title = updated.Title;
        article.Authors = updated.Authors;
        article.Abstract = updated.Abstract;
        article.Category = updated.Category;

        System.IO.File.WriteAllText(path, JsonSerializer.Serialize(articles, new JsonSerializerOptions { WriteIndented = true }));
        return Json(new { success = true, message = "Article updated successfully!" });
    }

    [HttpPost]
    public IActionResult DeleteArticle([FromBody] ArticleModel article)
    {
        try
        {
            string filePath = Path.Combine(_env.WebRootPath, "js", "articles.json");

            if (!System.IO.File.Exists(filePath))
                return Json(new { success = false, message = "Articles file not found." });

            var json = System.IO.File.ReadAllText(filePath);
            var articles = JsonSerializer.Deserialize<List<ArticleModel>>(json) ?? new List<ArticleModel>();

            var existing = articles.FirstOrDefault(a =>
                a.JournalName == article.JournalName &&
                a.Volume == article.Volume &&
                a.SubVolume == article.SubVolume &&
                a.Title == article.Title);

            if (existing == null)
                return Json(new { success = false, message = "Article not found." });

            articles.Remove(existing);
            System.IO.File.WriteAllText(filePath, JsonSerializer.Serialize(articles, new JsonSerializerOptions { WriteIndented = true }));

            return Json(new { success = true, message = "Article deleted successfully." });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = $"Error deleting article: {ex.Message}" });
        }
    }

    //
    // ===== News Functions (JSON File) =====
    //

    [HttpPost]
    public IActionResult AddNews(Microsoft.AspNetCore.Http.IFormFile ImageFile, string Title, string Content, string Category)
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

            var newNews = new NewsModel
            {
                Id = Guid.NewGuid().ToString(),
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

    [HttpPost]
    public IActionResult EditNews(Microsoft.AspNetCore.Http.IFormFile ImageFile, string Id, string Title, string Content, string Category)
    {
        try
        {
            string filePath = Path.Combine(_env.WebRootPath, "js", "news.json");
            if (!System.IO.File.Exists(filePath))
                return Json(new { success = false, message = "News file not found." });

            var json = System.IO.File.ReadAllText(filePath);
            var newsList = JsonSerializer.Deserialize<List<NewsModel>>(json) ?? new List<NewsModel>();
            var news = newsList.FirstOrDefault(n => n.Id == Id);

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

    //
    // ====== User Management (Database) ======
    //

    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] UserViewModel model)
    {
        if (model == null)
        {
            return BadRequest(new { success = false, message = "Invalid data." });
        }

        // Server-side validation
        if (await context.Accounts.AnyAsync(a => a.Email == model.email || a.StudentNumber == model.idno))
        {
            return Conflict(new { success = false, message = "Email or Student/Employee ID already exists." });
        }

        // Split "First Last" name into two parts
        string firstName = model.name;
        string lastName = "";
        var nameParts = model.name.Trim().Split(' ');
        if (nameParts.Length > 1)
        {
            firstName = nameParts[0];
            lastName = string.Join(" ", nameParts.Skip(1));
        }

        // Map ViewModel to the database Model
        var newAccount = new Account
        {
            FirstName = firstName,
            LastName = lastName,
            Email = model.email,
            StudentNumber = model.idno,
            Level = model.department, // Department
            isAdmin = model.role == "Admin" // Role
            // Password logic should be here if creating a real user
        };

        context.Accounts.Add(newAccount);
        await context.SaveChangesAsync();

        return Ok(new { success = true });
    }

    [HttpPost]
    public async Task<IActionResult> UpdateUser([FromBody] UserViewModel model)
    {
        if (model == null)
        {
            return BadRequest(new { success = false, message = "Invalid data." });
        }

        var userToUpdate = await context.Accounts.FindAsync(model.id);
        if (userToUpdate == null)
        {
            return NotFound(new { success = false, message = "User not found." });
        }

        // Server-side validation (check for duplicates *excluding* this user)
        if (await context.Accounts.AnyAsync(a => (a.Email == model.email || a.StudentNumber == model.idno) && a.Id != model.id))
        {
            return Conflict(new { success = false, message = "Email or Student/Employee ID already exists." });
        }

        // Split name
        string firstName = model.name;
        string lastName = "";
        var nameParts = model.name.Trim().Split(' ');
        if (nameParts.Length > 1)
        {
            firstName = nameParts[0];
            lastName = string.Join(" ", nameParts.Skip(1));
        }

        // Map updated data
        userToUpdate.FirstName = firstName;
        userToUpdate.LastName = lastName;
        userToUpdate.Email = model.email;
        userToUpdate.StudentNumber = model.idno;
        userToUpdate.Level = model.department;
        userToUpdate.isAdmin = model.role == "Admin";

        await context.SaveChangesAsync();

        return Ok(new { success = true });
    }

    [HttpPost]
    public async Task<IActionResult> DeleteUser([FromBody] DeleteViewModel model)
    {
        var userToDelete = await context.Accounts.FindAsync(model.id);
        if (userToDelete == null)
        {
            return NotFound(new { success = false, message = "User not found." });
        }

        context.Accounts.Remove(userToDelete);
        await context.SaveChangesAsync();

        return Ok(new { success = true });
    }

    //
    // ====== Service Progress Update (Database) ======
    //

    [HttpPost]
    public async Task<IActionResult> UpdateProgress([FromBody] ProgressUpdateModel model)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Invalid model state");
            return BadRequest(ModelState);
        }

        _logger.LogInformation($"========== UPDATE PROGRESS START ==========");
        _logger.LogInformation($"ServiceId: {model.ServiceId}");

        try
        {
            // Find the tracked entity (or attach it)
            var serviceProgress = await context.ServiceProgresses
                .FirstOrDefaultAsync(sp => sp.ServiceId == model.ServiceId);

            if (serviceProgress == null)
            {
                _logger.LogError($"ServiceProgress NOT FOUND for ServiceId: {model.ServiceId}");
                return NotFound(new { success = false, message = "Service progress record not found." });
            }

            // Store original values for verification
            var originalProgress1 = serviceProgress.Progress1;
            var originalRunCount = serviceProgress.RunCount;

            // Update the values
            serviceProgress.Progress1 = model.Progress1;
            serviceProgress.Progress2 = model.Progress2;
            serviceProgress.Progress3 = model.Progress3;
            serviceProgress.Progress4 = model.Progress4;
            serviceProgress.RunCount = model.RunCount;
            serviceProgress.Remarks = model.Remarks ?? "";

            // Save changes
            var changeCount = await context.SaveChangesAsync();

            _logger.LogInformation($"SaveChangesAsync returned: {changeCount} rows affected");

            // Verification logic (less heavy than clearing and re-querying everything)
            bool actuallyChanged = changeCount > 0;
            if (!actuallyChanged && (originalProgress1 != model.Progress1 || originalRunCount != model.RunCount))
            {
                // This means the SaveChanges didn't report a change even though a property changed
                // This is rare but the rest of the original log logic was trying to handle it.
                _logger.LogError("✗✗✗ DATABASE WAS NOT UPDATED - SaveChanges returned 0 despite changes ✗✗✗");
            }

            _logger.LogInformation($"========== UPDATE PROGRESS END ==========");

            return Ok(new
            {
                success = actuallyChanged,
                message = actuallyChanged
                    ? $"Successfully updated {changeCount} record(s)"
                    : "No changes were detected or saved to the database.",
                data = new
                {
                    ServiceId = model.ServiceId,
                    Progress1 = serviceProgress.Progress1,
                    Progress2 = serviceProgress.Progress2,
                    Progress3 = serviceProgress.Progress3,
                    Progress4 = serviceProgress.Progress4,
                    RunCount = serviceProgress.RunCount,
                    Remarks = serviceProgress.Remarks
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ERROR updating ServiceId: {ServiceId}", model.ServiceId);
            return StatusCode(500, new
            {
                success = false,
                message = $"Error: {ex.Message}",
                innerError = ex.InnerException?.Message
            });
        }
    }

    //
    // ====== Error Handling ======
    //

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        // NOTE: ErrorViewModel, Account, ApplicationDBContext, ServiceInformations, ServiceProgress need to be defined
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}


// --- View Models for User CRUD ---
public class UserViewModel
{
    public int id { get; set; } // Needed for update
    public string? name { get; set; }
    public string? email { get; set; }
    public string? idno { get; set; }
    public string? department { get; set; }
    public string? role { get; set; }
}

public class DeleteViewModel
{
    public int id { get; set; }
}


// --- Example/Placeholder Models to make the code compile in a vacuum ---
// You should ensure these point to your actual EF Core and data models.
// NOTE: Assuming these exist in your project, potentially in LetranRPD.Models
// For this fix, I'll place them here for completeness, but they should be moved.
// DO NOT LEAVE THEM HERE in a production app if they belong in a Models folder.
public class ApplicationDBContext : DbContext
{
    // These need to match your actual DbSet names
    public DbSet<Account> Accounts { get; set; }
    public DbSet<ServiceInformation> ServiceInformations { get; set; }
    public DbSet<ServiceProgress> ServiceProgresses { get; set; }

    // Minimal constructor for compilation
    public ApplicationDBContext(DbContextOptions options) : base(options) { }
}

public class Account
{
    public int Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? StudentNumber { get; set; }
    public string? Level { get; set; }
    public bool isAdmin { get; set; }
}

public class ServiceInformation
{
    public ServiceProgress? ServiceProgress { get; set; }
    public object StudentNumber { get; internal set; }
    public object Email { get; internal set; }
    public string ServiceType { get; internal set; }
    public object Title { get; internal set; }
    public object Author { get; internal set; }
    public object ContactPerson { get; internal set; }
    public object ContactNumber { get; internal set; }
    public object ResearchAdviser { get; internal set; }
    public object Subject { get; internal set; }
    public object LE_Index { get; internal set; }
    public object LE_Pages { get; internal set; }
    public object DA_Tool { get; internal set; }
    public object DA_Variable { get; internal set; }
    public object OC_ManuscriptType { get; internal set; }
    public object ServiceId { get; internal set; }
}

public class ServiceProgress
{
    public int Id { get; set; }
    public int ServiceId { get; set; } // Foreign Key to ServiceInformation
    public DateTime AppliedDate { get; set; } // Assumed for OrderByDescending
    public int Progress1 { get; set; }
    public int Progress2 { get; set; }
    public int Progress3 { get; set; }
    public int Progress4 { get; set; }
    public int RunCount { get; set; }
    public string? Remarks { get; set; }
    public List<string> Progress1files { get; internal set; }
    public List<string> Progress2files { get; internal set; }
    public List<string> Progress3files { get; internal set; }
    public List<string> Progress4files { get; internal set; }
}

public class ErrorViewModel
{
    public string? RequestId { get; set; }
}