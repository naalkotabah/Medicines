using AutoMapper;
using Medicines.Data;
using Medicines.Data.dto;
using Medicines.Data.Models;
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

            string imagePath = null;

            if (medicineDto.ImageMedicine != null)
            {
                // السماح فقط بامتداد PNG
                var allowedExtensions = new[] { ".png" };
                var fileExtension = Path.GetExtension(medicineDto.ImageMedicine.FileName).ToLower();

                if (!allowedExtensions.Contains(fileExtension))
                {
                    return BadRequest(new { message = "يُسمح فقط بتحميل صور بصيغة PNG." });
                }

                // إنشاء مجلد التخزين إذا لم يكن موجودًا
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "uploads");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                // إنشاء اسم فريد للملف
                var fileName = $"{Guid.NewGuid()}{fileExtension}";
                var filePath = Path.Combine(uploadsFolder, fileName);

                // حفظ الصورة في المجلد
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await medicineDto.ImageMedicine.CopyToAsync(stream);
                }

                // تعيين المسار النسبي للصورة
                imagePath = $"/uploads/{fileName}";
            }
            else
            {
                return BadRequest(new { message = "يجب تحميل صورة الدواء." });
            }

            try
            {
                // تحويل DTO إلى كيان وحفظ مسار الصورة
                var medicine = _mapper.Map<Medicine>(medicineDto);
                medicine.ImageMedicine = imagePath; // تعيين مسار الصورة في قاعدة البيانات

                await _context.Medicines.AddAsync(medicine);
                await _context.SaveChangesAsync();

                // إرجاع البيانات يدويًا بدلاً من AutoMapper
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
                        ImageMedicine = imagePath // التأكد من إرجاع الصورة
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
