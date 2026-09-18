using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IAuthRepository
    {
        Task<User?> GetUserByUsernameAsync(string username);

        Task<User?> GetUserByIdAsync(int userId);

        Task AddRefreshTokenAsync(RefreshToken refreshToken);

        Task<RefreshToken?> GetRefreshTokenAsync(string token);

        Task UpdateRefreshTokenAsync(RefreshToken refreshToken);

        Task SaveChangesAsync();
    }
}
