namespace Medicines.Services
{
    using AutoMapper;
    using Medicines.Data.dto;
    using Medicines.Data.Models;
    using Medicines.Repositories.Interfaces;
    using Medicines.Services.Interfaces;

    public class PharmacyService : IPharmacyService
    {
        private readonly IPharmacyRepository _repo;
        private readonly IMapper _mapper;
        private readonly FileUploadService _upload;

        public PharmacyService(IPharmacyRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
            _upload = new FileUploadService(Path.Combine(Directory.GetCurrentDirectory(), "uploads"));
        }

        public async Task<List<object>> GetAllAsync()
        {
            var list = await _repo.GetAllWithDetailsAsync();
            return list.Select(p => new
            {
                p.Id,
                p.Name,
                p.Address,
                p.Latitude,
                p.Longitude,
                p.City,
                p.LicenseNumber,
                p.ImagePharmacics,
                Medicines = p.Medicines?.Select(m => new
                {
                    m.Id,
                    m.ScientificName,
                    m.TradeName,
                    m.ProducingCompany
                }),
                Practitioner = p.Practitioner != null ? new
                {
                    p.Practitioner.Id,
                    p.Practitioner.NamePractitioner
                } : null
            }).ToList<object>();
        }

        public async Task<object?> GetByIdAsync(int id)
        {
            var pharmacy = await _repo.GetByIdWithDetailsAsync(id);
            if (pharmacy == null) return null;

            return new
            {
                pharmacy.Id,
                pharmacy.Name,
                pharmacy.Address,
                pharmacy.Latitude,
                pharmacy.Longitude,
                pharmacy.City,
                pharmacy.LicenseNumber,
                pharmacy.ImagePharmacics,
                Medicines = pharmacy.Medicines?.Select(m => new
                {
                    m.Id,
                    m.ScientificName,
                    m.TradeName,
                    m.ProducingCompany
                }),
                Practitioner = pharmacy.Practitioner != null ? new
                {
                    pharmacy.Practitioner.Id,
                    pharmacy.Practitioner.NamePractitioner
                } : null
            };
        }

        public async Task<(bool, string, object?)> AddAsync(PharmacicsDto dto)
        {
            if (dto.ImagePharmacics == null || dto.ImagePharmacics.Length == 0)
                return (false, "يجب رفع صورة للصيدلية", null);

            var practitioner = await _repo.GetPractitionerByIdAsync(dto.PractitionerId);
            if (practitioner == null)
                return (false, "المتمرس غير موجود", null);

            if (await _repo.IsPractitionerLinkedAsync(dto.PractitionerId))
                return (false, "هذا المتمرس مرتبط بصيدلية بالفعل", null);

            string fileName;
            try
            {
                fileName = await _upload.UploadImageAsync(dto.ImagePharmacics);
            }
            catch (Exception ex)
            {
                return (false, ex.Message, null);
            }

            var pharmacy = _mapper.Map<Pharmacics>(dto);
            pharmacy.ImagePharmacics = $"/uploads/{fileName}";

            await _repo.AddAsync(pharmacy);
           

            return (true, "تمت الإضافة بنجاح", new
            {
                pharmacy.Id,
                pharmacy.Name,
                pharmacy.Address,
                pharmacy.Latitude,
                pharmacy.Longitude,
                pharmacy.City,
                pharmacy.LicenseNumber,
                pharmacy.ImagePharmacics,
                pharmacy.PractitionerId
            });
        }

        public async Task<(bool, string, object?)> UpdateAsync(int id, PharmacicsDto dto)
        {
            var pharmacy = await _repo.GetByIdAsync(id);
            if (pharmacy == null)
                return (false, "الصيدلية غير موجودة", null);

            string fileName = pharmacy.ImagePharmacics ?? string.Empty; ;

            if (dto.ImagePharmacics != null && dto.ImagePharmacics.Length > 0)
            {
                string oldPath = Path.Combine(
      Directory.GetCurrentDirectory(),
      (pharmacy.ImagePharmacics ?? string.Empty).TrimStart('/')
  );

                if (File.Exists(oldPath)) File.Delete(oldPath);

                var newFile = await _upload.UploadImageAsync(dto.ImagePharmacics);
                fileName = $"/uploads/{newFile}";
            }

            _mapper.Map(dto, pharmacy);
            pharmacy.ImagePharmacics = fileName;

            await _repo.UpdateAsync(pharmacy);

            return (true, "تم التحديث بنجاح", pharmacy);
        }

        public async Task<(bool, string, object?)> DeleteAsync(int id)
        {
            var pharmacy = await _repo.GetByIdAsync(id);
            if (pharmacy == null)
                return (false, "لم يتم العثور على الصيدلية", null);

            await _repo.DeleteAsync(pharmacy);
            return (true, "تم الحذف", pharmacy);
        }
    }

}
