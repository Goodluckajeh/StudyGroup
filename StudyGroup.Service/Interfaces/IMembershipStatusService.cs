using System.Collections.Generic;
using System.Threading.Tasks;
using StudyGroup.Service.DTOs;

namespace StudyGroup.Service.Interfaces
{
    // Interface for MembershipStatus service, defines business logic methods
    public interface IMembershipStatusService
    {
        Task<IEnumerable<MembershipStatusDto>> GetAllStatusesAsync();
        Task<MembershipStatusDto?> GetStatusByIdAsync(int statusId);
        Task<int> CreateStatusAsync(CreateMembershipStatusDto statusDto);
        Task<bool> UpdateStatusAsync(int statusId, UpdateMembershipStatusDto statusDto);
        Task<bool> DeleteStatusAsync(int statusId);
    }
}
