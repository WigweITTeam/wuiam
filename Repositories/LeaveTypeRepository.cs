using System;
using WUIAM.Models; 
using Microsoft.EntityFrameworkCore;
using WUIAM.Repositories.IRepositories;

namespace WUIAM.Repositories { 
 public class LeaveTypeRepository : ILeaveTypeRepository
    {
        private readonly WUIAMDbContext _context;

        public LeaveTypeRepository(WUIAMDbContext context)
        {
            _context = context;
        }

        public async Task<List<LeaveType>> GetAllAsync()
        {
            return await _context.LeaveTypes.ToListAsync();
        }

        public async Task<LeaveType?> GetByIdAsync(Guid id)
        {
            return await _context.LeaveTypes.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task AddAsync(LeaveType leaveType)
        {
            await _context.LeaveTypes.AddAsync(leaveType);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(LeaveType leaveType)
        {
            _context.LeaveTypes.Update(leaveType);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var leaveType = await GetByIdAsync(id);
            if (leaveType != null)
            {
                _context.LeaveTypes.Remove(leaveType);
                await _context.SaveChangesAsync();
            }
        }
    }
}
