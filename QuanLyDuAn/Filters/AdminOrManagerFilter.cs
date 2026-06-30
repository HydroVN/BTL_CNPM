using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace QuanLyDuAn.Filters
{
    public class AdminOrManagerFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var session = context.HttpContext.Session;
            var maTaiKhoan = session.GetString("MaTaiKhoan");
            var vaiTro = session.GetString("VaiTro");

            if (string.IsNullOrEmpty(maTaiKhoan))
            {
                context.Result = new RedirectToActionResult("Login", "Account", null);
                return;
            }

            if (vaiTro != "Admin" && vaiTro != "Manager")
            {
                context.Result = new RedirectToActionResult("AccessDenied", "Account", null);
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}
