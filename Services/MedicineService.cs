namespace Medicines.Services
{
    using AutoMapper;
    using Medicines.Data.dto;
    using Medicines.Data.Models;
    using Medicines.Repositories.Interfaces;
    using Medicines.Services.Interfaces;

    public class MedicineService : IMedicineService
    {
        private readonly IMedicineRepository _repo;
        private readonly IMapper _mapper;
        private readonly FileUploadService _upload;

        public MedicineService(IMedicineRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            _upload = new FileUploadService(uploadsFolder);
        }

        public async Task<object> GetAllAsync(int pageNumber = 1, int pageSize = 10)
        {
           
            var skip = (pageNumber - 1) * pageSize;

       
            var meds = await _repo.GetAllAsync();

            var pagedMeds = meds.Skip(skip).Take(pageSize);


            var totalItems = meds.Count();
            var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

        
            return new
            {
                TotalItems = totalItems,
                TotalPages = totalPages,
                PageNumber = pageNumber,
                PageSize = pageSize,
                Data = pagedMeds.Select(m => new
                {
                    m.Id,
                    m.TradeName,
                    m.ScientificName,
                    m.ImageMedicine,
                    m.Dosage,
                    m.DrugTiming,
                    m.SideEffects,
                    m.ContraindicatedDrugs,
                    m.ManufacturerName,
                    m.ProducingCompany,
                    m.Price,
                    Pharmacy = new
                    {
                        m.Pharmacy?.Id,
                        m.Pharmacy?.Name,
                        m.Pharmacy?.Address,
                        m.Pharmacy?.ImagePharmacics,
                        m.Pharmacy?.CloseTime,
                        m.Pharmacy?.OpenTime,
                        m.Pharmacy?.Latitude,
                        m.Pharmacy?.Longitude,
                    }
                })
            };
        }


        public async Task<(bool, string, object?)> AddAsync(medicineDto dto)
        {
           
            var pharmacy = await _repo.GetPharmacyByIdAsync(dto.PharmacyId);
            if (pharmacy == null)
                return (false, "لم يتم العثور على الصيدلية", null);

            
            if (dto.ImageMedicine == null || dto.ImageMedicine.Length == 0)
                return (false, "يجب تحميل صورة للدواء", null);

            
            if (dto.Price <= 0)
                return (false, "السعر غير صالح", null);

           
            if (dto.Dosage <= 0)
                return (false, "الجرعة غير صالحة", null);

         
            var uploadResult = await _upload.UploadImageAsync(dto.ImageMedicine);
            if (!uploadResult.Success)
                return (false, uploadResult.Message, null);

           
            DateTime? expiryDate = null;
            var expiryRaw = dto.ExpiryDate ?? "";
            if (!string.IsNullOrWhiteSpace(expiryRaw))
            {
                if (!DateTime.TryParse(expiryRaw, out var parsedDate))
                    return (false, "صيغة التاريخ غير صحيحة، استخدم yyyy-MM-dd", null);

                // ✅ اجعل التاريخ بصيغة UTC
                expiryDate = DateTime.SpecifyKind(parsedDate, DateTimeKind.Utc);
            }


            // ✅ تكوين كائن الدواء
            var medicine = _mapper.Map<Medicine>(dto);
            medicine.ImageMedicine = $"/uploads/{uploadResult.FileName}";
            medicine.ExpiryDate = expiryDate;

            // ✅ محاولة الحفظ مع تتبع الأخطاء
            try
            {
                await _repo.AddAsync(medicine);
            }
            catch (Exception ex)
            {
                Console.WriteLine("🛑 فشل في الحفظ:");
                Console.WriteLine($"رسالة الخطأ: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
                Console.WriteLine($"TradeName: {dto.TradeName}, Price: {dto.Price}, Dosage: {dto.Dosage}, PharmacyId: {dto.PharmacyId}");

                return (false, "فشل في حفظ الدواء. تحقق من البيانات المدخلة.", null);
            }


            return (true, "تمت الإضافة بنجاح", medicine);
        }




        public async Task<(bool, string, object?)> UpdateAsync(int id, medicineDto dto)
        {
            var medicine = await _repo.GetByIdAsync(id);
            if (medicine == null)
                return (false, "لم يتم العثور على الدواء", null);

            if (medicine.PharmacyId != dto.PharmacyId)
            {
                var pharmacy = await _repo.GetPharmacyByIdAsync(dto.PharmacyId);
                if (pharmacy == null)
                    return (false, "الصيدلية غير موجودة", null);
            }

            if (dto.ImageMedicine != null && dto.ImageMedicine.Length > 0)
            {
                var uploadResult = await _upload.UploadImageAsync(dto.ImageMedicine);
                if (!uploadResult.Success)
                    return (false, uploadResult.Message, null);

                // حذف الصورة القديمة إن وجدت
                if (!string.IsNullOrEmpty(medicine.ImageMedicine))
                {
                    string imageNameOnly = Path.GetFileName(medicine.ImageMedicine);
                    string oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", imageNameOnly);

                    if (File.Exists(oldPath))
                        File.Delete(oldPath);
                }

                medicine.ImageMedicine = $"/uploads/{uploadResult.FileName}";
            }

            _mapper.Map(dto, medicine);
            await _repo.UpdateAsync(medicine);

            return (true, "تم التحديث بنجاح", medicine);
        }


        public async Task<(bool, string, object?)> DeleteAsync(int id)
        {
            var medicine = await _repo.GetByIdAsync(id);
            if (medicine == null)
                return (false, "الدواء غير موجود", null);

            await _repo.DeleteAsync(medicine);
            return (true, "تم الحذف", medicine);
        }
    }
}
