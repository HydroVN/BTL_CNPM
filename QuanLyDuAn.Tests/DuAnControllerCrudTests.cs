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
    }
}
