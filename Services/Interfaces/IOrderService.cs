namespace Medicines.Services.Interfaces
{
    using Medicines.Data.dto;
    using Medicines.Data.Models;

    public interface IOrderService
    {
        Task<(bool Success, string Message, object? Data)> CreateOrderAsync(OrderDto dto);
        Task<List<object>> GetAllOrdersAsync();



        Task<List<Order>> GetOrdersByUserIdAsync(int userId, int pageNumber, int pageSize);
        Task<List<Order>> GetOrdersByPharmacyIdAsync(int pharmacyId, int pageNumber, int pageSize);

        Task<List<Medicine>> GetMedicinesByIdsAsync(List<int> ids);
        Task<List<Order>> GetOrdersWithDetailsAsync();

    }

}
