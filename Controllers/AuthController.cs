using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WUIAM.Services;
using WUIAM.Models;
using WUIAM.Interfaces;
using WUIAM.DTOs;
using Microsoft.AspNetCore.Authorization;
using WUIAM.Enums;
using Azure.Core; 

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
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDto request)
        {
            var result = await _authService.LoginAsync(request);
            if (!result.Success)
                return Unauthorized(result.Message);

           return Ok(result);
        }
        [AllowAnonymous]
        [HttpPost("verify-login-token")]
        public async Task<IActionResult> VerifyLoginToken([FromBody] VerifyLoginTokenDto request)
        {
            var result = await _authService.VerifyLoginTokenAsync(request.Email, request.Token);
            if (!result.Success)
                return Unauthorized(result.Message);

            var refreshToken = result.refreshToken;
            Response.Cookies.Append("refresh_token", refreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true, // Use true in production
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddDays(7),
                Path = "/" // Scope of cookie
            });

            return Ok(result);
        }
        //[AllowAnonymous]
        [HttpGet("users")]
        public async Task<IActionResult> getUsers()
        {

            var result = await _authService.GetStaffListAsync();
            if (result == null)
                return BadRequest(ApiResponse<object>.Failure("Failed to fetch user types", new { reason = "No user found" }));

            return Ok(ApiResponse<IEnumerable<UserDto?>?>.Success("user types found", result));

             
        }
        [HasPermission(Permissions.ManageUsers, Permissions.CreateUser)]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] CreateUserDto request)
        {
            var result = await _authService.RegisterAsync(request);
            if (result == null)
                return BadRequest("User registration failed!");

            return Ok(result);
        }
        [AllowAnonymous]
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken()
        {
            var refreshToken = Request.Cookies["refresh_token"];
            if (string.IsNullOrEmpty(refreshToken)) return Unauthorized();

             
            var result = await _authService.GetRefreshTokenAsync(refreshToken);
            if (!result.Success)
                return Unauthorized(result.Message);

            return Ok(result);
        }
        //[AllowAnonymous]
        [HttpGet("get-user-type")]
        public async Task<IActionResult> GetUserTypes()
        {
            var result = await _authService.getUserTypes();
            if (!result.Status)
                return BadRequest(result.message);

            return Ok(result);
        }

        [AllowAnonymous]
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTo request)
        {
            var result = await _authService.ResetPasswordAsync(request);
            if (!result.status)
                return BadRequest(result.message);

            return Ok(result);
        }
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto request)
        {
            var result = await _authService.ChangePasswordAsync(request);
            if (!result.status)
                return BadRequest(result.message);
            return Ok(new { message = result.message, status = result.status });
        }
        [AllowAnonymous]
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto request)
        {
            var result = await _authService.ForgotPasswordAsync(request.Email);
            if (string.IsNullOrEmpty(result) || result == "User not found")
                return BadRequest("Failed to send reset password email.");

            return Ok("Reset password email sent successfully.");
        }
      
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("refresh_token");
            return Ok();
        }
    }

}