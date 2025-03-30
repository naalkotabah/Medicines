namespace Medicines.Services.Interfaces
{
    using Medicines.Data.dto;

    public interface IOrderService
    {
        Task<(bool Success, string Message, object? Data)> CreateOrderAsync(OrderDto dto);
        Task<List<object>> GetAllOrdersAsync();
    }

}
