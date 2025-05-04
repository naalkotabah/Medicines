using Medicines.Data.Models;
using Medicines.Data;
using Medicines.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Medicines.Repositories
{
    public class PharmacyRepository : IPharmacyRepository
    {
        private readonly AppDbContext _context;

        public PharmacyRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Pharmacics>> GetAllWithDetailsAsync()
        {
            return await _context.Pharmacies!
                .Include(p => p.Medicines)
                .Include(p => p.Practitioner)
                .ToListAsync();
        }

        public async Task<Pharmacics?> GetByIdWithDetailsAsync(int id)
        {
            return await _context.Pharmacies!
                .Include(p => p.Medicines)
                .Include(p => p.Practitioner)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Pharmacics?> GetByIdAsync(int id)
        {
            return await _context.Pharmacies!.FindAsync(id);
        }

        public async Task<Practitioner?> GetPractitionerByIdAsync(int id)
        {
            return await _context.Practitioners!.FindAsync(id);
        }

        public async Task<bool> IsPractitionerLinkedAsync(int practitionerId)
        {
            return await _context.Pharmacies!.AnyAsync(p => p.PractitionerId == practitionerId);
        }

        public async Task AddAsync(Pharmacics pharmacy)
        {
            await _context.Pharmacies!.AddAsync(pharmacy);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Pharmacics pharmacy)
        {
            _context.Pharmacies!.Update(pharmacy);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Pharmacics pharmacy)
        {
            _context.Pharmacies!.Remove(pharmacy);
            await _context.SaveChangesAsync();
        }

        public async Task<Order?> GetOrderByIdAsync(int orderId)
        {
            return await _context.Orders!
                .Where(o => o.Id == orderId)
                .FirstOrDefaultAsync();
        }

        public async Task UpdateOrderAsync(Order order)
        {
            _context.Orders?.Update(order);
            await _context.SaveChangesAsync();
        }

    }

}
