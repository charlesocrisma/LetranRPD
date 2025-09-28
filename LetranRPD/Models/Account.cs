using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
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
        public bool Progress1 { get; set; } = false;
        public bool Progress2 { get; set; } = false;
        public bool Progress3 { get; set; } = false;
        public bool Progress4 { get; set; } = false;
        public ServiceInformation SI { get; set; } = null!;

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

        public string? LE_Index { get; set; }
        public int? LE_Pages { get; set; }
        public string? DA_Variable { get; set; }
        public string? DA_Tool { get; set; }

        public ICollection<ServiceProgress> ServiceProgress { get; set; } = new List<ServiceProgress>();

    }

}
