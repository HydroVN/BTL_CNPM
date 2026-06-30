using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using QuanLyDuAn.Data;
using QuanLyDuAn.Filters;
using QuanLyDuAn.Models;

namespace QuanLyDuAn.Controllers
{
    public class ThanhVienController : Controller
    {
        private readonly BtlCnpmContext _context;
        private readonly PasswordHasher<Taikhoan> _hasher = new();

        public ThanhVienController(BtlCnpmContext context)
        {
            _context = context;
        }

        // ========== UC#14: QUẢN LÝ THÀNH VIÊN WORKSPACE ==========
        [AuthFilter]
        public async Task<IActionResult> Index()
        {
            var maTk = HttpContext.Session.GetString("MaTaiKhoan")!;
            var vaiTro = HttpContext.Session.GetString("VaiTro");

            var workspaces = vaiTro == "Admin"
                ? await _context.Workspaces.ToListAsync()
                : await _context.Workspaces.Where(w => w.MaTaiKhoan == maTk).ToListAsync();

            ViewBag.Workspaces = workspaces;

            // Get all members across user's workspaces (through project memberships)
            var wsIds = workspaces.Select(w => w.MaWorkspace).ToList();
            var members = await _context.Thanhviens
                .Include(tv => tv.MaTaiKhoanNavigation)
                .Include(tv => tv.MaDuAnNavigation)
                .Where(tv => wsIds.Contains(tv.MaDuAnNavigation.MaWorkspace))
                .ToListAsync();

            // Get invite links
            ViewBag.InviteLinks = await _context.Invitelinks
                .Where(i => wsIds.Contains(i.MaWorkspace))
                .OrderByDescending(i => i.NgayHetHan)
                .ToListAsync();

            return View(members);
        }

        // Generate Invite Link
        [AuthFilter]
        [HttpPost]
        public async Task<IActionResult> GenerateInviteLink(string maWorkspace, string vaiTroMacDinh)
        {
            var maTk = HttpContext.Session.GetString("MaTaiKhoan")!;
            var vaiTro = HttpContext.Session.GetString("VaiTro");

            var ws = await _context.Workspaces.FindAsync(maWorkspace);
            if (ws == null || (ws.MaTaiKhoan != maTk && vaiTro != "Admin"))
            {
                return Forbid();
            }

            var token = Guid.NewGuid().ToString("N");
            var invite = new Invitelink
            {
                MaLink = Guid.NewGuid().ToString("N")[..10].ToUpper(),
                MaWorkspace = maWorkspace,
                Token = token,
                VaiTroMacDinh = vaiTroMacDinh,
                NgayHetHan = QuanLyDuAn.Helpers.VnDateTime.Now.AddDays(7)
            };

            _context.Invitelinks.Add(invite);
            await _context.SaveChangesAsync();

            var url = $"{Request.Scheme}://{Request.Host}/ThanhVien/Join?token={token}";
            TempData["InviteLink"] = url;
            TempData["Success"] = "Đường dẫn mời đã được tạo thành công!";

            return RedirectToAction("Index");
        }

        // Join by invite link
        [HttpGet]
        public async Task<IActionResult> Join(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("ExpiredLink");
            }

            var invite = await _context.Invitelinks
                .Include(i => i.MaWorkspaceNavigation)
                    .ThenInclude(w => w.MaTaiKhoanNavigation)
                .FirstOrDefaultAsync(i => i.Token == token);

            if (invite == null || invite.NgayHetHan < QuanLyDuAn.Helpers.VnDateTime.Now)
            {
                return RedirectToAction("ExpiredLink");
            }

            // Check if user is logged in
            var maTaiKhoan = HttpContext.Session.GetString("MaTaiKhoan");
            if (string.IsNullOrEmpty(maTaiKhoan))
            {
                // Not logged in, redirect to register page with token
                return RedirectToAction("Register", "Account", new { token = token });
            }

            // Already logged in: show confirm invitation page
            return View(invite);
        }

        [HttpPost]
        public async Task<IActionResult> AcceptJoin(string token)
        {
            var maTaiKhoan = HttpContext.Session.GetString("MaTaiKhoan");
            if (string.IsNullOrEmpty(maTaiKhoan))
            {
                return RedirectToAction("Login", "Account");
            }

            var invite = await _context.Invitelinks
                .Include(i => i.MaWorkspaceNavigation)
                .FirstOrDefaultAsync(i => i.Token == token);

            if (invite == null || invite.NgayHetHan < QuanLyDuAn.Helpers.VnDateTime.Now)
            {
                return RedirectToAction("ExpiredLink");
            }

            // Add this user to all projects in this Workspace
            var projects = await _context.Duans
                .Where(d => d.MaWorkspace == invite.MaWorkspace)
                .ToListAsync();

            int addedCount = 0;
            foreach (var proj in projects)
            {
                // Check if already a member of this project
                var isMember = await _context.Thanhviens
                    .AnyAsync(tv => tv.MaDuAn == proj.MaDuAn && tv.MaTaiKhoan == maTaiKhoan);

                if (!isMember)
                {
                    var tv = new Thanhvien
                    {
                        MaThanhVien = Guid.NewGuid().ToString("N")[..10].ToUpper(),
                        MaDuAn = proj.MaDuAn,
                        MaTaiKhoan = maTaiKhoan,
                        VaiTroDuAn = invite.VaiTroMacDinh,
                        NgayThamGia = QuanLyDuAn.Helpers.VnDateTime.Now
                    };
                    _context.Thanhviens.Add(tv);
                    addedCount++;
                }
            }

            if (addedCount > 0)
            {
                await _context.SaveChangesAsync();
            }

            TempData["Success"] = $"Bạn đã tham gia thành công vào Không gian làm việc '{invite.MaWorkspaceNavigation.TenWorkspace}'!";
            return RedirectToAction("Index", "Workspace");
        }

        // Expired link page
        public IActionResult ExpiredLink()
        {
            return View();
        }

        // ========== UC#15: THÊM THÀNH VIÊN VÀO DỰ ÁN ==========
        [AuthFilter]
        [HttpGet]
        public async Task<IActionResult> AddToProject(string maDuAn)
        {
            var duAn = await _context.Duans
                .Include(d => d.MaWorkspaceNavigation)
                .Include(d => d.Thanhviens)
                .FirstOrDefaultAsync(d => d.MaDuAn == maDuAn);

            if (duAn == null) return RedirectToAction("Index", "DuAn");

            var maTk = HttpContext.Session.GetString("MaTaiKhoan")!;
            var vaiTro = HttpContext.Session.GetString("VaiTro");

            // Check if current user is owner of the workspace or is Admin
            var isOwner = duAn.MaWorkspaceNavigation.MaTaiKhoan == maTk;
            if (!isOwner && vaiTro != "Admin")
            {
                TempData["Error"] = "Bạn không có quyền mời thành viên vào dự án này.";
                return RedirectToAction("Details", "DuAn", new { id = maDuAn });
            }

            ViewBag.DuAn = duAn;
            return View();
        }

        [AuthFilter]
        [HttpGet]
        public async Task<IActionResult> CheckEmail(string email, string maDuAn)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return Json(new { success = false, message = "Email không được để trống" });
            }

            var myEmail = HttpContext.Session.GetString("Email");
            if (!string.IsNullOrEmpty(myEmail) && email.Equals(myEmail, StringComparison.OrdinalIgnoreCase))
            {
                return Json(new { success = false, message = "Bạn không thể mời chính bản thân mình vào dự án." });
            }

            var user = await _context.Taikhoans.FirstOrDefaultAsync(t => t.Email == email);
            if (user == null)
            {
                return Json(new { success = false, message = "Email này chưa đăng ký tài khoản trên hệ thống." });
            }

            // Check if already in the project
            var isMember = await _context.Thanhviens.AnyAsync(tv => tv.MaDuAn == maDuAn && tv.MaTaiKhoan == user.MaTaiKhoan);
            if (isMember)
            {
                return Json(new { success = false, message = "Người dùng này đã là thành viên của dự án." });
            }

            return Json(new { success = true, name = user.HoTen, email = user.Email });
        }

        [AuthFilter]
        [HttpPost]
        public async Task<IActionResult> SendProjectInvite(string maDuAn, string email, string vaiTroDuAn)
        {
            var duAn = await _context.Duans
                .Include(d => d.MaWorkspaceNavigation)
                .FirstOrDefaultAsync(d => d.MaDuAn == maDuAn);
            if (duAn == null) return RedirectToAction("Index", "DuAn");

            var maTk = HttpContext.Session.GetString("MaTaiKhoan")!;
            var vaiTro = HttpContext.Session.GetString("VaiTro");

            var isOwner = duAn.MaWorkspaceNavigation.MaTaiKhoan == maTk;
            if (!isOwner && vaiTro != "Admin")
            {
                TempData["Error"] = "Bạn không có quyền mời thành viên.";
                return RedirectToAction("Details", "DuAn", new { id = maDuAn });
            }

            var myEmail = HttpContext.Session.GetString("Email");
            if (!string.IsNullOrEmpty(myEmail) && email.Equals(myEmail, StringComparison.OrdinalIgnoreCase))
            {
                TempData["Error"] = "Bạn không thể mời chính bản thân mình vào dự án.";
                return RedirectToAction("Details", "DuAn", new { id = maDuAn });
            }

            var user = await _context.Taikhoans.FirstOrDefaultAsync(t => t.Email == email);
            if (user == null)
            {
                TempData["Error"] = "Email không tồn tại.";
                return RedirectToAction("Details", "DuAn", new { id = maDuAn });
            }

            var isMember = await _context.Thanhviens.AnyAsync(tv => tv.MaDuAn == maDuAn && tv.MaTaiKhoan == user.MaTaiKhoan);
            if (isMember)
            {
                TempData["Error"] = "Người dùng đã là thành viên của dự án.";
                return RedirectToAction("Details", "DuAn", new { id = maDuAn });
            }

            var hoTenNguoiMoi = HttpContext.Session.GetString("HoTen") ?? "Quản lý";
            var content = $"PROJECT_INVITE|{maDuAn}|{vaiTroDuAn}|Bạn được mời tham gia dự án '{duAn.TenDuAn}' bởi {hoTenNguoiMoi} với vai trò {vaiTroDuAn}.";

            var thongBao = new Thongbao
            {
                MaThongBao = Guid.NewGuid().ToString("N")[..10].ToUpper(),
                MaTaiKhoan = user.MaTaiKhoan,
                NoiDung = content,
                TrangThai = "ChuaDoc",
                ThoiGian = QuanLyDuAn.Helpers.VnDateTime.Now
            };

            _context.Thongbaos.Add(thongBao);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Đã gửi lời mời tham gia dự án tới {user.HoTen} ({email}).";
            return RedirectToAction("Details", "DuAn", new { id = maDuAn });
        }

        [AuthFilter]
        [HttpPost]
        public async Task<IActionResult> AcceptProjectInvite(string notificationId)
        {
            var thongBao = await _context.Thongbaos.FindAsync(notificationId);
            if (thongBao == null)
            {
                TempData["Error"] = "Thông báo không tồn tại.";
                return RedirectToAction("Index", "Workspace");
            }

            var maTk = HttpContext.Session.GetString("MaTaiKhoan")!;
            if (thongBao.MaTaiKhoan != maTk)
            {
                return Forbid();
            }

            var parts = thongBao.NoiDung.Split('|');
            if (parts.Length >= 4 && parts[0] == "PROJECT_INVITE")
            {
                var maDuAn = parts[1];
                var vaiTroDuAn = parts[2];

                var exists = await _context.Thanhviens.AnyAsync(tv => tv.MaDuAn == maDuAn && tv.MaTaiKhoan == maTk);
                if (!exists)
                {
                    var tv = new Thanhvien
                    {
                        MaThanhVien = Guid.NewGuid().ToString("N")[..10].ToUpper(),
                        MaDuAn = maDuAn,
                        MaTaiKhoan = maTk,
                        VaiTroDuAn = vaiTroDuAn,
                        NgayThamGia = QuanLyDuAn.Helpers.VnDateTime.Now
                    };
                    _context.Thanhviens.Add(tv);
                }

                thongBao.TrangThai = "DaDoc";
                thongBao.NoiDung = $"Bạn đã chấp nhận tham gia dự án.";
                await _context.SaveChangesAsync();

                TempData["Success"] = "Bạn đã tham gia dự án thành công!";
            }

            return RedirectToAction("Index", "Workspace");
        }

        [AuthFilter]
        [HttpPost]
        public async Task<IActionResult> DeclineProjectInvite(string notificationId)
        {
            var thongBao = await _context.Thongbaos.FindAsync(notificationId);
            if (thongBao == null)
            {
                TempData["Error"] = "Thông báo không tồn tại.";
                return RedirectToAction("Index", "Workspace");
            }

            var maTk = HttpContext.Session.GetString("MaTaiKhoan")!;
            if (thongBao.MaTaiKhoan != maTk)
            {
                return Forbid();
            }

            thongBao.TrangThai = "DaDoc";
            thongBao.NoiDung = "Bạn đã từ chối lời mời tham gia dự án.";
            await _context.SaveChangesAsync();

            TempData["Success"] = "Đã từ chối lời mời.";
            return RedirectToAction("Index", "Workspace");
        }

        // ========== ADMIN: QUẢN TRỊ TÀI KHOẢN ==========
        [AuthFilter]
        [HttpGet]
        public async Task<IActionResult> AdminUsers()
        {
            var vaiTro = HttpContext.Session.GetString("VaiTro");
            if (vaiTro != "Admin")
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            var users = await _context.Taikhoans
                .Include(t => t.MaGoiNavigation)
                .Where(t => t.VaiTro != "Admin")
                .OrderByDescending(t => t.NgayTao)
                .ToListAsync();

            return View(users);
        }

        [AuthFilter]
        [HttpPost]
        public async Task<IActionResult> AdminResetPassword(string maTaiKhoan, string newPassword)
        {
            var vaiTro = HttpContext.Session.GetString("VaiTro");
            if (vaiTro != "Admin")
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            if (string.IsNullOrWhiteSpace(newPassword))
            {
                TempData["Error"] = "Mật khẩu mới không được để trống.";
                return RedirectToAction("AdminUsers");
            }

            var user = await _context.Taikhoans.FindAsync(maTaiKhoan);
            if (user == null)
            {
                TempData["Error"] = "Tài khoản không tồn tại.";
                return RedirectToAction("AdminUsers");
            }

            if (user.VaiTro == "Admin")
            {
                TempData["Error"] = "Không được phép thay đổi mật khẩu của quản trị viên khác.";
                return RedirectToAction("AdminUsers");
            }

            user.MatKhau = _hasher.HashPassword(user, newPassword);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Đã đặt lại mật khẩu cho tài khoản {user.HoTen} ({user.Email}) thành công!";
            return RedirectToAction("AdminUsers");
        }

        [AuthFilter]
        [HttpPost]
        public async Task<IActionResult> AdminDeleteUser(string maTaiKhoan)
        {
            var vaiTro = HttpContext.Session.GetString("VaiTro");
            if (vaiTro != "Admin")
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            var user = await _context.Taikhoans.FindAsync(maTaiKhoan);
            if (user == null)
            {
                TempData["Error"] = "Tài khoản không tồn tại.";
                return RedirectToAction("AdminUsers");
            }

            if (user.VaiTro == "Admin")
            {
                TempData["Error"] = "Không được phép xóa tài khoản quản trị viên khác.";
                return RedirectToAction("AdminUsers");
            }

            // Remove comments of this user
            var comments = await _context.Binhluans.Where(b => b.MaTaiKhoan == maTaiKhoan).ToListAsync();
            _context.Binhluans.RemoveRange(comments);

            // Remove member records in projects
            var memberRecords = await _context.Thanhviens.Where(tv => tv.MaTaiKhoan == maTaiKhoan).ToListAsync();
            _context.Thanhviens.RemoveRange(memberRecords);

            // Remove notifications
            var notifications = await _context.Thongbaos.Where(n => n.MaTaiKhoan == maTaiKhoan).ToListAsync();
            _context.Thongbaos.RemoveRange(notifications);

            // Remove workspaces owned by this user
            var workspaces = await _context.Workspaces
                .Include(w => w.Duans).ThenInclude(d => d.Congviecs).ThenInclude(c => c.Binhluans)
                .Include(w => w.Duans).ThenInclude(d => d.Congviecs).ThenInclude(c => c.Tepdinhkems)
                .Include(w => w.Duans).ThenInclude(d => d.Thanhviens)
                .Where(w => w.MaTaiKhoan == maTaiKhoan)
                .ToListAsync();

            foreach (var ws in workspaces)
            {
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
            }

            _context.Taikhoans.Remove(user);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Đã xóa tài khoản {user.HoTen} ({user.Email}) thành công!";
            return RedirectToAction("AdminUsers");
        }

        // ========== UC#21: SỬA VAI TRÒ THÀNH VIÊN TRONG DỰ ÁN ==========
        [AuthFilter]
        [HttpPost]
        public async Task<IActionResult> UpdateMemberRole(string maThanhVien, string maDuAn, string vaiTroDuAn)
        {
            var tv = await _context.Thanhviens.FindAsync(maThanhVien);
            if (tv == null)
            {
                TempData["Error"] = "Thành viên không tồn tại.";
                return RedirectToAction("Details", "DuAn", new { id = maDuAn });
            }

            var duAn = await _context.Duans
                .Include(d => d.MaWorkspaceNavigation)
                .FirstOrDefaultAsync(d => d.MaDuAn == maDuAn);
            if (duAn == null) return RedirectToAction("Index", "DuAn");

            var maTk = HttpContext.Session.GetString("MaTaiKhoan")!;
            var vaiTro = HttpContext.Session.GetString("VaiTro");

            var isOwner = duAn.MaWorkspaceNavigation.MaTaiKhoan == maTk;
            if (!isOwner && vaiTro != "Admin")
            {
                TempData["Error"] = "Bạn không có quyền chỉnh sửa vai trò thành viên.";
                return RedirectToAction("Details", "DuAn", new { id = maDuAn });
            }

            tv.VaiTroDuAn = vaiTroDuAn;
            await _context.SaveChangesAsync();

            TempData["Success"] = "Cập nhật vai trò thành viên thành công!";
            return RedirectToAction("Details", "DuAn", new { id = maDuAn });
        }

        // ========== UC#22: XÓA THÀNH VIÊN KHỎI DỰ ÁN ==========
        [AuthFilter]
        [HttpPost]
        public async Task<IActionResult> RemoveMember(string maThanhVien, string maDuAn)
        {
            var tv = await _context.Thanhviens.FindAsync(maThanhVien);
            if (tv == null)
            {
                TempData["Error"] = "Thành viên không tồn tại.";
                return RedirectToAction("Details", "DuAn", new { id = maDuAn });
            }

            var duAn = await _context.Duans
                .Include(d => d.MaWorkspaceNavigation)
                .FirstOrDefaultAsync(d => d.MaDuAn == maDuAn);
            if (duAn == null) return RedirectToAction("Index", "DuAn");

            var maTk = HttpContext.Session.GetString("MaTaiKhoan")!;
            var vaiTro = HttpContext.Session.GetString("VaiTro");

            var isOwner = duAn.MaWorkspaceNavigation.MaTaiKhoan == maTk;
            if (!isOwner && vaiTro != "Admin")
            {
                TempData["Error"] = "Bạn không có quyền xóa thành viên.";
                return RedirectToAction("Details", "DuAn", new { id = maDuAn });
            }

            // Do not allow deleting the workspace creator
            if (tv.MaTaiKhoan == duAn.MaWorkspaceNavigation.MaTaiKhoan)
            {
                TempData["Error"] = "Không thể xóa chủ sở hữu dự án.";
                return RedirectToAction("Details", "DuAn", new { id = maDuAn });
            }

            // Unassign tasks assigned to this member in this project to prevent foreign key errors
            var tasks = await _context.Congviecs
                .Where(c => c.MaDuAn == maDuAn && c.MaThanhVien == maThanhVien)
                .ToListAsync();
            foreach (var task in tasks)
            {
                task.MaThanhVien = null;
            }

            _context.Thanhviens.Remove(tv);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Đã xóa thành viên khỏi dự án thành công.";
            return RedirectToAction("Details", "DuAn", new { id = maDuAn });
        }

        [AuthFilter]
        [HttpPost]
        public async Task<IActionResult> DeleteNotification(string notificationId)
        {
            var thongBao = await _context.Thongbaos.FindAsync(notificationId);
            if (thongBao == null)
            {
                return Json(new { success = false, message = "Thông báo không tồn tại." });
            }

            var maTk = HttpContext.Session.GetString("MaTaiKhoan")!;
            if (thongBao.MaTaiKhoan != maTk)
            {
                return Forbid();
            }

            // If it is a pending invite, block deletion
            if (thongBao.NoiDung.StartsWith("PROJECT_INVITE|"))
            {
                return Json(new { success = false, message = "Không thể xóa lời mời tham gia dự án đang chờ xử lý." });
            }

            _context.Thongbaos.Remove(thongBao);
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }
    }
}
