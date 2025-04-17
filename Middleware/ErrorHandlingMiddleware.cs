using System.Net;
using System.Text.Json;
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
        private readonly string _logFilePath;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger, IWebHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;

            // ✅ مسار ملف السجل
            var logDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Logs");
            Directory.CreateDirectory(logDirectory); // إنشاء المجلد إذا لم يكن موجودًا
            _logFilePath = Path.Combine(logDirectory, "log.txt");
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                // ✅ تسجيل في Console + ملف
                _logger.LogError(ex, "❌ خطأ غير متوقع");

                await File.AppendAllTextAsync(_logFilePath, $@"
==================== [{DateTime.UtcNow}] ====================
رسالة الخطأ: {ex.Message}
النوع: {ex.GetType().Name}
المسار: {context.Request.Path}
StackTrace: {ex.StackTrace}
============================================================
");

                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                var response = new
                {
                    message = "حدث خطأ أثناء تنفيذ الطلب، يرجى المحاولة لاحقًا."
                };

                var json = JsonSerializer.Serialize(response);
                await context.Response.WriteAsync(json);
            }
        }
    }
}
