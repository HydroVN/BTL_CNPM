using System;

namespace QuanLyDuAn.Helpers
{
    public static class VnDateTime
    {
        public static DateTime Now => DateTime.UtcNow.AddHours(7);
    }
}
