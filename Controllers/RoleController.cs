using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using WUIAM.Services;
using WUIAM.Models;
using WUIAM.Interfaces;
using WUIAM.DTOs;

namespace WUIAM.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Role>>> GetRoles()
        {
            var roles = await _roleService.GetAllRolesAsync();
            return Ok(roles);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Role>> GetRole(Guid id)
        {
            var role = await _roleService.GetRoleByIdAsync(id);
            if (role == null)
                return NotFound();
            return Ok(role);
        }

        [HttpPost]
        public async Task<ActionResult<Role>> CreateRole([FromBody] RoleCreateDto role)
        {
            var createdRole = await _roleService.CreateRoleAsync(role);
            return CreatedAtAction(nameof(GetRole), new { id = createdRole.Id }, createdRole);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRole(Guid id, [FromBody] RoleUpdateDto role)
        {
            var updated = await _roleService.UpdateRoleAsync(id, role);
            if (!updated)
                return NotFound();

            return Ok(new { Message = "Role updated successfully." });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRole(Guid id)
        {
            var deleted = await _roleService.DeleteRoleAsync(id);
            if (!deleted)
                return NotFound();

            return Ok(new { Message = "Role deleted successfully." });
        }

        [HttpPost("assign/{userId}/{roleId}")]
        public async Task<IActionResult> AssignRoleToUser(Guid userId, Guid roleId)
        {
            var assigned = await _roleService.AssignRoleToUserAsync(userId, roleId);
            if (!assigned)
                return NotFound();

            return Ok(new { Message = "Role assigned to user successfully." });
        }

        [HttpDelete("remove/{userId}/{roleId}")]
        public async Task<IActionResult> RemoveRoleFromUser(Guid userId, Guid roleId)
        {
            var removed = await _roleService.RemoveRoleFromUserAsync(userId, roleId);
            if (!removed)
                return NotFound();

            return Ok(new { Message = "Role removed from user successfully." });
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<Role>>> GetRolesForUser(Guid userId)
        {
            var roles = await _roleService.GetRolesForUserAsync(userId);
            return Ok(roles);
        }
        
        [HttpGet("users/{roleId}")]
        public async Task<ActionResult<IEnumerable<User>>> GetUsersInRole(Guid roleId)
        {
            var users = await _roleService.GetUsersInRoleAsync(roleId);
            if (users == null || users.Count == 0)
                return NotFound();

            return Ok(users);
        }
    }
}