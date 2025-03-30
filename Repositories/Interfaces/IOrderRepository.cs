using Medicines.Data.Models;

namespace Medicines.Repositories.Interfaces
{
    public interface IOrderRepository
    {
        Task AddOrderAsync(Order order);
        Task<List<Order>> GetOrdersWithDetailsAsync();
        Task<List<Medicine>> GetMedicinesByIdsAsync(List<int> ids);
        Task<string?> GetUserNameByIdAsync(int userId);
        Task<string?> GetPharmacyNameByIdAsync(int pharmacyId);
    }

}
