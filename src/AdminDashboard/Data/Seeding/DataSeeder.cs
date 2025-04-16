using AdminDashboard.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
namespace AdminDashboard.Data.Seeding
{
    public static class DataSeeder
    {
        public static async Task SeedData(ApplicationDbContext context)
        {
            // check if any data
            if (context.Companies.Any() || context.Employees.Any())
            {
                return; // seeded
            }

            // create companies
            var companies = new List<Company>
            {
                new Company
                {
                    Name = "Banana Computer, Inc",
                    Email = "banana@notapple.com",
                    Logo = "https://placehold.co/150",
                    Website = "https://bananacomputer.com"
                },
                new Company
                {
                    Name = "Doodle LLC",
                    Email = "info@doodle.com",
                    Logo = "https://placehold.co/150",
                    Website = "https://doodle.com"
                },
                new Company
                {
                    Name = "Beta Platforms, Inc",
                    Email = "hello@facecook.com",
                    Logo = "https://placehold.co/150",
                    Website = "https://facecook.com"
                }
            };

            // add companies to context
            await context.Companies.AddRangeAsync(companies);
            await context.SaveChangesAsync();

            // create Employees
            var employees = new List<Employee>
            {
                // employees for company 1
                new Employee
                {
                    FirstName = "John",
                    LastName = "Doe",
                    Email = "john.doe@companyone.com",
                    PhoneNumber = "01234567890",
                    CompanyId = companies[0].Id
                },
                new Employee
                {
                    FirstName = "Jane",
                    LastName = "Smith",
                    Email = "jane.smith@companyone.com",
                    PhoneNumber = "01234567891",
                    CompanyId = companies[0].Id
                },

                // employees for company 2
                new Employee
                {
                    FirstName = "Mike",
                    LastName = "Johnson",
                    Email = "mike.johnson@companytwo.com",
                    PhoneNumber = "01234567892",
                    CompanyId = companies[1].Id
                },
                new Employee
                {
                    FirstName = "Sarah",
                    LastName = "Williams",
                    Email = "sarah.williams@companytwo.com",
                    PhoneNumber = "01234567893",
                    CompanyId = companies[1].Id
                },

                // employees for company 3
                new Employee
                {
                    FirstName = "David",
                    LastName = "Brown",
                    Email = "david.brown@companythree.com",
                    PhoneNumber = "01234567894",
                    CompanyId = companies[2].Id
                },
                new Employee
                {
                    FirstName = "Emily",
                    LastName = "Davis",
                    Email = "emily.davis@companythree.com",
                    PhoneNumber = "01234567895",
                    CompanyId = companies[2].Id
                }
            };

            // add employees to context
            await context.Employees.AddRangeAsync(employees);
            await context.SaveChangesAsync();
        }
    }
}