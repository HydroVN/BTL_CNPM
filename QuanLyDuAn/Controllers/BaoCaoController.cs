using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyDuAn.Data;
using QuanLyDuAn.Filters;

namespace QuanLyDuAn.Controllers
{
    [AuthFilter]
    public class BaoCaoController : Controller
    {
        private readonly BtlCnpmContext _context;

        public BaoCaoController(BtlCnpmContext context)
        {
            _context = context;
        }

        // ========== UC#16: BÁO CÁO TIẾN ĐỘ DỰ ÁN ==========
        public async Task<IActionResult> TienDo(string? maDuAn)
        {
            var maTk = HttpContext.Session.GetString("MaTaiKhoan")!;
            var vaiTro = HttpContext.Session.GetString("VaiTro");

            var wsIds = vaiTro == "Admin"
                ? await _context.Workspaces.Select(w => w.MaWorkspace).ToListAsync()
                : await _context.Workspaces.Where(w => w.MaTaiKhoan == maTk).Select(w => w.MaWorkspace).ToListAsync();

            var projects = await _context.Duans
                .Include(d => d.Congviecs)
                .Where(d => wsIds.Contains(d.MaWorkspace))
                .ToListAsync();

            ViewBag.Projects = projects;
            ViewBag.SelectedProject = maDuAn;

            if (!string.IsNullOrEmpty(maDuAn))
            {
                var selected = projects.FirstOrDefault(p => p.MaDuAn == maDuAn);
                ViewBag.SelectedProjectData = selected;
            }

            // Overall stats
            var allTasks = projects.SelectMany(p => p.Congviecs).ToList();
            ViewBag.TotalTasks = allTasks.Count;
            ViewBag.TodoCount = allTasks.Count(c => c.TrangThai == "ToDo");
            ViewBag.InProgressCount = allTasks.Count(c => c.TrangThai == "InProgress");
            ViewBag.ReviewCount = allTasks.Count(c => c.TrangThai == "Review");
            ViewBag.DoneCount = allTasks.Count(c => c.TrangThai == "Done");

            return View();
        }

        // ========== UC#17: BÁO CÁO HIỆU SUẤT THÀNH VIÊN ==========
        public async Task<IActionResult> HieuSuat(string? maDuAn)
        {
            var maTk = HttpContext.Session.GetString("MaTaiKhoan")!;
            var vaiTro = HttpContext.Session.GetString("VaiTro");

            var wsIds = vaiTro == "Admin"
                ? await _context.Workspaces.Select(w => w.MaWorkspace).ToListAsync()
                : await _context.Workspaces.Where(w => w.MaTaiKhoan == maTk).Select(w => w.MaWorkspace).ToListAsync();

            var projects = await _context.Duans
                .Where(d => wsIds.Contains(d.MaWorkspace))
                .ToListAsync();
            ViewBag.Projects = projects;
            ViewBag.SelectedProject = maDuAn;

            // Get member performance data
            var query = _context.Thanhviens
                .Include(tv => tv.MaTaiKhoanNavigation)
                .Include(tv => tv.Congviecs)
                .Include(tv => tv.MaDuAnNavigation)
                .Where(tv => wsIds.Contains(tv.MaDuAnNavigation.MaWorkspace));

            if (!string.IsNullOrEmpty(maDuAn))
                query = query.Where(tv => tv.MaDuAn == maDuAn);

            var members = await query.ToListAsync();

            var memberStats = members.Select(m => new MemberPerformanceDto
            {
                HoTen = m.MaTaiKhoanNavigation.HoTen,
                DuAn = m.MaDuAnNavigation.TenDuAn,
                TotalTasks = m.Congviecs.Count,
                DoneTasks = m.Congviecs.Count(c => c.TrangThai == "Done"),
                OverdueTasks = m.Congviecs.Count(c => c.Deadline.HasValue && c.Deadline < QuanLyDuAn.Helpers.VnDateTime.Now && c.TrangThai != "Done"),
                InProgressTasks = m.Congviecs.Count(c => c.TrangThai == "InProgress")
            }).ToList();

            ViewBag.MemberStats = memberStats;

            return View();
        }

        // ========== UC#18: XUẤT EXCEL ==========
        public async Task<IActionResult> ExportExcel(string? maDuAn)
        {
            var maTk = HttpContext.Session.GetString("MaTaiKhoan")!;
            var vaiTro = HttpContext.Session.GetString("VaiTro");

            var wsIds = vaiTro == "Admin"
                ? await _context.Workspaces.Select(w => w.MaWorkspace).ToListAsync()
                : await _context.Workspaces.Where(w => w.MaTaiKhoan == maTk).Select(w => w.MaWorkspace).ToListAsync();

            var query = _context.Congviecs
                .Include(c => c.MaDuAnNavigation)
                .Include(c => c.MaThanhVienNavigation).ThenInclude(tv => tv!.MaTaiKhoanNavigation)
                .Where(c => wsIds.Contains(c.MaDuAnNavigation.MaWorkspace));

            if (!string.IsNullOrEmpty(maDuAn))
                query = query.Where(c => c.MaDuAn == maDuAn);

            var tasks = await query.OrderBy(c => c.MaDuAn).ThenBy(c => c.TrangThai).ToListAsync();

            using var workbook = new XLWorkbook();
            var ws = workbook.AddWorksheet("Báo cáo công việc");

            // Header
            ws.Cell(1, 1).Value = "Mã CV";
            ws.Cell(1, 2).Value = "Tên công việc";
            ws.Cell(1, 3).Value = "Dự án";
            ws.Cell(1, 4).Value = "Người phụ trách";
            ws.Cell(1, 5).Value = "Trạng thái";
            ws.Cell(1, 6).Value = "Ưu tiên";
            ws.Cell(1, 7).Value = "Deadline";
            ws.Cell(1, 8).Value = "Ngày tạo";

            var headerRange = ws.Range(1, 1, 1, 8);
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.LightBlue;

            for (int i = 0; i < tasks.Count; i++)
            {
                var t = tasks[i];
                ws.Cell(i + 2, 1).Value = t.MaCongViec;
                ws.Cell(i + 2, 2).Value = t.TenCongViec;
                ws.Cell(i + 2, 3).Value = t.MaDuAnNavigation.TenDuAn;
                ws.Cell(i + 2, 4).Value = t.MaThanhVienNavigation?.MaTaiKhoanNavigation?.HoTen ?? "Chưa phân công";
                ws.Cell(i + 2, 5).Value = t.TrangThai;
                ws.Cell(i + 2, 6).Value = t.MucDoUuTien;
                ws.Cell(i + 2, 7).Value = t.Deadline?.ToString("dd/MM/yyyy") ?? "";
                ws.Cell(i + 2, 8).Value = t.NgayTao.ToString("dd/MM/yyyy");
            }

            ws.Columns().AdjustToContents();

            var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Position = 0;

            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"BaoCao_{QuanLyDuAn.Helpers.VnDateTime.Now:yyyyMMdd_HHmmss}.xlsx");
        }

        // ========== UC#18: XUẤT PDF (HTML-based) ==========
        public async Task<IActionResult> ExportPdf(string? maDuAn)
        {
            var maTk = HttpContext.Session.GetString("MaTaiKhoan")!;
            var wsIds = await _context.Workspaces
                .Where(w => w.MaTaiKhoan == maTk)
                .Select(w => w.MaWorkspace).ToListAsync();

            var query = _context.Congviecs
                .Include(c => c.MaDuAnNavigation)
                .Include(c => c.MaThanhVienNavigation).ThenInclude(tv => tv!.MaTaiKhoanNavigation)
                .Where(c => wsIds.Contains(c.MaDuAnNavigation.MaWorkspace));

            if (!string.IsNullOrEmpty(maDuAn))
                query = query.Where(c => c.MaDuAn == maDuAn);

            var tasks = await query.OrderBy(c => c.MaDuAn).ToListAsync();

            return View("ExportPdf", tasks);
        }

        public class MemberPerformanceDto
        {
            public string HoTen { get; set; } = "";
            public string DuAn { get; set; } = "";
            public int TotalTasks { get; set; }
            public int DoneTasks { get; set; }
            public int OverdueTasks { get; set; }
            public int InProgressTasks { get; set; }
        }
    }
}
