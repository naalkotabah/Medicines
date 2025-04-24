using Medicines.Data.dto;
using Medicines.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Medicines.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MedicineController : ControllerBase
    {
        private readonly IMedicineService _medicineService;

        public MedicineController(IMedicineService medicineService)
        {
            _medicineService = medicineService;
        }

        [HttpGet]

        public async Task<IActionResult> GetMedicine()
        {
            var result = await _medicineService.GetAllAsync();
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Practitioner")]
        public async Task<IActionResult> AddMedicine([FromForm] medicineDto medicineDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    message = "الرجاء التحقق من البيانات.",
                    errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
                });
            }

            var (success, message, data) = await _medicineService.AddAsync(medicineDto);
            if (!success)
                return BadRequest(new { message });

            return Ok(new { message, medicine = data });
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Practitioner")]
        public async Task<IActionResult> UpdateMedicine(int id, [FromForm] medicineDto medicineDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    message = "الرجاء التحقق من البيانات.",
                    errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
                });
            }

            var (success, message, data) = await _medicineService.UpdateAsync(id, medicineDto);
            if (!success)
                return BadRequest(new { message });

            return Ok(new { message, medicine = data });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Practitioner")]
        public async Task<IActionResult> DeleteMedicine(int id)
        {
            var (success, message, data) = await _medicineService.DeleteAsync(id);
            if (!success)
                return NotFound(new { message });

            return Ok(new { message, medicine = data });
        }
    }
}
