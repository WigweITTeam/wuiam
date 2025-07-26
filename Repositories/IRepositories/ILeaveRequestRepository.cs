using WUIAM.Models;

namespace WUIAM.Repositories.IRepositories
{
    public interface ILeaveRequestRepository
    {
        Task<IEnumerable<LeaveRequest>> GetByUserIdAsync(Guid userId);
        Task<LeaveRequest?> GetByIdAsync(Guid id);

        Task<IEnumerable<LeaveRequest>> GetAllAsync();
        Task AddAsync(LeaveRequest request);

        Task UpdateAsync(LeaveRequest request);
    }
}
