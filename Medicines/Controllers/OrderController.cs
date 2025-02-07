using AutoMapper;
using Medicines.Data;
using Medicines.Data.dto;
using Medicines.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Medicines.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        public OrderController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpPost]

        public IActionResult CreateOrder([FromBody] OrderDto orderDto)
        {
            if (orderDto == null || orderDto.MedicineIds == null || !orderDto.MedicineIds.Any())
                return BadRequest("يجب إدخال الأدوية.");

            // جلب الأدوية المطلوبة مع الأسماء والأسعار
            var medicines = _context.Medicines
                .Where(m => orderDto.MedicineIds.Contains(m.Id))
                .Select(m => new { m.Id, m.TradeName, m.Price })
                .ToList();

            if (medicines.Count != orderDto.MedicineIds.Count)
                return BadRequest("بعض الأدوية غير موجودة.");

            // حساب السعر النهائي بناءً على أسعار الأدوية
            decimal finalPrice = medicines.Sum(m => m.Price);

            // إنشاء الطلب
            var order = new Order
            {
                Age = orderDto.Age,
                Weight = orderDto.Weight,
                ChronicDiseases = orderDto.ChronicDiseases,
                DrugAllergy = orderDto.DrugAllergy,
                Address = orderDto.Address,
                UserId = orderDto.UserId,
                PharmacyId = orderDto.PharmacyId,
                FinalPrice = finalPrice,
                dateTime = DateTime.Now,
                OrderMedicines = medicines.Select(m => new OrderMedicine { MedicineId = m.Id }).ToList()
            };

            _context.Orders.Add(order);
            _context.SaveChanges();

            // جلب اسم المستخدم واسم الصيدلية
            var userName = _context.Users
                .Where(u => u.Id == orderDto.UserId)
                .Select(u => u.Name)
                .FirstOrDefault();

            var pharmacyName = _context.Pharmacies
                .Where(p => p.Id == orderDto.PharmacyId)
                .Select(p => p.Name)
                .FirstOrDefault();

            // ✅ إرجاع البيانات المطلوبة فقط
            return Ok(new
            {
                order.Id,
                order.Age,
                order.Weight,
                order.ChronicDiseases,
                order.DrugAllergy,
                order.Address,
                order.dateTime,
                order.FinalPrice,
                UserName = userName,
                PharmacyName = pharmacyName,
                Medicines = medicines.Select(m => new { m.Id, m.TradeName, m.Price })
            });
        }


   

    }
}
