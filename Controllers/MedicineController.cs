using Medicines.Data.dto;
using Medicines.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Drawing.Printing;

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

        public async Task<IActionResult> GetMedicine(int pageNumber = 1, int pageSize = 10, int Pharmacice = 0)
        {
            var result = await _medicineService.GetAllAsync( pageNumber , pageSize , Pharmacice);
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



        [HttpGet("search")]
        public async Task<IActionResult> SearchForMedicineHome([FromQuery] string name)
        {
           
            if (string.IsNullOrWhiteSpace(name))
            {
                return BadRequest("اسم الدواء مطلوب للبحث.");
            }

    
            var medicines = await _medicineService.SerchForMedicineHome(name);

    
            if (medicines == null || medicines.Count == 0)
            {
                return NotFound("لم يتم العثور على أي دواء مطابق.");
            }

         
            return Ok(medicines);
        }
    }
}
