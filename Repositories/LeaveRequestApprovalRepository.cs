using System;
using Microsoft.EntityFrameworkCore;
using WUIAM.Models;
using WUIAM.Repositories.IRepositories;

namespace WUIAM.Repositories
{
    public class LeaveRequestApprovalRepository : ILeaveRequestApprovalRepository
    {
        private readonly WUIAMDbContext _context;

        public LeaveRequestApprovalRepository(WUIAMDbContext context)
        {
            _context = context;
        }

        public async Task<LeaveRequestApproval?> GetByIdAsync(Guid id)
        {
            return await _context.LeaveRequestApprovals.Include(a => a.ApprovalStep).FirstOrDefaultAsync(x => x.Id == id);
        }
        public async Task<List<LeaveRequestApproval>> GetByLeaveRequestIdAsync(Guid leaveRequestId)
        {
            return await _context.LeaveRequestApprovals.Include(a => a.ApprovalStep)
                .Where(x => x.LeaveRequestId == leaveRequestId)
                .ToListAsync();
        }

        public async Task AddAsync(LeaveRequestApproval approval)
        {
            await _context.LeaveRequestApprovals.AddAsync(approval);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(LeaveRequestApproval approval)
        {
            _context.LeaveRequestApprovals.Update(approval);
            await _context.SaveChangesAsync();
        }

        public Task<LeaveRequestApproval?> GetByStepOrderAndApprovalFlowIdAsync(Guid approvalFlowId, int stepOrder)
        {
            return _context.LeaveRequestApprovals.Include(a => a.ApprovalStep)
                .FirstOrDefaultAsync(x => x.ApprovalStep!.ApprovalFlowId == approvalFlowId && x.ApprovalStep.StepOrder == stepOrder);
        }

        public Task<LeaveRequestApproval?> GetByApprovalFlowIdAndApprovalPersonId(Guid? approvalFlowId, Guid? approvalPersonId)
        {
            return _context.LeaveRequestApprovals.Include(a => a.ApprovalStep)
                .FirstOrDefaultAsync(x => x.ApprovalStep!.ApprovalFlowId == approvalFlowId && x.ApproverPersonId == approvalPersonId);
        }
    }

}
