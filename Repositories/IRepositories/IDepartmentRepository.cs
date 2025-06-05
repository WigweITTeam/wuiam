using System.Collections.Generic;
using System.Threading.Tasks;
using WUIAM.Models;

namespace WUIAM.Repositories.IRepositories
{
    public interface IDepartmentRepository
    {
        Task<IEnumerable<Department>> GetAllAsync();
        Task<Department> GetByIdAsync(int id);
        Task AddAsync(Department department);
        Task UpdateAsync(Department department);
        Task DeleteAsync(int id);
    }
}