using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using StudyGroup.Service.Interfaces;
using StudyGroup.Service.DTOs;
using System.Security.Claims;

namespace StudyGroup.Api.Controllers
{
    /// <summary>
    /// API controller for GroupMember endpoints with instant membership management and role-based authorization.
    /// Users can join study groups immediately without approval process.
    /// Only group creators can remove members, but any member can leave voluntarily.
    /// Group member lists are only visible to actual members of each group.
    /// Requires JWT authentication for all endpoints.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Require JWT authentication for all endpoints
    public class GroupMembersController : ControllerBase
    {
        private readonly IGroupMemberService _service;
        private readonly IStudyGroupService _studyGroupService;

        /// <summary>
        /// Initializes a new instance of the GroupMembersController.
        /// </summary>
        /// <param name="service">The group member service for handling business logic</param>
        /// <param name="studyGroupService">The study group service for authorization checks</param>
        public GroupMembersController(IGroupMemberService service, IStudyGroupService studyGroupService)
        {
            _service = service;
            _studyGroupService = studyGroupService;
        }

        /// <summary>
        /// Gets the current user ID from JWT claims.
        /// </summary>
        /// <returns>Current user ID or null if not found</returns>
        private int? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out int userId) ? userId : null;
        }

        /// <summary>
        /// Checks if the current user is a member of the specified group.
        /// </summary>
        /// <param name="groupId">The group ID to check</param>
        /// <param name="userId">The user ID to check</param>
        /// <returns>True if user is an active member, false otherwise</returns>
        private async Task<bool> IsUserMemberOfGroupAsync(int groupId, int userId)
        {
            var userMemberships = await _service.GetUserGroupsAsync(userId);
            return userMemberships.Any(m => m.GroupId == groupId && m.StatusId == 2); // StatusId 2 = Active
        }

        /// <summary>
        /// Checks if the current user is the creator of the specified group.
        /// </summary>
        /// <param name="groupId">The group ID to check</param>
        /// <param name="userId">The user ID to check</param>
        /// <returns>True if user is the group creator, false otherwise</returns>
        private async Task<bool> IsUserCreatorOfGroupAsync(int groupId, int userId)
        {
            var studyGroup = await _studyGroupService.GetGroupByIdAsync(groupId);
            return studyGroup?.CreatorId == userId;
        }

        #region Basic CRUD Operations

        /// <summary>
        /// GET: api/groupmembers
        /// Retrieves all group memberships in the system.
        /// </summary>
        /// <returns>List of all group memberships with their status information</returns>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var members = await _service.GetAllGroupMembersAsync();
            return Ok(members);
        }

        /// <summary>
        /// GET: api/groupmembers/{id}
        /// Retrieves a specific group membership by its unique ID.
        /// </summary>
        /// <param name="id">The unique identifier of the group membership</param>
        /// <returns>The group membership details if found, otherwise NotFound</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var member = await _service.GetGroupMemberByIdAsync(id);
            if (member == null) return NotFound();
            return Ok(member);
        }

        /// <summary>
        /// POST: api/groupmembers
        /// Creates a new group membership directly with specified status.
        /// For instant joining, use this endpoint with StatusId = 2 (Approved).
        /// AUTHORIZATION: Users can only create memberships for themselves.
        /// 
        /// Example request body:
        /// {
        ///   "groupId": 5,
        ///   "userId": 123,
        ///   "statusId": 2
        /// }
        /// </summary>
        /// <param name="groupMemberDto">The group membership data including GroupId, UserId, and StatusId</param>
        /// <returns>The created membership with its new ID</returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateGroupMemberDto groupMemberDto)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                if (currentUserId == null)
                {
                    return Unauthorized("Invalid token");
                }

                // Authorization: Users can only create memberships for themselves
                if (groupMemberDto.UserId != currentUserId.Value)
                {
                    return Forbid("You can only join groups for yourself");
                }

                var id = await _service.CreateGroupMemberAsync(groupMemberDto);
                
                return CreatedAtAction(nameof(GetById), new { id }, new 
                { 
                    MembershipId = id,
                    Message = groupMemberDto.StatusId == 2 ? 
                        "?? Welcome! You've instantly joined the study group!" : 
                        "? Group membership created successfully!",
                    GroupId = groupMemberDto.GroupId,
                    UserId = groupMemberDto.UserId,
                    Status = groupMemberDto.StatusId switch
                    {
                        2 => "Active Member",
                        _ => "Custom Status"
                    },
                    MemberRights = new[]
                    {
                        "? Participate in group activities",
                        "? Leave the group anytime",
                        "? View other group members",
                        "?? Access to private group member list"
                    }
                });
            }
            catch (InvalidOperationException ex)
            {
                // Handle cases where user is already a member
                return BadRequest(new { Error = ex.Message, Note = "User might already be a member of this group" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = "Failed to create membership", Details = ex.Message });
            }
        }

        /// <summary>
        /// PUT: api/groupmembers/{id}
        /// Updates an existing group membership.
        /// AUTHORIZATION: Users can only update their own memberships.
        /// </summary>
        /// <param name="id">The ID of the membership to update</param>
        /// <param name="groupMemberDto">The updated membership data</param>
        /// <returns>NoContent if successful, NotFound if membership doesn't exist</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateGroupMemberDto groupMemberDto)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                if (currentUserId == null)
                {
                    return Unauthorized("Invalid token");
                }

                // Get the existing membership to check ownership
                var existingMembership = await _service.GetGroupMemberByIdAsync(id);
                if (existingMembership == null)
                {
                    return NotFound("Membership not found");
                }

                // Authorization: Users can only update their own memberships
                if (existingMembership.UserId != currentUserId.Value)
                {
                    return Forbid("You can only update your own memberships");
                }

                var success = await _service.UpdateGroupMemberAsync(id, groupMemberDto);
                if (!success) return NotFound();
                
                return Ok(new 
                { 
                    Message = "? Your membership updated successfully",
                    Authorization = "Updated as membership owner"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = "Failed to update membership", Details = ex.Message });
            }
        }

        /// <summary>
        /// DELETE: api/groupmembers/{id}
        /// Removes a group membership completely from the system.
        /// AUTHORIZATION: Users can only delete their own memberships OR group creators can delete any membership in their groups.
        /// </summary>
        /// <param name="id">The ID of the membership to delete</param>
        /// <returns>NoContent if successful, NotFound if membership doesn't exist</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                if (currentUserId == null)
                {
                    return Unauthorized("Invalid token");
                }

                // Get the membership details
                var membership = await _service.GetGroupMemberByIdAsync(id);
                if (membership == null)
                {
                    return NotFound("Membership not found");
                }

                // Get the study group details to check creator
                var studyGroup = await _studyGroupService.GetGroupByIdAsync(membership.GroupId);
                if (studyGroup == null)
                {
                    return NotFound("Study group not found");
                }

                // Authorization: User can delete their own membership OR group creator can delete any membership
                bool canDelete = membership.UserId == currentUserId.Value || studyGroup.CreatorId == currentUserId.Value;
                
                if (!canDelete)
                {
                    return StatusCode(403, new 
                    { 
                        Error = "Access denied - You can only remove your own membership or members from groups you created",
                        YourUserId = currentUserId.Value,
                        MembershipUserId = membership.UserId,
                        GroupCreatorId = studyGroup.CreatorId
                    });
                }

                var success = await _service.DeleteGroupMemberAsync(id);
                if (!success) return NotFound();

                // Determine the action type for response
                bool isCreatorRemoving = studyGroup.CreatorId == currentUserId.Value && membership.UserId != currentUserId.Value;
                bool isSelfLeaving = membership.UserId == currentUserId.Value;

                return Ok(new 
                { 
                    Message = isCreatorRemoving ? 
                        "? Member removed from group by creator" : 
                        "? You have successfully left the group",
                    Action = isCreatorRemoving ? "Member Removal" : "Self Leave",
                    Authorization = isCreatorRemoving ? "Performed as group creator" : "Performed as membership owner",
                    Note = "User can rejoin anytime using standard membership creation"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = "Failed to delete membership", Details = ex.Message });
            }
        }

        #endregion

        #region Group and User Membership Queries (Privacy Protected)

        /// <summary>
        /// GET: api/groupmembers/groups/{groupId}/members
        /// Retrieves all active members of a specific study group.
        /// PRIVACY PROTECTION: Only members of the group can view the member list.
        /// AUTHORIZATION: User must be an active member of the group OR the group creator.
        /// </summary>
        /// <param name="groupId">The ID of the study group</param>
        /// <returns>List of all active members for the specified group</returns>
        [HttpGet("groups/{groupId}/members")]
        public async Task<IActionResult> GetGroupMembers(int groupId)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                if (currentUserId == null)
                {
                    return Unauthorized("Invalid token");
                }

                // Check if current user is a member of this group or the creator
                bool isMember = await IsUserMemberOfGroupAsync(groupId, currentUserId.Value);
                bool isCreator = await IsUserCreatorOfGroupAsync(groupId, currentUserId.Value);

                if (!isMember && !isCreator)
                {
                    return StatusCode(403, new
                    {
                        Error = "Access denied - Only group members can view the member list",
                        GroupId = groupId,
                        YourUserId = currentUserId.Value,
                        RequiredAccess = "You must be a member of this group to see other members",
                        HowToJoin = "Use POST /api/groupmembers to join this group first"
                    });
                }

                var members = await _service.GetActiveMembersAsync(groupId);
                var membersList = members.ToList();

                return Ok(new
                {
                    Message = $"?? Active Members of Study Group {groupId}",
                    GroupId = groupId,
                    TotalMembers = membersList.Count,
                    Members = membersList,
                    YourRole = isCreator ? "Group Creator" : "Group Member",
                    PrivacyProtection = new
                    {
                        Note = "?? Member list is only visible to group members",
                        YourAccess = isMember ? "Granted as group member" : "Granted as group creator"
                    },
                    CreatorRights = isCreator ? new[]
                    {
                        "? Remove any member from the group",
                        "? Update group details",
                        "? Delete the entire group"
                    } : null,
                    MemberRights = isMember && !isCreator ? new[]
                    {
                        "? View other group members",
                        "? Leave the group anytime",
                        "? Participate in group activities"
                    } : null
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = "Failed to get group members", Details = ex.Message });
            }
        }

        /// <summary>
        /// GET: api/groupmembers/users/{userId}/groups
        /// Retrieves all study groups that a specific user is a member of.
        /// AUTHORIZATION: Users can only view their own group memberships.
        /// </summary>
        /// <param name="userId">The ID of the user</param>
        /// <returns>List of all active group memberships for the specified user</returns>
        [HttpGet("users/{userId}/groups")]
        public async Task<IActionResult> GetUserGroups(int userId)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                if (currentUserId == null)
                {
                    return Unauthorized("Invalid token");
                }

                // Authorization: Users can only view their own group memberships
                if (userId != currentUserId.Value)
                {
                    return Forbid("You can only view your own group memberships");
                }

                var groups = await _service.GetUserGroupsAsync(userId);
                // Filter to only show active memberships (StatusId = 2)
                var activeGroups = groups.Where(g => g.StatusId == 2).ToList();

                // Add creator information for each group
                var groupsWithCreatorInfo = new List<object>();
                foreach (var group in activeGroups)
                {
                    var studyGroup = await _studyGroupService.GetGroupByIdAsync(group.GroupId);
                    bool isCreator = studyGroup?.CreatorId == currentUserId.Value;
                    
                    groupsWithCreatorInfo.Add(new
                    {
                        Membership = group,
                        YourRole = isCreator ? "Creator" : "Member",
                        Rights = isCreator ? 
                            new[] { "Full management rights", "Can remove members", "Can delete group" } : 
                            new[] { "Can leave group", "Can participate", "Can view other members" },
                        PrivacyAccess = "?? You can view member lists for all these groups"
                    });
                }
                
                return Ok(new
                {
                    Message = $"?? Study Groups for User {userId}",
                    UserId = userId,
                    TotalGroups = activeGroups.Count,
                    Groups = groupsWithCreatorInfo,
                    PrivacyNote = "As a member of these groups, you have access to view their member lists"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = "Failed to get user groups", Details = ex.Message });
            }
        }

        /// <summary>
        /// GET: api/groupmembers/groups/{groupId}/members/active
        /// Gets active members (same as GetGroupMembers since all members are automatically active).
        /// PRIVACY PROTECTION: Only members of the group can view the member list.
        /// Kept for API compatibility.
        /// </summary>
        /// <param name="groupId">The ID of the study group</param>
        /// <returns>List of active members in the group</returns>
        [HttpGet("groups/{groupId}/members/active")]
        public async Task<IActionResult> GetActiveMembers(int groupId)
        {
            // Since all members are automatically approved, this is the same as GetGroupMembers
            // Privacy protection is handled by GetGroupMembers
            return await GetGroupMembers(groupId);
        }

        #endregion

        #region Member Management Actions

        /// <summary>
        /// DELETE: api/groupmembers/groups/{groupId}/leave
        /// Allows a user to leave a study group instantly.
        /// AUTHORIZATION: Users can only leave groups they are members of.
        /// </summary>
        /// <param name="groupId">The ID of the study group to leave</param>
        /// <param name="userId">The ID of the user leaving the group</param>
        /// <returns>Success message confirming the user left the group</returns>
        [HttpDelete("groups/{groupId}/leave")]
        public async Task<IActionResult> LeaveGroup(int groupId, [FromQuery] int userId)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                if (currentUserId == null)
                {
                    return Unauthorized("Invalid token");
                }

                // Authorization: Users can only leave groups for themselves
                if (userId != currentUserId.Value)
                {
                    return Forbid("You can only leave groups for yourself");
                }

                // Find the user's membership in this group
                var userMemberships = await _service.GetUserGroupsAsync(userId);
                var membership = userMemberships.FirstOrDefault(m => m.GroupId == groupId && m.StatusId == 2); // Only active memberships
                
                if (membership == null)
                {
                    return NotFound(new { Error = "You are not an active member of this group" });
                }

                // Remove the membership
                var success = await _service.DeleteGroupMemberAsync(membership.GroupMemberId);
                if (!success)
                {
                    return BadRequest(new { Error = "Failed to leave group" });
                }
                
                return Ok(new 
                { 
                    Message = "?? You have successfully left the study group",
                    GroupId = groupId,
                    UserId = userId,
                    Action = "Voluntary Leave",
                    PrivacyNote = "You will no longer have access to view the member list for this group",
                    RejoiningNote = "You can rejoin anytime using POST /api/groupmembers with the required details"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = "Failed to leave group", Details = ex.Message });
            }
        }

        /// <summary>
        /// DELETE: api/groupmembers/{id}/remove
        /// Removes a user from a group by deleting their membership record.
        /// AUTHORIZATION: Only the group creator can remove members (unless it's the user removing themselves).
        /// </summary>
        /// <param name="id">The ID of the membership to remove</param>
        /// <returns>Success message if removed, NotFound if membership doesn't exist</returns>
        [HttpDelete("{id}/remove")]
        public async Task<IActionResult> RemoveFromGroup(int id)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                if (currentUserId == null)
                {
                    return Unauthorized("Invalid token");
                }

                // Get the membership details
                var membership = await _service.GetGroupMemberByIdAsync(id);
                if (membership == null)
                {
                    return NotFound("Membership not found");
                }

                // Get the study group details to check creator
                var studyGroup = await _studyGroupService.GetGroupByIdAsync(membership.GroupId);
                if (studyGroup == null)
                {
                    return NotFound("Study group not found");
                }

                // Authorization: Only group creator can remove members (unless user is removing themselves)
                bool isCreatorRemoving = studyGroup.CreatorId == currentUserId.Value;
                bool isSelfRemoving = membership.UserId == currentUserId.Value;
                
                if (!isCreatorRemoving && !isSelfRemoving)
                {
                    return StatusCode(403, new 
                    { 
                        Error = "Access denied - Only the group creator can remove members",
                        GroupCreatorId = studyGroup.CreatorId,
                        YourUserId = currentUserId.Value,
                        Alternative = "Members can leave voluntarily using the leave endpoint"
                    });
                }

                var success = await _service.DeleteGroupMemberAsync(id);
                if (!success) return NotFound("Membership not found");
                
                return Ok(new 
                { 
                    Message = isCreatorRemoving && !isSelfRemoving ? 
                        "? Member removed from group by creator" : 
                        "? You have left the group",
                    Action = isCreatorRemoving && !isSelfRemoving ? "Creator Removal" : "Self Removal",
                    Authorization = isCreatorRemoving ? "Performed with creator rights" : "Performed as membership owner",
                    RemovedUserId = membership.UserId,
                    GroupId = membership.GroupId,
                    PrivacyNote = isCreatorRemoving && !isSelfRemoving ? 
                        "Removed user will no longer have access to view the member list" : 
                        "You will no longer have access to view the member list for this group",
                    RejoiningNote = "The user can rejoin the group anytime using standard membership creation"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = "Failed to remove member", Details = ex.Message });
            }
        }

        #endregion
    }
}
