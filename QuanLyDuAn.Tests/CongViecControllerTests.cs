using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using QuanLyDuAn.Controllers;
using QuanLyDuAn.Models;
using System;
using System.Threading.Tasks;
using Xunit;

namespace QuanLyDuAn.Tests
{
    public class CongViecControllerTests
    {
        [Fact]
        public async Task Create_Post_AuthorizedUser_AddsTaskToDatabase()
        {
            // Arrange
            using var context = TestHelper.GetInMemoryDbContext();
            var envMock = new Mock<IWebHostEnvironment>();
            var controller = new CongViecController(context, envMock.Object);
            var sessionMock = TestHelper.CreateMockSession();
            sessionMock.SetSessionString("MaTaiKhoan", "USER1"); // Workspace Owner
            controller.ControllerContext = TestHelper.CreateControllerContext(sessionMock.Object);
            controller.SetupTempData();

            // Seed Workspace & Project
            var ws = new Workspace { MaWorkspace = "WS1", TenWorkspace = "Workspace 1", MaTaiKhoan = "USER1" };
            var proj = new Duan { MaDuAn = "PROJ1", TenDuAn = "Project 1", MaWorkspace = "WS1", TrangThai = "ChuaThucHien" };
            context.Workspaces.Add(ws);
            context.Duans.Add(proj);
            await context.SaveChangesAsync();

            // Act
            var result = await controller.Create("PROJ1", "New Task", "Task Description", DateTime.Now.AddDays(5), "Cao", null, null);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Details", redirectResult.ActionName);
            Assert.Equal("DuAn", redirectResult.ControllerName);

            // Verify task was added in Db
            var dbTask = await context.Congviecs.FirstOrDefaultAsync(c => c.TenCongViec == "New Task");
            Assert.NotNull(dbTask);
            Assert.Equal("PROJ1", dbTask.MaDuAn);
            Assert.Equal("Cao", dbTask.MucDoUuTien);
            Assert.Equal("ToDo", dbTask.TrangThai);
        }

        [Fact]
        public async Task Create_Post_UnauthorizedUser_PreventsCreation()
        {
            // Arrange
            using var context = TestHelper.GetInMemoryDbContext();
            var envMock = new Mock<IWebHostEnvironment>();
            var controller = new CongViecController(context, envMock.Object);
            var sessionMock = TestHelper.CreateMockSession();
            sessionMock.SetSessionString("MaTaiKhoan", "USER_STRANGER"); // Not owner/PM
            controller.ControllerContext = TestHelper.CreateControllerContext(sessionMock.Object);
            controller.SetupTempData();

            // Seed Workspace & Project owned by USER1
            var ws = new Workspace { MaWorkspace = "WS1", TenWorkspace = "Workspace 1", MaTaiKhoan = "USER1" };
            var proj = new Duan { MaDuAn = "PROJ1", TenDuAn = "Project 1", MaWorkspace = "WS1", TrangThai = "ChuaThucHien" };
            context.Workspaces.Add(ws);
            context.Duans.Add(proj);
            await context.SaveChangesAsync();

            // Act
            var result = await controller.Create("PROJ1", "New Task", "Task Description", DateTime.Now.AddDays(5), "Cao", null, null);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Details", redirectResult.ActionName);
            Assert.Equal("DuAn", redirectResult.ControllerName);
            Assert.Equal("Chỉ người quản lý dự án mới có quyền thêm công việc.", controller.TempData["Error"]);

            // Verify task was NOT added in Db
            var dbTask = await context.Congviecs.FirstOrDefaultAsync(c => c.TenCongViec == "New Task");
            Assert.Null(dbTask);
        }

        [Fact]
        public async Task UpdateStatus_Post_WorkspaceOwner_UpdatesTaskStatus()
        {
            // Arrange
            using var context = TestHelper.GetInMemoryDbContext();
            var envMock = new Mock<IWebHostEnvironment>();
            var controller = new CongViecController(context, envMock.Object);
            var sessionMock = TestHelper.CreateMockSession();
            sessionMock.SetSessionString("MaTaiKhoan", "USER1"); // Workspace Owner
            controller.ControllerContext = TestHelper.CreateControllerContext(sessionMock.Object);

            // Seed Workspace & Project
            var ws = new Workspace { MaWorkspace = "WS1", TenWorkspace = "Workspace 1", MaTaiKhoan = "USER1" };
            var proj = new Duan { MaDuAn = "PROJ1", TenDuAn = "Project 1", MaWorkspace = "WS1", TrangThai = "ChuaThucHien" };
            var task = new Congviec { MaCongViec = "TASK1", TenCongViec = "Task", MaDuAn = "PROJ1", MucDoUuTien = "Cao", TrangThai = "ToDo" };
            context.Workspaces.Add(ws);
            context.Duans.Add(proj);
            context.Congviecs.Add(task);
            await context.SaveChangesAsync();

            // Act
            var request = new CongViecController.UpdateStatusRequest { MaCongViec = "TASK1", TrangThai = "InProgress" };
            var result = await controller.UpdateStatus(request);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            Assert.NotNull(jsonResult.Value);
            var successVal = (bool)jsonResult.Value.GetType().GetProperty("success")!.GetValue(jsonResult.Value)!;
            Assert.True(successVal);

            // Verify status updated in Db
            var dbTask = await context.Congviecs.FindAsync("TASK1");
            Assert.NotNull(dbTask);
            Assert.Equal("InProgress", dbTask.TrangThai);
        }

        [Fact]
        public async Task UpdateStatus_Post_AssignedMember_UpdatesTaskStatus()
        {
            // Arrange
            using var context = TestHelper.GetInMemoryDbContext();
            var envMock = new Mock<IWebHostEnvironment>();
            var controller = new CongViecController(context, envMock.Object);
            var sessionMock = TestHelper.CreateMockSession();
            sessionMock.SetSessionString("MaTaiKhoan", "USER2"); // Assigned User
            controller.ControllerContext = TestHelper.CreateControllerContext(sessionMock.Object);

            // Seed Workspace & Project
            var ws = new Workspace { MaWorkspace = "WS1", TenWorkspace = "Workspace 1", MaTaiKhoan = "USER1" };
            var proj = new Duan { MaDuAn = "PROJ1", TenDuAn = "Project 1", MaWorkspace = "WS1", TrangThai = "ChuaThucHien" };
            
            // USER2 is a member of this project
            var tv = new Thanhvien { MaThanhVien = "MB2", MaDuAn = "PROJ1", MaTaiKhoan = "USER2", VaiTroDuAn = "Developer", NgayThamGia = DateTime.Now };
            // Task is assigned to USER2 (MB2)
            var task = new Congviec { MaCongViec = "TASK1", TenCongViec = "Task", MaDuAn = "PROJ1", MaThanhVien = "MB2", MucDoUuTien = "Cao", TrangThai = "ToDo" };
            
            context.Workspaces.Add(ws);
            context.Duans.Add(proj);
            context.Thanhviens.Add(tv);
            context.Congviecs.Add(task);
            await context.SaveChangesAsync();

            // Act
            var request = new CongViecController.UpdateStatusRequest { MaCongViec = "TASK1", TrangThai = "InProgress" };
            var result = await controller.UpdateStatus(request);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            Assert.NotNull(jsonResult.Value);
            var successVal = (bool)jsonResult.Value.GetType().GetProperty("success")!.GetValue(jsonResult.Value)!;
            Assert.True(successVal);

            // Verify status updated in Db
            var dbTask = await context.Congviecs.FindAsync("TASK1");
            Assert.NotNull(dbTask);
            Assert.Equal("InProgress", dbTask.TrangThai);
        }

        [Fact]
        public async Task UpdateStatus_Post_UnauthorizedUser_PreventsStatusUpdate()
        {
            // Arrange
            using var context = TestHelper.GetInMemoryDbContext();
            var envMock = new Mock<IWebHostEnvironment>();
            var controller = new CongViecController(context, envMock.Object);
            var sessionMock = TestHelper.CreateMockSession();
            sessionMock.SetSessionString("MaTaiKhoan", "USER_STRANGER"); // Not manager nor assignee
            controller.ControllerContext = TestHelper.CreateControllerContext(sessionMock.Object);

            // Seed Workspace & Project
            var ws = new Workspace { MaWorkspace = "WS1", TenWorkspace = "Workspace 1", MaTaiKhoan = "USER1" };
            var proj = new Duan { MaDuAn = "PROJ1", TenDuAn = "Project 1", MaWorkspace = "WS1", TrangThai = "ChuaThucHien" };
            
            // Task is assigned to USER2 (MB2)
            var tv2 = new Thanhvien { MaThanhVien = "MB2", MaDuAn = "PROJ1", MaTaiKhoan = "USER2", VaiTroDuAn = "Developer", NgayThamGia = DateTime.Now };
            var task = new Congviec { MaCongViec = "TASK1", TenCongViec = "Task", MaDuAn = "PROJ1", MaThanhVien = "MB2", MucDoUuTien = "Cao", TrangThai = "ToDo" };
            
            // USER_STRANGER is also a member, but not assigned
            var tvStranger = new Thanhvien { MaThanhVien = "MB_STRANGER", MaDuAn = "PROJ1", MaTaiKhoan = "USER_STRANGER", VaiTroDuAn = "Tester", NgayThamGia = DateTime.Now };

            context.Workspaces.Add(ws);
            context.Duans.Add(proj);
            context.Thanhviens.AddRange(tv2, tvStranger);
            context.Congviecs.Add(task);
            await context.SaveChangesAsync();

            // Act
            var request = new CongViecController.UpdateStatusRequest { MaCongViec = "TASK1", TrangThai = "InProgress" };
            var result = await controller.UpdateStatus(request);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            Assert.NotNull(jsonResult.Value);
            var successVal = (bool)jsonResult.Value.GetType().GetProperty("success")!.GetValue(jsonResult.Value)!;
            Assert.False(successVal);
            var messageVal = (string)jsonResult.Value.GetType().GetProperty("message")!.GetValue(jsonResult.Value)!;
            Assert.Equal("Bạn chỉ có thể cập nhật tiến trình của công việc được giao cho bạn.", messageVal);

            // Verify status NOT changed in Db
            var dbTask = await context.Congviecs.FindAsync("TASK1");
            Assert.NotNull(dbTask);
            Assert.Equal("ToDo", dbTask.TrangThai);
        }
    }
}
