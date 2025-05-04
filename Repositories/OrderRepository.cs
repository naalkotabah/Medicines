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




    }

}
