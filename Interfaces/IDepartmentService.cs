 
using System.Collections.Generic;
using System.Threading.Tasks;
using WUIAM.Models;

namespace WUIAM.Interfaces
{
    public interface IDepartmentService
    {
        Task<List<Department>> GetDepartmentsAsync();
        Task<Department?> GetDepartmentByIdAsync(Guid id);
        Task<Department> CreateDepartmentAsync(Department department);
        Task<Department?> UpdateDepartmentAsync(Guid id, Department department);
        Task<bool> DeleteDepartmentAsync(Guid id);
    }

    // Assuming Department class exists elsewhere in your project
    // public class Department { ... }
}