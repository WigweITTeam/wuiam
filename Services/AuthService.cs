using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using WUIAM.DTOs;
using WUIAM.Interfaces;
using WUIAM.Models;
using WUIAM.Repositories.IRepositories;
using WUIAM.Services.Config;

namespace WUIAM.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly INotifyService _notifyService;
        private readonly IConfiguration _configuration;
        private readonly string _jwtSecret;
        private readonly string _jwtIssuer;
        private readonly string _jwtAudience;
        public AuthService(IAuthRepository authRepository, INotifyService notifyService, IConfiguration configuration)
        {
            _authRepository = authRepository;
            _notifyService = notifyService;
            _configuration = configuration;
            _jwtSecret = _configuration["Jwt:Key"]!;
            _jwtIssuer = _configuration["Jwt:Issuer"]!;
            _jwtAudience = _configuration["Jwt:Audience"]!;
        }
        

        public async Task<dynamic> LoginAsync(LoginDto loginDto)
        {
            var user = await _authRepository.FindUserByEmailOrUserNameAsync(loginDto.Email);
            if (user == null)
            {
                return new { Success = false, data = (object?)null,Message="Invalid username or password!" };
            }
            var isValidPassword = PasswordUtilService.VerifyPassword(password: loginDto.Password!, hashedPassword: user.Password!);
            if (!isValidPassword)
            {
                return new { Success = false, data = (object?)null, Message = "Invalid username or password!" };
            }
            // Generate login token for 2FA
            if (user.TwoFactorEnabled)
            {
                // Generate a 2FA token and send it to the user
                var twoFactorToken = PasswordUtilService.GenerateTwoFactorToken();
                // save the twoFactorToken to MFAToken table
                var savedToken = await _authRepository.SaveTwoFactorTokenAsync(user.UserId, PasswordUtilService.HashPassword(twoFactorToken));
              
                await _notifyService.SendEmailAsync(
                    to: [new EmailReceiver { Email = user.UserEmail!, Name = user.FullName! }],
                    subject: "Two-Factor Authentication Token",
                    body: EmailTemplateService.GenerateTwoFactorTokenEmailHtml(user.FullName!, twoFactorToken)
                );

                return new { Success = true, data = user, verifyToken = true , Message = "You have 2FA enable. A login verification email has being sent. Verify login to continue!" };
            }
            // If 2FA is not enabled, proceed with normal login
            //if (user.IsDefault == false)
            //{
            //    return new { Success = false, data = (object?)null };
            //}
            return LoginTokenResponse(user);
        }

        private async Task<dynamic> LoginTokenResponse(User user) 
        {
           var token = GenerateJwtToken(user);
            user.Password = null;
            user.SessionId = Guid.NewGuid().ToString();
            user.SessionTime = DateTime.Now;
            user.DateLastLoggedIn = DateTime.Now;
            await _authRepository.UpdateUserAsync(user);
            await _notifyService.SendEmailAsync(
              to: [new EmailReceiver { Email = user.UserEmail!, Name = user.FullName! }],
              subject: "Login Notification",
              body: EmailTemplateService.GenerateLoginNotificationEmailHtml(user.FullName!, DateTime.Now)
          );
            return new { Success = true, data = user, token };
        }

        private string GenerateJwtToken(User user)
        {
            var claims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
                    new Claim(JwtRegisteredClaimNames.Email, user.UserEmail ?? ""),
                    new Claim("id", user.UserId.ToString())
                };

            // Add roles as claims
            if (user.UserRoles != null)
            {
                foreach (var userRole in user.UserRoles)
                {
                    if (userRole.Role != null && !string.IsNullOrEmpty(userRole.Role.Name))
                    {
                        claims.Add(new Claim(ClaimTypes.Role, userRole.Role.Name));
                    }
                }
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSecret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtIssuer,
                audience: _jwtAudience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(10),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }




        public async Task<(string message, bool status)> ResetPasswordAsync(ResetPasswordDTo resetPasswordDTo)
        {
            var user = await _authRepository.FindUserByEmailAsync(resetPasswordDTo.Email);
            if (user == null)
            {
                return await Task.FromResult(("User not found", false));
            }
            if (resetPasswordDTo.NewPassword != resetPasswordDTo.ConfirmedPassword)
            {
                return await Task.FromResult(("Passwords do not match", false));
            }
            user.Password = PasswordUtilService.HashPassword(resetPasswordDTo.NewPassword.ToString());
            var isValidToken = (PasswordUtilService.VerifyPassword(password: resetPasswordDTo.ResetToken.ToString().Trim(), hashedPassword: user.ResetPassordToken.Trim()));


            if (!isValidToken) {
                return await Task.FromResult(("Invalid token", false));
            }
            var updatedUser = _authRepository.UpdateUserAsync(user);
            if (updatedUser != null)
            {
                //send email notification here  
                await _notifyService.SendEmailAsync(
                    to:[new EmailReceiver { Email = user.UserEmail!, Name = user.FullName! }],
                    subject: "Password Reset Successful",
                    body: EmailTemplateService.GeneratePasswordResetSuccessEmailHtml(user.FullName!)
                );
                return await Task.FromResult(("Password reset successfully", true));
            }
            return await Task.FromResult(("Failed to reset password", false));
        }

        public Task<(string message, bool status)> ChangePasswordAsync(ChangePasswordDto changePasswordDto)
        {
            throw new NotImplementedException();
        }

        public Task<(string message, bool status)> LogoutAsync(string email)
        {
            throw new NotImplementedException();
        }

        public async Task<User> RegisterAsync(CreateUserDto createUserDto)
        {
            var mapped = new User
            {
                UserEmail = createUserDto.UserEmail,
                UserName = createUserDto.UserName,
                FullName = createUserDto.FullName,
                Password = PasswordUtilService.HashPassword(createUserDto.Password!),
                CreatedById = 1, // Assuming the admin user is creating this user
                DateCreated = DateTime.Now,
                IsDefault = true, // Assuming new users are default
                UserTypeId = createUserDto.UserTypeId,
                DeptId = createUserDto.DepartmentId
            };
            var saved = await _authRepository.RegisterUserAsync(mapped);
            if (saved != null)
            {
                //send email notification here
                await _notifyService.SendEmailAsync(
                    to: [new EmailReceiver { Email = saved.UserEmail!, Name = saved.FullName! }],
                    subject: "Welcome to WuERP",
                    body: EmailTemplateService.GenerateWelcomeEmailHtml(saved.FullName!, saved.UserEmail!, saved.UserName!, createUserDto.Password!)
                );
            }
            return saved!;
        }

        public async Task<string> ForgotPasswordAsync(string email)
        {
            var user = await _authRepository.FindUserByEmailAsync(email);
            if (user == null)
            {
                return "User not found";
            }

            var random = new Random();
            var resetToken = random.Next(100000, 1000000).ToString();
            // Save the reset token to the user or database
            user.ResetPassordToken = PasswordUtilService.HashPassword(resetToken); // For demo purposes, using token as password
            await _authRepository.UpdateUserAsync(user);
            // Send reset password email
            await _notifyService.SendEmailAsync(
                to: [new EmailReceiver { Email = user.UserEmail!, Name = user.FullName! }],
                subject: "Password Reset Request",
                body: EmailTemplateService.GeneratePasswordResetTokenEmailHtml(user.FullName!, resetToken)
            );
            return "Password reset email sent successfully";
        }

        public async Task<dynamic> VerifyLoginTokenAsync(string email, string token)
        {
            var user = await _authRepository.FindUserByEmailOrUserNameAsync(email);
            if (user == null)
            {
            return new { Success = false, data = (object?)null };
            }

            // Retrieve the latest 2FA token for the user
            var mfaToken = await _authRepository.GetLatestTwoFactorTokenAsync(user.UserId);
            if (mfaToken == null)
            {
            return new { Success = false, data = (object?)null, Message = "No 2FA token found." };
            }

            // Verify the provided token against the hashed token
            var isValid = PasswordUtilService.VerifyPassword(password: token.Trim(), hashedPassword: mfaToken.Token);
            if (!isValid)
            {
            return new { Success = false, data = (object?)null, Message = "Invalid 2FA token." };
            }

            // Mark token as used/expired if needed (optional, for security)
             await _authRepository.ExpireTwoFactorTokenAsync(mfaToken.Id);

            // Proceed with login and return JWT token
            return await LoginTokenResponse(user);
        }
    }
}
