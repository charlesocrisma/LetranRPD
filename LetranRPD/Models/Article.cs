using System.ComponentModel.DataAnnotations;

namespace LetranRPD.Models
{
    public class Article
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Authors { get; set; }
        [Required]
        public string Abstract { get; set; }
        [Required]
        public string JournalName { get; set; }
        [Required]
        public string Volume { get; set; }
        [Required]
        public string SubVolume { get; set; }

        public string Category { get; set; }
        public string Date { get; set; }
    }
}
