using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WUIAM.Interfaces;
using WUIAM.Models;

namespace WUIAM.Services
{
    public class DepartmentService : IDepartmentService
    {
        // Example in-memory storage for departments
       WUIAMDbContext _dbContext;

        public DepartmentService(WUIAMDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<List<Department>> GetDepartmentsAsync()
        {
            return _dbContext.Departments.ToListAsync();
        }

        public async Task<Department?> GetDepartmentByIdAsync(Guid id)
        {
            return await _dbContext.Departments.FindAsync(id);
        }

        public Task<Department> CreateDepartmentAsync(Department department)
        {
            var save = _dbContext.Departments.Add(department);
            _dbContext.SaveChanges();
            return Task.FromResult(save.Entity);
        }

        public async Task<Department?> UpdateDepartmentAsync(Guid id, Department department)
        {
            var existingDepartment = await _dbContext.Departments.FindAsync(id);
            if (existingDepartment == null)
            {
            return null;
            }

            // Update properties
            existingDepartment.Name = department.Name;
            // Add other property updates as needed

            await _dbContext.SaveChangesAsync();
            return existingDepartment;
        }

        public async Task<bool> DeleteDepartmentAsync(Guid id)
        {
            var department = await _dbContext.Departments.FindAsync(id);
            if (department != null)
            {
                _dbContext.Departments.Remove(department);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }

}