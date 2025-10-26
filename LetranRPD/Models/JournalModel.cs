namespace LetranRPD.Models
{
    public class JournalModel
    {
        public string JournalName { get; set; }
        public string Volume { get; set; }
        public string SubVolume { get; set; }
        public string Date { get; set; }
        public List<ArticleModel> Articles { get; set; } = new();
    }
}