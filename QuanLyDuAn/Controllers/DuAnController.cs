using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyDuAn.Data;
using QuanLyDuAn.Filters;
using QuanLyDuAn.Models;

namespace QuanLyDuAn.Controllers
{
    [AuthFilter]
    public class DuAnController : Controller
    {
        private readonly BtlCnpmContext _context;

        public DuAnController(BtlCnpmContext context)
        {
            _context = context;
        }

        // ========== UC#09: DANH SÁCH + TÌM KIẾM DỰ ÁN ==========
        public async Task<IActionResult> Index(string? maWorkspace, string? search, string? trangThai)
        {
            var maTk = HttpContext.Session.GetString("MaTaiKhoan")!;

            // Get user's workspaces (owned + joined)
            var workspaces = await _context.Workspaces
                .Where(w => w.MaTaiKhoan == maTk || w.Duans.Any(d => d.Thanhviens.Any(tv => tv.MaTaiKhoan == maTk)))
                .ToListAsync();

            ViewBag.Workspaces = workspaces;
            ViewBag.CurrentWorkspace = maWorkspace;
            ViewBag.Search = search;
            ViewBag.TrangThai = trangThai;

            IQueryable<Duan> query = _context.Duans
                .Include(d => d.MaWorkspaceNavigation)
                .Include(d => d.Congviecs)
                .Include(d => d.Thanhviens);

            // Filter by workspace
            if (!string.IsNullOrEmpty(maWorkspace))
            {
                query = query.Where(d => d.MaWorkspace == maWorkspace);
            }
            else
            {
                var wsIds = workspaces.Select(w => w.MaWorkspace).ToList();
                query = query.Where(d => wsIds.Contains(d.MaWorkspace));
            }

            // Search
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(d => d.TenDuAn.Contains(search) || d.MaDuAn.Contains(search));
            }

            // Security: Only see projects in workspaces owned by user OR projects they are a member of
            query = query.Where(d => d.MaWorkspaceNavigation.MaTaiKhoan == maTk || d.Thanhviens.Any(tv => tv.MaTaiKhoan == maTk));

            var projects = await query.OrderByDescending(d => d.NgayBatDau).ToListAsync();

            // Filter by computed status
            if (!string.IsNullOrEmpty(trangThai))
            {
                projects = projects.Where(d => {
                    var total = d.Congviecs.Count;
                    if (total == 0) return d.TrangThai == trangThai;
                    var done = d.Congviecs.Count(c => c.TrangThai == "Done");
                    var computed = (done == total) ? "DaHoanThanh" : "DangThucHien";
                    return computed == trangThai;
                }).ToList();
            }

            return View(projects);
        }

        // ========== UC#06: TẠO DỰ ÁN ==========
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var maTk = HttpContext.Session.GetString("MaTaiKhoan")!;
            ViewBag.Workspaces = await _context.Workspaces
                .Where(w => w.MaTaiKhoan == maTk).ToListAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(string maWorkspace, string tenDuAn, string? moTa,
            DateTime? ngayBatDau, DateTime? ngayKetThuc)
        {
            var maTk = HttpContext.Session.GetString("MaTaiKhoan")!;
            ViewBag.Workspaces = await _context.Workspaces
                .Where(w => w.MaTaiKhoan == maTk).ToListAsync();

            if (string.IsNullOrWhiteSpace(tenDuAn) || string.IsNullOrWhiteSpace(maWorkspace))
            {
                ViewBag.Error = "Vui lòng điền đầy đủ các thông tin có dấu (*)";
                return View();
            }

            if (ngayBatDau.HasValue && ngayKetThuc.HasValue && ngayKetThuc < ngayBatDau)
            {
                ViewBag.Error = "Ngày kết thúc dự án không thể trước ngày bắt đầu";
                return View();
            }

            var duAn = new Duan
            {
                MaDuAn = Guid.NewGuid().ToString("N")[..10].ToUpper(),
                MaWorkspace = maWorkspace,
                TenDuAn = tenDuAn,
                MoTa = moTa,
                NgayBatDau = ngayBatDau,
                NgayKetThuc = ngayKetThuc,
                TrangThai = "ChuaThucHien"
            };

            _context.Duans.Add(duAn);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Tạo dự án thành công!";
            return RedirectToAction("Index", new { maWorkspace });
        }

        // ========== UC#07: SỬA DỰ ÁN ==========
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var duAn = await _context.Duans
                .Include(d => d.MaWorkspaceNavigation)
                .Include(d => d.Congviecs)
                .FirstOrDefaultAsync(d => d.MaDuAn == id);

            if (duAn == null)
            {
                TempData["Error"] = "Dự án không tồn tại hoặc đã bị xóa";
                return RedirectToAction("Index");
            }

            var maTk = HttpContext.Session.GetString("MaTaiKhoan")!;
            var vaiTro = HttpContext.Session.GetString("VaiTro");

            var isOwner = duAn.MaWorkspaceNavigation.MaTaiKhoan == maTk;
            var isPm = await _context.Thanhviens.AnyAsync(tv => tv.MaDuAn == id && tv.MaTaiKhoan == maTk && tv.VaiTroDuAn == "PM");

            if (!isOwner && !isPm && vaiTro != "Admin")
            {
                TempData["Error"] = "Chỉ người quản lý dự án mới có quyền chỉnh sửa dự án.";
                return RedirectToAction("Details", new { id });
            }

            return View(duAn);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string maDuAn, string tenDuAn, string? moTa,
            DateTime? ngayBatDau, DateTime? ngayKetThuc, string trangThai)
        {
            var duAn = await _context.Duans
                .Include(d => d.MaWorkspaceNavigation)
                .Include(d => d.Congviecs)
                .FirstOrDefaultAsync(d => d.MaDuAn == maDuAn);

            if (duAn == null)
            {
                TempData["Error"] = "Dự án không tồn tại hoặc đã bị xóa";
                return RedirectToAction("Index");
            }

            var maTk = HttpContext.Session.GetString("MaTaiKhoan")!;
            var vaiTro = HttpContext.Session.GetString("VaiTro");

            var isOwner = duAn.MaWorkspaceNavigation.MaTaiKhoan == maTk;
            var isPm = await _context.Thanhviens.AnyAsync(tv => tv.MaDuAn == maDuAn && tv.MaTaiKhoan == maTk && tv.VaiTroDuAn == "PM");

            if (!isOwner && !isPm && vaiTro != "Admin")
            {
                TempData["Error"] = "Chỉ người quản lý dự án mới có quyền chỉnh sửa dự án.";
                return RedirectToAction("Details", new { id = maDuAn });
            }

            if (ngayBatDau.HasValue && ngayKetThuc.HasValue && ngayKetThuc < ngayBatDau)
            {
                ViewBag.Error = "Ngày kết thúc dự án không thể trước ngày bắt đầu";
                return View(duAn);
            }

            duAn.TenDuAn = tenDuAn;
            duAn.MoTa = moTa;
            duAn.NgayBatDau = ngayBatDau;
            duAn.NgayKetThuc = ngayKetThuc;

            var totalTasks = duAn.Congviecs.Count;
            if (totalTasks > 0)
            {
                var doneTasks = duAn.Congviecs.Count(c => c.TrangThai == "Done");
                duAn.TrangThai = (doneTasks == totalTasks) ? "DaHoanThanh" : "DangThucHien";
            }
            else
            {
                duAn.TrangThai = trangThai;
            }

            await _context.SaveChangesAsync();

            TempData["Success"] = "Cập nhật dự án thành công!";
            return RedirectToAction("Details", new { id = maDuAn });
        }

        // ========== UC#08: XÓA DỰ ÁN ==========
        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            var duAn = await _context.Duans
                .Include(d => d.MaWorkspaceNavigation)
                .Include(d => d.Congviecs).ThenInclude(c => c.Binhluans)
                .Include(d => d.Congviecs).ThenInclude(c => c.Tepdinhkems)
                .Include(d => d.Thanhviens)
                .FirstOrDefaultAsync(d => d.MaDuAn == id);

            if (duAn == null)
            {
                TempData["Error"] = "Dự án không tồn tại hoặc đã bị xóa";
                return RedirectToAction("Index");
            }

            var maTk = HttpContext.Session.GetString("MaTaiKhoan")!;
            var vaiTro = HttpContext.Session.GetString("VaiTro");

            var isOwner = duAn.MaWorkspaceNavigation.MaTaiKhoan == maTk;
            var isPm = await _context.Thanhviens.AnyAsync(tv => tv.MaDuAn == id && tv.MaTaiKhoan == maTk && tv.VaiTroDuAn == "PM");

            if (!isOwner && !isPm && vaiTro != "Admin")
            {
                TempData["Error"] = "Chỉ người quản lý dự án mới có quyền xóa dự án.";
                return RedirectToAction("Details", new { id });
            }

            // Remove related data
            foreach (var cv in duAn.Congviecs)
            {
                _context.Binhluans.RemoveRange(cv.Binhluans);
                _context.Tepdinhkems.RemoveRange(cv.Tepdinhkems);
            }
            _context.Congviecs.RemoveRange(duAn.Congviecs);
            _context.Thanhviens.RemoveRange(duAn.Thanhviens);
            _context.Duans.Remove(duAn);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Xóa dự án thành công!";
            return RedirectToAction("Index");
        }

        // ========== UC#10: XEM CHI TIẾT DỰ ÁN ==========
        public async Task<IActionResult> Details(string id)
        {
            var duAn = await _context.Duans
                .Include(d => d.MaWorkspaceNavigation)
                .Include(d => d.Congviecs).ThenInclude(c => c.MaThanhVienNavigation)
                    .ThenInclude(tv => tv!.MaTaiKhoanNavigation)
                .Include(d => d.Thanhviens).ThenInclude(tv => tv.MaTaiKhoanNavigation)
                .FirstOrDefaultAsync(d => d.MaDuAn == id);

            if (duAn == null)
            {
                TempData["Error"] = "Dự án không tồn tại";
                return RedirectToAction("Index");
            }

            return View(duAn);
        }

        public static async Task SyncProjectStatusAsync(BtlCnpmContext context, string maDuAn)
        {
            var duAn = await context.Duans
                .Include(d => d.Congviecs)
                .FirstOrDefaultAsync(d => d.MaDuAn == maDuAn);
            if (duAn == null) return;

            var totalTasks = duAn.Congviecs.Count;
            if (totalTasks == 0)
            {
                duAn.TrangThai = "ChuaThucHien";
            }
            else
            {
                var doneTasks = duAn.Congviecs.Count(c => c.TrangThai == "Done");
                if (doneTasks == totalTasks)
                {
                    duAn.TrangThai = "DaHoanThanh";
                }
                else
                {
                    duAn.TrangThai = "DangThucHien";
                }
            }
            await context.SaveChangesAsync();
        }
    }
}
