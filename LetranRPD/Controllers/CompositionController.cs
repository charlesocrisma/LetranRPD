using LetranRPD.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class CompositionController : Controller
{
    private readonly IWebHostEnvironment _env;

    public CompositionController(IWebHostEnvironment env)
    {
        _env = env;
    }

    [HttpPost]
    public IActionResult AddArticle(ArticleModel article)
    {
        if (article == null)
            return Json(new { success = false, message = "Article is null" });

        var filePath = Path.Combine(_env.WebRootPath, "js", "articles.json");
        Directory.CreateDirectory(Path.GetDirectoryName(filePath));

        var json = System.IO.File.Exists(filePath) ? System.IO.File.ReadAllText(filePath) : "[]";
        var articles = JsonSerializer.Deserialize<List<ArticleModel>>(json) ?? new List<ArticleModel>();

        articles.Add(article);
        var updatedJson = JsonSerializer.Serialize(articles, new JsonSerializerOptions { WriteIndented = true });
        System.IO.File.WriteAllText(filePath, updatedJson);

        return Json(new { success = true });
    }

}
