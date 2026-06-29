using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyDuAn.Controllers;
using QuanLyDuAn.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace QuanLyDuAn.Tests
{
    public class BinhLuanControllerTests
    {
        [Fact]
        public async Task GetTaskDetail_ValidTaskId_ReturnsJsonWithComments()
        {
            // Arrange
            using var context = TestHelper.GetInMemoryDbContext();
            var controller = new BinhLuanController(context);
            var sessionMock = TestHelper.CreateMockSession();
            controller.ControllerContext = TestHelper.CreateControllerContext(sessionMock.Object);

            // Seed user
            var user = new Taikhoan { MaTaiKhoan = "USER1", HoTen = "Nguyen Van A", Email = "a@gmail.com", MatKhau = "123", VaiTro = "Member", TrangThai = "KichHoat" };
            context.Taikhoans.Add(user);

            // Seed project
            var proj = new Duan { MaDuAn = "PROJ1", TenDuAn = "Project 1", MaWorkspace = "WS1", TrangThai = "ChuaThucHien" };
            context.Duans.Add(proj);

            // Seed member
            var member = new Thanhvien { MaThanhVien = "MB1", MaDuAn = "PROJ1", MaTaiKhoan = "USER1", VaiTroDuAn = "Developer" };
            context.Thanhviens.Add(member);

            // Seed task
            var task = new Congviec
            {
                MaCongViec = "TASK1",
                TenCongViec = "Task One",
                MoTa = "Description One",
                Deadline = DateTime.Today.AddDays(5),
                MucDoUuTien = "Cao",
                TrangThai = "ToDo",
                MaThanhVien = "MB1",
                MaDuAn = "PROJ1"
            };
            context.Congviecs.Add(task);

            // Seed comment
            var comment = new Binhluan
            {
                MaBinhLuan = "BL1",
                MaCongViec = "TASK1",
                MaTaiKhoan = "USER1",
                NoiDung = "Hello World",
                ThoiGian = DateTime.Now
            };
            context.Binhluans.Add(comment);

            await context.SaveChangesAsync();

            // Act
            var result = await controller.GetTaskDetail("TASK1");

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var value = jsonResult.Value;
            var type = value.GetType();
            Assert.Equal("Task One", type.GetProperty("tenCongViec")!.GetValue(value));
            Assert.Equal("Description One", type.GetProperty("moTa")!.GetValue(value));
            Assert.Equal("Cao", type.GetProperty("mucDoUuTien")!.GetValue(value));
            Assert.Equal("ToDo", type.GetProperty("trangThai")!.GetValue(value));
            Assert.Equal("Nguyen Van A", type.GetProperty("assignee")!.GetValue(value));
            Assert.Equal("MB1", type.GetProperty("assigneeId")!.GetValue(value));

            // Force evaluation of LINQ query inside anonymous type comments property
            var comments = Assert.IsAssignableFrom<System.Collections.IEnumerable>(type.GetProperty("comments")!.GetValue(value));
            var commentsList = new List<object>();
            foreach (var commentItem in comments)
            {
                commentsList.Add(commentItem);
            }
            Assert.Single(commentsList);
            var cType = commentsList[0].GetType();
            Assert.Equal("N", cType.GetProperty("avatar")!.GetValue(commentsList[0]));
            Assert.Equal("Nguyen Van A", cType.GetProperty("hoTen")!.GetValue(commentsList[0]));
            Assert.Equal("Hello World", cType.GetProperty("noiDung")!.GetValue(commentsList[0]));
        }

        [Fact]
        public async Task GetTaskDetail_InvalidTaskId_ReturnsNotFound()
        {
            // Arrange
            using var context = TestHelper.GetInMemoryDbContext();
            var controller = new BinhLuanController(context);

            // Act
            var result = await controller.GetTaskDetail("INVALID");

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Create_ValidRequest_AddsCommentAndReturnsSuccess()
        {
            // Arrange
            using var context = TestHelper.GetInMemoryDbContext();
            var controller = new BinhLuanController(context);
            var sessionMock = TestHelper.CreateMockSession();
            sessionMock.SetSessionString("MaTaiKhoan", "USER1");
            sessionMock.SetSessionString("HoTen", "Nguyen Van A");
            controller.ControllerContext = TestHelper.CreateControllerContext(sessionMock.Object);

            var request = new BinhLuanController.CreateCommentRequest
            {
                MaCongViec = "TASK1",
                NoiDung = "My New Comment"
            };

            // Act
            var result = await controller.Create(request);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var value = jsonResult.Value;
            var type = value.GetType();
            Assert.True((bool)type.GetProperty("success")!.GetValue(value)!);
            Assert.Equal("N", type.GetProperty("avatar")!.GetValue(value));
            Assert.Equal("Nguyen Van A", type.GetProperty("hoTen")!.GetValue(value));

            // Verify comment added to database
            var bl = await context.Binhluans.FirstOrDefaultAsync(b => b.MaCongViec == "TASK1" && b.MaTaiKhoan == "USER1");
            Assert.NotNull(bl);
            Assert.Equal("My New Comment", bl.NoiDung);
        }

        [Fact]
        public async Task Create_InvalidRequest_Or_NullSession_ReturnsFail()
        {
            // Arrange
            using var context = TestHelper.GetInMemoryDbContext();
            var controller = new BinhLuanController(context);
            var sessionMock = TestHelper.CreateMockSession();
            // Case 1: No logged in user
            controller.ControllerContext = TestHelper.CreateControllerContext(sessionMock.Object);

            var request = new BinhLuanController.CreateCommentRequest
            {
                MaCongViec = "TASK1",
                NoiDung = "My New Comment"
            };

            // Act 1
            var result1 = await controller.Create(request);

            // Assert 1
            var jsonResult1 = Assert.IsType<JsonResult>(result1);
            var value1 = jsonResult1.Value;
            Assert.False((bool)value1.GetType().GetProperty("success")!.GetValue(value1)!);

            // Case 2: Logged in but empty content
            sessionMock.SetSessionString("MaTaiKhoan", "USER1");
            request.NoiDung = "";

            // Act 2
            var result2 = await controller.Create(request);

            // Assert 2
            var jsonResult2 = Assert.IsType<JsonResult>(result2);
            var value2 = jsonResult2.Value;
            Assert.False((bool)value2.GetType().GetProperty("success")!.GetValue(value2)!);
        }

        [Fact]
        public async Task Create_EmptyHoTen_ReturnsUAvatar()
        {
            // Arrange
            using var context = TestHelper.GetInMemoryDbContext();
            var controller = new BinhLuanController(context);
            var sessionMock = TestHelper.CreateMockSession();
            sessionMock.SetSessionString("MaTaiKhoan", "USER1");
            sessionMock.SetSessionString("HoTen", "");
            controller.ControllerContext = TestHelper.CreateControllerContext(sessionMock.Object);

            var request = new BinhLuanController.CreateCommentRequest
            {
                MaCongViec = "TASK1",
                NoiDung = "My Comment"
            };

            // Act
            var result = await controller.Create(request);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var value = jsonResult.Value;
            var type = value.GetType();
            Assert.True((bool)type.GetProperty("success")!.GetValue(value)!);
            Assert.Equal("U", type.GetProperty("avatar")!.GetValue(value));
            Assert.Equal("", type.GetProperty("hoTen")!.GetValue(value));
        }
    }
}
