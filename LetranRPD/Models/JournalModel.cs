using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LetranRPD.Models
{
    public class JournalModel
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string JournalName { get; set; }
        [Required]
        public string Volume { get; set; }
        [Required]
        public string SubVolume { get; set; }

        // 👇 optional: this property should be ignored if not saving nested articles
        [NotMapped]
        public List<Article> Articless { get; set; } = new();
    }
}