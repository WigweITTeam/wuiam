using Microsoft.EntityFrameworkCore;
using WUIAM.DTOs;
using WUIAM.Models;

namespace WUIAM.Repositories.IRepositories
{
    public interface IAuthRepository
    {
        Task ExpireTwoFactorTokenAsync(Guid id);
        public Task<User?> FindUserByEmailAsync(string Email);

        public Task<User?> FindUserByEmailOrUserNameAsync(string Email);
        public Task<User?> FindUserByIdAsync(Guid userId);
        public Task<MFAToken?> GetLatestTwoFactorTokenAsync(Guid userId);
        public Task<User> RegisterUserAsync(User user);
        public Task<int> SaveTwoFactorTokenAsync(Guid userId, string twoFactorToken);
        public Task<User> UpdateUserAsync(User user);
        public Task<RefreshToken?> GetRefreshTokenAsync(string token);
        public Task<RefreshToken> CreateRefreshTokenAsync(RefreshToken refreshToken);
        void ExpireRefreshTokenAsync(RefreshToken token);
    }
}
