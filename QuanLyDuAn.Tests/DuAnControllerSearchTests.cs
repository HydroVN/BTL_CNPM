using Microsoft.AspNetCore.Mvc;
using QuanLyDuAn.Controllers;
using QuanLyDuAn.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace QuanLyDuAn.Tests
{
    public class DuAnControllerSearchTests
    {
        [Fact]
        public async Task Index_SearchByProjectName_ReturnsFilteredProjects()
        {
            // Arrange
            using var context = TestHelper.GetInMemoryDbContext();
            var controller = new DuAnController(context);
            var sessionMock = TestHelper.CreateMockSession();
            sessionMock.SetSessionString("MaTaiKhoan", "USER1");
            controller.ControllerContext = TestHelper.CreateControllerContext(sessionMock.Object);

            // Seed Workspace
            var ws = new Workspace
            {
                MaWorkspace = "WS1",
                TenWorkspace = "Main Workspace",
                MaTaiKhoan = "USER1",
                NgayTao = DateTime.Now
            };
            context.Workspaces.Add(ws);

            // Seed Projects
            var proj1 = new Duan
            {
                MaDuAn = "PROJ1",
                TenDuAn = "Website Bán Hàng",
                MaWorkspace = "WS1",
                TrangThai = "ChuaThucHien",
                NgayBatDau = DateTime.Now
            };
            var proj2 = new Duan
            {
                MaDuAn = "PROJ2",
                TenDuAn = "Mobile Application",
                MaWorkspace = "WS1",
                TrangThai = "DangThucHien",
                NgayBatDau = DateTime.Now
            };
            context.Duans.AddRange(proj1, proj2);
            await context.SaveChangesAsync();

            // Act - Search for "Website"
            var result = await controller.Index(null, "Website", null);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Duan>>(viewResult.Model);
            var projectList = model.ToList();

            Assert.Single(projectList);
            Assert.Equal("PROJ1", projectList[0].MaDuAn);
            Assert.Equal("Website Bán Hàng", projectList[0].TenDuAn);
        }

        [Fact]
        public async Task Index_FilterByProjectStatus_ReturnsFilteredProjects()
        {
            // Arrange
            using var context = TestHelper.GetInMemoryDbContext();
            var controller = new DuAnController(context);
            var sessionMock = TestHelper.CreateMockSession();
            sessionMock.SetSessionString("MaTaiKhoan", "USER1");
            controller.ControllerContext = TestHelper.CreateControllerContext(sessionMock.Object);

            // Seed Workspace
            var ws = new Workspace
            {
                MaWorkspace = "WS1",
                TenWorkspace = "Main Workspace",
                MaTaiKhoan = "USER1",
                NgayTao = DateTime.Now
            };
            context.Workspaces.Add(ws);

            // Seed Projects
            var proj1 = new Duan
            {
                MaDuAn = "PROJ1",
                TenDuAn = "Website Bán Hàng",
                MaWorkspace = "WS1",
                TrangThai = "ChuaThucHien",
                NgayBatDau = DateTime.Now
            };
            var proj2 = new Duan
            {
                MaDuAn = "PROJ2",
                TenDuAn = "Mobile Application",
                MaWorkspace = "WS1",
                TrangThai = "DangThucHien",
                NgayBatDau = DateTime.Now
            };
            context.Duans.AddRange(proj1, proj2);
            await context.SaveChangesAsync();

            // Act - Filter status "DangThucHien"
            var result = await controller.Index(null, null, "DangThucHien");

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Duan>>(viewResult.Model);
            var projectList = model.ToList();

            Assert.Single(projectList);
            Assert.Equal("PROJ2", projectList[0].MaDuAn);
            Assert.Equal("Mobile Application", projectList[0].TenDuAn);
        }
    }
}
