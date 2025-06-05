using System.Collections.Generic;
using System.Threading.Tasks;
using WUIAM.DTOs;
using WUIAM.Models;

namespace WUIAM.Interfaces
{
    public interface IRoleService
    {
        Task<List<Role>> GetAllRolesAsync();
        Task<Role> GetRoleByIdAsync(int roleId);
        Task<Role> CreateRoleAsync(RoleCreateDto roleCreateDto);
        Task<bool> UpdateRoleAsync(int roleId, RoleUpdateDto roleUpdateDto);
        Task<bool> DeleteRoleAsync(int roleId);
        Task<bool> AssignRoleToUserAsync(int userId, int roleId);
        Task<bool> RemoveRoleFromUserAsync(int userId, int roleId);
        Task<List<Role>> GetRolesForUserAsync(int userId);
        Task<List<User>> GetUsersInRoleAsync(int roleId);
    }

}