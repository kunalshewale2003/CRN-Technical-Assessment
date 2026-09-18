using Application.DTOs.Auth;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly ITokenService _tokenService;
        private readonly PasswordHasher<User> _passwordHasher;

        public AuthService(
            IAuthRepository authRepository,
            ITokenService tokenService)
        {
            _authRepository = authRepository;
            _tokenService = tokenService;

            _passwordHasher = new PasswordHasher<User>();
        }

        public async Task<AuthResponseDto?> LoginAsync(
            LoginDto request)
        {
            var user = await _authRepository
                .GetUserByUsernameAsync(request.Username);

            if (user == null)
            {
                return null;
            }

            var passwordResult =
                _passwordHasher.VerifyHashedPassword(
                    user,
                    user.PasswordHash,
                    request.Password);

            if (passwordResult ==
                PasswordVerificationResult.Failed)
            {
                return null;
            }

            return await CreateAuthResponseAsync(user);
        }

        public async Task<AuthResponseDto?> RefreshTokenAsync(
            string refreshToken)
        {
            var existingToken =
                await _authRepository
                    .GetRefreshTokenAsync(refreshToken);

            if (existingToken == null)
            {
                return null;
            }

            if (existingToken.IsRevoked ||
                existingToken.ExpiresOn <= DateTime.UtcNow)
            {
                return null;
            }

            // Revoke the old refresh token
            existingToken.IsRevoked = true;

            await _authRepository
                .UpdateRefreshTokenAsync(existingToken);

            // Generate new tokens
            var response =
                await CreateAuthResponseAsync(
                    existingToken.User);

            await _authRepository.SaveChangesAsync();

            return response;
        }

        public async Task<bool> RevokeRefreshTokenAsync(
            string refreshToken)
        {
            var existingToken =
                await _authRepository
                    .GetRefreshTokenAsync(refreshToken);

            if (existingToken == null ||
                existingToken.IsRevoked)
            {
                return false;
            }

            existingToken.IsRevoked = true;

            await _authRepository
                .UpdateRefreshTokenAsync(existingToken);

            await _authRepository.SaveChangesAsync();

            return true;
        }

        private async Task<AuthResponseDto>
            CreateAuthResponseAsync(User user)
        {
            var accessToken =
                _tokenService.GenerateAccessToken(user);

            var refreshToken =
                _tokenService.GenerateRefreshToken(user.Id);

            await _authRepository
                .AddRefreshTokenAsync(refreshToken);

            await _authRepository.SaveChangesAsync();

            return new AuthResponseDto
            {
                AccessToken = accessToken,

                RefreshToken = refreshToken.Token,

                AccessTokenExpiresOn =
                    _tokenService.GetAccessTokenExpiration(),

                RefreshTokenExpiresOn =
                    refreshToken.ExpiresOn,

                UserId = user.Id,

                Username = user.Username,

                Role = user.Role
            };
        }
    }
}
