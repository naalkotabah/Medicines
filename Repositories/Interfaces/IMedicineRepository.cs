using Medicines.Data.Models;

namespace Medicines.Repositories.Interfaces
{
    public interface IMedicineRepository
    {
        Task<List<Medicine>> GetAllAsync();
        Task<Medicine?> GetByIdAsync(int id);
        Task AddAsync(Medicine medicine);
        Task UpdateAsync(Medicine medicine);
        Task DeleteAsync(Medicine medicine);
        Task<Pharmacics?> GetPharmacyByIdAsync(int id);
    }

}
