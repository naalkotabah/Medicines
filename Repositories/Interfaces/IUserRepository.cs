using Medicines.Data.Models;

namespace Medicines.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<bool> IsUsernameTaken(string username);
        Task<Users?> GetUserWithPractitionerAsync(string name, string password);

        Task AddUserAsync(Users user);
        Task<Users?> GetUserByCredentialsAsync(string name, string password);
        Task<List<Users>> GetAllUsersAsync();
    }

}
