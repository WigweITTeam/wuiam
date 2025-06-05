using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WUIAM.Services;
using WUIAM.Models;
using WUIAM.Interfaces;
using WUIAM.DTOs;

namespace WUIAM.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto request)
        {
            var result = await _authService.LoginAsync(request);
            if (!result.Success)
                return Unauthorized(result.Message);

            return Ok(result);
        }
        [HttpPost("verify-login-token")]
        public async Task<IActionResult> VerifyLoginToken([FromBody] VerifyLoginTokenDto request)
        {
            var result = await _authService.VerifyLoginTokenAsync(request.Email, request.Token);
            if (!result.Success)
                return Unauthorized(result.Message);

            return Ok(result);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] CreateUserDto request)
        {
            var result = await _authService.RegisterAsync(request);
            if (result == null)
                return BadRequest("User registration failed!");

            return Ok(result);
        }

        // [HttpPost("refresh-token")]
        // public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        // {
        //     var result = await _authService.RefreshTokenAsync(request);
        //     if (!result.Success)
        //         return Unauthorized(result.Message);

        //     return Ok(result);
        // }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTo request)
        {
            var result = await _authService.ResetPasswordAsync(request);
            if (!result.status)
                return BadRequest(result.message);

            return Ok(result.message);
        }
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto request)
        {
            var result = await _authService.ChangePasswordAsync(request);
            if (!result.status)
                return BadRequest(result.message);

            return Ok(result.message);
        }
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto request)
        {
            var result = await _authService.ForgotPasswordAsync(request.Email);
            if (string.IsNullOrEmpty(result) || result == "User not found")
                return BadRequest("Failed to send reset password email.");

            return Ok("Reset password email sent successfully.");
        }
        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] string email)
        {
            var result = await _authService.LogoutAsync(email);
            if (!result.status)
                return BadRequest(result.message);

            return Ok(result.message);
        }
    }

}