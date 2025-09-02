using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using WUIAM.DTOs;
using WUIAM.Enums;
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
        private readonly IRoleService _roleService;
        private readonly string _jwtSecret;
        private readonly string _jwtIssuer;
        private readonly string _jwtAudience;
        IHttpContextAccessor _context;
        public AuthService(IAuthRepository authRepository, INotifyService notifyService,
        IRoleService roleService, IConfiguration configuration,
         IHttpContextAccessor httpContextAccessor)
        {
            _authRepository = authRepository;
            _notifyService = notifyService;
            _configuration = configuration;
            _roleService = roleService;
            _context = httpContextAccessor;
            _jwtSecret = _configuration["Jwt:Key"]!;
            _jwtIssuer = _configuration["Jwt:Issuer"]!;
            _jwtAudience = _configuration["Jwt:Audience"]!;
        }


        public async Task<dynamic> LoginAsync(LoginDto loginDto)
        {
            var user = await _authRepository.FindUserByEmailOrUserNameAsync(loginDto.Email);
            Console.WriteLine("user email address: " + loginDto.ToString(), user?.ToString());
            if (user == null)
            {
                return new { Success = false, data = (object?)null, Message = "Invalid username or password!" };
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
                var savedToken = await _authRepository.SaveTwoFactorTokenAsync(user.Id, PasswordUtilService.HashPassword(twoFactorToken));

                await _notifyService.SendEmailAsync(
                    to: [new EmailReceiver { Email = user.UserEmail!, Name = user.FullName! }],
                    subject: "Two-Factor Authentication Token",
                    body: EmailTemplateService.GenerateTwoFactorTokenEmailHtml(user.FullName!, twoFactorToken)
                );
                user.Password = null;
                return new { Success = true, data = user, verifyToken = true, Message = "You have 2FA enable. A login verification email has being sent. Verify login to continue!" };
            }
            // If 2FA is not enabled, proceed with normal login
            //if (user.IsDefault == false)
            //{
            //    return new { Success = false, data = (object?)null };
            //}
            return await LoginTokenResponse(user);
        }

        private async Task<dynamic> LoginTokenResponse(User user, bool sendEmail = true, bool generateRefToken = true)
        {
            var token = GenerateJwtToken(user);

            user.SessionId = Guid.NewGuid().ToString();
            user.SessionTime = DateTime.Now;
            user.DateLastLoggedIn = DateTime.Now;
            await _authRepository.UpdateUserAsync(user);
            if (sendEmail)
            {
                // Send login notification email
                await _notifyService.SendEmailAsync(
                    to: [new EmailReceiver { Email = user.UserEmail!, Name = user.FullName! }],
                    subject: "Login Notification",
                    body: EmailTemplateService.GenerateLoginNotificationEmailHtml(user.FullName!, DateTime.Now)
                );
                await _authRepository.ExpireTwoFactorTokenAsync(user.Id);
            }
            if (generateRefToken)
            {
                var refreshToken = await CreateRefreshTokenAsync(user);
                return new { Success = true, data = user, token, refreshToken = refreshToken.Token, Message = "Login successful!" };

            }
            user.Password = null;
            return new { Success = true, data = user, token, Message = "Login successful!" };
        }

        private string GenerateJwtToken(User user)
        {
            var claims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                    new Claim(JwtRegisteredClaimNames.Email, user.UserEmail ?? ""),
                    new Claim("id", user.Id.ToString())
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
                expires: DateTime.UtcNow.AddMinutes(1), // Token expiration in 15 minutes time
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

            // Ensure ResetPassordToken is not null before accessing it  
            if (string.IsNullOrWhiteSpace(user.ResetPasswordToken))
            {
                return await Task.FromResult(("Invalid token", false));
            }

            user.Password = PasswordUtilService.HashPassword(resetPasswordDTo.NewPassword.ToString());
            bool isValidToken = PasswordUtilService.VerifyPassword(password: resetPasswordDTo.ResetToken.ToString().Trim(), hashedPassword: user.ResetPasswordToken.Trim());

            if (!isValidToken)
            {
                return await Task.FromResult(("Invalid token", false));
            }

            var updatedUser = _authRepository.UpdateUserAsync(user);
            if (updatedUser != null)
            {
                // Send email notification here  
                await _notifyService.SendEmailAsync(
                    to: [new EmailReceiver { Email = user.UserEmail!, Name = user.FullName! }],
                    subject: "Password Reset Successful",
                    body: EmailTemplateService.GeneratePasswordResetSuccessEmailHtml(user.FullName!)
                );
                return await Task.FromResult(("Password reset successfully", true));
            }
            return await Task.FromResult(("Failed to reset password", false));
        }

        public async Task<(string message, bool status)> ChangePasswordAsync(ChangePasswordDto changePasswordDto)
        {
            // Step 1: Find user by email
            var user = await _authRepository.FindUserByEmailAsync(changePasswordDto.Email);
            if (user == null)
            {
                return ("User not found", false);
            }

            // Step 2: Verify old password
            var isOldPasswordValid = PasswordUtilService.VerifyPassword(password: changePasswordDto.OldPassword.Trim(), hashedPassword: user.Password!);
            if (!isOldPasswordValid)
            {
                return ("Old password is incorrect", false);
            }

            // Step 3: Check new password and confirmation match
            if (changePasswordDto.NewPassword != changePasswordDto.ConfirmPassword)
            {
                return ("New password and confirmation do not match", false);
            }

            // Step 4: Update password
            user.Password = PasswordUtilService.HashPassword(changePasswordDto.NewPassword);
            user.IsDefault = false;
            await _authRepository.UpdateUserAsync(user);

            // Step 5: Send notification email
            await _notifyService.SendEmailAsync(
                to: [new EmailReceiver { Email = user.UserEmail!, Name = user.FullName! }],
                subject: "Password Changed Successfully",
                body: EmailTemplateService.GeneratePasswordResetSuccessEmailHtml(user.FullName!)
            );

            return ("Password changed successfully", true);
        }

        public Task<(string message, bool status)> LogoutAsync(string email)
        {
            throw new NotImplementedException();
        }

        public async Task<User> RegisterAsync(CreateUserDto createUserDto)
        {
            var mapped = new User
            {
                UserEmail = createUserDto.UserEmail!,
                UserName = createUserDto.UserName,
                FirstName = createUserDto.FirstName,
                LastName = createUserDto.LastName,
                Password = PasswordUtilService.HashPassword(createUserDto.Password!),
                CreatedById = Guid.NewGuid(), // Assuming the admin user is creating this user
                DateCreated = DateTime.Now,
                IsDefault = true, // Assuming new users are default
                UserTypeId = createUserDto.UserTypeId,
                DeptId = createUserDto.DepartmentId,
                EmploymentTypeId = createUserDto.EmploymentTypeId
            };
            var saved = await _authRepository.RegisterUserAsync(mapped);
            if (saved != null)
            {
                //assign default role

                Claim? userIdClaim = _context.HttpContext.User?.FindFirst(ClaimTypes.NameIdentifier);
                var staffRole = (await _roleService.GetAllRolesAsync()).FirstOrDefault(a => a.Name.ToLower().Contains("staff"));
                if (staffRole != null)
                {
                    Guid assignedBy;
                    if (!Guid.TryParse(userIdClaim.Value, out assignedBy))
                    {
                        throw new UnauthorizedAccessException("Invalid or missing user ID claim.");

                    }
                    else
                    {
                        var urole = new UserRole
                        {
                            UserId = saved.Id,
                            RoleId = staffRole.Id,
                            AssignedAt = DateTime.UtcNow,
                            AssignedBy = assignedBy
                        };
                        await _roleService.AssignRoleToUserAsync(saved.Id, staffRole.Id);

                    }
                }
                //send email notification here
                await _notifyService.SendEmailAsync(
                    to: [new EmailReceiver { Email = saved.UserEmail!, Name = saved.FullName! }],
                    subject: "Welcome to WuERP",
                    body: EmailTemplateService.GenerateWelcomeEmailHtml(saved.FullName!, saved.UserEmail!, saved.UserName!, createUserDto.Password!)
                );
            }
            saved.Password = null;
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
            user.ResetPasswordToken = PasswordUtilService.HashPassword(resetToken); // For demo purposes, using token as password
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
            var mfaToken = await _authRepository.GetLatestTwoFactorTokenAsync(user.Id);
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

        public Task<RefreshToken> CreateRefreshTokenAsync(User user)
        {
            RefreshToken refreshToken = PasswordUtilService.GenerateRefreshToken(user);
            var savedRefToken = _authRepository.CreateRefreshTokenAsync(refreshToken);
            return savedRefToken;
        }
        public async Task<dynamic> GetRefreshTokenAsync(string refreshToken)
        {
            var token = await _authRepository.GetRefreshTokenAsync(refreshToken)
                ?? throw new InvalidOperationException("Refresh token not found.");

            if (token.IsExpired)
                throw new InvalidOperationException("Refresh token has expired.");

            var user = await _authRepository.FindUserByIdAsync(token.UserId) ?? throw new InvalidOperationException("User not found for this refresh token.");
            //_authRepository.ExpireRefreshTokenAsync(token);
            return await LoginTokenResponse(user, false, false);
        }

        public async Task<dynamic> getUserTypes()
        {
            Claim? userIdClaim = _context.HttpContext.User?.FindFirst(ClaimTypes.NameIdentifier);

            var userTypes = await _authRepository.getUserTypes();
            if (userTypes == null)
            {
                return new { Message = "No user type registered!", Status = false };
            }
            return new { Message = "user types found", Status = true, data = userTypes };
        }
        public async Task<IEnumerable<EmploymentType>> GetEmploymentTypes()
        {
            var employmentTypes = await _authRepository.GetEmploymentTypes();
            return employmentTypes;
        }


        public async Task<IEnumerable<UserDto?>?> GetStaffListAsync()
        {
            return await _authRepository.GetStaffListAsync();


        }

        public async Task<ApiResponse<UserType>> CreateUserTypeAsync(UserTypeDto request)
        {
            var userType = new UserType
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Description = request.Description,
                IsActive = request.IsActive
            };

            var result = await _authRepository.CreateUserTypeAsync(userType);
            if (result == null)
            {
                return ApiResponse<UserType>.Failure("Failed to create user type");
            }
            return ApiResponse<UserType>.Success("User Type created successfully!", result);
        }

        public async Task<ApiResponse<EmploymentType>> CreateEmploymentTypeAsync(EmploymentType request)
        {
            var employmentType = new EmploymentType
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Description = request.Description,
                IsActive = request.IsActive
            };
            var result = await _authRepository.CreateEmploymentTypeAsync(employmentType);
            if (result == null)
            {
                return ApiResponse<EmploymentType>.Failure("Failed to create employment type");
            }
            return ApiResponse<EmploymentType>.Success("Employment Type created successfully!", result);
        }
    }
}