public class Article
{
    public int Id { get; set; }
    public int JournalId { get; set; }
    public string Title { get; set; }
    public string Authors { get; set; }
    public string Category { get; set; }
    public string Abstract { get; set; }
    public string PdfUrl { get; set; } // Optional: if downloadable
    public string ImageUrl { get; set; }

    public virtual Journal Journal { get; set; }
}
