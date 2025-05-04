using Medicines.Data.dto;
using Medicines.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
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
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> GetPharmacics()
        {
            var result = await _service.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Practitioner")]
        public async Task<IActionResult> GetPharmacics(int id)
        {
            var pharmacy = await _service.GetByIdAsync(id);
            if (pharmacy == null)
                return NotFound(new { message = "الصيدلية غير موجودة." });

            return Ok(new { message = "تم جلب البيانات", pharmacy });
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Practitioner")]
        public async Task<IActionResult> AddPharmacy([FromForm] PharmacicsDto model)
        {
            var (success, message, data) = await _service.AddAsync(model);
            if (!success)
                return BadRequest(new { message });

            return Ok(new { message, pharmacy = data });
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Practitioner")]
        public async Task<IActionResult> UpdatePharmacy(int id, [FromForm] PharmacicsDto model)
        {
            var (success, message, data) = await _service.UpdateAsync(id, model);
            if (!success)
                return BadRequest(new { message });

            return Ok(new { message, pharmacy = data });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Practitioner")]
        public async Task<IActionResult> DeletePharmacy(int id)
        {
            var (success, message, data) = await _service.DeleteAsync(id);
            if (!success)
                return NotFound(new { message });

            return Ok(new { message, pharmacy = data });
        }

        [Authorize(Roles = "Admin,Practitioner")]
        [HttpPost("approve-cancel/{orderId}")]
        public async Task<IActionResult> ApproveOrCancelOrder(int orderId, [FromQuery] bool approve)
        {
            var (success, message) = await _service.ApproveOrCancelOrderAsync(orderId, approve);

            if (!success)
                return BadRequest(new { message });

            return Ok(new { message });
        }
        [Authorize(Roles = "Admin,Practitioner")]
        [HttpPost("mark-done/{orderId}")]
        public async Task<IActionResult> MarkOrderAsDone(int orderId)
        {
            var (success, message) = await _service.MarkOrderAsDoneAsync(orderId);

            if (!success)
                return BadRequest(new { message });

            return Ok(new { message });
        }
        [Authorize]
        [HttpPost("cancel/{orderId}")]
        public async Task<IActionResult> CancelOrder(int orderId)
        {
            var (success, message) = await _service.CancelOrder(orderId);

            if (!success)
                return BadRequest(new { message });

            return Ok(new { message });
        }

    }
}
