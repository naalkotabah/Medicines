using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Medicines.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext); // تمرير الطلب إلى الميدل وير التالي
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ خطأ: {ex.Message}");

                httpContext.Response.ContentType = "application/json";
                httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                var errorResponse = new
                {
                    message = "حدث خطأ غير متوقع في السيرفر.",
                    details = ex.Message // ⚠️ يمكنك تعطيل هذا في بيئة الإنتاج
                };

                var jsonResponse = JsonSerializer.Serialize(errorResponse);
                await httpContext.Response.WriteAsync(jsonResponse);
            }
        }
    }
}
