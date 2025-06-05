using System.Collections.Generic;
using System.Threading.Tasks;
using WUIAM.Models;

namespace WUIAM.Repositories.IRepositories
{
    public interface IRoleRepository
    {
        Task<List<Role>> GetAllRolesAsync();
        Task<Role> GetRoleByIdAsync(int roleId);
        Task<Role> GetRoleByNameAsync(string roleName);
        Task<Role?> AddRoleAsync(Role role);
        Task<Role?> UpdateRoleAsync(Role role);
        Task<bool> DeleteRoleAsync(int roleId);
        Task<bool> RoleExistsAsync(int roleId);
        Task<List<User>> GetUsersInRoleAsync(int roleId);
        Task<bool> AssignUserToRoleAsync(int userId, int roleId);
        Task<bool> RemoveUserFromRoleAsync(int userId, int roleId);
        Task<List<Role>> GetRolesForUserAsync(int userId);
    }
}