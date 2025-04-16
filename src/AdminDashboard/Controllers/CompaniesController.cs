using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AdminDashboard.Data;
using AdminDashboard.Models;
using System.Drawing;  
using System.IO;       
using Microsoft.AspNetCore.Authorization;

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
                .FirstOrDefaultAsync(m => m.Id == id);
            if (company == null)
            {
                return NotFound();
            }

            return View(company);
        }

        // POST: Companies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var company = await _context.Companies.FindAsync(id);
            _context.Companies.Remove(company);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CompanyExists(int id)
        {
            return _context.Companies.Any(e => e.Id == id);
        }
    }
}