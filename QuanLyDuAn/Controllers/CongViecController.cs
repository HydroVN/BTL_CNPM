using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyDuAn.Data;
using QuanLyDuAn.Filters;
using QuanLyDuAn.Models;

namespace QuanLyDuAn.Controllers
{
    [AuthFilter]
    public class CongViecController : Controller
    {
        private readonly BtlCnpmContext _context;
        private readonly IWebHostEnvironment _env;

        public CongViecController(BtlCnpmContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // ========== UC#11: THÊM CÔNG VIỆC ==========
        [HttpGet]
        public async Task<IActionResult> Create(string maDuAn)
        {
            var duAn = await _context.Duans
                .Include(d => d.MaWorkspaceNavigation)
                .FirstOrDefaultAsync(d => d.MaDuAn == maDuAn);
            if (duAn == null) return RedirectToAction("Index", "DuAn");

            var maTk = HttpContext.Session.GetString("MaTaiKhoan")!;
            var vaiTro = HttpContext.Session.GetString("VaiTro");

            var isOwner = duAn.MaWorkspaceNavigation.MaTaiKhoan == maTk;
            var isPm = await _context.Thanhviens.AnyAsync(tv => tv.MaDuAn == maDuAn && tv.MaTaiKhoan == maTk && tv.VaiTroDuAn == "PM");

            if (!isOwner && !isPm && vaiTro != "Admin")
            {
                TempData["Error"] = "Chỉ người quản lý dự án mới có quyền thêm công việc.";
                return RedirectToAction("Details", "DuAn", new { id = maDuAn });
            }

            ViewBag.DuAn = duAn;
            ViewBag.ThanhViens = await _context.Thanhviens
                .Include(tv => tv.MaTaiKhoanNavigation)
                .Where(tv => tv.MaDuAn == maDuAn)
                .ToListAsync();

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(string maDuAn, string tenCongViec, string? moTa,
            DateTime? deadline, string mucDoUuTien, string? maThanhVien, IFormFile? tepDinhKem)
        {
            var duAn = await _context.Duans
                .Include(d => d.MaWorkspaceNavigation)
                .FirstOrDefaultAsync(d => d.MaDuAn == maDuAn);
            if (duAn == null) return RedirectToAction("Index", "DuAn");

            var maTk = HttpContext.Session.GetString("MaTaiKhoan")!;
            var vaiTro = HttpContext.Session.GetString("VaiTro");

            var isOwner = duAn.MaWorkspaceNavigation.MaTaiKhoan == maTk;
            var isPm = await _context.Thanhviens.AnyAsync(tv => tv.MaDuAn == maDuAn && tv.MaTaiKhoan == maTk && tv.VaiTroDuAn == "PM");

            if (!isOwner && !isPm && vaiTro != "Admin")
            {
                TempData["Error"] = "Chỉ người quản lý dự án mới có quyền thêm công việc.";
                return RedirectToAction("Details", "DuAn", new { id = maDuAn });
            }

            ViewBag.DuAn = duAn;
            ViewBag.ThanhViens = await _context.Thanhviens
                .Include(tv => tv.MaTaiKhoanNavigation)
                .Where(tv => tv.MaDuAn == maDuAn)
                .ToListAsync();

            if (string.IsNullOrWhiteSpace(tenCongViec))
            {
                ViewBag.Error = "Tên công việc không được để trống";
                return View();
            }

            // Validate deadline within project dates
            if (deadline.HasValue && duAn.NgayKetThuc.HasValue && deadline > duAn.NgayKetThuc)
            {
                ViewBag.Error = "Hạn hoàn thành công việc vượt quá thời gian cho phép của dự án";
                return View();
            }

            var congViec = new Congviec
            {
                MaCongViec = Guid.NewGuid().ToString("N")[..10].ToUpper(),
                MaDuAn = maDuAn,
                MaThanhVien = string.IsNullOrEmpty(maThanhVien) ? null : maThanhVien,
                TenCongViec = tenCongViec,
                MoTa = moTa,
                Deadline = deadline,
                MucDoUuTien = mucDoUuTien ?? "TrungBinh",
                TrangThai = "ToDo",
                NgayTao = DateTime.Now
            };

            _context.Congviecs.Add(congViec);
            await _context.SaveChangesAsync();

            // Upload attachment if provided
            if (tepDinhKem != null && tepDinhKem.Length > 0)
            {
                var uploadsDir = Path.Combine(_env.WebRootPath, "uploads");
                Directory.CreateDirectory(uploadsDir);
                var fileName = Guid.NewGuid().ToString("N")[..8] + "_" + tepDinhKem.FileName;
                var filePath = Path.Combine(uploadsDir, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await tepDinhKem.CopyToAsync(stream);
                }

                var tep = new Tepdinhkem
                {
                    MaTep = Guid.NewGuid().ToString("N")[..10].ToUpper(),
                    MaCongViec = congViec.MaCongViec,
                    TenTep = tepDinhKem.FileName,
                    DuongDan = "/uploads/" + fileName,
                    DungLuong = (int)(tepDinhKem.Length / 1024)
                };
                _context.Tepdinhkems.Add(tep);
                await _context.SaveChangesAsync();
            }

            // Send notification if assigned
            if (!string.IsNullOrEmpty(maThanhVien))
            {
                var tv = await _context.Thanhviens.Include(t => t.MaTaiKhoanNavigation)
                    .FirstOrDefaultAsync(t => t.MaThanhVien == maThanhVien);
                if (tv != null)
                {
                    _context.Thongbaos.Add(new Thongbao
                    {
                        MaThongBao = Guid.NewGuid().ToString("N")[..10].ToUpper(),
                        MaTaiKhoan = tv.MaTaiKhoan,
                        NoiDung = $"Bạn được phân công công việc \"{tenCongViec}\" trong dự án \"{duAn.TenDuAn}\"",
                        TrangThai = "ChuaDoc",
                        ThoiGian = DateTime.Now
                    });
                    await _context.SaveChangesAsync();
                }
            }

            TempData["Success"] = "Thêm công việc thành công!";
            return RedirectToAction("Details", "DuAn", new { id = maDuAn });
        }

        // ========== UC#12: PHÂN CÔNG CÔNG VIỆC ==========
        [HttpPost]
        public async Task<IActionResult> Assign(string maCongViec, string maThanhVien)
        {
            var cv = await _context.Congviecs.Include(c => c.MaDuAnNavigation).FirstOrDefaultAsync(c => c.MaCongViec == maCongViec);
            if (cv == null) return NotFound();

            cv.MaThanhVien = maThanhVien;
            await _context.SaveChangesAsync();

            // Notification
            var tv = await _context.Thanhviens.FindAsync(maThanhVien);
            if (tv != null)
            {
                _context.Thongbaos.Add(new Thongbao
                {
                    MaThongBao = Guid.NewGuid().ToString("N")[..10].ToUpper(),
                    MaTaiKhoan = tv.MaTaiKhoan,
                    NoiDung = $"Bạn được phân công công việc \"{cv.TenCongViec}\"",
                    TrangThai = "ChuaDoc",
                    ThoiGian = DateTime.Now
                });
                await _context.SaveChangesAsync();
            }

            return Json(new { success = true });
        }

        // ========== UC#13: XÓA CÔNG VIỆC ==========
        [HttpPost]
        public async Task<IActionResult> Delete(string id, string maDuAn)
        {
            var cv = await _context.Congviecs
                .Include(c => c.Binhluans)
                .Include(c => c.Tepdinhkems)
                .FirstOrDefaultAsync(c => c.MaCongViec == id);

            if (cv != null)
            {
                _context.Binhluans.RemoveRange(cv.Binhluans);
                _context.Tepdinhkems.RemoveRange(cv.Tepdinhkems);
                _context.Congviecs.Remove(cv);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Xóa công việc thành công!";
            }

            return RedirectToAction("Details", "DuAn", new { id = maDuAn });
        }

        // ========== UC#21: KANBAN BOARD ==========
        public async Task<IActionResult> Board(string maDuAn)
        {
            var duAn = await _context.Duans
                .Include(d => d.Congviecs).ThenInclude(c => c.MaThanhVienNavigation)
                    .ThenInclude(tv => tv!.MaTaiKhoanNavigation)
                .Include(d => d.Thanhviens).ThenInclude(tv => tv.MaTaiKhoanNavigation)
                .FirstOrDefaultAsync(d => d.MaDuAn == maDuAn);

            if (duAn == null) return RedirectToAction("Index", "DuAn");

            return View(duAn);
        }

        // API for drag-drop status update
        [HttpPost]
        public async Task<IActionResult> UpdateStatus([FromBody] UpdateStatusRequest request)
        {
            var cv = await _context.Congviecs.FindAsync(request.MaCongViec);
            if (cv == null) return NotFound();

            cv.TrangThai = request.TrangThai;
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }

        public class UpdateStatusRequest
        {
            public string MaCongViec { get; set; } = "";
            public string TrangThai { get; set; } = "";
        }
    }
}
