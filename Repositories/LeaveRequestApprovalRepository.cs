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

        public async Task<IEnumerable<LeaveRequestApproval?>> GetByApprovalFlowIdAndApprovalPersonId(Guid? approvalFlowId, Guid? approvalPersonId)
        {
            var result = await _context.LeaveRequestApprovals.Include(a => a.ApprovalStep)
                .Where(x => x.ApprovalStep!.ApprovalFlowId == approvalFlowId && x.ApproverPersonId == approvalPersonId).ToListAsync();
            return result;
        }

        public async Task<IEnumerable<LeaveRequestApproval>> GetByApproverPersonIdAsync(Guid approverPersonId)
        {
            var directApprovalsTask = _context.LeaveRequestApprovals
            .Include(a => a.ApprovalStep)
            .Where(x => x.ApproverPersonId == approverPersonId)
            .ToListAsync();

            var delegationStepIdsTask = _context.ApprovalDelegations
            .Where(d => d.DelegatePersonId == approverPersonId)
            .Select(d => d.ApprovalStepId)
            .ToListAsync();

            await Task.WhenAll(directApprovalsTask, delegationStepIdsTask);

            var delegatedApprovals = await _context.LeaveRequestApprovals
            .Include(a => a.ApprovalStep)
            .Where(x => delegationStepIdsTask.Result.Contains(x.ApprovalStepId))
            .ToListAsync();

            // Merge both lists, avoiding duplicates
            return directApprovalsTask.Result
            .Concat(delegatedApprovals)
            .GroupBy(x => x.Id)
            .Select(g => g.First())
            .ToList();
        }

        public async Task<IEnumerable<LeaveRequestApproval>> GetByApproverDelegationPersonIdAsync(Guid approvalDelegationPersonId)
        {
            var delegationStepIds = await _context.ApprovalDelegations
                .Where(d => d.DelegatePersonId == approvalDelegationPersonId)
                .Select(d => d.ApprovalStepId)
                .ToListAsync();

            return await _context.LeaveRequestApprovals.Include(a => a.ApprovalStep)
                .Where(x => delegationStepIds.Contains(x.ApprovalStepId))
                .ToListAsync();
        }
    }

}
