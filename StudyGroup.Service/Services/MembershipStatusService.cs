using System.Collections.Generic;
using System.Threading.Tasks;
using StudyGroup.Data.Models;
using StudyGroup.Data.Interfaces;
using StudyGroup.Service.Interfaces;
using StudyGroup.Service.DTOs;
using System.Linq;

namespace StudyGroup.Service.Services
{
    /// <summary>
    /// Implements IMembershipStatusService, contains business logic for membership statuses.
    /// Membership statuses are lookup/enum data (Pending, Active, Rejected, etc.)
    /// The API controller exposes only read operations, but full CRUD is available for admin tools.
    /// </summary>
    public class MembershipStatusService : IMembershipStatusService
    {
        private readonly IMembershipStatusRepository _repository;

        /// <summary>
        /// Initializes a new instance of the MembershipStatusService.
        /// </summary>
        /// <param name="repository">The membership status repository for data access</param>
        public MembershipStatusService(IMembershipStatusRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Maps MembershipStatus model (PascalCase) to MembershipStatusDto (PascalCase).
        /// </summary>
        /// <param name="status">The membership status model from database</param>
        /// <returns>DTO representation for API responses</returns>
        private MembershipStatusDto MapToDto(MembershipStatus status) => new MembershipStatusDto
        {
            StatusId = status.StatusId,
            StatusName = status.StatusName
        };

        /// <summary>
        /// Retrieves all membership status types.
        /// Used by API for dropdowns, filtering, and status display.
        /// </summary>
        /// <returns>All available membership status types</returns>
        public async Task<IEnumerable<MembershipStatusDto>> GetAllStatusesAsync()
        {
            var statuses = await _repository.GetAllAsync();
            return statuses.Select(MapToDto);
        }

        /// <summary>
        /// Gets a specific membership status by its ID.
        /// Used by API to get display names for status IDs.
        /// </summary>
        /// <param name="statusId">The unique identifier of the status</param>
        /// <returns>The membership status if found, null otherwise</returns>
        public async Task<MembershipStatusDto?> GetStatusByIdAsync(int statusId)
        {
            var status = await _repository.GetByIdAsync(statusId);
            return status == null ? null : MapToDto(status);
        }

        /// <summary>
        /// Creates a new membership status.
        /// NOTE: Not exposed via API - only for admin tools or seeding scripts.
        /// </summary>
        /// <param name="statusDto">The status data to create</param>
        /// <returns>The ID of the created status</returns>
        public async Task<int> CreateStatusAsync(CreateMembershipStatusDto statusDto)
        {
            var status = new MembershipStatus
            {
                StatusName = statusDto.StatusName
            };
            return await _repository.CreateAsync(status);
        }

        /// <summary>
        /// Updates an existing membership status.
        /// NOTE: Not exposed via API - only for admin tools or migration scripts.
        /// </summary>
        /// <param name="statusId">The ID of the status to update</param>
        /// <param name="statusDto">The updated status data</param>
        /// <returns>True if updated successfully, false if not found</returns>
        public async Task<bool> UpdateStatusAsync(int statusId, UpdateMembershipStatusDto statusDto)
        {
            var status = await _repository.GetByIdAsync(statusId);
            if (status == null) return false;
            status.StatusName = statusDto.StatusName;
            return await _repository.UpdateAsync(status);
        }

        /// <summary>
        /// Deletes a membership status by ID.
        /// NOTE: Not exposed via API - only for admin tools. Use with caution as this may affect existing memberships.
        /// </summary>
        /// <param name="statusId">The ID of the status to delete</param>
        /// <returns>True if deleted successfully, false if not found</returns>
        public async Task<bool> DeleteStatusAsync(int statusId)
        {
            return await _repository.DeleteAsync(statusId);
        }
    }
}
