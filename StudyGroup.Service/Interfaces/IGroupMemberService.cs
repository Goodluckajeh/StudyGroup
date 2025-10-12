using System.Collections.Generic;
using System.Threading.Tasks;
using StudyGroup.Service.DTOs;

namespace StudyGroup.Service.Interfaces
{
    // Interface for GroupMember service, defines business logic methods
    public interface IGroupMemberService
    {
        // Basic CRUD operations
        Task<IEnumerable<GroupMemberDto>> GetAllGroupMembersAsync();
        Task<GroupMemberDto?> GetGroupMemberByIdAsync(int groupMemberId);
        Task<int> CreateGroupMemberAsync(CreateGroupMemberDto groupMemberDto);
        Task<bool> UpdateGroupMemberAsync(int groupMemberId, UpdateGroupMemberDto groupMemberDto);
        Task<bool> DeleteGroupMemberAsync(int groupMemberId);

        // Specialized membership management methods
        Task<IEnumerable<GroupMemberDto>> GetGroupMembersAsync(int groupId);
        Task<IEnumerable<GroupMemberDto>> GetUserGroupsAsync(int userId);
        Task<IEnumerable<GroupMemberDto>> GetPendingMembersAsync(int groupId);
        Task<IEnumerable<GroupMemberDto>> GetActiveMembersAsync(int groupId);
        Task<int> JoinGroupAsync(int groupId, int userId);
        Task<bool> ApproveMembershipAsync(int groupMemberId);
        Task<bool> RejectMembershipAsync(int groupMemberId);
        Task<bool> RemoveFromGroupAsync(int groupMemberId);
    }
}
