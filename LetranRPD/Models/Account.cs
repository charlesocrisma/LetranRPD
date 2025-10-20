using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Org.BouncyCastle.Bcpg;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
namespace LetranRPD.Models

{
    public class Account
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string Email { get; set; } = "";
        public string StudentNumber { get; set; } = "";
        public string Level { get; set; }
        public string password { get; set; } = "";
        public bool? isAdmin { get; set; } = false;
    }

    public class ServiceProgress
    {
        public int Id { get; set; }
        [JsonIgnore]
        public ServiceInformation SI { get; set; } = null!;

        [ForeignKey("SI")]
        public int ServiceId { get; set; }
        public int Progress1 { get; set; } = 1;
        public List<string>? Progress1files { get; set; }
        public int Progress2 { get; set; } = 0;
        public List<string>? Progress2files { get; set; }
        public int Progress3 { get; set; } = 0;
        public List<string>? Progress3files { get; set; }
        public int Progress4 { get; set; } = 0;
        public List<string>? Progress4files { get; set; }

        public DateTime AppliedDate { get; set; }
}
    public class ServiceInformation
    {
        [Key]
        public int ServiceId { get; set; }
        public string ServiceType { get; set; } = "";
        public string StudentNumber { get; set; } = "";
        public string Email { get; set; } = "";
        public string Title { get; set; } = "";
        public string Author { get; set; } = "";
        public string ContactPerson { get; set; } = "";
        public string ContactNumber { get; set; } = "";
        public string ResearchAdviser { get; set; } = "";
        public string Subject { get; set; } = "";

        public string? OC_ManuscriptType { get; set; }
        public string? LE_Index { get; set; }
        public int? LE_Pages { get; set; }
        public string? DA_Variable { get; set; }
        public string? DA_Tool { get; set; }

        
        public ServiceProgress ServiceProgress { get; set; } = new ServiceProgress();

    }

}
