using Medicines.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Medicines.Repositories.Interfaces
{
    public interface IOrderRepository
    {
        Task AddOrderAsync(Order order);
        Task<List<Order>> GetOrdersWithDetailsAsync();
        Task<List<Medicine>> GetMedicinesByIdsAsync(List<int> ids);
        Task<List<Order>> GetOrdersByUserIdAsync(int userId);
        Task<List<Order>> GetOrdersByPharmacyIdAsync(int pharmacyId);

        Task<Practitioner> GetPractitionerByPharmacyIdAsync(int pharmacyId);
        
       

        Task<string?> GetPharmacyNameByIdAsync(int pharmacyId);

    }

}
