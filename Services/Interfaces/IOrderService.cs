namespace Medicines.Services.Interfaces
{
    using Medicines.Data.dto;
    using Medicines.Data.Models;

    public interface IOrderService
    {
        Task<(bool Success, string Message, object? Data)> CreateOrderAsync(OrderDto dto);
        Task<List<object>> GetAllOrdersAsync();



        Task<List<Order>> GetOrdersByUserIdAsync(int userId);
        Task<List<Order>> GetOrdersByPharmacyIdAsync(int pharmacyId);

        Task<List<Medicine>> GetMedicinesByIdsAsync(List<int> ids);
        Task<List<Order>> GetOrdersWithDetailsAsync();

    }

}
