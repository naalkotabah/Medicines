using Medicines.Data.dto;
using Medicines.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Medicines.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : BaseController
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateOrder([FromBody] OrderDto orderDto)
        {

            orderDto.UserId = int.Parse(GetUserId()!);
            var (success, message, data) = await _orderService.CreateOrderAsync(orderDto);

            if (!success)
                return BadRequest(new { message });

            return Ok(new { message, order = data });
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetOrders()
        {
            var orders = await _orderService.GetAllOrdersAsync();
            return Ok(orders);
        }

        [Authorize]
        [HttpGet("user-orders")]
   
        public async Task<IActionResult> GetOrdersByUserId(int pageNumber = 1, int pageSize = 10)
        {
            var userId = int.Parse(GetUserId()!);
            var orders = await _orderService.GetOrdersByUserIdAsync(userId, pageNumber, pageSize);

            if (orders == null || !orders.Any())
                return NotFound("لا توجد طلبات لهذا المستخدم");

            var result = orders.Select(o => new
            {
                o.Id,
                o.OrderDate,
                o.FinalPrice,
                o.Address,
                PharmacyName = o.Pharmacy?.Name,
                Medicines = o.OrderMedicines.Select(om => new
                {
                    om.MedicineId,
                    om.Medicine?.TradeName
                }).ToList()
            });

            return Ok(new
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                Orders = result
            });
        }


        [Authorize]
        [HttpGet("pharmacy-orders/{pharmacyId}")]
        public async Task<IActionResult> GetOrdersByPharmacyId(int pharmacyId, int pageNumber = 1, int pageSize = 10)
        {
            var orders = await _orderService.GetOrdersByPharmacyIdAsync(pharmacyId, pageNumber, pageSize);

            if (orders == null || !orders.Any())
                return NotFound("لا توجد طلبات لهذه الصيدلية");

            var result = orders.Select(o => new
            {
                o.Id,
                o.OrderDate,
                o.FinalPrice,
                o.Address,
                CustomerName = o.User?.Name,
                Medicines = o.OrderMedicines.Select(om => new
                {
                    om.MedicineId,
                    om.Medicine?.TradeName
                }).ToList()
            });

            return Ok(new
            {
                CurrentPage = pageNumber,
                PageSize = pageSize,
                Orders = result
            });
        }




    }
}
