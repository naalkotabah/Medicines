﻿using Medicines.Data.Models;
using Medicines.Data;
using Medicines.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Medicines.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> IsUsernameTaken(string username)
        {
            return await _context.Users.AnyAsync(u => u.Name == username);
        }

        public async Task AddUserAsync(Users user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        public async Task<Users?> GetUserByCredentialsAsync(string name, string password)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Name == name && u.Password == password && !u.IsDleted);
        }

        public async Task<List<Users>> GetAllUsersAsync()
        {
            return await _context.Users.ToListAsync();
        }
    }

}
