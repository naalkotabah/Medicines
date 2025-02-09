using AutoMapper;
using Medicines.Data;
using Medicines.Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;
using Medicines.Data.dto;
using Microsoft.AspNetCore.Authorization;
using Medicines.Services;

namespace Medicines.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PharmacicsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        public PharmacicsController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetPharmacics()
        {
            var pharmacics = await _context.Pharmacies
                .Include(p => p.Medicines)
                .Include(p => p.Practitioner)
                .Select(p => new
                {
                    p.Id,
                    p.Name,
                    p.Address,
                    p.Latitude,
                    p.Longitude,
                    p.City,
                    p.LicenseNumber,

                    Medicines = p.Medicines.Select(m => new
                    {
                        m.Id,
                        m.ScientificName,
                        m.TradeName,
                        m.ProducingCompany
                    }).ToList(),

               
                    Practitioner = p.Practitioner != null ?new
                    {
                        p.Practitioner.Id,
                        p.Practitioner.NamePractitioner
                    } : null
                })
                .ToListAsync();

            return Ok(pharmacics);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPharmacics(int id)
        {
            var pharmacy = await _context.Pharmacies
                .Include(p => p.Medicines)  // جلب الأدوية المرتبطة بالصيدلية
                .Include(p => p.Practitioner) // جلب بيانات المتمرس المرتبط بالصيدلية
                .Where(p => p.Id == id)
                .Select(p => new
                {
                    p.Id,
                    p.Name,
                    p.Address,
                    p.Latitude,
                    p.Longitude,
                    p.City,
                    p.LicenseNumber,

                    Medicines = p.Medicines.Select(m => new
                    {
                        m.Id,
                        m.ScientificName,
                        m.TradeName,
                        m.ProducingCompany
                    }).ToList(),

                    Practitioner = p.Practitioner != null ? new
                    {
                        p.Practitioner.Id,
                        p.Practitioner.NamePractitioner
                    } : null
                })
                .FirstOrDefaultAsync();

            if (pharmacy == null)
                return NotFound(new { message = "الصيدلية غير موجودة." });

            return Ok(new
            {
                message = "تم جلب بيانات الصيدلية بنجاح.",
                pharmacy
            });
        }



        [HttpPost]
        [Route("api/pharmacics/add")]
        public async Task<IActionResult> AddPharmacics([FromForm] PharmacicsDto model)
        {
            if (model.ImagePharmacics == null || model.ImagePharmacics.Length == 0)
                return BadRequest("يجب رفع صورة للصيدلية");

            
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
            var fileUploadService = new FileUploadService(uploadsFolder);

            string fileName;
            try
            {
                fileName = await fileUploadService.UploadImageAsync(model.ImagePharmacics);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }

            // التحقق من صحة البيانات
            if (model == null)
                return BadRequest(new { message = "البيانات غير صحيحة." });

            var practitioner = await _context.Practitioners.FindAsync(model.PractitionerId);
            if (practitioner == null)
                return BadRequest(new { message = "الصيدلاني غير موجود" });

            bool isPractitionerLinkedToPharmacy = await _context.Pharmacies
                .AnyAsync(p => p.PractitionerId == model.PractitionerId);

            if (isPractitionerLinkedToPharmacy)
                return BadRequest(new { message = "لا يمكن انشاء أكثر من صيدلية لنفس المتمرس." });

            // تحويل الـ DTO إلى كيان
            var pharmacy = _mapper.Map<Pharmacics>(model);
            pharmacy.PractitionerId = model.PractitionerId;
            pharmacy.ImagePharmacics = fileName; // حفظ اسم الصورة فقط

            try
            {
                await _context.Pharmacies.AddAsync(pharmacy);
                await _context.SaveChangesAsync();

                var savedPharmacy = await _context.Pharmacies
                    .Include(p => p.Practitioner)
                    .FirstOrDefaultAsync(p => p.Id == pharmacy.Id);

                return Ok(new
                {
                    message = "تمت إضافة الصيدلية بنجاح.",
                    pharmacy = new
                    {
                        savedPharmacy.Id,
                        savedPharmacy.Name,
                        savedPharmacy.Address,
                        savedPharmacy.Latitude,
                        savedPharmacy.Longitude,
                        savedPharmacy.City,
                        savedPharmacy.LicenseNumber,
                        savedPharmacy.ImagePharmacics,
                        PractitionerName = savedPharmacy.Practitioner?.NamePractitioner,
                        savedPharmacy.PractitionerId
                    }
                });
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



        [HttpPut]
    
        public async Task<IActionResult> UpdatePharmacy(int id, [FromBody] PharmacicsDto model)
        {
            var pharmacy = await _context.Pharmacies.FindAsync(id);

            if (pharmacy == null)
            {
                return NotFound(new { message = "لم يتم العثور على الصيدلية." });
            }

        
            _mapper.Map(model, pharmacy);

            _context.Pharmacies.Update(pharmacy);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "تم تحديث بيانات الصيدلية بنجاح.",
                pharmacy = new
                {
                    pharmacy.Id,
                    pharmacy.Name,
                    pharmacy.Address,
                    pharmacy.Latitude,
                    pharmacy.Longitude,
                    pharmacy.City,
                    pharmacy.LicenseNumber,
       
                }
            });
        }


        [HttpDelete]
 
        public async Task<IActionResult> DeletePharmacy(int id)
        {
            var pharmacy = await _context.Pharmacies.FindAsync(id);

            if (pharmacy == null)
            {
                return NotFound(new { message = "لم يتم العثور على الصيدلية." });
            }

            _context.Pharmacies.Remove(pharmacy);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "تم حذف بيانات الصيدلية بنجاح.",
                pharmacy = new
                {
                    pharmacy.Id,
                    pharmacy.Name,
                    pharmacy.Address,
                    pharmacy.Latitude,
                    pharmacy.Longitude,
                    pharmacy.City,
                    pharmacy.LicenseNumber,
             
                }
            });
        }


    }
}
