using Microsoft.AspNetCore.Mvc;
using WUIAM.DTOs;
using WUIAM.Interfaces;
using WUIAM.Models;

namespace WUIAM.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LeaveApprovalsController : ControllerBase
    {
        ILeaveApprovalService _leaveApprovalService;
        public LeaveApprovalsController(ILeaveApprovalService leaveService)
        {
            _leaveApprovalService = leaveService;
        }

        // GET: /api/leave/get-leave-request-approvals
        [HttpGet("get-leave-request-approvals")]
        public async Task<ActionResult<ApiResponse<IEnumerable<LeaveRequestApproval>>>> GetLeaveRequestApprovals(Guid leaveRequestId)
        {
            var result = await _leaveApprovalService.GetLeaveRequestApprovals(leaveRequestId);
            if (result.Status)
                return Ok(result);
            return BadRequest(result);
        }

        // GET: /api/leave/get-leave-request-approvals-by-approver
        [HttpGet("get-by-approver-person/{approverId}")]
        public async Task<ActionResult<ApiResponse<IEnumerable<LeaveRequestApproval>>>> GetLeaveRequestApprovalsByApprover(Guid approverId)
        {
            var result = await _leaveApprovalService.GetByApproverPersonId(approverId);
            if (result.Status)
                return Ok(result);
            return BadRequest(result);
        }

        // GET: /api/leave/get-by-approver-delegation-person/{delegationId}
        [HttpGet("get-by-approver-delegation-person/{delegationId}")]
        public async Task<ActionResult<ApiResponse<IEnumerable<LeaveRequestApproval>>>> GetLeaveRequestApprovalsByDelegation(Guid delegationId)
        {
            var result = await _leaveApprovalService.GetByApproverDelegationPersonId(delegationId);
            if (result.Status)
                return Ok(result);
            return BadRequest(result);
        }
        // GET: /api/leave/get-by-approval-flow-and-step
        [HttpGet("get-by-approval-flow-and-step/{approvalFlowId}/{stepOrder}")]
        public async Task<ActionResult<ApiResponse<LeaveRequestApproval>>> GetByStepOrderAndApprovalFlowIdAsync(Guid approvalFlowId, int stepOrder)
        {
            var result = await _leaveApprovalService.GetByStepOrderAndApprovalFlowIdAsync(approvalFlowId, stepOrder);
            if (result != null && result.Status)
                return Ok(result);
            return BadRequest(result);
        }
        // GET: /api/leave/get-by-approval-flow-and-person
        [HttpGet("get-by-approval-flow-and-person/{approvalFlowId}/{personId}")]
        public async Task<ActionResult<ApiResponse<LeaveRequestApproval>>> GetByApprovalFlowAndPersonIdAsync(Guid approvalFlowId, Guid personId)
        {
            var result = await _leaveApprovalService.GetByApprovalFlowIdAndApprovalPersonId(approvalFlowId, personId);
            if (result != null && result.Status)
                return Ok(result);
            return BadRequest(result);
        }
    }
}
