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
                _logger.LogError($"❌ خطأ: {ex.Message}");

                httpContext.Response.ContentType = "application/json";
                httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                var errorResponse = new
                {
                    message = "حدث خطأ غير متوقع في السيرفر.",
                    details = _env.IsDevelopment() ? ex.Message : null
                };


                var jsonResponse = JsonSerializer.Serialize(errorResponse);
                await httpContext.Response.WriteAsync(jsonResponse);
            }
        }
    }
}
