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

        public async Task<string> UploadImageAsync(IFormFile imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
                throw new ArgumentException("يجب رفع صورة.");

            var allowedExtension = ".png";
            var fileExtension = Path.GetExtension(imageFile.FileName).ToLower();

            if (fileExtension != allowedExtension)
                throw new ArgumentException("يُسمح فقط بملفات PNG.");

         
            var fileName = $"{Guid.NewGuid()}{fileExtension}";
            var filePath = Path.Combine(_uploadsFolder, fileName);

            // حفظ الصورة في المجلد
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            return fileName; // إرجاع اسم الملف المحفوظ
        }
    }
}
