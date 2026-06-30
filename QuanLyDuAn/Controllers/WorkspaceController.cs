using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyDuAn.Data;
using QuanLyDuAn.Filters;
using QuanLyDuAn.Models;

namespace QuanLyDuAn.Controllers
{
    [AuthFilter]
    public class WorkspaceController : Controller
    {
        private readonly BtlCnpmContext _context;

        public WorkspaceController(BtlCnpmContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var maTk = HttpContext.Session.GetString("MaTaiKhoan")!;
            var workspaces = await _context.Workspaces
                .Include(w => w.Duans)
                .Where(w => w.MaTaiKhoan == maTk || w.Duans.Any(d => d.Thanhviens.Any(tv => tv.MaTaiKhoan == maTk)))
                .OrderByDescending(w => w.NgayTao)
                .ToListAsync();

            return View(workspaces);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(string tenWorkspace, string? moTa)
        {
            if (string.IsNullOrWhiteSpace(tenWorkspace))
            {
                ViewBag.Error = "Tên Workspace không được để trống";
                return View();
            }

            var maTk = HttpContext.Session.GetString("MaTaiKhoan")!;

            // Check subscription limit for Workspace creation
            var user = await _context.Taikhoans
                .Include(t => t.MaGoiNavigation)
                .FirstOrDefaultAsync(t => t.MaTaiKhoan == maTk);

            string maGoi = user?.MaGoi ?? "FREE";
            int maxWorkspaces = maGoi switch
            {
                "FREE" => 1,
                "PRO" => 5,
                "ENT" => 9999,
                _ => 1
            };

            var currentWorkspaceCount = await _context.Workspaces.CountAsync(w => w.MaTaiKhoan == maTk);
            if (currentWorkspaceCount >= maxWorkspaces)
            {
                ViewBag.Error = $"Gói dịch vụ hiện tại chỉ cho phép tạo tối đa {maxWorkspaces} Workspace.";
                return View();
            }

            var ws = new Workspace
            {
                MaWorkspace = Guid.NewGuid().ToString("N")[..10].ToUpper(),
                MaTaiKhoan = maTk,
                TenWorkspace = tenWorkspace,
                MoTa = moTa,
                NgayTao = QuanLyDuAn.Helpers.VnDateTime.Now
            };

            _context.Workspaces.Add(ws);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Tạo Workspace thành công!";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            var ws = await _context.Workspaces
                .Include(w => w.Duans).ThenInclude(d => d.Congviecs).ThenInclude(c => c.Binhluans)
                .Include(w => w.Duans).ThenInclude(d => d.Congviecs).ThenInclude(c => c.Tepdinhkems)
                .Include(w => w.Duans).ThenInclude(d => d.Thanhviens)
                .FirstOrDefaultAsync(w => w.MaWorkspace == id);

            if (ws == null)
            {
                TempData["Error"] = "Workspace không tồn tại.";
                return RedirectToAction("Index");
            }

            var maTk = HttpContext.Session.GetString("MaTaiKhoan")!;
            var vaiTro = HttpContext.Session.GetString("VaiTro");

            if (ws.MaTaiKhoan != maTk && vaiTro != "Admin")
            {
                TempData["Error"] = "Bạn không có quyền xóa Workspace này.";
                return RedirectToAction("Index");
            }

            // Remove all projects, tasks, comments, files, and members inside this workspace
            foreach (var duAn in ws.Duans)
            {
                foreach (var cv in duAn.Congviecs)
                {
                    _context.Binhluans.RemoveRange(cv.Binhluans);
                    _context.Tepdinhkems.RemoveRange(cv.Tepdinhkems);
                }
                _context.Congviecs.RemoveRange(duAn.Congviecs);
                _context.Thanhviens.RemoveRange(duAn.Thanhviens);
            }
            _context.Duans.RemoveRange(ws.Duans);
            _context.Workspaces.Remove(ws);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Đã xóa Workspace thành công!";
            return RedirectToAction("Index");
        }
    }
}
