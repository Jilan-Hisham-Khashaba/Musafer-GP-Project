using GP.core.Entities.identity;
using GP.core.Services;
using GP.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace AdminDashBoard.Middelwares
{
    //    public class TokenValidationMiddleware
    //    {
    //        private readonly RequestDelegate _next;

    //        public TokenValidationMiddleware(RequestDelegate next)
    //        {
    //            _next = next;
    //        }

    //        public async Task Invoke(HttpContext context)
    //        {
    //            var token = context.Request.Cookies["token"]; // الحصول على التوكن من الـ cookies
    //            if (!string.IsNullOrEmpty(token))
    //            {
    //                // تأكد من إزالة أي توكن قديم موجود في الرأس
    //                if (context.Request.Headers.ContainsKey("Authorization"))
    //                {
    //                    context.Request.Headers.Remove("Authorization");
    //                }

    //                // إضافة التوكن إلى الرأس
    //                context.Request.Headers.Add("Authorization", "Bearer " + token);
    //            }

    //            await _next(context);
    //        }
    //    }
}
//}




