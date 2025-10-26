using LetranRPD.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace LetranRPD.Controllers
{
    public class AdminController : Controller
    {
        private readonly ILogger<AdminController> _logger;
        private readonly IWebHostEnvironment _env;

        public AdminController(ILogger<AdminController> logger, IWebHostEnvironment env)
        {
            _logger = logger;
            _env = env;
        }

        // ====== Admin Views ======
        public IActionResult Dashboard() => View();
        public IActionResult Research() => View();
        public IActionResult Create() => View();
        public IActionResult Submission() => View();
        public new IActionResult User() => View();
        public IActionResult Delete(string Name) => RedirectToAction("User");
        public IActionResult Privacy() => View();
        public IActionResult Content() => View();

        // ====== Save Journal ======
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

            // ✅ Prevent duplicate subVolume for the same journal and volume
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

                // ✅ Force the date to a readable format regardless of how it was sent
                if (string.IsNullOrWhiteSpace(article.Date))
                {
                    article.Date = DateTime.Now.ToString("MMMM dd, yyyy");
                }
                else
                {
                    if (DateTime.TryParse(article.Date, out var parsedDate))
                        article.Date = parsedDate.ToString("MMMM dd, yyyy");
                    else
                        article.Date = DateTime.Now.ToString("MMMM dd, yyyy");
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

        // ====== Edit Article ======
        [HttpPost]
        public IActionResult EditArticle([FromBody] ArticleModel updated)
        {
            string path = Path.Combine(_env.WebRootPath, "js", "articles.json");
            var articles = System.IO.File.Exists(path)
                ? JsonSerializer.Deserialize<List<ArticleModel>>(System.IO.File.ReadAllText(path)) ?? new()
                : new();

            var article = articles.FirstOrDefault(a =>
                a.JournalName == updated.JournalName &&
                a.Volume == updated.Volume &&
                a.SubVolume == updated.SubVolume &&
                a.Title == updated.Title);

            if (article == null)
                return Json(new { success = false, message = "Article not found." });

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


        // ====== Display Journals with Articles ======
        public IActionResult Journals()
        {
            var journalsPath = Path.Combine(_env.WebRootPath, "js", "journal-data.json");
            var articlesPath = Path.Combine(_env.WebRootPath, "js", "articles.json");

            var journalsJson = System.IO.File.Exists(journalsPath)
                ? System.IO.File.ReadAllText(journalsPath)
                : "[]";

            var articlesJson = System.IO.File.Exists(articlesPath)
                ? System.IO.File.ReadAllText(articlesPath)
                : "[]";

            var journals = JsonSerializer.Deserialize<List<JournalModel>>(journalsJson,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<JournalModel>();

            var articles = JsonSerializer.Deserialize<List<ArticleModel>>(articlesJson,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<ArticleModel>();

            foreach (var journal in journals)
            {
                if (journal == null) continue;

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

        // ===== News Functions =====
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


    }
}
