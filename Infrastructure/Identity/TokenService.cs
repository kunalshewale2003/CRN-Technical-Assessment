using Application.Interfaces;
using Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Identity
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;

        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateAccessToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(
                    ClaimTypes.NameIdentifier,
                    user.Id.ToString()),

                new Claim(
                    ClaimTypes.Name,
                    user.Username),

                new Claim(
                    ClaimTypes.Role,
                    user.Role)
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    _configuration["Jwt:Key"]!));

            var credentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: GetAccessTokenExpiration(),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler()
                .WriteToken(token);
        }

        public RefreshToken GenerateRefreshToken(int userId)
        {
            return new RefreshToken
            {
                Token = Convert.ToBase64String(
                    RandomNumberGenerator.GetBytes(64)),

                UserId = userId,

                ExpiresOn = GetRefreshTokenExpiration(),

                IsRevoked = false
            };
        }

        public DateTime GetAccessTokenExpiration()
        {
            var minutes = double.Parse(
                _configuration[
                    "Jwt:AccessTokenExpirationMinutes"]!);

            return DateTime.UtcNow.AddMinutes(minutes);
        }

        public DateTime GetRefreshTokenExpiration()
        {
            var days = double.Parse(
                _configuration[
                    "Jwt:RefreshTokenExpirationDays"]!);

            return DateTime.UtcNow.AddDays(days);
        }
    }
}
