namespace Medicines.Services.Interfaces
{
    using Medicines.Data.dto;
    using Medicines.Data.Models;

    public interface IUserService
    {
        Task<(bool Success, string Message, string? Token)> RegisterAsync(RegisterDto registerDto);
        Task<LoginDto?> LoginAsync(UserDto userDto);
        Task<List<Users>> GetUsersAsync();
    }

}
