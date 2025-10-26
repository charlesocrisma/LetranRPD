using System.Text.Json.Serialization;

public class NewsModel
{
    public string Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public string ImagePath { get; set; }
    public string Category { get; set; }
    public string Date { get; set; }
    [JsonIgnore] // prevent issues when serializing
    public IFormFile? ImageFile { get; set; }
}
