using Medicines.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Medicines.Repositories.Interfaces
{
    public interface IPharmacyRepository
    {
        Task<List<Pharmacics>> GetAllWithDetailsAsync();
        Task<Pharmacics?> GetByIdWithDetailsAsync(int id);
        Task<Pharmacics?> GetByIdAsync(int id);
        Task<Practitioner?> GetPractitionerByIdAsync(int id);
        Task<bool> IsPractitionerLinkedAsync(int practitionerId);
        Task AddAsync(Pharmacics pharmacy);
        Task UpdateAsync(Pharmacics pharmacy);
        Task DeleteAsync(Pharmacics pharmacy);

        Task<Order?> GetOrderByIdAsync(int orderId);
        Task UpdateOrderAsync(Order order);
       
    }

}
