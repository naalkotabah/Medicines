namespace Medicines.Services
{
    using Medicines.Data.Models;
    using Medicines.Data.dto;
    using Medicines.Repositories.Interfaces;
    using Medicines.Services.Interfaces;

    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _repo;

        public OrderService(IOrderRepository repo)
        {
            _repo = repo;
        }

        public async Task<(bool, string, object?)> CreateOrderAsync(OrderDto dto)
        {
            if (dto.MedicineIds == null || !dto.MedicineIds.Any())
                return (false, "يجب إدخال الأدوية.", null);

            var medicines = await _repo.GetMedicinesByIdsAsync(dto.MedicineIds);

            if (medicines.Count != dto.MedicineIds.Count)
                return (false, "بعض الأدوية غير موجودة.", null);

            decimal finalPrice = medicines.Sum(m => m.Price);

            var order = new Order
            {
                Age = dto.Age,
                Weight = dto.Weight,
                ChronicDiseases = dto.ChronicDiseases,
                DrugAllergy = dto.DrugAllergy,
                Address = dto.Address,
                UserId = dto.UserId,
                PharmacyId = dto.PharmacyId,
                FinalPrice = finalPrice,
                OrderDate = DateTime.Now,
                OrderMedicines = medicines.Select(m => new OrderMedicine
                {
                    MedicineId = m.Id
                }).ToList()
            };

            await _repo.AddOrderAsync(order);

            var userName = await _repo.GetUserNameByIdAsync(dto.UserId);
            var pharmacyName = await _repo.GetPharmacyNameByIdAsync(dto.PharmacyId);

            var response = new
            {
                order.Id,
                order.Age,
                order.Weight,
                order.ChronicDiseases,
                order.DrugAllergy,
                order.Address,
                order.OrderDate,
                order.FinalPrice,
                UserName = userName,
                PharmacyName = pharmacyName,
                Medicines = medicines.Select(m => new { m.Id, m.TradeName, m.Price })
            };

            return (true, "تم إنشاء الطلب", response);
        }

        public async Task<List<object>> GetAllOrdersAsync()
        {
            var orders = await _repo.GetOrdersWithDetailsAsync();

            return orders
                .Where(o => o.User != null && o.Pharmacy != null && o.OrderMedicines != null)
                .Select(o => new
                {
                    OrderId = o.Id,
                    CustomerName = o.User!.Name, // بعد التحقق نقدر نحط !
                    PharmacyName = o.Pharmacy!.Name,
                    FinalPrice = o.FinalPrice,
                    Medicines = o.OrderMedicines!
                        .Where(om => om.Medicine != null)
                        .Select(om => new
                        {
                            MedicineName = om.Medicine!.TradeName
                        }).ToList()
                })
                .ToList<object>();
        }

    }

}
