using WUIAM.Models;

namespace WUIAM.Repositories.IRepositories
{
    public interface ILeaveTypeRepository
    {
        Task<List<LeaveType>> GetAllAsync();
        Task<LeaveType?> GetByIdAsync(Guid id);

        Task AddAsync(LeaveType leaveType);

        Task UpdateAsync(LeaveType leaveType);

        Task DeleteAsync(Guid id);
    }
}
