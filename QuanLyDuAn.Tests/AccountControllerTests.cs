using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using QuanLyDuAn.Controllers;
using QuanLyDuAn.Models;
using System.Threading.Tasks;
using Xunit;

namespace QuanLyDuAn.Tests
{
    public class AccountControllerTests
    {
        [Fact]
        public async Task Login_Post_EmptyCredentials_ReturnsViewWithError()
        {
            // Arrange
            using var context = TestHelper.GetInMemoryDbContext();
            var controller = new AccountController(context);
            var sessionMock = TestHelper.CreateMockSession();
            controller.ControllerContext = TestHelper.CreateControllerContext(sessionMock.Object);

            // Act
            var result = await controller.Login("", "");

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Tên đăng nhập và mật khẩu không để trống", controller.ViewBag.Error);
        }

        [Fact]
        public async Task Login_Post_InvalidCredentials_ReturnsViewWithError()
        {
            // Arrange
            using var context = TestHelper.GetInMemoryDbContext();
            var controller = new AccountController(context);
            var sessionMock = TestHelper.CreateMockSession();
            controller.ControllerContext = TestHelper.CreateControllerContext(sessionMock.Object);

            // Seed a user
            var hasher = new PasswordHasher<Taikhoan>();
            var user = new Taikhoan
            {
                MaTaiKhoan = "USER1",
                Email = "test@example.com",
                HoTen = "Test User",
                VaiTro = "Member",
                TrangThai = "KichHoat"
            };
            user.MatKhau = hasher.HashPassword(user, "correctpassword");
            context.Taikhoans.Add(user);
            await context.SaveChangesAsync();

            // Act
            var result = await controller.Login("test@example.com", "wrongpassword");

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Tên đăng nhập hoặc mật khẩu không đúng", controller.ViewBag.Error);
        }

        [Fact]
        public async Task Login_Post_ValidCredentials_SetsSessionAndRedirects()
        {
            // Arrange
            using var context = TestHelper.GetInMemoryDbContext();
            var controller = new AccountController(context);
            var sessionMock = TestHelper.CreateMockSession();
            controller.ControllerContext = TestHelper.CreateControllerContext(sessionMock.Object);

            // Seed a user
            var hasher = new PasswordHasher<Taikhoan>();
            var user = new Taikhoan
            {
                MaTaiKhoan = "USER1",
                Email = "test@example.com",
                HoTen = "Test User",
                VaiTro = "Member",
                TrangThai = "KichHoat"
            };
            user.MatKhau = hasher.HashPassword(user, "correctpassword");
            context.Taikhoans.Add(user);
            await context.SaveChangesAsync();

            // Act
            var result = await controller.Login("test@example.com", "correctpassword");

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            Assert.Equal("Workspace", redirectResult.ControllerName);

            // Verify session values
            Assert.Equal("USER1", sessionMock.GetSessionString("MaTaiKhoan"));
            Assert.Equal("Test User", sessionMock.GetSessionString("HoTen"));
            Assert.Equal("test@example.com", sessionMock.GetSessionString("Email"));
            Assert.Equal("Member", sessionMock.GetSessionString("VaiTro"));
        }

        [Fact]
        public async Task Register_Post_EmptyFields_ReturnsViewWithError()
        {
            // Arrange
            using var context = TestHelper.GetInMemoryDbContext();
            var controller = new AccountController(context);
            var sessionMock = TestHelper.CreateMockSession();
            controller.ControllerContext = TestHelper.CreateControllerContext(sessionMock.Object);

            // Act
            var result = await controller.Register("", "", "", "", null, null);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Họ tên, email, mật khẩu không được để trống", controller.ViewBag.Error);
        }

        [Fact]
        public async Task Register_Post_ExistingEmail_ReturnsViewWithError()
        {
            // Arrange
            using var context = TestHelper.GetInMemoryDbContext();
            var controller = new AccountController(context);
            var sessionMock = TestHelper.CreateMockSession();
            controller.ControllerContext = TestHelper.CreateControllerContext(sessionMock.Object);

            // Seed existing user
            context.Taikhoans.Add(new Taikhoan
            {
                MaTaiKhoan = "USER_EXISTING",
                Email = "existing@example.com",
                HoTen = "Existing",
                MatKhau = "password",
                VaiTro = "Member",
                TrangThai = "KichHoat"
            });
            await context.SaveChangesAsync();

            // Act
            var result = await controller.Register("New Name", "0123456789", "existing@example.com", "newpassword", null, null);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Tài khoản đã tồn tại, vui lòng chọn email khác", controller.ViewBag.Error);
        }

        [Fact]
        public async Task Register_Post_ValidUser_AddsUserAndRedirectsToLogin()
        {
            // Arrange
            using var context = TestHelper.GetInMemoryDbContext();
            var controller = new AccountController(context);
            var sessionMock = TestHelper.CreateMockSession();
            controller.ControllerContext = TestHelper.CreateControllerContext(sessionMock.Object);
            controller.SetupTempData();

            // Seed Goidichvu package
            context.Goidichvus.Add(new Goidichvu { MaGoi = "PRO", TenGoi = "Pro Plan", Gia = 150000 });
            await context.SaveChangesAsync();

            // Act
            var result = await controller.Register("New Name", "0123456789", "new@example.com", "newpassword", null, "PRO");

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Login", redirectResult.ActionName);

            // Verify user added to Db with correct package
            var dbUser = await context.Taikhoans.FirstOrDefaultAsync(t => t.Email == "new@example.com");
            Assert.NotNull(dbUser);
            Assert.Equal("New Name", dbUser.HoTen);
            Assert.Equal("PRO", dbUser.MaGoi);
        }

        [Fact]
        public async Task Logout_ClearsSessionAndRedirectsToLogin()
        {
            // Arrange
            using var context = TestHelper.GetInMemoryDbContext();
            var controller = new AccountController(context);
            var sessionMock = TestHelper.CreateMockSession();
            sessionMock.SetSessionString("MaTaiKhoan", "USER1");
            controller.ControllerContext = TestHelper.CreateControllerContext(sessionMock.Object);

            // Act
            var result = controller.Logout();

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Login", redirectResult.ActionName);

            // Verify session cleared
            sessionMock.Verify(s => s.Clear(), Times.Once);
        }
    }
}
