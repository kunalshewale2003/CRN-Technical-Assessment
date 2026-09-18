using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Auth
{
    public class AuthResponseDto
    {
        public string AccessToken { get; set; } = string.Empty;

        public string RefreshToken { get; set; } = string.Empty;

        public DateTime AccessTokenExpiresOn { get; set; }

        public DateTime RefreshTokenExpiresOn { get; set; }

        public int UserId { get; set; }

        public string Username { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;
    }
}
