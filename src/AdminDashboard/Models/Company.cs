namespace AdminDashboard.Models
{
    public class Company
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? Email { get; set; }
        public string? Logo { get; set; }
        public string? Website { get; set; }
        public ICollection<Employee> Employees { get; set; } = new List<Employee>();
    }
}
