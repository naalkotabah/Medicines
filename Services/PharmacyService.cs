﻿namespace Medicines.Services
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
                p.OpenTime,
                p.CloseTime,
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
                pharmacy.OpenTime,
                pharmacy.CloseTime,
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

            var uploadResult = await _upload.UploadImageAsync(dto.ImagePharmacics);
            if (!uploadResult.Success)
                return (false, uploadResult.Message, null);

            var pharmacy = _mapper.Map<Pharmacics>(dto);
            pharmacy.ImagePharmacics = $"/uploads/{uploadResult.FileName}";

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
                pharmacy.OpenTime,
                pharmacy.CloseTime,
                pharmacy.PractitionerId
            });
        }


        public async Task<(bool, string, object?)> UpdateAsync(int id, PharmacicsDto dto)
        {
            var pharmacy = await _repo.GetByIdAsync(id);
            if (pharmacy == null)
                return (false, "الصيدلية غير موجودة", null);

            string fileName = pharmacy.ImagePharmacics ?? string.Empty;

            if (dto.ImagePharmacics != null && dto.ImagePharmacics.Length > 0)
            {
                var uploadResult = await _upload.UploadImageAsync(dto.ImagePharmacics);
                if (!uploadResult.Success)
                    return (false, uploadResult.Message, null);

                // حذف الصورة القديمة
                string oldPath = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    (pharmacy.ImagePharmacics ?? string.Empty).TrimStart('/')
                );

                if (File.Exists(oldPath))
                    File.Delete(oldPath);

                fileName = $"/uploads/{uploadResult.FileName}";
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


        public async Task<(bool, string)> ApproveOrCancelOrderAsync(int orderId, bool approve)
        {
         
            var order = await _repo.GetOrderByIdAsync(orderId);

            if (order == null)
                return (false, "الطلب غير موجود.");

        
            order.Status = approve ? OrderStatus.Accepted : OrderStatus.Rejected;

          
            await _repo.UpdateOrderAsync(order);

           
            return (true, approve ? "تمت الموافقة على الطلب." : "تم رفض  الطلب.");
        }


        public async Task<(bool, string)> CancelOrder(int orderId)
        {

            var order = await _repo.GetOrderByIdAsync(orderId);

            if (order == null)
                return (false, "الطلب غير موجود.");


            order.Status =  OrderStatus.Canceled;


            await _repo.UpdateOrderAsync(order);


            return (true, "تم إلغاء الطلب.");
        }


        public async Task<(bool, string)> MarkOrderAsDoneAsync(int orderId)
        {
            var order = await _repo.GetOrderByIdAsync(orderId);

            if (order == null)
                return (false, "الطلب غير موجود.");

          
            if (order.Status != OrderStatus.Accepted && order.Status != OrderStatus.Pending)
                return (false, "لا يمكن تغيير حالة الطلب إلى 'تم' من الحالة الحالية.");

           
            order.Status = OrderStatus.Done;

           
            await _repo.UpdateOrderAsync(order);

            return (true, "تم إتمام الطلب بنجاح.");
        }


    }

}
