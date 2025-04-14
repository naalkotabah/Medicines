using Medicines.Data.dto;
using Medicines.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Medicines.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PractitionerController : ControllerBase
    {
        private readonly IPractitionerService _service;

        public PractitionerController(IPractitionerService service)
        {
            _service = service;
        }

        //[HttpPost("Login/Practitioner")]
        //public async Task<IActionResult> Login([FromBody] Login_Practitioner dto)
        //{
        //    if (dto == null || string.IsNullOrWhiteSpace(dto.NamePractitioner) || string.IsNullOrWhiteSpace(dto.Password))
        //        return BadRequest(new { message = "Name and password are required" });

        //    var result = await _service.LoginAsync(dto);

        //    if (result == null)
        //        return Unauthorized(new { message = "Invalid username or password" });

        //    return Ok(result);
        //}

        [HttpGet]
        public async Task<IActionResult> GetPractitioners()
        {
            var result = await _service.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("GetForselect")]
        public async Task<IActionResult> GetPractitioners_ForSelect()
        {
            var result = await _service.GetForSelectAsync();
            return Ok(result);
        }

        [HttpGet("{Practitionersid}")]
        public async Task<IActionResult> GetMyPharmacy(int Practitionersid)
        {
            var result = await _service.GetMyPharmacyAsync(Practitionersid);
            if (result == null)
                return NotFound(new { message = "الممارس غير موجود أو لا يملك صيدلية." });

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> AddPractitioner([FromForm] PractitionerCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var (success, message, data) = await _service.AddAsync(dto);

            if (!success)
                return BadRequest(new { message });

            return Ok(new { message, practitioner = data });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePractitioner(int id, [FromForm] PractitionerCreateDto dto)
        {
            var (success, message, data) = await _service.UpdateAsync(id, dto);

            if (!success)
                return BadRequest(new { message });

            return Ok(new { message, practitioner = data });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePractitioner(int id)
        {
            var (success, message, data) = await _service.DeleteAsync(id);

            if (!success)
                return BadRequest(new { message });

            return Ok(new { message, practitioner = data });
        }
    }
}
