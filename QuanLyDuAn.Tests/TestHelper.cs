using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Moq;
using QuanLyDuAn.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuanLyDuAn.Tests
{
    public static class TestHelper
    {
        public static BtlCnpmContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<BtlCnpmContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new BtlCnpmContext(options);
            context.Database.EnsureCreated();
            return context;
        }

        public static Mock<ISession> CreateMockSession()
        {
            var sessionMock = new Mock<ISession>();
            var sessionData = new Dictionary<string, byte[]>();

            sessionMock.Setup(s => s.Set(It.IsAny<string>(), It.IsAny<byte[]>()))
                .Callback<string, byte[]>((key, val) => sessionData[key] = val);

            sessionMock.Setup(s => s.TryGetValue(It.IsAny<string>(), out It.Ref<byte[]>.IsAny))
                .Returns((string key, out byte[]? val) =>
                {
                    return sessionData.TryGetValue(key, out val);
                });

            sessionMock.Setup(s => s.Remove(It.IsAny<string>()))
                .Callback<string>(key => sessionData.Remove(key));

            sessionMock.Setup(s => s.Clear())
                .Callback(() => sessionData.Clear());

            return sessionMock;
        }

        public static void SetSessionString(this Mock<ISession> sessionMock, string key, string value)
        {
            sessionMock.Object.Set(key, Encoding.UTF8.GetBytes(value));
        }

        public static string? GetSessionString(this Mock<ISession> sessionMock, string key)
        {
            if (sessionMock.Object.TryGetValue(key, out var val))
            {
                return Encoding.UTF8.GetString(val);
            }
            return null;
        }

        public static ControllerContext CreateControllerContext(ISession session)
        {
            var httpContext = new DefaultHttpContext();
            httpContext.Session = session;
            return new ControllerContext
            {
                HttpContext = httpContext
            };
        }

        public static void SetupTempData(this Controller controller)
        {
            controller.TempData = new TempDataDictionary(
                new DefaultHttpContext(),
                Mock.Of<ITempDataProvider>()
            );
        }
    }
}
