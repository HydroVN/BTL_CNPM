using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyDuAn.Data;
using QuanLyDuAn.Filters;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace QuanLyDuAn.Controllers
{
    [AuthFilter]
    public class AdminController : Controller
    {
        private readonly BtlCnpmContext _context;

        public AdminController(BtlCnpmContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            var vaiTro = HttpContext.Session.GetString("VaiTro");
            if (vaiTro != "Admin")
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            // 1. Total accounts (excluding Admin)
            var totalUsers = await _context.Taikhoans.CountAsync(t => t.VaiTro != "Admin");

            // 2. Count of PRO and Enterprise (ENT) plans
            var proCount = await _context.Taikhoans.CountAsync(t => t.VaiTro != "Admin" && t.MaGoi == "PRO");
            var entCount = await _context.Taikhoans.CountAsync(t => t.VaiTro != "Admin" && t.MaGoi == "ENT");

            // 3. Month with the most registrations
            var regMonths = await _context.Taikhoans
                .Where(t => t.VaiTro != "Admin")
                .GroupBy(t => new { t.NgayTao.Year, t.NgayTao.Month })
                .Select(g => new MonthlyRegistrationDto
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    Count = g.Count()
                })
                .OrderByDescending(m => m.Count)
                .ToListAsync();

            ViewBag.TotalUsers = totalUsers;
            ViewBag.ProCount = proCount;
            ViewBag.EntCount = entCount;
            ViewBag.RegMonths = regMonths;

            // 4. Peak activity time per day (WITHOUT database modifications)
            var commentTimes = await _context.Binhluans.Select(b => b.ThoiGian).ToListAsync();
            var taskTimes = await _context.Congviecs.Select(c => c.NgayTao).ToListAsync();

            var allActivities = commentTimes.Concat(taskTimes).ToList();

            var peakTimes = allActivities
                .GroupBy(a => a.DayOfWeek)
                .Select(g => {
                    var peakHourGroup = g.GroupBy(x => x.Hour)
                                         .OrderByDescending(hg => hg.Count())
                                         .FirstOrDefault();
                    return new PeakActivityDto
                    {
                        DayOfWeek = g.Key,
                        PeakHourStart = peakHourGroup != null ? peakHourGroup.Key : 9,
                        Count = peakHourGroup != null ? peakHourGroup.Count() : 0
                    };
                })
                .OrderBy(p => p.DayOfWeek == DayOfWeek.Sunday ? 7 : (int)p.DayOfWeek) // Mon=1, Tue=2, ..., Sun=7
                .ToList();

            // Ensure all days of the week are represented even if they have 0 activity
            var finalPeakTimes = new List<PeakActivityDto>();
            var days = new[] { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday, DayOfWeek.Sunday };
            foreach (var d in days)
            {
                var existing = peakTimes.FirstOrDefault(p => p.DayOfWeek == d);
                if (existing != null)
                {
                    finalPeakTimes.Add(existing);
                }
                else
                {
                    finalPeakTimes.Add(new PeakActivityDto { DayOfWeek = d, PeakHourStart = 9, Count = 0 });
                }
            }

            ViewBag.PeakTimes = finalPeakTimes;

            return View();
        }
    }

    public class MonthlyRegistrationDto
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public int Count { get; set; }
    }

    public class PeakActivityDto
    {
        public DayOfWeek DayOfWeek { get; set; }
        public int PeakHourStart { get; set; }
        public int Count { get; set; }
    }
}
