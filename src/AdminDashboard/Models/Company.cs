using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdminDashboard.Models
{
    public class Company
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Company Name is required")]
        [Display(Name = "Company Name")]
        [StringLength(100, ErrorMessage = "Company Name cannot be longer than 100 characters")]
        public string Name { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string? Email { get; set; }

        [Display(Name = "Logo")]
        public string? Logo { get; set; }

        [Url(ErrorMessage = "Invalid URL format")]
        public string? Website { get; set; }

        // Not mapped to database, just for file upload
        [NotMapped]
        [Display(Name = "Logo File")]
        public IFormFile? LogoFile { get; set; }

        // Navigation property
        public ICollection<Employee> Employees { get; set; } = new List<Employee>();
    }
}