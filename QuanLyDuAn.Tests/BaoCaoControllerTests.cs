using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyDuAn.Controllers;
using QuanLyDuAn.Data;
using QuanLyDuAn.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace QuanLyDuAn.Tests
{
    public class BaoCaoControllerTests
    {
        private async Task SeedDataAsync(BtlCnpmContext context)
        {
            // Seed User
            var user = new Taikhoan { MaTaiKhoan = "USER1", HoTen = "Nguyen Van A", Email = "a@gmail.com", MatKhau = "123", VaiTro = "Member", TrangThai = "KichHoat" };
            context.Taikhoans.Add(user);

            // Seed Workspace
            var ws = new Workspace { MaWorkspace = "WS1", TenWorkspace = "Workspace 1", MaTaiKhoan = "USER1" };
            context.Workspaces.Add(ws);

            // Seed Project
            var proj = new Duan { MaDuAn = "PROJ1", TenDuAn = "Project 1", MaWorkspace = "WS1", TrangThai = "ChuaThucHien" };
            context.Duans.Add(proj);

            // Seed Member
            var member = new Thanhvien { MaThanhVien = "MB1", MaDuAn = "PROJ1", MaTaiKhoan = "USER1", VaiTroDuAn = "Developer" };
            context.Thanhviens.Add(member);

            // Seed Tasks
            var task1 = new Congviec
            {
                MaCongViec = "TASK1",
                TenCongViec = "Task One",
                TrangThai = "ToDo",
                MucDoUuTien = "Cao",
                MaDuAn = "PROJ1",
                MaThanhVien = "MB1",
                NgayTao = DateTime.Now
            };
            var task2 = new Congviec
            {
                MaCongViec = "TASK2",
                TenCongViec = "Task Two",
                TrangThai = "Done",
                MucDoUuTien = "TrungBinh",
                MaDuAn = "PROJ1",
                MaThanhVien = "MB1",
                NgayTao = DateTime.Now,
                Deadline = DateTime.Now.AddDays(-2) // Past deadline
            };
            var task3 = new Congviec
            {
                MaCongViec = "TASK3",
                TenCongViec = "Task Three",
                TrangThai = "InProgress",
                MucDoUuTien = "Thap",
                MaDuAn = "PROJ1",
                MaThanhVien = "MB1",
                NgayTao = DateTime.Now,
                Deadline = DateTime.Now.AddDays(-1) // Overdue
            };

            context.Congviecs.AddRange(task1, task2, task3);
            await context.SaveChangesAsync();
        }

        [Fact]
        public async Task TienDo_ReturnsViewWithStats()
        {
            // Arrange
            using var context = TestHelper.GetInMemoryDbContext();
            await SeedDataAsync(context);
            var controller = new BaoCaoController(context);
            var sessionMock = TestHelper.CreateMockSession();
            sessionMock.SetSessionString("MaTaiKhoan", "USER1");
            controller.ControllerContext = TestHelper.CreateControllerContext(sessionMock.Object);

            // Act
            var result = await controller.TienDo("PROJ1");

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(3, controller.ViewBag.TotalTasks);
            Assert.Equal(1, controller.ViewBag.TodoCount);
            Assert.Equal(1, controller.ViewBag.InProgressCount);
            Assert.Equal(0, controller.ViewBag.ReviewCount);
            Assert.Equal(1, controller.ViewBag.DoneCount);
            Assert.Equal("PROJ1", controller.ViewBag.SelectedProject);
            Assert.NotNull(controller.ViewBag.Projects);
        }

        [Fact]
        public async Task HieuSuat_CalculatesMemberPerformance()
        {
            // Arrange
            using var context = TestHelper.GetInMemoryDbContext();
            await SeedDataAsync(context);
            var controller = new BaoCaoController(context);
            var sessionMock = TestHelper.CreateMockSession();
            sessionMock.SetSessionString("MaTaiKhoan", "USER1");
            controller.ControllerContext = TestHelper.CreateControllerContext(sessionMock.Object);

            // Act
            var result = await controller.HieuSuat("PROJ1");

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var stats = Assert.IsType<List<BaoCaoController.MemberPerformanceDto>>(controller.ViewBag.MemberStats);
            Assert.Single(stats);
            var memberStat = stats[0];
            Assert.Equal("Nguyen Van A", memberStat.HoTen);
            Assert.Equal("Project 1", memberStat.DuAn);
            Assert.Equal(3, memberStat.TotalTasks);
            Assert.Equal(1, memberStat.DoneTasks);
            Assert.Equal(1, memberStat.OverdueTasks); // Only TASK3 is overdue (TASK2 is Done, TASK1 has no deadline)
            Assert.Equal(1, memberStat.InProgressTasks);
        }

        [Fact]
        public async Task ExportExcel_ReturnsExcelFile()
        {
            // Arrange
            using var context = TestHelper.GetInMemoryDbContext();
            await SeedDataAsync(context);
            var controller = new BaoCaoController(context);
            var sessionMock = TestHelper.CreateMockSession();
            sessionMock.SetSessionString("MaTaiKhoan", "USER1");
            controller.ControllerContext = TestHelper.CreateControllerContext(sessionMock.Object);

            // Act
            var result = await controller.ExportExcel("PROJ1");

            // Assert
            var fileResult = Assert.IsType<FileStreamResult>(result);
            Assert.Equal("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileResult.ContentType);
            Assert.Contains("BaoCao_", fileResult.FileDownloadName);
            Assert.True(fileResult.FileStream.Length > 0);

            // Let's verify we can read it as a workbook
            using var workbook = new XLWorkbook(fileResult.FileStream);
            var sheet = workbook.Worksheet("Báo cáo công việc");
            Assert.NotNull(sheet);
            Assert.Equal("Mã CV", sheet.Cell(1, 1).Value.ToString());
            Assert.Equal("TASK2", sheet.Cell(2, 1).Value.ToString());
        }

        [Fact]
        public async Task ExportPdf_ReturnsViewWithTasks()
        {
            // Arrange
            using var context = TestHelper.GetInMemoryDbContext();
            await SeedDataAsync(context);
            var controller = new BaoCaoController(context);
            var sessionMock = TestHelper.CreateMockSession();
            sessionMock.SetSessionString("MaTaiKhoan", "USER1");
            controller.ControllerContext = TestHelper.CreateControllerContext(sessionMock.Object);

            // Act
            var result = await controller.ExportPdf("PROJ1");

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("ExportPdf", viewResult.ViewName);
            var model = Assert.IsType<List<Congviec>>(viewResult.Model);
            Assert.Equal(3, model.Count);
        }
    }
}
