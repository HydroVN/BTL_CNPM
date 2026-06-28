using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyDuAn.Data;
using QuanLyDuAn.Filters;
using QuanLyDuAn.Models;

namespace QuanLyDuAn.Controllers
{
    [AuthFilter]
    public class BinhLuanController : Controller
    {
        private readonly BtlCnpmContext _context;

        public BinhLuanController(BtlCnpmContext context)
        {
            _context = context;
        }

        // Get task detail + comments (AJAX)
        [HttpGet]
        public async Task<IActionResult> GetTaskDetail(string maCongViec)
        {
            var cv = await _context.Congviecs
                .Include(c => c.MaThanhVienNavigation).ThenInclude(tv => tv!.MaTaiKhoanNavigation)
                .Include(c => c.Binhluans).ThenInclude(b => b.MaTaiKhoanNavigation)
                .FirstOrDefaultAsync(c => c.MaCongViec == maCongViec);

            if (cv == null) return NotFound();

            var assignee = cv.MaThanhVienNavigation?.MaTaiKhoanNavigation?.HoTen;

            return Json(new
            {
                tenCongViec = cv.TenCongViec,
                moTa = cv.MoTa,
                deadline = cv.Deadline?.ToString("dd/MM/yyyy"),
                mucDoUuTien = cv.MucDoUuTien,
                trangThai = cv.TrangThai,
                assignee = assignee,
                comments = cv.Binhluans.OrderBy(b => b.ThoiGian).Select(b => new
                {
                    avatar = b.MaTaiKhoanNavigation.HoTen[0].ToString().ToUpper(),
                    hoTen = b.MaTaiKhoanNavigation.HoTen,
                    noiDung = b.NoiDung,
                    thoiGian = b.ThoiGian.ToString("dd/MM HH:mm")
                })
            });
        }

        // UC#22: Create comment (AJAX)
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCommentRequest request)
        {
            var maTk = HttpContext.Session.GetString("MaTaiKhoan");
            var hoTen = HttpContext.Session.GetString("HoTen") ?? "";

            if (string.IsNullOrEmpty(maTk) || string.IsNullOrWhiteSpace(request.NoiDung))
                return Json(new { success = false });

            var bl = new Binhluan
            {
                MaBinhLuan = Guid.NewGuid().ToString("N")[..10].ToUpper(),
                MaCongViec = request.MaCongViec,
                MaTaiKhoan = maTk,
                NoiDung = request.NoiDung,
                ThoiGian = DateTime.Now
            };

            _context.Binhluans.Add(bl);
            await _context.SaveChangesAsync();

            return Json(new
            {
                success = true,
                avatar = hoTen.Length > 0 ? hoTen[0].ToString().ToUpper() : "U",
                hoTen = hoTen
            });
        }

        public class CreateCommentRequest
        {
            public string MaCongViec { get; set; } = "";
            public string NoiDung { get; set; } = "";
        }
    }
}
