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
                .Include(p => p.Medicines) // Load medicines to avoid Lazy Loading issues
                .Select(p => new
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
                    }).ToList() 
                })
                .ToListAsync();

            return Ok(pharmacics);
        }


        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddPharmacics([FromBody] PharmacicsDto model)
        {
         
        

            
            var pharmacy = _mapper.Map<Pharmacics>(model);

           
            await _context.Pharmacies.AddAsync(pharmacy);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "تمت إضافة الصيدلية بنجاح.",
                pharmacy = new
                {
                    pharmacy.Id,
                    pharmacy.Name,
                    pharmacy.Address,
                    pharmacy.Latitude,
                    pharmacy.Longitude,
                    pharmacy.City,
                    pharmacy.LicenseNumber,
                    pharmacy.PharmacistName
                }
            });
        }


    }
}
