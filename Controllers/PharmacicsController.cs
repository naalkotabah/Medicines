using Medicines.Data.dto;
using Medicines.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Medicines.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PharmacicsController : ControllerBase
    {
        private readonly IPharmacyService _service;

        public PharmacicsController(IPharmacyService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetPharmacics()
        {
            var result = await _service.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPharmacics(int id)
        {
            var pharmacy = await _service.GetByIdAsync(id);
            if (pharmacy == null)
                return NotFound(new { message = "الصيدلية غير موجودة." });

            return Ok(new { message = "تم جلب البيانات", pharmacy });
        }

        [HttpPost]
        public async Task<IActionResult> AddPharmacy([FromForm] PharmacicsDto model)
        {
            var (success, message, data) = await _service.AddAsync(model);
            if (!success)
                return BadRequest(new { message });

            return Ok(new { message, pharmacy = data });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePharmacy(int id, [FromForm] PharmacicsDto model)
        {
            var (success, message, data) = await _service.UpdateAsync(id, model);
            if (!success)
                return BadRequest(new { message });

            return Ok(new { message, pharmacy = data });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePharmacy(int id)
        {
            var (success, message, data) = await _service.DeleteAsync(id);
            if (!success)
                return NotFound(new { message });

            return Ok(new { message, pharmacy = data });
        }
    }
}
