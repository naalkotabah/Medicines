using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;

namespace Medicines.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;
        private readonly IWebHostEnvironment _env;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger, IWebHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext); // تمرير الطلب إلى الميدل وير التالي
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ حدث استثناء غير متوقع");

                httpContext.Response.ContentType = "application/json";
                httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                // ✅ عرض التفاصيل بشكل دائم (مؤقتًا لحين انتهاء التصحيح)
                var errorResponse = new
                {
                    message = "حدث خطأ غير متوقع في السيرفر.",
                    exception = ex.Message,
                    stackTrace = ex.StackTrace,
                    type = ex.GetType().Name
                };

                var jsonResponse = JsonSerializer.Serialize(errorResponse);
                await httpContext.Response.WriteAsync(jsonResponse);
            }
        }
    }
}
