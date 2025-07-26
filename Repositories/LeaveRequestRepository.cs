using System;
using Microsoft.EntityFrameworkCore;
using WUIAM.Models;
using WUIAM.Repositories.IRepositories;

namespace WUIAM.Repositories
{
    public class LeaveRequestRepository : ILeaveRequestRepository
    {
        private readonly WUIAMDbContext _context;

        public LeaveRequestRepository(WUIAMDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<LeaveRequest>> GetByUserIdAsync(Guid userId)
        {
            return await _context.LeaveRequests
                .Where(l => l.UserId == userId)
                .ToListAsync();
        }

        public async Task<LeaveRequest?> GetByIdAsync(Guid id)
        {
            return await _context.LeaveRequests.FirstOrDefaultAsync(l => l.Id == id);
        }

        public async Task<IEnumerable<LeaveRequest>> GetAllAsync()
        {
            return await _context.LeaveRequests.ToListAsync();
        }

        public async Task AddAsync(LeaveRequest request)
        {
            await _context.LeaveRequests.AddAsync(request);
            await _context.SaveChangesAsync();
        }


        public async Task UpdateAsync(LeaveRequest request)
        {
            _context.LeaveRequests.Update(request);
            await _context.SaveChangesAsync();
        }
    }
}
