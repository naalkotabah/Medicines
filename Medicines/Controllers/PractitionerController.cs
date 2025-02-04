using AutoMapper;
using Medicines.Data;
using Medicines.Data.dto;
using Medicines.Data.Models;
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

        // جلب جميع الممارسين
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



        [HttpGet("{id}")]
        public async Task<IActionResult> GetPractitioner(int id)
        {
            var practitioner = await _context.Practitioners.FindAsync(id);
            if (practitioner == null)
                return NotFound();
            return Ok(practitioner);
        }

        // إضافة ممارس جديد
        [HttpPost]
        public async Task<IActionResult> AddPractitioner([FromBody] PractitionerCreateDto practitionerDto)
        {
            if (practitionerDto == null)
                return BadRequest("Invalid data");

            var practitioner = new Practitioner
            {
                NamePractitioner = practitionerDto.NamePractitioner,
                Address = practitionerDto.Address,
                PhonNumber = practitionerDto.PhonNumber,
                Studies = practitionerDto.Studies
            };

            await _context.Practitioners.AddAsync(practitioner);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPractitioners), new { id = practitioner.Id }, practitioner);
        }


        // تحديث بيانات ممارس
        [HttpPut("{id}")]
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


        // حذف ممارس
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