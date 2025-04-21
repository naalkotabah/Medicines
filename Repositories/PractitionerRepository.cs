using Medicines.Data.Models;
using Medicines.Data;
using Medicines.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Medicines.Repositories
{
    public class PractitionerRepository : IPractitionerRepository
    {
        private readonly AppDbContext _context;

        public PractitionerRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Practitioner?> GetByLoginAsync(string name, string password)
        {
            return await _context.Practitioners!
                .FirstOrDefaultAsync(p => p.NamePractitioner == name && p.Password == password);
        }

        public async Task<List<Practitioner>> GetAllWithPharmacyAsync()
        {
            return await _context.Practitioners!.Include(p => p.Pharmacy).ToListAsync();
        }

        public async Task<List<Practitioner>> GetForSelectAsync()
        {
            return await _context.Practitioners!.ToListAsync();
        }

        public async Task<Practitioner?> GetPharmacyByPractitionerIdAsync(int id)
        {
            return await _context.Practitioners!
                .Include(p => p.Pharmacy)
                    .ThenInclude(ph => ph.Medicines) // <-- هذا المطلوب
                .FirstOrDefaultAsync(p => p.Id == id);
        }


        public async Task<Practitioner?> GetByIdAsync(int id)
        {
            return await _context.Practitioners!.FindAsync(id);
        }

        public async Task<bool> IsPractitionerLinkedToPharmacy(int id)
        {
            return await _context.Pharmacies!.AnyAsync(p => p.PractitionerId == id);
        }

        public async Task AddAsync(Practitioner practitioner)
        {
            await _context.Practitioners!.AddAsync(practitioner);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Practitioner practitioner)
        {
            _context.Practitioners!.Update(practitioner);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Practitioner practitioner)
        {
            _context.Practitioners!.Remove(practitioner);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Medicine>> SearchMedicinesForPractitioner(int practitionerId, string search)
        {
            return await _context.Medicines!
                .Where(m =>
                    m.Pharmacy != null && // اذا لم يساوي null
                    m.Pharmacy.PractitionerId == practitionerId &&
                    (
                        m.TradeName!.Contains(search) ||
                        m.ScientificName!.Contains(search)
                    )
                )
                .ToListAsync();
        }

    }

}
