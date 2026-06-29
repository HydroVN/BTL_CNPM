using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyDuAn.Controllers;
using QuanLyDuAn.Data;
using QuanLyDuAn.Models;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Xunit;

namespace QuanLyDuAn.Tests
{
    public class HomeControllerTests
    {
        [Fact]
        public void Index_NotLoggedIn_ReturnsView()
        {
            // Arrange
            using var context = TestHelper.GetInMemoryDbContext();
            var controller = new HomeController(context);
            var sessionMock = TestHelper.CreateMockSession(); // No MaTaiKhoan
            controller.ControllerContext = TestHelper.CreateControllerContext(sessionMock.Object);

            // Act
            var result = controller.Index();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Index_LoggedIn_RedirectsToWorkspace()
        {
            // Arrange
            using var context = TestHelper.GetInMemoryDbContext();
            var controller = new HomeController(context);
            var sessionMock = TestHelper.CreateMockSession();
            sessionMock.SetSessionString("MaTaiKhoan", "USER1");
            controller.ControllerContext = TestHelper.CreateControllerContext(sessionMock.Object);

            // Act
            var result = controller.Index();

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            Assert.Equal("Workspace", redirectResult.ControllerName);
        }

        [Fact]
        public async Task Pricing_ReturnsViewWithPackages()
        {
            // Arrange
            using var context = TestHelper.GetInMemoryDbContext();
            var controller = new HomeController(context);
            
            // Seed service packages
            context.Goidichvus.AddRange(
                new Goidichvu { MaGoi = "G1", TenGoi = "Free", Gia = 0, SoDuAnToiDa = 1, SoTvToiDa = 5, MoTa = "Free plan", DungLuongMax = 100 },
                new Goidichvu { MaGoi = "G2", TenGoi = "Pro", Gia = 99000, SoDuAnToiDa = 10, SoTvToiDa = 50, MoTa = "Pro plan", DungLuongMax = 1000 }
            );
            await context.SaveChangesAsync();

            // Act
            var result = await controller.Pricing();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<List<Goidichvu>>(viewResult.Model);
            Assert.Equal(2, model.Count);
            Assert.Equal("Free", model[0].TenGoi);
        }

        [Fact]
        public void Privacy_ReturnsView()
        {
            // Arrange
            using var context = TestHelper.GetInMemoryDbContext();
            var controller = new HomeController(context);

            // Act
            var result = controller.Privacy();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Error_ReturnsViewWithErrorModel()
        {
            // Arrange
            using var context = TestHelper.GetInMemoryDbContext();
            var controller = new HomeController(context);
            var sessionMock = TestHelper.CreateMockSession();
            controller.ControllerContext = TestHelper.CreateControllerContext(sessionMock.Object);
            controller.HttpContext.TraceIdentifier = "TEST_TRACE_ID";

            // Act
            var result = controller.Error();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ErrorViewModel>(viewResult.Model);
            Assert.Equal("TEST_TRACE_ID", model.RequestId);
        }
    }
}
