namespace Medicines.Services.Interfaces
{
    using Medicines.Data.dto;

    public interface IPharmacyService
    {
        Task<List<object>> GetAllAsync();
        Task<object?> GetByIdAsync(int id);
        Task<(bool Success, string Message, object? Data)> AddAsync(PharmacicsDto dto);
        Task<(bool Success, string Message, object? Data)> UpdateAsync(int id, PharmacicsDto dto);
        Task<(bool Success, string Message, object? Data)> DeleteAsync(int id);
    }

}
