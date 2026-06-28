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

            var ws = new Workspace
            {
                MaWorkspace = Guid.NewGuid().ToString("N")[..10].ToUpper(),
                MaTaiKhoan = maTk,
                TenWorkspace = tenWorkspace,
                MoTa = moTa,
                NgayTao = DateTime.Now
            };

            _context.Workspaces.Add(ws);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Tạo Workspace thành công!";
            return RedirectToAction("Index");
        }
    }
}
