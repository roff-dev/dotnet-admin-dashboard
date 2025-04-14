using System.ComponentModel.DataAnnotations; // for validation attributes

namespace AdminDashboard.Models
{
    public class Company
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Company Name is required")]
        [Display(Name = "Company Name")] // proper display name in forms
        [StringLength(100, ErrorMessage = "Company Name cannot be longer than 100 characters")]
        public string Name { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string? Email { get; set; }

        [Url(ErrorMessage = "Invalid URL format")]
        [Display(Name = "Logo URL")]
        public string? Logo { get; set; }

        [Url(ErrorMessage = "Invalid URL format")]
        public string? Website { get; set; }

        // Navigation property
        public ICollection<Employee> Employees { get; set; } = new List<Employee>();
    }
}