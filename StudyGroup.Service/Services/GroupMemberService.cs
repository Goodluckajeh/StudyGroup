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
    /// Implements IGroupMemberService, contains business logic for group members.
    /// Handles membership management including join requests, approvals, and status tracking.
    /// </summary>
    public class GroupMemberService : IGroupMemberService
    {
        private readonly IGroupMemberRepository _repository;
        
        /// <summary>
        /// Membership status constants. These should match the StatusId values in your MembershipStatuses table.
        /// Update these constants based on your actual database values.
        /// </summary>
        private const int PendingStatusId = 1;   // User has requested to join but not yet approved
        private const int ActiveStatusId = 2;    // User is an active member of the group
        private const int RejectedStatusId = 3;  // User's join request was rejected

        /// <summary>
        /// Initializes a new instance of the GroupMemberService.
        /// </summary>
        /// <param name="repository">The group member repository for data access</param>
        public GroupMemberService(IGroupMemberRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Maps GroupMember model (PascalCase) to GroupMemberDto (PascalCase).
        /// </summary>
        /// <param name="member">The group member model from database</param>
        /// <returns>DTO representation for API responses</returns>
        private GroupMemberDto MapToDto(GroupMember member) => new GroupMemberDto
        {
            GroupMemberId = member.GroupMemberId,
            GroupId = member.GroupId,
            UserId = member.UserId,
            StatusId = member.StatusId
        };

        #region Basic CRUD Operations

        /// <summary>
        /// Retrieves all group memberships in the system.
        /// Administrative function - use specialized endpoints for filtered results.
        /// </summary>
        /// <returns>All group memberships regardless of status</returns>
        public async Task<IEnumerable<GroupMemberDto>> GetAllGroupMembersAsync()
        {
            var members = await _repository.GetAllAsync();
            return members.Select(MapToDto);
        }

        /// <summary>
        /// Gets a specific group membership by its ID.
        /// </summary>
        /// <param name="groupMemberId">The unique identifier of the membership</param>
        /// <returns>The group membership if found, null otherwise</returns>
        public async Task<GroupMemberDto?> GetGroupMemberByIdAsync(int groupMemberId)
        {
            var member = await _repository.GetByIdAsync(groupMemberId);
            return member == null ? null : MapToDto(member);
        }

        /// <summary>
        /// Creates a new group membership directly (administrative function).
        /// For user join requests, use JoinGroupAsync instead.
        /// </summary>
        /// <param name="groupMemberDto">The membership data to create</param>
        /// <returns>The ID of the created membership</returns>
        public async Task<int> CreateGroupMemberAsync(CreateGroupMemberDto groupMemberDto)
        {
            var member = new GroupMember
            {
                GroupId = groupMemberDto.GroupId,
                UserId = groupMemberDto.UserId,
                StatusId = groupMemberDto.StatusId
            };
            return await _repository.CreateAsync(member);
        }

        /// <summary>
        /// Updates an existing group membership (administrative function).
        /// For approval workflows, use ApproveMembershipAsync/RejectMembershipAsync instead.
        /// </summary>
        /// <param name="groupMemberId">The ID of the membership to update</param>
        /// <param name="groupMemberDto">The updated membership data</param>
        /// <returns>True if updated successfully, false if not found</returns>
        public async Task<bool> UpdateGroupMemberAsync(int groupMemberId, UpdateGroupMemberDto groupMemberDto)
        {
            var member = await _repository.GetByIdAsync(groupMemberId);
            if (member == null) return false;
            member.GroupId = groupMemberDto.GroupId;
            member.UserId = groupMemberDto.UserId;
            member.StatusId = groupMemberDto.StatusId;
            return await _repository.UpdateAsync(member);
        }

        /// <summary>
        /// Deletes a group membership by ID.
        /// Consider using RemoveFromGroupAsync for better semantic clarity.
        /// </summary>
        /// <param name="groupMemberId">The ID of the membership to delete</param>
        /// <returns>True if deleted successfully, false if not found</returns>
        public async Task<bool> DeleteGroupMemberAsync(int groupMemberId)
        {
            return await _repository.DeleteAsync(groupMemberId);
        }

        #endregion

        #region Specialized Membership Management Methods

        /// <summary>
        /// Retrieves all members of a specific study group regardless of status.
        /// </summary>
        /// <param name="groupId">The ID of the study group</param>
        /// <returns>All memberships for the specified group</returns>
        public async Task<IEnumerable<GroupMemberDto>> GetGroupMembersAsync(int groupId)
        {
            var members = await _repository.GetByGroupIdAsync(groupId);
            return members.Select(MapToDto);
        }

        /// <summary>
        /// Retrieves all study groups that a specific user is associated with.
        /// </summary>
        /// <param name="userId">The ID of the user</param>
        /// <returns>All group memberships for the specified user</returns>
        public async Task<IEnumerable<GroupMemberDto>> GetUserGroupsAsync(int userId)
        {
            var memberships = await _repository.GetByUserIdAsync(userId);
            return memberships.Select(MapToDto);
        }

        /// <summary>
        /// Retrieves all pending membership requests for a specific group.
        /// Used by group admins to see who is waiting for approval.
        /// </summary>
        /// <param name="groupId">The ID of the study group</param>
        /// <returns>All pending membership requests for the group</returns>
        public async Task<IEnumerable<GroupMemberDto>> GetPendingMembersAsync(int groupId)
        {
            var pendingMembers = await _repository.GetByGroupIdAndStatusAsync(groupId, PendingStatusId);
            return pendingMembers.Select(MapToDto);
        }

        /// <summary>
        /// Retrieves only the active (approved) members of a specific group.
        /// Use this to display actual participating members.
        /// </summary>
        /// <param name="groupId">The ID of the study group</param>
        /// <returns>All active members in the group</returns>
        public async Task<IEnumerable<GroupMemberDto>> GetActiveMembersAsync(int groupId)
        {
            var activeMembers = await _repository.GetByGroupIdAndStatusAsync(groupId, ActiveStatusId);
            return activeMembers.Select(MapToDto);
        }

        /// <summary>
        /// Allows a user to request to join a study group.
        /// Creates membership with "Pending" status that requires approval.
        /// Prevents duplicate membership requests.
        /// </summary>
        /// <param name="groupId">The ID of the study group to join</param>
        /// <param name="userId">The ID of the user requesting to join</param>
        /// <returns>The ID of the created membership</returns>
        /// <exception cref="InvalidOperationException">Thrown if user already has a membership in this group</exception>
        public async Task<int> JoinGroupAsync(int groupId, int userId)
        {
            // Check if user is already a member (any status)
            var existingMembership = await _repository.GetByGroupIdAndUserIdAsync(groupId, userId);
            if (existingMembership != null)
            {
                throw new InvalidOperationException("User already has a membership in this group");
            }

            // Create new membership with "Pending" status
            var member = new GroupMember
            {
                GroupId = groupId,
                UserId = userId,
                StatusId = PendingStatusId
            };
            return await _repository.CreateAsync(member);
        }

        /// <summary>
        /// Approves a pending membership request, changing status to "Active".
        /// Only works on memberships currently in pending status.
        /// </summary>
        /// <param name="groupMemberId">The ID of the membership to approve</param>
        /// <returns>True if approved successfully, false if not found or not pending</returns>
        public async Task<bool> ApproveMembershipAsync(int groupMemberId)
        {
            var member = await _repository.GetByIdAsync(groupMemberId);
            if (member == null || member.StatusId != PendingStatusId) 
                return false;
            
            member.StatusId = ActiveStatusId;
            return await _repository.UpdateAsync(member);
        }

        /// <summary>
        /// Rejects a pending membership request, changing status to "Rejected".
        /// Only works on memberships currently in pending status.
        /// </summary>
        /// <param name="groupMemberId">The ID of the membership to reject</param>
        /// <returns>True if rejected successfully, false if not found or not pending</returns>
        public async Task<bool> RejectMembershipAsync(int groupMemberId)
        {
            var member = await _repository.GetByIdAsync(groupMemberId);
            if (member == null || member.StatusId != PendingStatusId) 
                return false;
            
            member.StatusId = RejectedStatusId;
            return await _repository.UpdateAsync(member);
        }

        /// <summary>
        /// Removes a user from a group by deleting their membership record.
        /// This provides semantic clarity over the generic DeleteGroupMemberAsync method.
        /// </summary>
        /// <param name="groupMemberId">The ID of the membership to remove</param>
        /// <returns>True if removed successfully, false if not found</returns>
        public async Task<bool> RemoveFromGroupAsync(int groupMemberId)
        {
            return await _repository.DeleteAsync(groupMemberId);
        }

        #endregion
    }
}
