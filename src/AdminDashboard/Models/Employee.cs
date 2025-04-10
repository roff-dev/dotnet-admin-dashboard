namespace AdminDashboard.Models
{
    public class Employee
    {
        public int Id { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public int CompanyId { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }

        // Navigation property
        public Company Company { get; set; } = null!;
    }
}
