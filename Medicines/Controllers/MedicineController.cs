using AutoMapper;
using Medicines.Data;
using Medicines.Data.dto;
using Medicines.Data.Models;
using Medicines.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Medicines.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MedicineController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public MedicineController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }


        [HttpGet]
        public async Task<IActionResult> GetMedicine()
        {
            var medicines = await _context.Medicines
                .Select(m => new
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
                        m.Pharmacy.Id,
                        m.Pharmacy.Name,
                        m.Pharmacy.Address
                    }
                })
                .ToListAsync();

            return Ok(medicines);
        }




        [HttpPost]
        public async Task<IActionResult> AddMedicine([FromForm] medicineDto medicineDto)
        {
            if (medicineDto == null)
            {
                return BadRequest(new { message = "بيانات الدواء غير صحيحة." });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    message = "الرجاء التحقق من البيانات المدخلة.",
                    errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
                });
            }

            var pharmacy = await _context.Pharmacies.FirstOrDefaultAsync(p => p.Id == medicineDto.PharmacyId);
            if (pharmacy == null)
            {
                return NotFound(new { message = "لم يتم العثور على الصيدلية المرتبطة بهذا الدواء." });
            }

            if (medicineDto.ImageMedicine == null || medicineDto.ImageMedicine.Length == 0)
            {
                return BadRequest(new { message = "يجب تحميل صورة الدواء." });
            }

            // 1️⃣ إنشاء كائن من `FileUploadService` وتمرير المسار
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
            var fileUploadService = new FileUploadService(uploadsFolder);

            string fileName;
            try
            {
                // 2️⃣ استدعاء `UploadImageAsync` لحفظ الصورة
                fileName = await fileUploadService.UploadImageAsync(medicineDto.ImageMedicine);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }

            try
            {
                // 3️⃣ تحويل DTO إلى كيان وحفظ اسم الصورة فقط
                var medicine = _mapper.Map<Medicine>(medicineDto);
                medicine.ImageMedicine = $"/uploads/{fileName}";

                await _context.Medicines.AddAsync(medicine);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = "تمت إضافة الدواء بنجاح.",
                    medicine = new
                    {
                        medicine.Id,
                        medicine.TradeName,
                        medicine.ScientificName,
                        medicine.Dosage,
                        medicine.DrugTiming,
                        medicine.SideEffects,
                        medicine.ContraindicatedDrugs,
                        medicine.ManufacturerName,
                        medicine.ProducingCompany,
                        medicine.Price,
                        medicine.PharmacyId,
                        ImageMedicine = medicine.ImageMedicine
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "حدث خطأ أثناء حفظ البيانات.", error = ex.Message });
            }
        }



        [HttpPut("{id}")]
       
        public async Task<IActionResult> UpdateMedicine(int id, [FromBody] medicineDto medicineDto)
        {   
            if (medicineDto == null)
            {
                return BadRequest(new { message = "بيانات الدواء غير صحيحة." });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    message = "الرجاء التحقق من البيانات المدخلة.",
                    errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
                });
            }

            // البحث عن الدواء المراد تعديله
            var existingMedicine = await _context.Medicines.FirstOrDefaultAsync(m => m.Id == id);
            if (existingMedicine == null)
            {
                return NotFound(new { message = "لم يتم العثور على الدواء." });
            }

            // التحقق مما إذا كانت الصيدلية الجديدة موجودة (إذا تم تغيير `PharmacyId`)
            if (existingMedicine.PharmacyId != medicineDto.PharmacyId)
            {
                var pharmacy = await _context.Pharmacies.FirstOrDefaultAsync(p => p.Id == medicineDto.PharmacyId);
                if (pharmacy == null)
                {
                    return NotFound(new { message = "لم يتم العثور على الصيدلية المرتبطة بهذا الدواء." });
                }
            }

            try
            {
                // تحديث بيانات الدواء باستخدام AutoMapper
                _mapper.Map(medicineDto, existingMedicine);

                // حفظ التعديلات في قاعدة البيانات
                await _context.SaveChangesAsync();

                // تحويل الكيان المعدل إلى DTO للإرجاع
                var resultDto = _mapper.Map<medicineDto>(existingMedicine);

                return Ok(new
                {
                    message = "تم تحديث بيانات الدواء بنجاح.",
                    medicine = resultDto
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "حدث خطأ أثناء تحديث البيانات.", error = ex.Message });
            }
        }


        [HttpDelete]
       
        public async Task<IActionResult> DeleteMedicine(int id)
        {
            var Medicines = await _context.Medicines.FindAsync(id);

            if (Medicines == null)
            {
                return NotFound(new { message = "لم يتم العثور على الصيدلية." });
            }

            _context.Medicines.Remove(Medicines);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "تم حذف بيانات الصيدلية بنجاح.",
                pharmacy = new
                {
                    Medicines.Id,
                    Medicines.ImageMedicine,
                    Medicines.DrugTiming,
                    Medicines.ManufacturerName,
                    Medicines.SideEffects,
                    Medicines.ScientificName,
                    Medicines.TradeName,
                    Medicines.Price
                }
            });
        }
    }
}
