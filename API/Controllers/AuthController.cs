using Application.DTOs;
using Application.DTOs.Auth;
using Application.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/v1/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IValidator<LoginDto> _loginValidator;
        private readonly IValidator<RefreshTokenDto> _refreshTokenValidator;

        public AuthController(
            IAuthService authService,
            IValidator<LoginDto> loginValidator,
            IValidator<RefreshTokenDto> refreshTokenValidator)
        {
            _authService = authService;
            _loginValidator = loginValidator;
            _refreshTokenValidator = refreshTokenValidator;
        }

        /// <summary>
        /// Authenticates a user and returns access and refresh tokens.
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login(
            [FromBody] LoginDto request)
        {
            var validationResult =
                await _loginValidator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .Select(x => x.ErrorMessage)
                    .ToList();

                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Validation failed.",
                    Data = errors
                });
            }

            var result =
                await _authService.LoginAsync(request);

            if (result == null)
            {
                return Unauthorized(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Invalid username or password."
                });
            }

            return Ok(new ApiResponse<AuthResponseDto>
            {
                Success = true,
                Message = "Login successful.",
                Data = result
            });
        }

        /// <summary>
        /// Generates a new access token using a valid refresh token.
        /// </summary>
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh(
            [FromBody] RefreshTokenDto request)
        {
            var validationResult =
                await _refreshTokenValidator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .Select(x => x.ErrorMessage)
                    .ToList();

                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Validation failed.",
                    Data = errors
                });
            }

            var result =
                await _authService.RefreshTokenAsync(
                    request.RefreshToken);

            if (result == null)
            {
                return Unauthorized(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Invalid or expired refresh token."
                });
            }

            return Ok(new ApiResponse<AuthResponseDto>
            {
                Success = true,
                Message = "Token refreshed successfully.",
                Data = result
            });
        }

        /// <summary>
        /// Revokes a refresh token.
        /// </summary>
        [HttpPost("revoke")]
        public async Task<IActionResult> Revoke(
            [FromBody] RefreshTokenDto request)
        {
            var validationResult =
                await _refreshTokenValidator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .Select(x => x.ErrorMessage)
                    .ToList();

                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Validation failed.",
                    Data = errors
                });
            }

            var revoked =
                await _authService.RevokeRefreshTokenAsync(
                    request.RefreshToken);

            if (!revoked)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Refresh token not found."
                });
            }

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Refresh token revoked successfully."
            });
        }
    }
}