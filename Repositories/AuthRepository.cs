﻿using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WUIAM.DTOs;
using WUIAM.Models;
using WUIAM.Repositories.IRepositories;

namespace WUIAM.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly WUIAMDbContext dbContext;
        public AuthRepository(WUIAMDbContext _context)
        {
            dbContext = _context;
        }

        public async Task ExpireTwoFactorTokenAsync(Guid id)
        {
            var token = await dbContext.MFATokens.FindAsync(id);
            if (token != null)
            {
                dbContext.MFATokens.Remove(token);
                await dbContext.SaveChangesAsync();
            }

        }

        public Task<User?> FindUserByEmailAsync(string Email)
        {
            return dbContext.Users.FirstOrDefaultAsync(u => u.UserEmail == Email);
        }
        public async Task<User?> FindUserByEmailOrUserNameAsync(string Email)
        {
            var found = await dbContext.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .Include(up =>up.UserPermissions)
                .ThenInclude(up =>up.Permission)
                .FirstOrDefaultAsync(u => u.UserEmail == Email || u.UserName == Email);
            return found;
        }

        public Task<User?> FindUserByIdAsync(Guid userId)
        {
            return dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<MFAToken?> GetLatestTwoFactorTokenAsync(Guid userId)
        {
            var token = await dbContext.MFATokens.FirstOrDefaultAsync(t => t.UserId == userId);
            return token;
        }

        public async Task<User> RegisterUserAsync(User user)
        {
            var s = await dbContext.Users.AddAsync(user);
            return await dbContext.SaveChangesAsync().ContinueWith(t => user);
        }

        public async Task<int> SaveTwoFactorTokenAsync(Guid userId, string twoFactorToken)
        {
            var mfaToken = new MFAToken
            {
                UserId = userId!,
                Token = twoFactorToken,
                CreatedAt = DateTime.UtcNow,
                ClientId = "Web",
                ExpiresOn = DateTime.Now.AddMinutes(5)
            };
            var fAToken = await dbContext.MFATokens.FirstOrDefaultAsync(t => t.UserId == userId!);
            if (fAToken == null)
            {
                dbContext.MFATokens.Add(mfaToken);
            }
            else
            {
                fAToken.Token = twoFactorToken;
                fAToken.CreatedAt = DateTime.UtcNow;
                dbContext.MFATokens.Update(fAToken);
            }
            var saved = await dbContext.SaveChangesAsync();
            return saved;
        }

        public Task<User> UpdateUserAsync(User user)
        {
            dbContext.Users.Update(user);
            return dbContext.SaveChangesAsync().ContinueWith(t => user);
        }
        public async Task<RefreshToken?> GetRefreshTokenAsync(string token)
        {
            return await dbContext.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == token);
        }

        public async Task<RefreshToken> CreateRefreshTokenAsync(RefreshToken refreshToken)
        {
            await dbContext.RefreshTokens.AddAsync(refreshToken);
            await dbContext.SaveChangesAsync();
            return refreshToken;
        }
        public  void ExpireRefreshTokenAsync(RefreshToken refreshToken)
        {
            dbContext.RefreshTokens.Remove(refreshToken);
        }

        public async Task<IEnumerable<UserType?>> getUserTypes()
        {
           return await dbContext.UserTypes.ToListAsync();

        }
        public async Task<IEnumerable<UserDto?>?> GetStaffListAsync()
        { var staffType = dbContext.UserTypes.FirstOrDefault(ut => ut.Name.ToLower().Contains("staff"));
            if(staffType !=null)
            return  await dbContext.Users.Where(u => u.UserTypeId == staffType.Id).Select(u =>new
          UserDto  {
                Id = u.Id,
                FullName = u.FullName,
                Email = u.UserEmail!,
                UserTypeId = u.UserTypeId,
                DepartmentId = u.DeptId.GetValueOrDefault()
            }).ToListAsync();
            return null;
        }
    }
}
