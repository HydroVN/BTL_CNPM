using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyDuAn.Controllers;
using QuanLyDuAn.Models;
using System;
using System.Threading.Tasks;
using Xunit;

namespace QuanLyDuAn.Tests
{
    public class WorkspaceControllerTests
    {
        [Fact]
        public async Task Delete_Post_AuthorizedUser_RemovesWorkspaceAndAllRelatedData()
        {
            // Arrange
            using var context = TestHelper.GetInMemoryDbContext();
            var controller = new WorkspaceController(context);
            var sessionMock = TestHelper.CreateMockSession();
            sessionMock.SetSessionString("MaTaiKhoan", "USER1");
            controller.ControllerContext = TestHelper.CreateControllerContext(sessionMock.Object);
            controller.SetupTempData();

            // Seed Workspace
            var ws = new Workspace { MaWorkspace = "WS1", TenWorkspace = "Workspace 1", MaTaiKhoan = "USER1" };
            context.Workspaces.Add(ws);

            // Seed Project
            var proj = new Duan { MaDuAn = "PROJ1", TenDuAn = "Project 1", MaWorkspace = "WS1", TrangThai = "ChuaThucHien" };
            context.Duans.Add(proj);

            // Seed Member
            var member = new Thanhvien
            {
                MaThanhVien = "MB1",
                MaDuAn = "PROJ1",
                MaTaiKhoan = "USER2",
                VaiTroDuAn = "Developer",
                NgayThamGia = DateTime.Now
            };
            context.Thanhviens.Add(member);

            // Seed Task
            var task = new Congviec
            {
                MaCongViec = "TASK1",
                MaDuAn = "PROJ1",
                TenCongViec = "Test Task",
                MucDoUuTien = "Cao",
                TrangThai = "ToDo"
            };
            context.Congviecs.Add(task);

            await context.SaveChangesAsync();

            // Act
            var result = await controller.Delete("WS1");

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            Assert.Equal("Đã xóa Workspace thành công!", controller.TempData["Success"]);

            // Verify Workspace deleted
            Assert.Null(await context.Workspaces.FindAsync("WS1"));

            // Verify related Project deleted
            Assert.Null(await context.Duans.FindAsync("PROJ1"));

            // Verify related Member deleted
            Assert.Null(await context.Thanhviens.FindAsync("MB1"));

            // Verify related Task deleted
            Assert.Null(await context.Congviecs.FindAsync("TASK1"));
        }

        [Fact]
        public async Task Delete_Post_UnauthorizedUser_PreventsDeletion()
        {
            // Arrange
            using var context = TestHelper.GetInMemoryDbContext();
            var controller = new WorkspaceController(context);
            var sessionMock = TestHelper.CreateMockSession();
            sessionMock.SetSessionString("MaTaiKhoan", "USER_STRANGER"); // Not owner
            controller.ControllerContext = TestHelper.CreateControllerContext(sessionMock.Object);
            controller.SetupTempData();

            // Seed Workspace owned by USER1
            var ws = new Workspace { MaWorkspace = "WS1", TenWorkspace = "Workspace 1", MaTaiKhoan = "USER1" };
            context.Workspaces.Add(ws);
            await context.SaveChangesAsync();

            // Act
            var result = await controller.Delete("WS1");

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            Assert.Equal("Bạn không có quyền xóa Workspace này.", controller.TempData["Error"]);

            // Verify Workspace STILL exists
            Assert.NotNull(await context.Workspaces.FindAsync("WS1"));
        }

        [Fact]
        public async Task Create_Post_ExceedsLimit_ReturnsErrorView()
        {
            // Arrange
            using var context = TestHelper.GetInMemoryDbContext();
            var controller = new WorkspaceController(context);
            var sessionMock = TestHelper.CreateMockSession();
            sessionMock.SetSessionString("MaTaiKhoan", "USER1");
            controller.ControllerContext = TestHelper.CreateControllerContext(sessionMock.Object);
            controller.SetupTempData();

            // Seed user with FREE package
            var user = new Taikhoan { MaTaiKhoan = "USER1", Email = "user1@example.com", HoTen = "User One", MatKhau = "pwd", VaiTro = "User", TrangThai = "Active", MaGoi = "FREE" };
            context.Taikhoans.Add(user);

            // Seed existing workspace (Limit for FREE is 1)
            var ws = new Workspace { MaWorkspace = "WS1", TenWorkspace = "Workspace 1", MaTaiKhoan = "USER1" };
            context.Workspaces.Add(ws);

            await context.SaveChangesAsync();

            // Act
            var result = await controller.Create("Workspace 2", "Description 2");

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Gói dịch vụ hiện tại chỉ cho phép tạo tối đa 1 Workspace.", controller.ViewBag.Error);

            // Verify second workspace NOT saved
            var count = await context.Workspaces.CountAsync(w => w.MaTaiKhoan == "USER1");
            Assert.Equal(1, count);
        }

        [Fact]
        public async Task Create_Post_UnderLimit_CreatesWorkspace()
        {
            // Arrange
            using var context = TestHelper.GetInMemoryDbContext();
            var controller = new WorkspaceController(context);
            var sessionMock = TestHelper.CreateMockSession();
            sessionMock.SetSessionString("MaTaiKhoan", "USER1");
            controller.ControllerContext = TestHelper.CreateControllerContext(sessionMock.Object);
            controller.SetupTempData();

            // Seed user with PRO package
            var user = new Taikhoan { MaTaiKhoan = "USER1", Email = "user1@example.com", HoTen = "User One", MatKhau = "pwd", VaiTro = "User", TrangThai = "Active", MaGoi = "PRO" };
            context.Taikhoans.Add(user);

            // Seed existing workspaces (Limit for PRO is 5)
            context.Workspaces.Add(new Workspace { MaWorkspace = "WS1", TenWorkspace = "Workspace 1", MaTaiKhoan = "USER1" });
            context.Workspaces.Add(new Workspace { MaWorkspace = "WS2", TenWorkspace = "Workspace 2", MaTaiKhoan = "USER1" });

            await context.SaveChangesAsync();

            // Act
            var result = await controller.Create("Workspace 3", "Description 3");

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);

            // Verify third workspace is created
            var count = await context.Workspaces.CountAsync(w => w.MaTaiKhoan == "USER1");
            Assert.Equal(3, count);
        }

        [Fact]
        public async Task Create_Post_ProPlanExceedsLimit_ReturnsErrorView()
        {
            // Arrange
            using var context = TestHelper.GetInMemoryDbContext();
            var controller = new WorkspaceController(context);
            var sessionMock = TestHelper.CreateMockSession();
            sessionMock.SetSessionString("MaTaiKhoan", "USER1");
            controller.ControllerContext = TestHelper.CreateControllerContext(sessionMock.Object);
            controller.SetupTempData();

            // Seed user with PRO package
            var user = new Taikhoan { MaTaiKhoan = "USER1", Email = "user1@example.com", HoTen = "User One", MatKhau = "pwd", VaiTro = "User", TrangThai = "Active", MaGoi = "PRO" };
            context.Taikhoans.Add(user);

            // Seed 5 existing workspaces (Limit for PRO is 5)
            for (int i = 1; i <= 5; i++)
            {
                context.Workspaces.Add(new Workspace { MaWorkspace = $"WS{i}", TenWorkspace = $"Workspace {i}", MaTaiKhoan = "USER1" });
            }

            await context.SaveChangesAsync();

            // Act
            var result = await controller.Create("Workspace 6", "Description 6");

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Gói dịch vụ hiện tại chỉ cho phép tạo tối đa 5 Workspace.", controller.ViewBag.Error);

            // Verify count remains 5
            var count = await context.Workspaces.CountAsync(w => w.MaTaiKhoan == "USER1");
            Assert.Equal(5, count);
        }
    }
}
