using Microsoft.AspNetCore.Mvc;
using AdminDashboard.Data;
using Microsoft.EntityFrameworkCore;

namespace AdminDashboard.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.CompanyCount = await _context.Companies.CountAsync();
            ViewBag.EmployeeCount = await _context.Employees.CountAsync();

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }
    }
}