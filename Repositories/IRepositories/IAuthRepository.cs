using Microsoft.EntityFrameworkCore;
using WUIAM.DTOs;
using WUIAM.Models;

namespace WUIAM.Repositories.IRepositories
{
    public interface IAuthRepository
    {
        Task ExpireTwoFactorTokenAsync(object id);
        public Task<User?> FindUserByEmailAsync(string Email);

        public Task<User?> FindUserByEmailOrUserNameAsync(string Email);
        public Task<User?> FindUserByIdAsync(int userId);    
        public Task<MFAToken?> GetLatestTwoFactorTokenAsync(int userId);
        public Task<User> RegisterUserAsync(User user);
        public Task<int> SaveTwoFactorTokenAsync(int userId, string twoFactorToken);
        public Task<User> UpdateUserAsync(User user);


    }
}
