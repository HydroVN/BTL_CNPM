using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyDuAn.Controllers;
using QuanLyDuAn.Data;
using QuanLyDuAn.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace QuanLyDuAn.Tests
{
    public class ThanhVienControllerTests
    {
        private async Task SeedDataAsync(BtlCnpmContext context)
        {
            // Seed Users
            var admin = new Taikhoan { MaTaiKhoan = "ADMIN", HoTen = "System Admin", Email = "admin@gmail.com", MatKhau = "123", VaiTro = "Admin", TrangThai = "KichHoat" };
            var user1 = new Taikhoan { MaTaiKhoan = "USER1", HoTen = "Nguyen Van A", Email = "a@gmail.com", MatKhau = "123", VaiTro = "User", TrangThai = "KichHoat" };
            var user2 = new Taikhoan { MaTaiKhoan = "USER2", HoTen = "Tran Van B", Email = "b@gmail.com", MatKhau = "123", VaiTro = "User", TrangThai = "KichHoat" };
            var user3 = new Taikhoan { MaTaiKhoan = "USER3", HoTen = "Le Thi C", Email = "c@gmail.com", MatKhau = "123", VaiTro = "User", TrangThai = "KichHoat" };

            context.Taikhoans.AddRange(admin, user1, user2, user3);

            // Seed Workspace
            var ws = new Workspace { MaWorkspace = "WS1", TenWorkspace = "Workspace 1", MaTaiKhoan = "USER1" };
            context.Workspaces.Add(ws);

            // Seed Projects
            var proj1 = new Duan { MaDuAn = "PROJ1", TenDuAn = "Project 1", MaWorkspace = "WS1", TrangThai = "ChuaThucHien" };
            var proj2 = new Duan { MaDuAn = "PROJ2", TenDuAn = "Project 2", MaWorkspace = "WS1", TrangThai = "ChuaThucHien" };
            context.Duans.AddRange(proj1, proj2);

            // Seed Members
            var member1 = new Thanhvien { MaThanhVien = "MB1", MaDuAn = "PROJ1", MaTaiKhoan = "USER1", VaiTroDuAn = "Manager" };
            var member2 = new Thanhvien { MaThanhVien = "MB2", MaDuAn = "PROJ1", MaTaiKhoan = "USER2", VaiTroDuAn = "Developer" };
            context.Thanhviens.AddRange(member1, member2);

            await context.SaveChangesAsync();
        }

        // ==================== WORKSPACE GROUP TESTS ====================

        [Fact]
        public async Task Index_ReturnsViewWithMembersAndLinks()
        {
            // Arrange
            using var context = TestHelper.GetInMemoryDbContext();
            await SeedDataAsync(context);
            var controller = new ThanhVienController(context);
            var sessionMock = TestHelper.CreateMockSession();
            sessionMock.SetSessionString("MaTaiKhoan", "USER1");
            controller.ControllerContext = TestHelper.CreateControllerContext(sessionMock.Object);

            // Seed invite link
            context.Invitelinks.Add(new Invitelink
            {
                MaLink = "LINK1",
                MaWorkspace = "WS1",
                Token = "TOKEN123",
                VaiTroMacDinh = "Developer",
                NgayHetHan = DateTime.Now.AddDays(1)
            });
            await context.SaveChangesAsync();

            // Act
            var result = await controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<List<Thanhvien>>(viewResult.Model);
            Assert.Equal(2, model.Count); // MB1 and MB2
            Assert.NotNull(controller.ViewBag.Workspaces);
            Assert.NotNull(controller.ViewBag.InviteLinks);
        }

        [Fact]
        public async Task GenerateInviteLink_Owner_GeneratesSuccess()
        {
            // Arrange
            using var context = TestHelper.GetInMemoryDbContext();
            await SeedDataAsync(context);
            var controller = new ThanhVienController(context);
            var sessionMock = TestHelper.CreateMockSession();
            sessionMock.SetSessionString("MaTaiKhoan", "USER1");
            sessionMock.SetSessionString("VaiTro", "User");
            controller.ControllerContext = TestHelper.CreateControllerContext(sessionMock.Object);
            controller.SetupTempData();

            controller.Request.Scheme = "http";
            controller.Request.Host = new HostString("localhost");

            // Act
            var result = await controller.GenerateInviteLink("WS1", "Developer");

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            Assert.Contains("/ThanhVien/Join?token=", controller.TempData["InviteLink"]?.ToString());
            Assert.Equal("Đường dẫn mời đã được tạo thành công!", controller.TempData["Success"]);

            var invite = await context.Invitelinks.FirstOrDefaultAsync(i => i.MaWorkspace == "WS1");
            Assert.NotNull(invite);
            Assert.Equal("Developer", invite.VaiTroMacDinh);
        }

        [Fact]
        public async Task GenerateInviteLink_NonOwner_ReturnsForbid()
        {
            // Arrange
            using var context = TestHelper.GetInMemoryDbContext();
            await SeedDataAsync(context);
            var controller = new ThanhVienController(context);
            var sessionMock = TestHelper.CreateMockSession();
            sessionMock.SetSessionString("MaTaiKhoan", "USER2"); // Not owner
            sessionMock.SetSessionString("VaiTro", "User");
            controller.ControllerContext = TestHelper.CreateControllerContext(sessionMock.Object);

            // Act
            var result = await controller.GenerateInviteLink("WS1", "Developer");

            // Assert
            Assert.IsType<ForbidResult>(result);
        }

        [Fact]
        public async Task Join_ValidToken_NotLoggedIn_RedirectsToRegister()
        {
            // Arrange
            using var context = TestHelper.GetInMemoryDbContext();
            await SeedDataAsync(context);
            var controller = new ThanhVienController(context);
            var sessionMock = TestHelper.CreateMockSession(); // No MaTaiKhoan
            controller.ControllerContext = TestHelper.CreateControllerContext(sessionMock.Object);

            var invite = new Invitelink { MaLink = "L1", MaWorkspace = "WS1", Token = "TOKEN1", VaiTroMacDinh = "Developer", NgayHetHan = DateTime.Now.AddDays(1) };
            context.Invitelinks.Add(invite);
            await context.SaveChangesAsync();

            // Act
            var result = await controller.Join("TOKEN1");

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Register", redirectResult.ActionName);
            Assert.Equal("Account", redirectResult.ControllerName);
            Assert.Equal("TOKEN1", redirectResult.RouteValues?["token"]);
        }

        [Fact]
        public async Task Join_ValidToken_LoggedIn_ReturnsConfirmView()
        {
            // Arrange
            using var context = TestHelper.GetInMemoryDbContext();
            await SeedDataAsync(context);
            var controller = new ThanhVienController(context);
            var sessionMock = TestHelper.CreateMockSession();
            sessionMock.SetSessionString("MaTaiKhoan", "USER3");
            controller.ControllerContext = TestHelper.CreateControllerContext(sessionMock.Object);

            var invite = new Invitelink { MaLink = "L1", MaWorkspace = "WS1", Token = "TOKEN1", VaiTroMacDinh = "Developer", NgayHetHan = DateTime.Now.AddDays(1) };
            context.Invitelinks.Add(invite);
            await context.SaveChangesAsync();

            // Act
            var result = await controller.Join("TOKEN1");

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<Invitelink>(viewResult.Model);
            Assert.Equal("TOKEN1", model.Token);
        }

        [Fact]
        public async Task AcceptJoin_ValidToken_AddsUserToAllWorkspaceProjects()
        {
            // Arrange
            using var context = TestHelper.GetInMemoryDbContext();
            await SeedDataAsync(context);
            var controller = new ThanhVienController(context);
            var sessionMock = TestHelper.CreateMockSession();
            sessionMock.SetSessionString("MaTaiKhoan", "USER3"); // C là user mới tham gia
            controller.ControllerContext = TestHelper.CreateControllerContext(sessionMock.Object);
            controller.SetupTempData();

            var invite = new Invitelink { MaLink = "L1", MaWorkspace = "WS1", Token = "TOKEN1", VaiTroMacDinh = "Developer", NgayHetHan = DateTime.Now.AddDays(1) };
            context.Invitelinks.Add(invite);
            await context.SaveChangesAsync();

            // Act
            var result = await controller.AcceptJoin("TOKEN1");

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            Assert.Equal("Workspace", redirectResult.ControllerName);

            // User3 should be added to PROJ1 and PROJ2
            var memberships = await context.Thanhviens.Where(t => t.MaTaiKhoan == "USER3").ToListAsync();
            Assert.Equal(2, memberships.Count);
            Assert.True(memberships.All(m => m.VaiTroDuAn == "Developer"));
        }

        [Fact]
        public void ExpiredLink_ReturnsView()
        {
            // Arrange
            using var context = TestHelper.GetInMemoryDbContext();
            var controller = new ThanhVienController(context);

            // Act
            var result = controller.ExpiredLink();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        // ==================== PROJECT INVITE GROUP TESTS ====================

        [Fact]
        public async Task AddToProject_Owner_ReturnsView()
        {
            // Arrange
            using var context = TestHelper.GetInMemoryDbContext();
            await SeedDataAsync(context);
            var controller = new ThanhVienController(context);
            var sessionMock = TestHelper.CreateMockSession();
            sessionMock.SetSessionString("MaTaiKhoan", "USER1"); // Owner of Workspace
            controller.ControllerContext = TestHelper.CreateControllerContext(sessionMock.Object);

            // Act
            var result = await controller.AddToProject("PROJ1");

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var duAn = Assert.IsType<Duan>(controller.ViewBag.DuAn);
            Assert.Equal("PROJ1", duAn.MaDuAn);
        }

        [Fact]
        public async Task AddToProject_NonOwner_RedirectsToDetails()
        {
            // Arrange
            using var context = TestHelper.GetInMemoryDbContext();
            await SeedDataAsync(context);
            var controller = new ThanhVienController(context);
            var sessionMock = TestHelper.CreateMockSession();
            sessionMock.SetSessionString("MaTaiKhoan", "USER2"); // Member, not Owner
            sessionMock.SetSessionString("VaiTro", "User");
            controller.ControllerContext = TestHelper.CreateControllerContext(sessionMock.Object);
            controller.SetupTempData();

            // Act
            var result = await controller.AddToProject("PROJ1");

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Details", redirectResult.ActionName);
            Assert.Equal("DuAn", redirectResult.ControllerName);
            Assert.Equal("Bạn không có quyền mời thành viên vào dự án này.", controller.TempData["Error"]);
        }

        [Fact]
        public async Task CheckEmail_Self_ReturnsError()
        {
            // Arrange
            using var context = TestHelper.GetInMemoryDbContext();
            await SeedDataAsync(context);
            var controller = new ThanhVienController(context);
            var sessionMock = TestHelper.CreateMockSession();
            sessionMock.SetSessionString("Email", "a@gmail.com");
            controller.ControllerContext = TestHelper.CreateControllerContext(sessionMock.Object);

            // Act
            var result = await controller.CheckEmail("a@gmail.com", "PROJ1");

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var value = jsonResult.Value;
            var type = value.GetType();
            Assert.False((bool)type.GetProperty("success")!.GetValue(value)!);
            Assert.Equal("Bạn không thể mời chính bản thân mình vào dự án.", type.GetProperty("message")!.GetValue(value));
        }

        [Fact]
        public async Task CheckEmail_Unregistered_ReturnsError()
        {
            // Arrange
            using var context = TestHelper.GetInMemoryDbContext();
            await SeedDataAsync(context);
            var controller = new ThanhVienController(context);
            var sessionMock = TestHelper.CreateMockSession();
            sessionMock.SetSessionString("Email", "a@gmail.com");
            controller.ControllerContext = TestHelper.CreateControllerContext(sessionMock.Object);

            // Act
            var result = await controller.CheckEmail("unknown@gmail.com", "PROJ1");

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var value = jsonResult.Value;
            var type = value.GetType();
            Assert.False((bool)type.GetProperty("success")!.GetValue(value)!);
            Assert.Equal("Email này chưa đăng ký tài khoản trên hệ thống.", type.GetProperty("message")!.GetValue(value));
        }

        [Fact]
        public async Task CheckEmail_AlreadyMember_ReturnsError()
        {
            // Arrange
            using var context = TestHelper.GetInMemoryDbContext();
            await SeedDataAsync(context);
            var controller = new ThanhVienController(context);
            var sessionMock = TestHelper.CreateMockSession();
            sessionMock.SetSessionString("Email", "a@gmail.com");
            controller.ControllerContext = TestHelper.CreateControllerContext(sessionMock.Object);

            // Act
            var result = await controller.CheckEmail("b@gmail.com", "PROJ1"); // USER2 is already in PROJ1

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var value = jsonResult.Value;
            var type = value.GetType();
            Assert.False((bool)type.GetProperty("success")!.GetValue(value)!);
            Assert.Equal("Người dùng này đã là thành viên của dự án.", type.GetProperty("message")!.GetValue(value));
        }

        [Fact]
        public async Task CheckEmail_Valid_ReturnsSuccess()
        {
            // Arrange
            using var context = TestHelper.GetInMemoryDbContext();
            await SeedDataAsync(context);
            var controller = new ThanhVienController(context);
            var sessionMock = TestHelper.CreateMockSession();
            sessionMock.SetSessionString("Email", "a@gmail.com");
            controller.ControllerContext = TestHelper.CreateControllerContext(sessionMock.Object);

            // Act
            var result = await controller.CheckEmail("c@gmail.com", "PROJ1"); // USER3 Le Thi C

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var value = jsonResult.Value;
            var type = value.GetType();
            Assert.True((bool)type.GetProperty("success")!.GetValue(value)!);
            Assert.Equal("Le Thi C", type.GetProperty("name")!.GetValue(value));
            Assert.Equal("c@gmail.com", type.GetProperty("email")!.GetValue(value));
        }

        [Fact]
        public async Task SendProjectInvite_CreatesNotification()
        {
            // Arrange
            using var context = TestHelper.GetInMemoryDbContext();
            await SeedDataAsync(context);
            var controller = new ThanhVienController(context);
            var sessionMock = TestHelper.CreateMockSession();
            sessionMock.SetSessionString("MaTaiKhoan", "USER1");
            sessionMock.SetSessionString("HoTen", "Nguyen Van A");
            controller.ControllerContext = TestHelper.CreateControllerContext(sessionMock.Object);
            controller.SetupTempData();

            // Act
            var result = await controller.SendProjectInvite("PROJ1", "c@gmail.com", "Developer");

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Details", redirectResult.ActionName);
            Assert.Equal("DuAn", redirectResult.ControllerName);
            Assert.Equal("PROJ1", redirectResult.RouteValues?["id"]);

            // Verify notification created for USER3 (c@gmail.com)
            var notification = await context.Thongbaos.FirstOrDefaultAsync(t => t.MaTaiKhoan == "USER3");
            Assert.NotNull(notification);
            Assert.Contains("PROJECT_INVITE|PROJ1|Developer", notification.NoiDung);
            Assert.Equal("ChuaDoc", notification.TrangThai);
        }

        [Fact]
        public async Task AcceptProjectInvite_Success_AddsMember()
        {
            // Arrange
            using var context = TestHelper.GetInMemoryDbContext();
            await SeedDataAsync(context);
            var controller = new ThanhVienController(context);
            var sessionMock = TestHelper.CreateMockSession();
            sessionMock.SetSessionString("MaTaiKhoan", "USER3"); // recipient is Le Thi C
            controller.ControllerContext = TestHelper.CreateControllerContext(sessionMock.Object);
            controller.SetupTempData();

            var thongBao = new Thongbao
            {
                MaThongBao = "TB1",
                MaTaiKhoan = "USER3",
                NoiDung = "PROJECT_INVITE|PROJ1|Developer|Bạn được mời tham gia...",
                TrangThai = "ChuaDoc",
                ThoiGian = DateTime.Now
            };
            context.Thongbaos.Add(thongBao);
            await context.SaveChangesAsync();

            // Act
            var result = await controller.AcceptProjectInvite("TB1");

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            Assert.Equal("Workspace", redirectResult.ControllerName);

            // User3 should be added as member of PROJ1
            var isMember = await context.Thanhviens.AnyAsync(t => t.MaDuAn == "PROJ1" && t.MaTaiKhoan == "USER3" && t.VaiTroDuAn == "Developer");
            Assert.True(isMember);

            // Notification should be marked read
            var tb = await context.Thongbaos.FindAsync("TB1");
            Assert.Equal("DaDoc", tb!.TrangThai);
            Assert.Equal("Bạn đã chấp nhận tham gia dự án.", tb.NoiDung);
        }

        [Fact]
        public async Task DeclineProjectInvite_Success_MarksRead()
        {
            // Arrange
            using var context = TestHelper.GetInMemoryDbContext();
            await SeedDataAsync(context);
            var controller = new ThanhVienController(context);
            var sessionMock = TestHelper.CreateMockSession();
            sessionMock.SetSessionString("MaTaiKhoan", "USER3");
            controller.ControllerContext = TestHelper.CreateControllerContext(sessionMock.Object);
            controller.SetupTempData();

            var thongBao = new Thongbao
            {
                MaThongBao = "TB1",
                MaTaiKhoan = "USER3",
                NoiDung = "PROJECT_INVITE|PROJ1|Developer|...",
                TrangThai = "ChuaDoc",
                ThoiGian = DateTime.Now
            };
            context.Thongbaos.Add(thongBao);
            await context.SaveChangesAsync();

            // Act
            var result = await controller.DeclineProjectInvite("TB1");

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            Assert.Equal("Workspace", redirectResult.ControllerName);

            // User3 should NOT be added
            var isMember = await context.Thanhviens.AnyAsync(t => t.MaDuAn == "PROJ1" && t.MaTaiKhoan == "USER3");
            Assert.False(isMember);

            // Notification should be marked read
            var tb = await context.Thongbaos.FindAsync("TB1");
            Assert.Equal("DaDoc", tb!.TrangThai);
            Assert.Equal("Bạn đã từ chối lời mời tham gia dự án.", tb.NoiDung);
        }

        // ==================== ADMIN ACTIONS ====================

        [Fact]
        public async Task AdminUsers_AsAdmin_ReturnsView()
        {
            // Arrange
            using var context = TestHelper.GetInMemoryDbContext();
            await SeedDataAsync(context);
            var controller = new ThanhVienController(context);
            var sessionMock = TestHelper.CreateMockSession();
            sessionMock.SetSessionString("VaiTro", "Admin");
            controller.ControllerContext = TestHelper.CreateControllerContext(sessionMock.Object);

            // Act
            var result = await controller.AdminUsers();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<List<Taikhoan>>(viewResult.Model);
            Assert.Equal(4, model.Count);
        }

        [Fact]
        public async Task AdminUsers_AsUser_RedirectsToWorkspace()
        {
            // Arrange
            using var context = TestHelper.GetInMemoryDbContext();
            await SeedDataAsync(context);
            var controller = new ThanhVienController(context);
            var sessionMock = TestHelper.CreateMockSession();
            sessionMock.SetSessionString("VaiTro", "User");
            controller.ControllerContext = TestHelper.CreateControllerContext(sessionMock.Object);

            // Act
            var result = await controller.AdminUsers();

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            Assert.Equal("Workspace", redirectResult.ControllerName);
        }

        [Fact]
        public async Task AdminResetPassword_Success()
        {
            // Arrange
            using var context = TestHelper.GetInMemoryDbContext();
            await SeedDataAsync(context);
            var controller = new ThanhVienController(context);
            var sessionMock = TestHelper.CreateMockSession();
            sessionMock.SetSessionString("VaiTro", "Admin");
            controller.ControllerContext = TestHelper.CreateControllerContext(sessionMock.Object);
            controller.SetupTempData();

            // Act
            var result = await controller.AdminResetPassword("USER2", "newsecurepass123");

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("AdminUsers", redirectResult.ActionName);
            Assert.Equal("Đã đặt lại mật khẩu cho tài khoản Tran Van B (b@gmail.com) thành công!", controller.TempData["Success"]);

            var user = await context.Taikhoans.FindAsync("USER2");
            Assert.NotEqual("123", user!.MatKhau); // Should be hashed
        }

        // ==================== ROLE & REMOVE MEMBER ACTIONS ====================

        [Fact]
        public async Task UpdateMemberRole_Owner_UpdatesSuccessfully()
        {
            // Arrange
            using var context = TestHelper.GetInMemoryDbContext();
            await SeedDataAsync(context);
            var controller = new ThanhVienController(context);
            var sessionMock = TestHelper.CreateMockSession();
            sessionMock.SetSessionString("MaTaiKhoan", "USER1"); // Owner
            controller.ControllerContext = TestHelper.CreateControllerContext(sessionMock.Object);
            controller.SetupTempData();

            // Act
            var result = await controller.UpdateMemberRole("MB2", "PROJ1", "Tester");

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Details", redirectResult.ActionName);
            Assert.Equal("DuAn", redirectResult.ControllerName);
            Assert.Equal("PROJ1", redirectResult.RouteValues?["id"]);

            var tv = await context.Thanhviens.FindAsync("MB2");
            Assert.Equal("Tester", tv!.VaiTroDuAn);
        }

        [Fact]
        public async Task RemoveMember_Owner_RemovesSuccessfully()
        {
            // Arrange
            using var context = TestHelper.GetInMemoryDbContext();
            await SeedDataAsync(context);
            var controller = new ThanhVienController(context);
            var sessionMock = TestHelper.CreateMockSession();
            sessionMock.SetSessionString("MaTaiKhoan", "USER1"); // Owner
            controller.ControllerContext = TestHelper.CreateControllerContext(sessionMock.Object);
            controller.SetupTempData();

            // Act
            var result = await controller.RemoveMember("MB2", "PROJ1"); // Remove USER2

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Details", redirectResult.ActionName);
            Assert.Equal("DuAn", redirectResult.ControllerName);

            var tv = await context.Thanhviens.FindAsync("MB2");
            Assert.Null(tv); // Should be removed
        }

        [Fact]
        public async Task RemoveMember_CannotRemoveOwner()
        {
            // Arrange
            using var context = TestHelper.GetInMemoryDbContext();
            await SeedDataAsync(context);
            var controller = new ThanhVienController(context);
            var sessionMock = TestHelper.CreateMockSession();
            sessionMock.SetSessionString("MaTaiKhoan", "USER1"); // Owner
            controller.ControllerContext = TestHelper.CreateControllerContext(sessionMock.Object);
            controller.SetupTempData();

            // Act
            var result = await controller.RemoveMember("MB1", "PROJ1"); // Try to remove USER1 (Owner of Workspace WS1)

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Details", redirectResult.ActionName);
            Assert.Equal("DuAn", redirectResult.ControllerName);
            Assert.Equal("Không thể xóa chủ sở hữu dự án.", controller.TempData["Error"]);

            var tv = await context.Thanhviens.FindAsync("MB1");
            Assert.NotNull(tv); // Should still exist
        }

        // ==================== ADDITIONAL EXHAUSTIVE ERROR BRANCH TESTS ====================

        [Fact]
        public async Task Join_NullOrEmptyToken_RedirectsToExpiredLink()
        {
            using var context = TestHelper.GetInMemoryDbContext();
            var controller = new ThanhVienController(context);

            var result = await controller.Join("");
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("ExpiredLink", redirectResult.ActionName);
        }

        [Fact]
        public async Task Join_ExpiredOrInvalidToken_RedirectsToExpiredLink()
        {
            using var context = TestHelper.GetInMemoryDbContext();
            await SeedDataAsync(context);
            var controller = new ThanhVienController(context);
            var sessionMock = TestHelper.CreateMockSession();
            controller.ControllerContext = TestHelper.CreateControllerContext(sessionMock.Object);

            // Seed expired invite link
            context.Invitelinks.Add(new Invitelink
            {
                MaLink = "L_EXP",
                MaWorkspace = "WS1",
                Token = "TOKEN_EXP",
                VaiTroMacDinh = "Developer",
                NgayHetHan = DateTime.Now.AddDays(-1) // Expired
            });
            await context.SaveChangesAsync();

            var result = await controller.Join("TOKEN_EXP");
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("ExpiredLink", redirectResult.ActionName);

            var resultInvalid = await controller.Join("INVALID_TOKEN");
            var redirectResultInvalid = Assert.IsType<RedirectToActionResult>(resultInvalid);
            Assert.Equal("ExpiredLink", redirectResultInvalid.ActionName);
        }

        [Fact]
        public async Task AcceptJoin_ExpiredOrInvalidToken_RedirectsToExpiredLink()
        {
            using var context = TestHelper.GetInMemoryDbContext();
            await SeedDataAsync(context);
            var controller = new ThanhVienController(context);
            var sessionMock = TestHelper.CreateMockSession();
            sessionMock.SetSessionString("MaTaiKhoan", "USER3");
            controller.ControllerContext = TestHelper.CreateControllerContext(sessionMock.Object);

            context.Invitelinks.Add(new Invitelink
            {
                MaLink = "L_EXP",
                MaWorkspace = "WS1",
                Token = "TOKEN_EXP",
                VaiTroMacDinh = "Developer",
                NgayHetHan = DateTime.Now.AddDays(-1) // Expired
            });
            await context.SaveChangesAsync();

            var result = await controller.AcceptJoin("TOKEN_EXP");
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("ExpiredLink", redirectResult.ActionName);
        }

        [Fact]
        public async Task AcceptJoin_NotLoggedIn_RedirectsToLogin()
        {
            using var context = TestHelper.GetInMemoryDbContext();
            var controller = new ThanhVienController(context);
            var sessionMock = TestHelper.CreateMockSession(); // No logged in user
            controller.ControllerContext = TestHelper.CreateControllerContext(sessionMock.Object);

            var result = await controller.AcceptJoin("SOME_TOKEN");
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Login", redirectResult.ActionName);
            Assert.Equal("Account", redirectResult.ControllerName);
        }

        [Fact]
        public async Task CheckEmail_EmptyEmail_ReturnsError()
        {
            using var context = TestHelper.GetInMemoryDbContext();
            var controller = new ThanhVienController(context);
            var sessionMock = TestHelper.CreateMockSession();
            controller.ControllerContext = TestHelper.CreateControllerContext(sessionMock.Object);

            var result = await controller.CheckEmail("", "PROJ1");
            var jsonResult = Assert.IsType<JsonResult>(result);
            var val = jsonResult.Value;
            var type = val.GetType();
            Assert.False((bool)type.GetProperty("success")!.GetValue(val)!);
            Assert.Equal("Email không được để trống", type.GetProperty("message")!.GetValue(val));
        }

        [Fact]
        public async Task SendProjectInvite_ProjectNotFound_RedirectsToDuAnIndex()
        {
            using var context = TestHelper.GetInMemoryDbContext();
            var controller = new ThanhVienController(context);

            var result = await controller.SendProjectInvite("NOT_FOUND", "email@test.com", "Developer");
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            Assert.Equal("DuAn", redirectResult.ControllerName);
        }

        [Fact]
        public async Task SendProjectInvite_NonOwner_RedirectsToDetailsWithError()
        {
            using var context = TestHelper.GetInMemoryDbContext();
            await SeedDataAsync(context);
            var controller = new ThanhVienController(context);
            var sessionMock = TestHelper.CreateMockSession();
            sessionMock.SetSessionString("MaTaiKhoan", "USER2"); // Not owner of WS1
            sessionMock.SetSessionString("VaiTro", "User");
            controller.ControllerContext = TestHelper.CreateControllerContext(sessionMock.Object);
            controller.SetupTempData();

            var result = await controller.SendProjectInvite("PROJ1", "c@gmail.com", "Developer");
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Details", redirectResult.ActionName);
            Assert.Equal("DuAn", redirectResult.ControllerName);
            Assert.Equal("Bạn không có quyền mời thành viên.", controller.TempData["Error"]);
        }

        [Fact]
        public async Task SendProjectInvite_SelfInvite_RedirectsToDetailsWithError()
        {
            using var context = TestHelper.GetInMemoryDbContext();
            await SeedDataAsync(context);
            var controller = new ThanhVienController(context);
            var sessionMock = TestHelper.CreateMockSession();
            sessionMock.SetSessionString("MaTaiKhoan", "USER1"); // Owner
            sessionMock.SetSessionString("Email", "a@gmail.com");
            controller.ControllerContext = TestHelper.CreateControllerContext(sessionMock.Object);
            controller.SetupTempData();

            var result = await controller.SendProjectInvite("PROJ1", "a@gmail.com", "Developer");
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Details", redirectResult.ActionName);
            Assert.Equal("DuAn", redirectResult.ControllerName);
            Assert.Equal("Bạn không thể mời chính bản thân mình vào dự án.", controller.TempData["Error"]);
        }

        [Fact]
        public async Task SendProjectInvite_UnregisteredEmail_RedirectsToDetailsWithError()
        {
            using var context = TestHelper.GetInMemoryDbContext();
            await SeedDataAsync(context);
            var controller = new ThanhVienController(context);
            var sessionMock = TestHelper.CreateMockSession();
            sessionMock.SetSessionString("MaTaiKhoan", "USER1"); // Owner
            sessionMock.SetSessionString("Email", "a@gmail.com");
            controller.ControllerContext = TestHelper.CreateControllerContext(sessionMock.Object);
            controller.SetupTempData();

            var result = await controller.SendProjectInvite("PROJ1", "unregistered@test.com", "Developer");
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Details", redirectResult.ActionName);
            Assert.Equal("DuAn", redirectResult.ControllerName);
            Assert.Equal("Email không tồn tại.", controller.TempData["Error"]);
        }

        [Fact]
        public async Task SendProjectInvite_AlreadyMember_RedirectsToDetailsWithError()
        {
            using var context = TestHelper.GetInMemoryDbContext();
            await SeedDataAsync(context);
            var controller = new ThanhVienController(context);
            var sessionMock = TestHelper.CreateMockSession();
            sessionMock.SetSessionString("MaTaiKhoan", "USER1"); // Owner
            sessionMock.SetSessionString("Email", "a@gmail.com");
            controller.ControllerContext = TestHelper.CreateControllerContext(sessionMock.Object);
            controller.SetupTempData();

            var result = await controller.SendProjectInvite("PROJ1", "b@gmail.com", "Developer"); // USER2 is already in PROJ1
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Details", redirectResult.ActionName);
            Assert.Equal("DuAn", redirectResult.ControllerName);
            Assert.Equal("Người dùng đã là thành viên của dự án.", controller.TempData["Error"]);
        }

        [Fact]
        public async Task AcceptProjectInvite_NotificationNotFound_RedirectsToWorkspace()
        {
            using var context = TestHelper.GetInMemoryDbContext();
            var controller = new ThanhVienController(context);
            controller.SetupTempData();

            var result = await controller.AcceptProjectInvite("NOT_FOUND_NOTIFICATION");
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            Assert.Equal("Workspace", redirectResult.ControllerName);
            Assert.Equal("Thông báo không tồn tại.", controller.TempData["Error"]);
        }

        [Fact]
        public async Task AcceptProjectInvite_UnauthorizedUser_ReturnsForbid()
        {
            using var context = TestHelper.GetInMemoryDbContext();
            await SeedDataAsync(context);
            var controller = new ThanhVienController(context);
            var sessionMock = TestHelper.CreateMockSession();
            sessionMock.SetSessionString("MaTaiKhoan", "USER2"); // Receiver is USER3 in notification
            controller.ControllerContext = TestHelper.CreateControllerContext(sessionMock.Object);

            var tb = new Thongbao
            {
                MaThongBao = "TB1",
                MaTaiKhoan = "USER3",
                NoiDung = "PROJECT_INVITE|PROJ1|Developer",
                TrangThai = "ChuaDoc",
                ThoiGian = DateTime.Now
            };
            context.Thongbaos.Add(tb);
            await context.SaveChangesAsync();

            var result = await controller.AcceptProjectInvite("TB1");
            Assert.IsType<ForbidResult>(result);
        }

        [Fact]
        public async Task DeclineProjectInvite_NotificationNotFound_RedirectsToWorkspace()
        {
            using var context = TestHelper.GetInMemoryDbContext();
            var controller = new ThanhVienController(context);
            controller.SetupTempData();

            var result = await controller.DeclineProjectInvite("NOT_FOUND_NOTIFICATION");
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            Assert.Equal("Workspace", redirectResult.ControllerName);
            Assert.Equal("Thông báo không tồn tại.", controller.TempData["Error"]);
        }

        [Fact]
        public async Task DeclineProjectInvite_UnauthorizedUser_ReturnsForbid()
        {
            using var context = TestHelper.GetInMemoryDbContext();
            await SeedDataAsync(context);
            var controller = new ThanhVienController(context);
            var sessionMock = TestHelper.CreateMockSession();
            sessionMock.SetSessionString("MaTaiKhoan", "USER2"); // Receiver is USER3
            controller.ControllerContext = TestHelper.CreateControllerContext(sessionMock.Object);

            var tb = new Thongbao
            {
                MaThongBao = "TB1",
                MaTaiKhoan = "USER3",
                NoiDung = "PROJECT_INVITE|PROJ1|Developer",
                TrangThai = "ChuaDoc",
                ThoiGian = DateTime.Now
            };
            context.Thongbaos.Add(tb);
            await context.SaveChangesAsync();

            var result = await controller.DeclineProjectInvite("TB1");
            Assert.IsType<ForbidResult>(result);
        }

        [Fact]
        public async Task AdminResetPassword_EmptyPassword_ReturnsError()
        {
            using var context = TestHelper.GetInMemoryDbContext();
            var controller = new ThanhVienController(context);
            var sessionMock = TestHelper.CreateMockSession();
            sessionMock.SetSessionString("VaiTro", "Admin");
            controller.ControllerContext = TestHelper.CreateControllerContext(sessionMock.Object);
            controller.SetupTempData();

            var result = await controller.AdminResetPassword("USER2", "   ");
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("AdminUsers", redirectResult.ActionName);
            Assert.Equal("Mật khẩu mới không được để trống.", controller.TempData["Error"]);
        }

        [Fact]
        public async Task AdminResetPassword_UserNotFound_ReturnsError()
        {
            using var context = TestHelper.GetInMemoryDbContext();
            var controller = new ThanhVienController(context);
            var sessionMock = TestHelper.CreateMockSession();
            sessionMock.SetSessionString("VaiTro", "Admin");
            controller.ControllerContext = TestHelper.CreateControllerContext(sessionMock.Object);
            controller.SetupTempData();

            var result = await controller.AdminResetPassword("NON_EXISTENT", "newpass123");
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("AdminUsers", redirectResult.ActionName);
            Assert.Equal("Tài khoản không tồn tại.", controller.TempData["Error"]);
        }

        [Fact]
        public async Task UpdateMemberRole_MemberNotFound_RedirectsToDetailsWithError()
        {
            using var context = TestHelper.GetInMemoryDbContext();
            var controller = new ThanhVienController(context);
            controller.SetupTempData();

            var result = await controller.UpdateMemberRole("NOT_FOUND", "PROJ1", "Tester");
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Details", redirectResult.ActionName);
            Assert.Equal("DuAn", redirectResult.ControllerName);
            Assert.Equal("PROJ1", redirectResult.RouteValues?["id"]);
            Assert.Equal("Thành viên không tồn tại.", controller.TempData["Error"]);
        }

        [Fact]
        public async Task UpdateMemberRole_ProjectNotFound_RedirectsToDuAnIndex()
        {
            using var context = TestHelper.GetInMemoryDbContext();
            await SeedDataAsync(context);
            var controller = new ThanhVienController(context);

            var result = await controller.UpdateMemberRole("MB2", "NOT_FOUND_PROJECT", "Tester");
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            Assert.Equal("DuAn", redirectResult.ControllerName);
        }

        [Fact]
        public async Task UpdateMemberRole_NonOwner_RedirectsToDetailsWithError()
        {
            using var context = TestHelper.GetInMemoryDbContext();
            await SeedDataAsync(context);
            var controller = new ThanhVienController(context);
            var sessionMock = TestHelper.CreateMockSession();
            sessionMock.SetSessionString("MaTaiKhoan", "USER2"); // Not Owner of workspace
            sessionMock.SetSessionString("VaiTro", "User");
            controller.ControllerContext = TestHelper.CreateControllerContext(sessionMock.Object);
            controller.SetupTempData();

            var result = await controller.UpdateMemberRole("MB2", "PROJ1", "Tester");
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Details", redirectResult.ActionName);
            Assert.Equal("DuAn", redirectResult.ControllerName);
            Assert.Equal("Bạn không có quyền chỉnh sửa vai trò thành viên.", controller.TempData["Error"]);
        }

        [Fact]
        public async Task RemoveMember_MemberNotFound_RedirectsToDetailsWithError()
        {
            using var context = TestHelper.GetInMemoryDbContext();
            var controller = new ThanhVienController(context);
            controller.SetupTempData();

            var result = await controller.RemoveMember("NOT_FOUND", "PROJ1");
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Details", redirectResult.ActionName);
            Assert.Equal("DuAn", redirectResult.ControllerName);
            Assert.Equal("Thành viên không tồn tại.", controller.TempData["Error"]);
        }

        [Fact]
        public async Task RemoveMember_ProjectNotFound_RedirectsToDuAnIndex()
        {
            using var context = TestHelper.GetInMemoryDbContext();
            await SeedDataAsync(context);
            var controller = new ThanhVienController(context);

            var result = await controller.RemoveMember("MB2", "NOT_FOUND_PROJECT");
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            Assert.Equal("DuAn", redirectResult.ControllerName);
        }

        [Fact]
        public async Task RemoveMember_NonOwner_RedirectsToDetailsWithError()
        {
            using var context = TestHelper.GetInMemoryDbContext();
            await SeedDataAsync(context);
            var controller = new ThanhVienController(context);
            var sessionMock = TestHelper.CreateMockSession();
            sessionMock.SetSessionString("MaTaiKhoan", "USER2"); // Not Owner
            sessionMock.SetSessionString("VaiTro", "User");
            controller.ControllerContext = TestHelper.CreateControllerContext(sessionMock.Object);
            controller.SetupTempData();

            var result = await controller.RemoveMember("MB2", "PROJ1");
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Details", redirectResult.ActionName);
            Assert.Equal("DuAn", redirectResult.ControllerName);
            Assert.Equal("Bạn không có quyền xóa thành viên.", controller.TempData["Error"]);
        }

        [Fact]
        public async Task AdminResetPassword_NonAdmin_RedirectsToWorkspace()
        {
            // Arrange
            using var context = TestHelper.GetInMemoryDbContext();
            var controller = new ThanhVienController(context);
            var sessionMock = TestHelper.CreateMockSession();
            sessionMock.SetSessionString("VaiTro", "User"); // Not Admin
            controller.ControllerContext = TestHelper.CreateControllerContext(sessionMock.Object);

            // Act
            var result = await controller.AdminResetPassword("USER2", "newpass");

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            Assert.Equal("Workspace", redirectResult.ControllerName);
        }
    }
}
