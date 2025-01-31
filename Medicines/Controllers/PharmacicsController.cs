using AutoMapper;
using Medicines.Data;
using Medicines.Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Medicines.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PharmacicsController : ControllerBase
    {
        private readonly AppDbContext context;
        public PharmacicsController(AppDbContext _context)
        {
            this.context = _context;
           
        }


        [HttpGet]
        public IActionResult GetPharmacics()
        {
            var Phrancucs = context.Pharmacies.Select(
                p => new
                {
                    p.Id,
                    p.Name,
                    p.Address,
                    p.Latitude,
                    p.Longitude,
                    p.City,
                    p.LicenseNumber,
                    p.PharmacistName,
                    Medicines = p.Medicines.Select(m => new
                    {
                        m.Id,
                        m.ScientificName,
                        m.TradeName,
                        m.ProducingCompany
                    }).ToList(),
                });

            return Ok(Phrancucs);
        }
    }

 


}
