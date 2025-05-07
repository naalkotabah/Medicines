using Medicines.Data.Models;
using Medicines.Data;
using Medicines.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Medicines.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _context;

        public OrderRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddOrderAsync(Order order)
        {
            _context.Orders!.Add(order);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Medicine>> GetMedicinesByIdsAsync(List<int> ids)
        {
            return await _context.Medicines!
                .Where(m => ids.Contains(m.Id))
                .ToListAsync();
        }

        public async Task<List<Order>> GetOrdersByUserIdAsync(int userId)
        {
            return await _context.Orders!
                .Where(o => o.UserId == userId)
                .Include(o => o.Pharmacy)
                .Include(o => o.OrderMedicines)
                    .ThenInclude(om => om.Medicine)
                .ToListAsync();
        }


        public async Task<List<Order>> GetOrdersByPharmacyIdAsync(int pharmacyId)
        {
            return await _context.Orders!
                .Where(o => o.PharmacyId == pharmacyId)
                .Include(o => o.User)
                .Include(o => o.OrderMedicines)
                    .ThenInclude(om => om.Medicine)
                .ToListAsync();
        }



        public async Task<List<Order>> GetOrdersWithDetailsAsync()
        {
            return await _context!.Orders!
                .Include(o => o.User)
                .Include(o => o.Pharmacy)
                .Include(o => o.OrderMedicines)
                    .ThenInclude(om => om.Medicine)
                .ToListAsync();
        }

        public async Task<string?> GetPharmacyNameByIdAsync(int pharmacyId)
        {
            return await _context.Pharmacies!
                .Where(p => p.Id == pharmacyId)
                .Select(p => p.Name)
                .FirstOrDefaultAsync();
        }


        public async Task<Practitioner> GetPractitionerByPharmacyIdAsync(int pharmacyId)
        {
            var practitioner = await _context.Practitioners!
                .Include(p => p.Pharmacy)  // تحميل Pharmacy مع Practitioner
                .FirstOrDefaultAsync(p => p.Pharmacy!.Id == pharmacyId);  // التحقق من PharmacyId

            // تأكد أن Practitioner و Pharmacy غير null
            if (practitioner == null || practitioner.Pharmacy == null)
            {
                return null!;  // أو يمكنك إرجاع رسالة خطأ
            }

            return practitioner;
        }





    }

}
