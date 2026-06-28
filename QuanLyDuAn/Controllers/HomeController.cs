using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyDuAn.Data;
using QuanLyDuAn.Models;

namespace QuanLyDuAn.Controllers
{
    public class HomeController : Controller
    {
        private readonly BtlCnpmContext _context;

        public HomeController(BtlCnpmContext context)
        {
            _context = context;
        }

        // ========== UC#19: LANDING PAGE ==========
        public IActionResult Index()
        {
            // If already logged in, redirect to workspace
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("MaTaiKhoan")))
            {
                return RedirectToAction("Index", "Workspace");
            }
            return View();
        }

        // ========== UC#20: PRICING PAGE ==========
        public async Task<IActionResult> Pricing()
        {
            var packages = await _context.Goidichvus.OrderBy(g => g.Gia).ToListAsync();
            return View(packages);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
