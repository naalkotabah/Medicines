namespace Medicines.Services
{
    using AutoMapper;
    using Medicines.Data.dto;
    using Medicines.Data.Models;
    using Medicines.Repositories.Interfaces;
    using Medicines.Services.Interfaces;

    public class PractitionerService : IPractitionerService
    {
        private readonly IPractitionerRepository _repo;
        private readonly IMapper _mapper;
        private readonly FileUploadService _upload;

        public PractitionerService(IPractitionerRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
            _upload = new FileUploadService(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads"));
        }

        public async Task<object?> LoginAsync(Login_Practitioner dto)
        {
            if (string.IsNullOrWhiteSpace(dto.NamePractitioner) || string.IsNullOrWhiteSpace(dto.Password))
                return null;

            var user = await _repo.GetByLoginAsync(dto.NamePractitioner, dto.Password);
            return user != null ? dto : null;
        }



        public async Task<List<object>> GetAllAsync()
        {
            var list = await _repo.GetAllWithPharmacyAsync();
            return list.Select(p => new
            {
                p.Id,
                p.NamePractitioner,
                p.Address,
                p.PhonNumber,
                p.Studies,
                p.ImagePractitioner,
                Pharmacy = p.Pharmacy != null ? new
                {
                    p.Pharmacy.Id,
                    p.Pharmacy.Name,
                    p.Pharmacy.Address
                } : null
            }).ToList<object>();
        }

        public async Task<List<object>> GetForSelectAsync()
        {
            var list = await _repo.GetForSelectAsync();
            return list.Select(p => new { p.Id, p.NamePractitioner }).ToList<object>();
        }

        public async Task<object?> GetMyPharmacyAsync(int id)
        {
            var practitioner = await _repo.GetPharmacyByPractitionerIdAsync(id);
            if (practitioner == null || practitioner.Pharmacy == null)
                return null;

            var p = practitioner.Pharmacy;
            return new
            {
                p.Id,
                p.Name,
                p.Address,
                p.Latitude,
                p.Longitude,
                p.City,
                p.LicenseNumber
            };
        }

        public async Task<(bool, string, object?)> AddAsync(PractitionerCreateDto dto)
        {
            if (dto.ImagePractitioner == null || dto.ImagePractitioner.Length == 0)
                return (false, "الصورة مطلوبة", null);

            var uploadResult = await _upload.UploadImageAsync(dto.ImagePractitioner);

            if (!uploadResult.Success)
                return (false, uploadResult.Message, null);

            var practitioner = new Practitioner
            {
                NamePractitioner = dto.NamePractitioner,
                Password = dto.Password,
                Address = dto.Address,
                PhonNumber = dto.PhonNumber,
                Studies = dto.Studies,
                ImagePractitioner = $"/uploads/{uploadResult.FileName}"
            };

            await _repo.AddAsync(practitioner);
            return (true, "تمت الإضافة بنجاح", practitioner);
        }


        public async Task<(bool, string, object?)> UpdateAsync(int id, PractitionerCreateDto dto)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null)
                return (false, "الممارس غير موجود", null);

            string fileName = existing.ImagePractitioner ?? string.Empty;

            if (dto.ImagePractitioner != null && dto.ImagePractitioner.Length > 0)
            {
                // ارفع الصورة الجديدة
                var uploadResult = await _upload.UploadImageAsync(dto.ImagePractitioner);

                if (!uploadResult.Success)
                    return (false, uploadResult.Message, null);

                // حذف الصورة القديمة إن وُجدت
                string imageNameOnly = Path.GetFileName(fileName);
                string oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", imageNameOnly);

                if (File.Exists(oldPath))
                    File.Delete(oldPath);

                fileName = $"/uploads/{uploadResult.FileName}";
            }

            existing.NamePractitioner = dto.NamePractitioner;
            existing.Address = dto.Address;
            existing.PhonNumber = dto.PhonNumber;
            existing.Studies = dto.Studies;
            existing.ImagePractitioner = fileName;

            await _repo.UpdateAsync(existing);
            return (true, "تم التحديث", existing);
        }


        public async Task<(bool, string, object?)> DeleteAsync(int id)
        {
            var practitioner = await _repo.GetByIdAsync(id);
            if (practitioner == null)
                return (false, "الممارس غير موجود", null);

            if (await _repo.IsPractitionerLinkedToPharmacy(id))
                return (false, "مرتبط بصيدلية، احذفها أولاً", null);

            await _repo.DeleteAsync(practitioner);
            return (true, "تم الحذف", practitioner);
        }
    }
}
