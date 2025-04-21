namespace Medicines.Services
{
    using AutoMapper;
    using Medicines.Data;
    using Medicines.Data.dto;
    using Medicines.Data.Models;
    using Medicines.Repositories.Interfaces;
    using Medicines.Services.Interfaces;
    using Microsoft.EntityFrameworkCore;
    using NuGet.Protocol.Core.Types;

    public class PractitionerService : IPractitionerService
    {
        private readonly IPractitionerRepository _repo;
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly FileUploadService _upload;

        public PractitionerService(IPractitionerRepository repo, IMapper mapper , AppDbContext context)
        {
            _repo = repo;
            _mapper = mapper;
            _context = context;
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

            var medicinesList = new List<object>();

            if (practitioner.Pharmacy.Medicines != null)
            {
                medicinesList = practitioner.Pharmacy.Medicines.Select(m => new
                {
                    m.Id,
                    m.TradeName,
                    m.ManufacturerName,
                    m.SideEffects,
                    m.DrugTiming,
                    m.ContraindicatedDrugs,
                    m.ScientificName,
                    m.Dosage,
                    m.ImageMedicine,
                    m.Price,
                    m.ProducingCompany
                }).Cast<object>().ToList(); // Cast to object to match List<object>
            }

            return new
            {
                Practitioner = new
                {
                    practitioner.Id,
                    practitioner.NamePractitioner,
                    practitioner.Address,
                    practitioner.PhonNumber,
                    practitioner.Studies,
                    practitioner.ImagePractitioner
                },
                Pharmacy = new
                {
                    practitioner.Pharmacy.Id,
                    practitioner.Pharmacy.Name,
                    practitioner.Pharmacy.Address,
                    practitioner.Pharmacy.Latitude,
                    practitioner.Pharmacy.Longitude,
                    practitioner.Pharmacy.City,
                    practitioner.Pharmacy.LicenseNumber,
                    practitioner.Pharmacy.ImagePharmacics,
                    practitioner.Pharmacy.OpenTime,
                    practitioner.Pharmacy.CloseTime,
                    Medicines = medicinesList
                }
            };
        }




        public async Task<(bool, string, object?)> AddAsync(PractitionerCreateDto dto)
        {
            if (dto.ImagePractitioner == null || dto.ImagePractitioner.Length == 0)
                return (false, "الصورة مطلوبة", null);

            var uploadResult = await _upload.UploadImageAsync(dto.ImagePractitioner);
            if (!uploadResult.Success)
                return (false, uploadResult.Message, null);

            // 1. أنشئ المستخدم أولاً
            var user = new Users
            {
                Name = dto.UserName,
                UserName = dto.UserName,
                Password = dto.Password,
                RoleId = 3,
                IsDleted = false
            };

            await _context.Users!.AddAsync(user);
            await _context.SaveChangesAsync(); // ✅ مهم: لحفظ الـ Id

            // 2. أنشئ الصيدلاني بعد توليد User.Id
            var practitioner = new Practitioner
            {
                NamePractitioner = dto.UserName,
                Password = dto.Password,
                Address = dto.Address,
                PhonNumber = dto.PhonNumber,
                Studies = dto.Studies,
                ImagePractitioner = $"/uploads/{uploadResult.FileName}",
                UserId = user.Id // ✅ الربط هنا
            };
            var response = new PractitionerDto
            {
               
                NamePractitioner = practitioner.NamePractitioner,
                Address = practitioner.Address,
                PhonNumber = practitioner.PhonNumber,
                Studies = practitioner.Studies,
                ImagePractitioner = practitioner.ImagePractitioner
            };

          

            await _context.Practitioners!.AddAsync(practitioner);
            await _context.SaveChangesAsync();

            return (true, "تمت الإضافة بنجاح", response);
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

            existing.NamePractitioner = dto.UserName;
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

        public async Task<List<SearchMedicinesDto>> SearchMedicinesForPractitioner(int practitionerId, string search)
        {
            var medicines = await _repo.SearchMedicinesForPractitioner(practitionerId, search);

              

            return medicines.Select(m => new SearchMedicinesDto
            {
                id = m.Id,
                TradeName = m.TradeName ?? "لا توجد نتائج",
                ScientificName = m.ScientificName ?? "لا توجد نتائج",
                Price = m.Price
            }).ToList();
        }
    }
}
