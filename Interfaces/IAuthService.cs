using WUIAM.DTOs;
using WUIAM.Models;
using WUIAM.Services;

namespace WUIAM.Interfaces
{
    public interface IAuthService
    {

        public Task<dynamic> LoginAsync(LoginDto loginDto);
        public Task<dynamic> VerifyLoginTokenAsync(string email, string token);
        public Task<User> RegisterAsync(CreateUserDto createUserDto);
        public Task<(string message, bool status)> ResetPasswordAsync(ResetPasswordDTo resetPasswordDTo);
        public Task<(string message, bool status)> ChangePasswordAsync(ChangePasswordDto changePasswordDto);
        public Task<(string message, bool status)> LogoutAsync(string email);
        public Task<string> ForgotPasswordAsync(string email);
        public Task<RefreshToken> CreateRefreshTokenAsync(User user);
        public Task<dynamic> GetRefreshTokenAsync(string refreshToken);
    }
}
