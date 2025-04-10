// Data/ApplicationDbContext.cs
using Microsoft.EntityFrameworkCore;
using AdminDashboard.Models;
using System.Collections.Generic;

namespace AdminDashboard.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Company> Companies { get; set; }
        public DbSet<Employee> Employees { get; set; }
    }
}