using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Medicines.Services
{
    public class FileUploadService
    {
        private readonly string _uploadsFolder;

        public FileUploadService(string uploadsFolder)
        {
            _uploadsFolder = uploadsFolder;

            if (!Directory.Exists(_uploadsFolder))
            {
                Directory.CreateDirectory(_uploadsFolder);
            }
        }

        public async Task<(bool Success, string Message, string? FileName)> UploadImageAsync(IFormFile imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
                return (false, "يجب رفع ملف.", null);

            var allowedExtensions = new[] { ".png", ".jpg", ".jpeg", ".pdf" };
            var fileExtension = Path.GetExtension(imageFile.FileName).ToLower();

            if (!allowedExtensions.Contains(fileExtension))
                return (false, "امتداد الملف غير مسموح. يُسمح فقط بـ PNG, JPG, JPEG, PDF.", null);

            var fileName = $"{Guid.NewGuid()}{fileExtension}";
            var filePath = Path.Combine(_uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            return (true, "تم رفع الملف بنجاح", fileName);
        }


    }
}
