using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyDuAn.Controllers;
using QuanLyDuAn.Models;
using System;
using System.Threading.Tasks;
using Xunit;

namespace QuanLyDuAn.Tests
{
    public class AdminControllerTests
    {
        [Fact]
        public async Task Dashboard_UserNotAdmin_RedirectsToAccessDenied()
        {
            // Arrange
            using var context = TestHelper.GetInMemoryDbContext();
            var controller = new AdminController(context);
            var sessionMock = TestHelper.CreateMockSession();
            sessionMock.SetSessionString("MaTaiKhoan", "USER1");
            sessionMock.SetSessionString("VaiTro", "User");
            controller.ControllerContext = TestHelper.CreateControllerContext(sessionMock.Object);

            // Act
            var result = await controller.Dashboard();

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("AccessDenied", redirectResult.ActionName);
            Assert.Equal("Account", redirectResult.ControllerName);
        }

        [Fact]
        public async Task Dashboard_AdminAccess_ReturnsViewWithStats()
        {
            // Arrange
            using var context = TestHelper.GetInMemoryDbContext();
            var controller = new AdminController(context);
            var sessionMock = TestHelper.CreateMockSession();
            sessionMock.SetSessionString("MaTaiKhoan", "ADMIN1");
            sessionMock.SetSessionString("VaiTro", "Admin");
            controller.ControllerContext = TestHelper.CreateControllerContext(sessionMock.Object);

            // Seed some accounts
            context.Taikhoans.Add(new Taikhoan { MaTaiKhoan = "USER1", Email = "u1@ex.com", HoTen = "U1", MatKhau = "p", VaiTro = "User", MaGoi = "FREE", NgayTao = DateTime.Now, TrangThai = "Active" });
            context.Taikhoans.Add(new Taikhoan { MaTaiKhoan = "USER2", Email = "u2@ex.com", HoTen = "U2", MatKhau = "p", VaiTro = "User", MaGoi = "PRO", NgayTao = DateTime.Now, TrangThai = "Active" });
            context.Taikhoans.Add(new Taikhoan { MaTaiKhoan = "USER3", Email = "u3@ex.com", HoTen = "U3", MatKhau = "p", VaiTro = "User", MaGoi = "ENT", NgayTao = DateTime.Now, TrangThai = "Active" });
            context.Taikhoans.Add(new Taikhoan { MaTaiKhoan = "ADMIN2", Email = "a2@ex.com", HoTen = "A2", MatKhau = "p", VaiTro = "Admin", MaGoi = "FREE", NgayTao = DateTime.Now, TrangThai = "Active" });

            await context.SaveChangesAsync();

            // Act
            var result = await controller.Dashboard();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            
            // Check stats passed in ViewBag
            Assert.Equal(3, controller.ViewBag.TotalUsers); // USER1, USER2, USER3 (Excludes admins ADMIN2 and current admin)
            Assert.Equal(1, controller.ViewBag.ProCount);
            Assert.Equal(1, controller.ViewBag.EntCount);
            Assert.NotNull(controller.ViewBag.RegMonths);
            Assert.NotNull(controller.ViewBag.PeakTimes);
        }
    }
}
