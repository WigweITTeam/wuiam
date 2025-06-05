using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using WUIAM.Services; // Adjust namespace as needed
using WUIAM.Models;
using WUIAM.Interfaces;
using WUIAM.DTOs;   // Adjust namespace as needed

namespace WUIAM.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DepartmentController : ControllerBase
    {
        private readonly IDepartmentService _departmentService;

        public DepartmentController(IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }

        // GET: api/Department
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Department>>> GetAll()
        {
            var departments = await _departmentService.GetDepartmentsAsync();
            return Ok(departments);
        }

        // GET: api/Department/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Department>> GetById(int id)
        {
            var department = await _departmentService.GetDepartmentByIdAsync(id);
            if (department == null)
                return NotFound();
            return Ok(department);
        }

        // POST: api/Department
        [HttpPost]
        public async Task<ActionResult<Department>> Create([FromBody] CreateDepartmentDto createDept)
        {
            var department = new Department(
            name: createDept.Name,
            description: createDept.Description,
            headOfDepartmentId: createDept.HeadOfDepartmentId
            );
            
            var created = await _departmentService.CreateDepartmentAsync(department);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        // PUT: api/Department/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Department department)
        {
            if (id != department.Id)
                return BadRequest();

            var updated = await _departmentService.UpdateDepartmentAsync(id,department);
            if (updated==null)
                return NotFound();

            return NoContent();
        }

        // DELETE: api/Department/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _departmentService.DeleteDepartmentAsync(id);
            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}