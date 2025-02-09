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
    public class PractitionerController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public PractitionerController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }


        [HttpGet]
        public async Task<IActionResult> GetPractitioners()
        {
            var practitioners = await _context.Practitioners
                .Include(p => p.Pharmacy)
                .Select(p => new
                {
                    p.Id,
                    p.NamePractitioner,
                    p.Address,
                    p.PhonNumber,
                    Pharmacy = p.Pharmacy != null ? new
                    {
                        p.Pharmacy.Id,
                        p.Pharmacy.Name,
                        p.Pharmacy.Address
                    } : null
                })
                .ToListAsync();

            return Ok(practitioners);
        }

        [HttpGet("{Practitionersid}")]
        public async Task<IActionResult> GetMyPharmcic(int Practitionersid)
        {
            var practitioner = await _context.Practitioners
                .Where(p => p.Id == Practitionersid) // تصفية النتيجة بناءً على Practitionersid
                .Include(p => p.Pharmacy) // تضمين بيانات الصيدلية المرتبطة
                .Select(p => new
                {
                    Pharmacy = p.Pharmacy != null ? new
                    {
                        p.Pharmacy.Id,
                        p.Pharmacy.Name,
                        p.Pharmacy.Address,
                        p.Pharmacy.Latitude,
                        p.Pharmacy.Longitude,
                        p.Pharmacy.City,
                        p.Pharmacy.LicenseNumber
                    } : null
                })
                .FirstOrDefaultAsync(); // جلب أول نتيجة فقط بدلاً من قائمة

            if (practitioner == null)
            {
                return NotFound(new { message = "لم يتم العثور على الممارس أو لا يملك صيدلية." });
            }

            return Ok(practitioner);
        }



        [HttpPost]
        public async Task<IActionResult> AddPractitioner([FromForm] PractitionerCreateDto practitionerDto)
        {
            if (practitionerDto == null)
                return BadRequest("Invalid data");

            if (practitionerDto.ImagePractitioner == null || practitionerDto.ImagePractitioner.Length == 0)
                return BadRequest("Image is required.");

            // 1️⃣ إنشاء كائن من `FileUploadService` وتمرير المسار
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
            var fileUploadService = new FileUploadService(uploadsFolder);

            string fileName;
            try
            {
                // 2️⃣ استدعاء `UploadImageAsync` لحفظ الصورة
                fileName = await fileUploadService.UploadImageAsync(practitionerDto.ImagePractitioner);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }

            // 3️⃣ إنشاء الكيان وتخزين اسم الملف فقط
            var practitioner = new Practitioner
            {
                NamePractitioner = practitionerDto.NamePractitioner,
                Address = practitionerDto.Address,
                PhonNumber = practitionerDto.PhonNumber,
                Studies = practitionerDto.Studies,
                ImagePractitioner = $"/uploads/{fileName}" // حفظ المسار النسبي
            };

            try
            {
                await _context.Practitioners.AddAsync(practitioner);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetPractitioners), new { id = practitioner.Id }, practitioner);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "حدث خطأ أثناء حفظ البيانات.",
                    error = ex.InnerException?.Message ?? ex.Message
                });
            }
        }





        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdatePractitioner(int id, [FromBody] PractitionerCreateDto practitionerDto)
        {
            var existingPractitioner = await _context.Practitioners.FindAsync(id);
            if (existingPractitioner == null)
                return NotFound();

            // تحديث القيم باستخدام AutoMapper
            _mapper.Map(practitionerDto, existingPractitioner);

            await _context.SaveChangesAsync();

            return Ok(existingPractitioner);
        }


   
        [HttpDelete("{id}")]

        public async Task<IActionResult> DeletePractitioner(int id)
        {
            var practitioner = await _context.Practitioners
                .Include(p => p.Pharmacy) // جلب الصيدلية المرتبطة بالطبيب
                .FirstOrDefaultAsync(p => p.Id == id);

            if (practitioner == null)
                return NotFound();

            // التحقق مما إذا كان الطبيب مرتبطًا بصيدلية
            if (practitioner.Pharmacy != null && practitioner.Pharmacy.PractitionerId == id)
            {
                return BadRequest(new { message = "لا يمكن حذف الطبيب لأنه مرتبط بصيدلية، يرجى حذف الصيدلية أولاً." });
            }

            _context.Practitioners.Remove(practitioner);
            await _context.SaveChangesAsync();

            return Ok(practitioner);
        }


    }
}