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

        public async Task<object> GetAllAsync()
        {
            var meds = await _repo.GetAllAsync();
            return meds.Select(m => new
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
                    m.Pharmacy?.Address
                }
            });
        }

        public async Task<(bool, string, object?)> AddAsync(medicineDto dto)
        {
            var pharmacy = await _repo.GetPharmacyByIdAsync(dto.PharmacyId);
            if (pharmacy == null)
                return (false, "لم يتم العثور على الصيدلية", null);

            if (dto.ImageMedicine == null || dto.ImageMedicine.Length == 0)
                return (false, "يجب تحميل صورة الدواء", null);

            var uploadResult = await _upload.UploadImageAsync(dto.ImageMedicine);
            if (!uploadResult.Success)
                return (false, uploadResult.Message, null);

            var medicine = _mapper.Map<Medicine>(dto);
            medicine.ImageMedicine = $"/uploads/{uploadResult.FileName}";

            await _repo.AddAsync(medicine);

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
