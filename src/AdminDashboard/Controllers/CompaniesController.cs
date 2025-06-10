using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AdminDashboard.Data;
using AdminDashboard.Models;
using System.Drawing;
using System.IO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AdminDashboard.Controllers
{
    public class CompaniesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CompaniesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Companies
        public async Task<IActionResult> Index()
        {
            return View(await _context.Companies.ToListAsync());
        }

        // GET: Companies/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Companies/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Email,Logo,Website,LogoFile")] Company company)
        {
            if (ModelState.IsValid)
            {
                // Handle file upload if provided
                if (company.LogoFile != null && company.LogoFile.Length > 0)
                {
                    // Validate file is an image
                    if (!company.LogoFile.ContentType.StartsWith("image/"))
                    {
                        ModelState.AddModelError("LogoFile", "Please upload a valid image file.");
                        return View(company);
                    }

                    // Validate image dimensions
                    using var image = Image.FromStream(company.LogoFile.OpenReadStream());
                    if (image.Width < 100 || image.Height < 100)
                    {
                        ModelState.AddModelError("LogoFile", "Logo must be at least 100x100 pixels.");
                        return View(company);
                    }

                    // Create unique filename
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(company.LogoFile.FileName);
                    string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "logos", fileName);

                    // Ensure directory exists
                    Directory.CreateDirectory(Path.GetDirectoryName(filePath));

                    // Save file
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await company.LogoFile.CopyToAsync(fileStream);
                    }

                    // Set logo url to the saved file path
                    company.Logo = "/images/logos/" + fileName;
                }

                _context.Add(company);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(company);
        }

        // GET: Companies/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var company = await _context.Companies.FindAsync(id);
            if (company == null)
            {
                return NotFound();
            }
            return View(company);
        }

        // POST: Companies/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Email,Logo,Website,LogoFile")] Company company)
        {
            if (id != company.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Handle file upload if provided
                    if (company.LogoFile != null && company.LogoFile.Length > 0)
                    {
                        // Validate file is an image
                        if (!company.LogoFile.ContentType.StartsWith("image/"))
                        {
                            ModelState.AddModelError("LogoFile", "Please upload a valid image file.");
                            return View(company);
                        }

                        // Validate image dimensions
                        using var image = Image.FromStream(company.LogoFile.OpenReadStream());
                        if (image.Width < 100 || image.Height < 100)
                        {
                            ModelState.AddModelError("LogoFile", "Logo must be at least 100x100 pixels.");
                            return View(company);
                        }

                        // Create unique filename
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(company.LogoFile.FileName);
                        string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "logos", fileName);

                        // Ensure directory exists
                        Directory.CreateDirectory(Path.GetDirectoryName(filePath));

                        // Save file
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await company.LogoFile.CopyToAsync(fileStream);
                        }

                        // Set logo url to the saved file path
                        company.Logo = "/images/logos/" + fileName;
                    }
                    // If no file is uploaded but Logo is empty, get the existing Logo
                    else if (string.IsNullOrEmpty(company.Logo))
                    {
                        var existingCompany = await _context.Companies.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
                        company.Logo = existingCompany?.Logo;
                    }

                    _context.Update(company);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CompanyExists(company.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(company);
        }

        // GET: Companies/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var company = await _context.Companies
                .Include(c => c.Employees) // Include employees to show count and details
                .FirstOrDefaultAsync(m => m.Id == id);

            if (company == null)
            {
                return NotFound();
            }

            // Create a view model with additional data
            var viewModel = new CompanyDeleteViewModel
            {
                Company = company,
                EmployeeCount = company.Employees.Count,
                Employees = company.Employees.ToList(),
                AvailableCompanies = await _context.Companies
                    .Where(c => c.Id != id) // Exclude the company being deleted
                    .Select(c => new SelectListItem
                    {
                        Value = c.Id.ToString(),
                        Text = c.Name
                    })
                    .ToListAsync()
            };

            return View(viewModel);
        }

        // POST: Companies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, string deleteAction, int? reassignCompanyId)
        {
            Console.WriteLine($"Delete action: {deleteAction}, Reassign to: {reassignCompanyId}");

            var company = await _context.Companies
                .Include(c => c.Employees)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (company == null)
            {
                return NotFound();
            }

            try
            {
                if (deleteAction == "reassign" && reassignCompanyId.HasValue)
                {
                    // Reassign employees to another company
                    var targetCompany = await _context.Companies.FindAsync(reassignCompanyId.Value);
                    if (targetCompany == null)
                    {
                        TempData["ErrorMessage"] = "Selected company for reassignment not found.";
                        return RedirectToAction(nameof(Delete), new { id = id });
                    }

                    Console.WriteLine($"Reassigning {company.Employees.Count} employees to company {targetCompany.Name}");

                    foreach (var employee in company.Employees)
                    {
                        employee.CompanyId = reassignCompanyId.Value;
                    }

                    await _context.SaveChangesAsync();
                    Console.WriteLine("Employees reassigned successfully");
                }
                else if (deleteAction == "deleteAll")
                {
                    // Delete all employees with the company
                    Console.WriteLine($"Deleting {company.Employees.Count} employees along with company");
                    _context.Employees.RemoveRange(company.Employees);
                }
                else
                {
                    // Invalid action
                    TempData["ErrorMessage"] = "Invalid delete action specified.";
                    return RedirectToAction(nameof(Delete), new { id = id });
                }

                // Remove the company
                _context.Companies.Remove(company);
                await _context.SaveChangesAsync();

                if (deleteAction == "reassign")
                {
                    TempData["SuccessMessage"] = $"Company '{company.Name}' deleted successfully. {company.Employees.Count} employees were reassigned.";
                }
                else
                {
                    TempData["SuccessMessage"] = $"Company '{company.Name}' and {company.Employees.Count} associated employees deleted successfully.";
                }

                Console.WriteLine("Company deletion completed successfully");
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during company deletion: {ex.Message}");
                TempData["ErrorMessage"] = "An error occurred while deleting the company. Please try again.";
                return RedirectToAction(nameof(Delete), new { id = id });
            }
        }

        private bool CompanyExists(int id)
        {
            return _context.Companies.Any(e => e.Id == id);
        }
    }

    // View model for the delete confirmation page
    public class CompanyDeleteViewModel
    {
        public Company Company { get; set; } = null!;
        public int EmployeeCount { get; set; }
        public List<Employee> Employees { get; set; } = new List<Employee>();
        public List<SelectListItem> AvailableCompanies { get; set; } = new List<SelectListItem>();
    }
}