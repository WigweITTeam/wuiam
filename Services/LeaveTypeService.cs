using WUIAM.DTOs;
using WUIAM.Interfaces;
using WUIAM.Models;
using WUIAM.Repositories.IRepositories;

namespace WUIAM.Services
{
    public class LeaveTypeService : ILeaveTypeService
    {
        private readonly ILeaveTypeRepository _leaveTypeRepository;
        public LeaveTypeService(ILeaveTypeRepository leaveTypeRepository)
        {
            _leaveTypeRepository = leaveTypeRepository;
        }

        public async Task<LeaveType> CreateLeaveType(CreateLeaveTypeDto createLeaveTypeDto)
        {
            var leaveType = new LeaveType
            {
                Id = Guid.NewGuid(),
                Name = createLeaveTypeDto.Name,
                MaxDays = createLeaveTypeDto.MaxDays,
                IsPaid = createLeaveTypeDto.IsPaid, 
                ApprovalFlowId=createLeaveTypeDto.ApprovalFlowId,
                
            };
            await _leaveTypeRepository.AddAsync(leaveType);
            return leaveType;
        }

        public async Task<LeaveType> DeleteLeaveType(Guid leaveTypeId)
        {
            var leaveType = await _leaveTypeRepository.GetByIdAsync(leaveTypeId);
            if (leaveType == null)
                throw new KeyNotFoundException($"LeaveType with ID {leaveTypeId} not found.");
            await _leaveTypeRepository.DeleteAsync(leaveTypeId);
            return leaveType;
        }

        public async Task<IEnumerable<LeaveType>> GetAllLeaveTypes()
        {
            return await _leaveTypeRepository.GetAllAsync();
        }

        public async Task<LeaveType> GetLeaveTypeById(Guid leaveTypeId)
        {
            var leaveType = await _leaveTypeRepository.GetByIdAsync(leaveTypeId);
            if (leaveType == null)
                throw new KeyNotFoundException($"LeaveType with ID {leaveTypeId} not found.");
            return leaveType;
        }

        public async Task<LeaveType> UpdateLeaveType(LeaveType leaveType)
        {
            var existing = await _leaveTypeRepository.GetByIdAsync(leaveType.Id);
            if (existing == null)
                throw new KeyNotFoundException($"LeaveType with ID {leaveType.Id} not found.");

            existing.Name = leaveType.Name;
            existing.MaxDays = leaveType.MaxDays;
            existing.IsPaid = leaveType.IsPaid;
            existing.ApprovalFlowId = leaveType.ApprovalFlowId;

            await _leaveTypeRepository.UpdateAsync(existing);
            return existing;
        }

         
    }
}
