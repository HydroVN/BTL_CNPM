using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyDuAn.Controllers;
using QuanLyDuAn.Models;
using System;
using System.Threading.Tasks;
using Xunit;

namespace QuanLyDuAn.Tests
{
    public class DuAnControllerCrudTests
    {
        [Fact]
        public async Task Create_Post_ValidProject_AddsToDatabase()
        {
            // Arrange
            using var context = TestHelper.GetInMemoryDbContext();
            var controller = new DuAnController(context);
            var sessionMock = TestHelper.CreateMockSession();
            sessionMock.SetSessionString("MaTaiKhoan", "USER1");
            controller.ControllerContext = TestHelper.CreateControllerContext(sessionMock.Object);
            controller.SetupTempData();

            // Seed Workspace
            var ws = new Workspace { MaWorkspace = "WS1", TenWorkspace = "Workspace 1", MaTaiKhoan = "USER1" };
            context.Workspaces.Add(ws);
            await context.SaveChangesAsync();

            // Act
            var result = await controller.Create("WS1", "New Project", "Description", DateTime.Now, DateTime.Now.AddDays(10));

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            Assert.Equal("WS1", redirectResult.RouteValues?["maWorkspace"]?.ToString());

            // Verify project exists in Db
            var dbProj = await context.Duans.FirstOrDefaultAsync(d => d.TenDuAn == "New Project");
            Assert.NotNull(dbProj);
            Assert.Equal("WS1", dbProj.MaWorkspace);
        }

        [Fact]
        public async Task Edit_Post_AuthorizedUser_UpdatesProjectDetails()
        {
            // Arrange
            using var context = TestHelper.GetInMemoryDbContext();
            var controller = new DuAnController(context);
            var sessionMock = TestHelper.CreateMockSession();
            sessionMock.SetSessionString("MaTaiKhoan", "USER1"); // Owner
            controller.ControllerContext = TestHelper.CreateControllerContext(sessionMock.Object);
            controller.SetupTempData();

            // Seed Workspace & Project
            var ws = new Workspace { MaWorkspace = "WS1", TenWorkspace = "Workspace 1", MaTaiKhoan = "USER1" };
            var proj = new Duan { MaDuAn = "PROJ1", TenDuAn = "Old Name", MaWorkspace = "WS1", TrangThai = "ChuaThucHien" };
            context.Workspaces.Add(ws);
            context.Duans.Add(proj);
            await context.SaveChangesAsync();

            // Act
            var result = await controller.Edit("PROJ1", "Updated Name", "New Description", DateTime.Now, DateTime.Now.AddDays(5), "DangThucHien");

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Details", redirectResult.ActionName);

            // Verify project details updated in Db
            var dbProj = await context.Duans.FindAsync("PROJ1");
            Assert.NotNull(dbProj);
            Assert.Equal("Updated Name", dbProj.TenDuAn);
            Assert.Equal("DangThucHien", dbProj.TrangThai);
        }

        [Fact]
        public async Task Edit_Post_UnauthorizedUser_PreventsEdit()
        {
            // Arrange
            using var context = TestHelper.GetInMemoryDbContext();
            var controller = new DuAnController(context);
            var sessionMock = TestHelper.CreateMockSession();
            sessionMock.SetSessionString("MaTaiKhoan", "USER_STRANGER"); // Not owner/PM
            controller.ControllerContext = TestHelper.CreateControllerContext(sessionMock.Object);
            controller.SetupTempData();

            // Seed Workspace & Project owned by USER1
            var ws = new Workspace { MaWorkspace = "WS1", TenWorkspace = "Workspace 1", MaTaiKhoan = "USER1" };
            var proj = new Duan { MaDuAn = "PROJ1", TenDuAn = "Old Name", MaWorkspace = "WS1", TrangThai = "ChuaThucHien" };
            context.Workspaces.Add(ws);
            context.Duans.Add(proj);
            await context.SaveChangesAsync();

            // Act
            var result = await controller.Edit("PROJ1", "Hacked Name", "Hacked Desc", DateTime.Now, DateTime.Now.AddDays(5), "DangThucHien");

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Details", redirectResult.ActionName);
            Assert.Equal("Chỉ người quản lý dự án mới có quyền chỉnh sửa dự án.", controller.TempData["Error"]);

            // Verify project details NOT changed in Db
            var dbProj = await context.Duans.FindAsync("PROJ1");
            Assert.NotNull(dbProj);
            Assert.Equal("Old Name", dbProj.TenDuAn);
        }

        [Fact]
        public async Task Delete_Post_AuthorizedUser_RemovesProject()
        {
            // Arrange
            using var context = TestHelper.GetInMemoryDbContext();
            var controller = new DuAnController(context);
            var sessionMock = TestHelper.CreateMockSession();
            sessionMock.SetSessionString("MaTaiKhoan", "USER1"); // Owner
            controller.ControllerContext = TestHelper.CreateControllerContext(sessionMock.Object);
            controller.SetupTempData();

            // Seed Workspace & Project
            var ws = new Workspace { MaWorkspace = "WS1", TenWorkspace = "Workspace 1", MaTaiKhoan = "USER1" };
            var proj = new Duan { MaDuAn = "PROJ1", TenDuAn = "Delete Me", MaWorkspace = "WS1", TrangThai = "ChuaThucHien" };
            context.Workspaces.Add(ws);
            context.Duans.Add(proj);
            await context.SaveChangesAsync();

            // Act
            var result = await controller.Delete("PROJ1");

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);

            // Verify project removed from Db
            var dbProj = await context.Duans.FindAsync("PROJ1");
            Assert.Null(dbProj);
        }

        [Fact]
        public async Task Create_Post_ExceedsLimit_ReturnsErrorView()
        {
            // Arrange
            using var context = TestHelper.GetInMemoryDbContext();
            var controller = new DuAnController(context);
            var sessionMock = TestHelper.CreateMockSession();
            sessionMock.SetSessionString("MaTaiKhoan", "USER1");
            controller.ControllerContext = TestHelper.CreateControllerContext(sessionMock.Object);
            controller.SetupTempData();

            // Seed subscription package
            var package = new Goidichvu { MaGoi = "FREE", TenGoi = "Free Plan", Gia = 0, SoDuAnToiDa = 5, SoTvToiDa = 10, DungLuongMax = 1024 };
            context.Goidichvus.Add(package);

            // Seed Account
            var user = new Taikhoan { MaTaiKhoan = "USER1", Email = "user1@example.com", HoTen = "User One", MatKhau = "pwd", VaiTro = "User", TrangThai = "Active", MaGoi = "FREE" };
            context.Taikhoans.Add(user);

            // Seed Workspace
            var ws = new Workspace { MaWorkspace = "WS1", TenWorkspace = "Workspace 1", MaTaiKhoan = "USER1" };
            context.Workspaces.Add(ws);

            // Seed 5 existing projects in the workspace (limit is 5)
            for (int i = 1; i <= 5; i++)
            {
                context.Duans.Add(new Duan { MaDuAn = $"PROJ{i}", TenDuAn = $"Project {i}", MaWorkspace = "WS1", TrangThai = "ChuaThucHien" });
            }

            await context.SaveChangesAsync();

            // Act
            var result = await controller.Create("WS1", "Project 6", "Description 6", DateTime.Now, DateTime.Now.AddDays(5));

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Gói dịch vụ hiện tại chỉ cho phép tạo tối đa 5 dự án.", controller.ViewBag.Error);

            // Verify project count remains 5
            var count = await context.Duans.CountAsync(d => d.MaWorkspace == "WS1");
            Assert.Equal(5, count);
        }

        [Fact]
        public async Task Create_Post_UnderLimit_CreatesProject()
        {
            // Arrange
            using var context = TestHelper.GetInMemoryDbContext();
            var controller = new DuAnController(context);
            var sessionMock = TestHelper.CreateMockSession();
            sessionMock.SetSessionString("MaTaiKhoan", "USER1");
            controller.ControllerContext = TestHelper.CreateControllerContext(sessionMock.Object);
            controller.SetupTempData();

            // Seed subscription package
            var package = new Goidichvu { MaGoi = "FREE", TenGoi = "Free Plan", Gia = 0, SoDuAnToiDa = 5, SoTvToiDa = 10, DungLuongMax = 1024 };
            context.Goidichvus.Add(package);

            // Seed Account
            var user = new Taikhoan { MaTaiKhoan = "USER1", Email = "user1@example.com", HoTen = "User One", MatKhau = "pwd", VaiTro = "User", TrangThai = "Active", MaGoi = "FREE" };
            context.Taikhoans.Add(user);

            // Seed Workspace
            var ws = new Workspace { MaWorkspace = "WS1", TenWorkspace = "Workspace 1", MaTaiKhoan = "USER1" };
            context.Workspaces.Add(ws);

            // Seed 3 existing projects (under limit of 5)
            for (int i = 1; i <= 3; i++)
            {
                context.Duans.Add(new Duan { MaDuAn = $"PROJ{i}", TenDuAn = $"Project {i}", MaWorkspace = "WS1", TrangThai = "ChuaThucHien" });
            }

            await context.SaveChangesAsync();

            // Act
            var result = await controller.Create("WS1", "Project 4", "Description 4", DateTime.Now, DateTime.Now.AddDays(5));

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);

            // Verify project count is now 4
            var count = await context.Duans.CountAsync(d => d.MaWorkspace == "WS1");
            Assert.Equal(4, count);
        }

        [Fact]
        public async Task Create_Post_ProPlanExceedsLimit_ReturnsErrorView()
        {
            // Arrange
            using var context = TestHelper.GetInMemoryDbContext();
            var controller = new DuAnController(context);
            var sessionMock = TestHelper.CreateMockSession();
            sessionMock.SetSessionString("MaTaiKhoan", "USER1");
            controller.ControllerContext = TestHelper.CreateControllerContext(sessionMock.Object);
            controller.SetupTempData();

            // Seed subscription package (PRO plan has limit of 50 projects)
            var package = new Goidichvu { MaGoi = "PRO", TenGoi = "Pro Plan", Gia = 150000, SoDuAnToiDa = 50, SoTvToiDa = 100, DungLuongMax = 10240 };
            context.Goidichvus.Add(package);

            // Seed Account
            var user = new Taikhoan { MaTaiKhoan = "USER1", Email = "user1@example.com", HoTen = "User One", MatKhau = "pwd", VaiTro = "User", TrangThai = "Active", MaGoi = "PRO" };
            context.Taikhoans.Add(user);

            // Seed Workspace
            var ws = new Workspace { MaWorkspace = "WS1", TenWorkspace = "Workspace 1", MaTaiKhoan = "USER1" };
            context.Workspaces.Add(ws);

            // Seed 50 existing projects (limit is 50)
            for (int i = 1; i <= 50; i++)
            {
                context.Duans.Add(new Duan { MaDuAn = $"PROJ{i}", TenDuAn = $"Project {i}", MaWorkspace = "WS1", TrangThai = "ChuaThucHien" });
            }

            await context.SaveChangesAsync();

            // Act
            var result = await controller.Create("WS1", "Project 51", "Description 51", DateTime.Now, DateTime.Now.AddDays(5));

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Gói dịch vụ hiện tại chỉ cho phép tạo tối đa 50 dự án.", controller.ViewBag.Error);

            // Verify project count remains 50
            var count = await context.Duans.CountAsync(d => d.MaWorkspace == "WS1");
            Assert.Equal(50, count);
        }
    }
}
