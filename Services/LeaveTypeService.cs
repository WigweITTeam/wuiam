using System.Text.Json;
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

        public async Task<LeaveType> CreateLeaveType(CreateLeaveTypeDto dto)
        {
            var visibilityRules = dto.VisibilityRules.Select(v => new LeaveTypeVisibility
            {
                VisibilityType = v.VisibilityType,
                Value = v.Value
            }).ToList();

            var leaveType = new LeaveType
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                MaxDays = dto.MaxDays,
                IsPaid = dto.IsPaid,
                Description = dto.Description,
                ColorTag = dto.ColorTag,
                IsActive = dto.IsActive,
                RequireDocument = dto.RequireDocument,
                ApprovalFlowId = dto.ApprovalFlowId,
                VisibilityRules = visibilityRules
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
