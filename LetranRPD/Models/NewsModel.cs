using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;

public class NewsModel
{
    [Key]
    public int Id { get; set; } // changed to int if it’s a key

    [Required]
    public string Title { get; set; }

    [Required]
    public string Content { get; set; }

    [Required]
    public string ImagePath { get; set; }

    public string Category { get; set; }

    public string Date { get; set; }

    [NotMapped] // ✅ ignore during EF mapping
    [JsonIgnore] // optional: avoid serialization issues
    public IFormFile? ImageFile { get; set; }
}
