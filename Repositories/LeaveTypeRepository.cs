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
            var leaveTypes = await _context.LeaveTypes
                .AsSplitQuery() // avoids cartesian explosion with multiple includes
                .Include(l => l.VisibilityRules)
                .Include(l => l.ApprovalFlow)
                    .ThenInclude(af => af.Steps)
                .ToListAsync(); 
            foreach (var lt in leaveTypes)
            {
                if (lt.ApprovalFlow?.Steps != null)
                {
                    lt.ApprovalFlow.Steps = lt.ApprovalFlow.Steps
                        .OrderBy(s => s.StepOrder)  
                        .ToList();  
                }
            }

            return leaveTypes;
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

        public   async Task<LeaveTypeVisibility> getLeaveTypeVisibiltyById(Guid id)
        {
           var found = await _context.LeaveTypeVisibilities.FindAsync(id);
            return found;
        }
        public async Task<List<LeaveType>> GetVisibleLeaveTypesForUser(Guid userId)
        {
            var user = await _context.Users.FindAsync(userId);
            var leaveTypes = await _context.LeaveTypes
                .Include(lt => lt.VisibilityRules)
                .ToListAsync();

            return leaveTypes.Where(lt =>
            {
                // If no visibility rules, visible to all
                if (lt.VisibilityRules == null || lt.VisibilityRules.Count == 0) return true;

                // At least one rule must match
                return lt.VisibilityRules.Any(rule =>
                {
                    var type = rule.VisibilityType.ToUpper();

                    return type switch
                    {
                        "ROLE" => user.UserRoles.Any(ur => ur.Role.Name == rule.Value),
                        "DEPARTMENT" => user.Department != null && user.Department.Name == rule.Value,
                        "EMPLOYMENT_TYPE" => user.EmploymentType != null && user.EmploymentType.Name == rule.Value,
                        "USER_TYPE" => user.UserType != null && user.UserType.Name == rule.Value,
                        _ => false
                    };
                });
            }).ToList();
        }
        public async Task<bool> MatchesVisibility(User user, LeaveType leaveType)
        {
            var userLeaveVisibility = await GetVisibleLeaveTypesForUser(user.Id);
            if (userLeaveVisibility != null)
            {
                return userLeaveVisibility.Any(a => a == leaveType);
            }
            return false;
        }

    }
}
