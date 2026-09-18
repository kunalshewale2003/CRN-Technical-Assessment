using Application.DTOs.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto?> LoginAsync(LoginDto request);

        Task<AuthResponseDto?> RefreshTokenAsync(string refreshToken);

        Task<bool> RevokeRefreshTokenAsync(string refreshToken);
    }
}
