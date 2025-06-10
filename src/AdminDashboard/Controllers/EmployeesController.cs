using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AdminDashboard.Data;
using AdminDashboard.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AdminDashboard.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EmployeesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Employees
        public async Task<IActionResult> Index()
        {
            var employees = _context.Employees.Include(e => e.Company);
            return View(await employees.ToListAsync());
        }

        // GET: Employees/Create
        public IActionResult Create()
        {
            // Use ViewBag instead of ViewData to match your view
            ViewBag.CompanyId = new SelectList(_context.Companies, "Id", "Name");
            return View();
        }

        // POST: Employees/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FirstName,LastName,CompanyId,Email,PhoneNumber")] Employee employee)
        {
            // Add logging to debug the issue
            Console.WriteLine($"Create POST action called");
            Console.WriteLine($"Employee FirstName: {employee.FirstName}");
            Console.WriteLine($"Employee LastName: {employee.LastName}");
            Console.WriteLine($"Employee CompanyId: {employee.CompanyId}");
            Console.WriteLine($"Employee Email: {employee.Email}");
            Console.WriteLine($"Employee PhoneNumber: {employee.PhoneNumber}");

            // Check ModelState
            if (!ModelState.IsValid)
            {
                Console.WriteLine("ModelState is invalid:");
                foreach (var state in ModelState)
                {
                    if (state.Value.Errors.Count > 0)
                    {
                        foreach (var error in state.Value.Errors)
                        {
                            Console.WriteLine($"- Error in {state.Key}: {error.ErrorMessage}");
                        }
                    }
                }

                // Re-populate the dropdown for the view
                ViewBag.CompanyId = new SelectList(_context.Companies, "Id", "Name", employee.CompanyId);
                return View(employee);
            }

            try
            {
                Console.WriteLine("ModelState is valid, attempting to save employee");
                _context.Add(employee);
                var result = await _context.SaveChangesAsync();
                Console.WriteLine($"SaveChangesAsync completed, rows affected: {result}");

                Console.WriteLine("Redirecting to Index");
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception during save: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
                ModelState.AddModelError("", "An error occurred while saving the employee.");

                // Re-populate the dropdown for the view
                ViewBag.CompanyId = new SelectList(_context.Companies, "Id", "Name", employee.CompanyId);
                return View(employee);
            }
        }

        // GET: Employees/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }
            ViewBag.CompanyId = new SelectList(_context.Companies, "Id", "Name", employee.CompanyId);
            return View(employee);
        }

        // POST: Employees/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Employee employee)
        {
            // Add extensive logging
            Console.WriteLine($"Edit POST action called");
            Console.WriteLine($"Route ID: {id}");
            Console.WriteLine($"Employee ID: {employee.Id}");
            Console.WriteLine($"Employee FirstName: {employee.FirstName}");
            Console.WriteLine($"Employee LastName: {employee.LastName}");

            if (id != employee.Id)
            {
                Console.WriteLine("ID mismatch, returning NotFound");
                return NotFound();
            }

            // Log ModelState errors if any
            if (!ModelState.IsValid)
            {
                Console.WriteLine("ModelState is invalid:");
                foreach (var state in ModelState)
                {
                    if (state.Value.Errors.Count > 0)
                    {
                        foreach (var error in state.Value.Errors)
                        {
                            Console.WriteLine($"- Error in {state.Key}: {error.ErrorMessage}");
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("ModelState is valid");
            }

            // Even if ModelState is invalid, try to update the employee
            try
            {
                // Get the existing employee
                var existingEmployee = await _context.Employees.FindAsync(id);
                if (existingEmployee == null)
                {
                    Console.WriteLine($"Employee with ID {id} not found in database");
                    return NotFound();
                }

                Console.WriteLine("Found existing employee, updating properties");

                // Update each property manually and log any issues
                try { existingEmployee.FirstName = employee.FirstName; }
                catch (Exception ex) { Console.WriteLine($"Error updating FirstName: {ex.Message}"); }

                try { existingEmployee.LastName = employee.LastName; }
                catch (Exception ex) { Console.WriteLine($"Error updating LastName: {ex.Message}"); }

                try { existingEmployee.Email = employee.Email; }
                catch (Exception ex) { Console.WriteLine($"Error updating Email: {ex.Message}"); }

                try { existingEmployee.PhoneNumber = employee.PhoneNumber; }
                catch (Exception ex) { Console.WriteLine($"Error updating PhoneNumber: {ex.Message}"); }

                try { existingEmployee.CompanyId = employee.CompanyId; }
                catch (Exception ex) { Console.WriteLine($"Error updating CompanyId: {ex.Message}"); }

                try
                {
                    Console.WriteLine("Attempting to save changes");
                    int rowsAffected = await _context.SaveChangesAsync();
                    Console.WriteLine($"SaveChangesAsync completed, rows affected: {rowsAffected}");

                    // Force a commit
                    using var transaction = await _context.Database.BeginTransactionAsync();
                    await transaction.CommitAsync();
                    Console.WriteLine("Transaction committed");

                    Console.WriteLine("Redirecting to Index");
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    Console.WriteLine($"DbUpdateConcurrencyException: {ex.Message}");
                    if (!EmployeeExists(employee.Id))
                    {
                        Console.WriteLine("Employee no longer exists");
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception during save: {ex.Message}");
                    if (ex.InnerException != null)
                    {
                        Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                    }
                    ModelState.AddModelError("", "An error occurred while saving changes.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Top-level exception: {ex.Message}");
                ModelState.AddModelError("", "An unexpected error occurred.");
            }

            Console.WriteLine("Preparing to re-render Edit view");
            ViewBag.CompanyId = new SelectList(_context.Companies, "Id", "Name", employee.CompanyId);
            return View(employee);
        }

        // GET: Employees/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .Include(e => e.Company)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee != null)
            {
                _context.Employees.Remove(employee);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool EmployeeExists(int id)
        {
            return _context.Employees.Any(e => e.Id == id);
        }
    }
}