using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using WUIAM.Services;
using WUIAM.Models;
using WUIAM.Interfaces;
using WUIAM.DTOs;
using WUIAM.Enums;
using Microsoft.AspNetCore.Authorization;

namespace WUIAM.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [HasPermission([Permissions.ManagePermissions,Permissions.AdminAccess])]
    public class PermissionController : ControllerBase
    {
        private readonly IPermissionService _permissionService;

        public PermissionController(IPermissionService permissionService)
        {
            _permissionService = permissionService;
        }

        // GET: api/Permission

         
        [HttpGet]
       
        public async Task<ActionResult> GetAll()
        {
            var permissions = await _permissionService.GetAllPermissionsAsync();
            //return Ok(permissions);
            if (permissions !=null)
                 return Ok(ApiResponse<IEnumerable<Permission>>.Success("Permissions found!",permissions));

            return Ok(ApiResponse<IEnumerable<Permission>>.Success("No Permission found!", permissions));
        }

        // GET: api/Permission/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Permission>> GetById(Guid id)
        {
            var permission = await _permissionService.GetPermissionByIdAsync(id);
            if (permission == null)
                return NotFound();
            return Ok(permission);
        }

        // POST: api/Permission
        [HttpPost]
        public async Task<ActionResult<Permission>> Create([FromBody] PermissionDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _permissionService.AddPermissionAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        // PUT: api/Permission/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] PermissionDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updated = await _permissionService.UpdatePermissionAsync(id, dto);
            if (updated == null)
                return NotFound();

            return Ok(updated);
        }

        // DELETE: api/Permission/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _permissionService.DeletePermissionAsync(id);
            if (!deleted)
                return NotFound();

            return NoContent();
        }
        // POST: api/Permission/GrantRolePermission
        [HttpPost("GrantRolePermission")]
        public async Task<IActionResult> GrantRolePermission([FromBody] GrantRolePermissionDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _permissionService.GrantPermissionToRoleAsync(dto.RoleId, dto.Permission);
            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result);
        }
        // POST: api/Permission/RevokeRolePermission
        [HttpPost("RevokeRolePermission")]
        public async Task<IActionResult> RevokeRolePermission([FromBody] GrantRolePermissionDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _permissionService.RevokePermissionFromRoleAsync(dto.RoleId, dto.Permission);
            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result);
        }
        // POST: api/Permission/HasRolePermission
        [HttpPost("RoleHasPermission")]
        public async Task<IActionResult> RoleHasPermission([FromBody] GrantRolePermissionDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _permissionService.RoleHasPermissionAsync(dto.RoleId, dto.Permission);
            if (!result)
                return NotFound();

            return Ok(result);
        }
        // POST: api/Permission/GrantPermission
        [HttpPost("GrantPermissionToUser")]
        public async Task<IActionResult> GrantPermissionToUser([FromBody] GrantPermissionDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _permissionService.GrantPermissionToUserAsync(dto.UserId, dto.Permission);
            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result);
        }
        // POST: api/Permission/RevokePermission
        [HttpPost("RevokePermissionFromUser")]
        public async Task<IActionResult> RevokePermissionFromUser([FromBody] GrantPermissionDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _permissionService.RevokePermissionFromUserAsync(dto.UserId, dto.Permission);
            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result);
        }
        // POST: api/Permission/HasPermission
        [HttpPost("UserHasPermission")]
        public async Task<IActionResult> UserHasPermission([FromBody] GrantPermissionDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _permissionService.UserHasPermissionAsync(dto.UserId, dto.Permission);
            if (!result)
                return NotFound();

            return Ok(result);
        }

    }
}