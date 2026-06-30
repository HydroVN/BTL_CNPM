using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyDuAn.Data;
using QuanLyDuAn.Filters;
using QuanLyDuAn.Models;

namespace QuanLyDuAn.Controllers
{
    public class AccountController : Controller
    {
        private readonly BtlCnpmContext _context;
        private readonly PasswordHasher<Taikhoan> _hasher = new();

        public AccountController(BtlCnpmContext context)
        {
            _context = context;
        }

        // ========== UC#01: ĐĂNG NHẬP ==========
        [HttpGet]
        public IActionResult Login(string? token)
        {
            ViewBag.Token = token;
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("MaTaiKhoan")))
            {
                if (!string.IsNullOrEmpty(token))
                {
                    return RedirectToAction("Join", "ThanhVien", new { token = token });
                }
                return RedirectToAction("Index", "Workspace");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string matKhau, string? token = null)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(matKhau))
            {
                ViewBag.Error = "Tên đăng nhập và mật khẩu không để trống";
                ViewBag.Token = token;
                return View();
            }

            var user = await _context.Taikhoans.FirstOrDefaultAsync(t => t.Email == email);
            if (user == null)
            {
                ViewBag.Error = "Tên đăng nhập hoặc mật khẩu không đúng";
                ViewBag.Token = token;
                return View();
            }

            var result = _hasher.VerifyHashedPassword(user, user.MatKhau, matKhau);
            if (result == PasswordVerificationResult.Failed)
            {
                ViewBag.Error = "Tên đăng nhập hoặc mật khẩu không đúng";
                ViewBag.Token = token;
                return View();
            }

            // Set session
            HttpContext.Session.SetString("MaTaiKhoan", user.MaTaiKhoan);
            HttpContext.Session.SetString("HoTen", user.HoTen);
            HttpContext.Session.SetString("Email", user.Email);
            HttpContext.Session.SetString("VaiTro", user.VaiTro);

            if (!string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Join", "ThanhVien", new { token = token });
            }

            return RedirectToAction("Index", "Workspace");
        }

        // ========== UC#02: ĐĂNG KÝ ==========
        [HttpGet]
        public IActionResult Register(string? token, string? plan)
        {
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("MaTaiKhoan")))
            {
                if (!string.IsNullOrEmpty(token))
                {
                    return RedirectToAction("Join", "ThanhVien", new { token = token });
                }
                return RedirectToAction("Index", "Workspace");
            }
            ViewBag.Token = token;
            ViewBag.Plan = plan;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(string hoTen, string soDienThoai, string email, string matKhau, string? token, string? plan)
        {
            if (string.IsNullOrWhiteSpace(hoTen) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(matKhau))
            {
                ViewBag.Error = "Họ tên, email, mật khẩu không được để trống";
                ViewBag.Token = token;
                ViewBag.Plan = plan;
                return View();
            }

            var exists = await _context.Taikhoans.AnyAsync(t => t.Email == email);
            if (exists)
            {
                ViewBag.Error = "Tài khoản đã tồn tại, vui lòng chọn email khác";
                ViewBag.Token = token;
                ViewBag.Plan = plan;
                return View();
            }

            var planCode = "FREE";
            if (!string.IsNullOrEmpty(plan))
            {
                var planExists = await _context.Goidichvus.AnyAsync(g => g.MaGoi == plan);
                if (planExists) planCode = plan;
            }

            var newUser = new Taikhoan
            {
                MaTaiKhoan = Guid.NewGuid().ToString("N")[..10].ToUpper(),
                MaGoi = planCode,
                HoTen = hoTen,
                Email = email,
                SoDienThoai = soDienThoai,
                VaiTro = "Member",
                TrangThai = "KichHoat",
                NgayTao = DateTime.Now
            };
            newUser.MatKhau = _hasher.HashPassword(newUser, matKhau);

            _context.Taikhoans.Add(newUser);
            await _context.SaveChangesAsync();

            // If invite token exists, auto-join workspace
            if (!string.IsNullOrEmpty(token))
            {
                var invite = await _context.Invitelinks
                    .Include(i => i.MaWorkspaceNavigation)
                    .FirstOrDefaultAsync(i => i.Token == token && i.NgayHetHan > DateTime.Now);

                if (invite != null)
                {
                    newUser.VaiTro = invite.VaiTroMacDinh;
                    await _context.SaveChangesAsync();

                    // Add user to all projects in this Workspace
                    var projects = await _context.Duans
                        .Where(d => d.MaWorkspace == invite.MaWorkspace)
                        .ToListAsync();

                    foreach (var proj in projects)
                    {
                        var tv = new Thanhvien
                        {
                            MaThanhVien = Guid.NewGuid().ToString("N")[..10].ToUpper(),
                            MaDuAn = proj.MaDuAn,
                            MaTaiKhoan = newUser.MaTaiKhoan,
                            VaiTroDuAn = invite.VaiTroMacDinh,
                            NgayThamGia = DateTime.Now
                        };
                        _context.Thanhviens.Add(tv);
                    }
                    await _context.SaveChangesAsync();
                }
            }

            TempData["Success"] = "Đăng ký thành công! Vui lòng đăng nhập.";
            return RedirectToAction("Login");
        }

        // ========== UC#03: ĐĂNG XUẤT ==========
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        // ========== UC#04: ĐỔI MẬT KHẨU ==========
        [AuthFilter]
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [AuthFilter]
        [HttpPost]
        public async Task<IActionResult> ChangePassword(string matKhauCu, string matKhauMoi, string xacNhanMatKhau)
        {
            var maTk = HttpContext.Session.GetString("MaTaiKhoan");
            var user = await _context.Taikhoans.FindAsync(maTk);
            if (user == null) return RedirectToAction("Login");

            var verify = _hasher.VerifyHashedPassword(user, user.MatKhau, matKhauCu);
            if (verify == PasswordVerificationResult.Failed)
            {
                ViewBag.Error = "Mật khẩu hiện tại không đúng";
                return View();
            }

            if (matKhauMoi != xacNhanMatKhau)
            {
                ViewBag.Error = "Mật khẩu mới và mật khẩu xác nhận không khớp";
                return View();
            }

            user.MatKhau = _hasher.HashPassword(user, matKhauMoi);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Đổi mật khẩu thành công!";
            return RedirectToAction("Profile");
        }

        // ========== UC#05: QUẢN LÝ THÔNG TIN CÁ NHÂN ==========
        [AuthFilter]
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var maTk = HttpContext.Session.GetString("MaTaiKhoan");
            var user = await _context.Taikhoans
                .Include(t => t.MaGoiNavigation)
                .FirstOrDefaultAsync(t => t.MaTaiKhoan == maTk);
            if (user == null) return RedirectToAction("Login");

            ViewBag.Packages = await _context.Goidichvus.OrderBy(g => g.Gia).ToListAsync();
            return View(user);
        }

        [AuthFilter]
        [HttpPost]
        public async Task<IActionResult> Profile(string hoTen, string soDienThoai, string email)
        {
            var maTk = HttpContext.Session.GetString("MaTaiKhoan");
            var user = await _context.Taikhoans
                .Include(t => t.MaGoiNavigation)
                .FirstOrDefaultAsync(t => t.MaTaiKhoan == maTk);
            if (user == null) return RedirectToAction("Login");

            ViewBag.Packages = await _context.Goidichvus.OrderBy(g => g.Gia).ToListAsync();

            if (string.IsNullOrWhiteSpace(hoTen))
            {
                ViewBag.Error = "Họ tên không được để trống";
                return View(user);
            }

            // Check email unique if changed
            if (email != user.Email)
            {
                var emailExists = await _context.Taikhoans.AnyAsync(t => t.Email == email && t.MaTaiKhoan != maTk);
                if (emailExists)
                {
                    ViewBag.Error = "Email đã được sử dụng bởi tài khoản khác";
                    return View(user);
                }
            }

            user.HoTen = hoTen;
            user.SoDienThoai = soDienThoai;
            user.Email = email;
            await _context.SaveChangesAsync();

            // Update session
            HttpContext.Session.SetString("HoTen", hoTen);
            HttpContext.Session.SetString("Email", email);

            TempData["Success"] = "Cập nhật thông tin thành công!";
            return View(user);
        }

        [AuthFilter]
        [HttpPost]
        public async Task<IActionResult> UpgradePlan(string maGoi)
        {
            var maTk = HttpContext.Session.GetString("MaTaiKhoan");
            var user = await _context.Taikhoans.FindAsync(maTk);
            if (user == null) return RedirectToAction("Login");

            var package = await _context.Goidichvus.FindAsync(maGoi);
            if (package == null)
            {
                TempData["Error"] = "Gói dịch vụ không tồn tại";
                return RedirectToAction("Profile");
            }

            user.MaGoi = maGoi;
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Đăng ký thành công gói {package.TenGoi}!";
            return RedirectToAction("Profile");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
