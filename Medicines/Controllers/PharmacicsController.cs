using Medicines.Data;
using Medicines.Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Xml.Linq;

namespace Medicines.Controllers
{
    [Route("api/[controller]")]
    [ApiController]


    public class PharmacicsController : ControllerBase
    {
        public PharmacicsController(AppDbContext db)
        {
            context = db;
        }
        private readonly AppDbContext context;



        [HttpGet]
        public async Task<IActionResult> GetPharmacics()
        {
            var Pharmacics = await context.Pharmacies.Include(p => p.Medicines).ToListAsync();

            return Ok(Pharmacics);
        }



        [HttpPost]
        public async Task<IActionResult> AddPharmacics(string Name, string Address, decimal Latitude, decimal Longitude, string City, string LicenseNumber, string PharmacistName)
        {
            Pharmacics formdata = new()
            {
                Name = Name,
                Address = Address,
                Latitude = Latitude,
                Longitude = Longitude,
                City = City,
                LicenseNumber = LicenseNumber,
                PharmacistName = PharmacistName


            };

            await context.Pharmacies.AddAsync(formdata);
            context.SaveChanges();

            return Ok(formdata);
        }
    }
}
