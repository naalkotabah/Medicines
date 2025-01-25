using Medicines.Data.Models;
using Medicines.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Medicines.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MedicineController : ControllerBase
    {


        public MedicineController(AppDbContext db)
        {
            context = db;
        }
        private readonly AppDbContext context;



        [HttpGet]
        public async Task<IActionResult> GetMedicines()
        {
            var GetMedicines = await context.Medicines.ToListAsync();

            return Ok(GetMedicines);
        }



        [HttpPost]
        public async Task<IActionResult> AddMedicines(string TradeName, string ScientificName, decimal Dosage, string DrugTiming, string SideEffects, string ContraindicatedDrugs,int PharmacyId , string ManufacturerName, string ProducingCompany, decimal Price)
        {
            Medicine formdata = new()
            {
                TradeName = TradeName,
                ScientificName = ScientificName,
                Dosage = Dosage,
                DrugTiming = DrugTiming,
                SideEffects = SideEffects,
                ContraindicatedDrugs = ContraindicatedDrugs,
                ManufacturerName = ManufacturerName,
                ProducingCompany = ProducingCompany,
                Price = Price,


                PharmacyId = PharmacyId


            };

            await context.Medicines.AddAsync(formdata);
            context.SaveChanges();

            return Ok(formdata);
        }
    }
}
