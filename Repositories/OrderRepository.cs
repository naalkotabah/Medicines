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

        public async Task<string?> GetUserNameByIdAsync(int userId)
        {
            return await _context.Users!
                .Where(u => u.Id == userId)
                .Select(u => u.Name)
                .FirstOrDefaultAsync();
        }

        public async Task<string?> GetPharmacyNameByIdAsync(int pharmacyId)
        {
            return await _context.Pharmacies!
                .Where(p => p.Id == pharmacyId)
                .Select(p => p.Name)
                .FirstOrDefaultAsync();
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
