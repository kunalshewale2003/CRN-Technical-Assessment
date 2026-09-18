using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly ApplicationDbContext _context;

        public AuthRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            return await _context.Users
                .Include(u => u.RefreshTokens)
                .FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<User?> GetUserByIdAsync(int userId)
        {
            return await _context.Users
                .Include(u => u.RefreshTokens)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task AddRefreshTokenAsync(
            RefreshToken refreshToken)
        {
            await _context.RefreshTokens.AddAsync(refreshToken);
        }

        public async Task<RefreshToken?> GetRefreshTokenAsync(
            string token)
        {
            return await _context.RefreshTokens
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Token == token);
        }

        public async Task UpdateRefreshTokenAsync(
            RefreshToken refreshToken)
        {
            _context.RefreshTokens.Update(refreshToken);

            await Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }

}
