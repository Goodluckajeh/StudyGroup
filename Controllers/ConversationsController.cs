using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using StudyGroup.Api.Hubs;
using StudyGroup.Service.DTOs;
using StudyGroup.Service.Interfaces;
using System.Security.Claims;

namespace StudyGroup.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ConversationsController : ControllerBase
    {
        private readonly IConversationService _conversationService;
        private readonly IGroupMemberService _groupMemberService;
        private readonly IHubContext<ChatHub> _hubContext;

        public ConversationsController(
            IConversationService conversationService, 
            IGroupMemberService groupMemberService,
            IHubContext<ChatHub> hubContext)
        {
            _conversationService = conversationService;
            _groupMemberService = groupMemberService;
            _hubContext = hubContext;
        }

        private int? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out int userId) ? userId : null;
        }

        /// <summary>
        /// GET: api/conversations
        /// Get all conversations for the current user
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetUserConversations()
        {
            var currentUserId = GetCurrentUserId();
            if (currentUserId == null) return Unauthorized();

            var conversations = await _conversationService.GetUserConversationsAsync(currentUserId.Value);
            
            return Ok(new
            {
                Message = "?? Your Conversations",
                UserId = currentUserId.Value,
                Conversations = conversations,
                TotalCount = conversations.Count()
            });
        }

        /// <summary>
        /// GET: api/conversations/{id}
        /// Get a specific conversation
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetConversation(int id)
        {
            var currentUserId = GetCurrentUserId();
            if (currentUserId == null) return Unauthorized();

            var isParticipant = await _conversationService.IsUserParticipantAsync(id, currentUserId.Value);
            if (!isParticipant) return StatusCode(403, new { Error = "You are not a participant in this conversation" });

            var conversation = await _conversationService.GetConversationByIdAsync(id);
            if (conversation == null) return NotFound();

            return Ok(new
            {
                Conversation = conversation,
                Message = "?? Conversation Details"
            });
        }

        /// <summary>
        /// POST: api/conversations/direct-message
        /// Start a direct message conversation
        /// </summary>
        [HttpPost("direct-message")]
        public async Task<IActionResult> StartDirectMessage([FromBody] StartDirectMessageDto startDmDto)
        {
            var currentUserId = GetCurrentUserId();
            if (currentUserId == null) return Unauthorized();

            try
            {
                var conversation = await _conversationService.StartDirectMessageAsync(currentUserId.Value, startDmDto);
                if (conversation == null) return BadRequest("Failed to start direct message");

                // Notify the recipient via SignalR
                await _hubContext.Clients.User(startDmDto.RecipientUserId.ToString())
                    .SendAsync("NewConversation", new
                    {
                        Conversation = conversation,
                        Type = "DirectMessage",
                        Message = "New direct message conversation started"
                    });

                return CreatedAtAction(nameof(GetConversation), new { id = conversation.ConversationId }, new
                {
                    Conversation = conversation,
                    Message = "?? Direct message conversation started successfully"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = "Failed to start direct message", Details = ex.Message });
            }
        }

        /// <summary>
        /// GET: api/conversations/study-group/{studyGroupId}
        /// Get or create conversation for a study group
        /// </summary>
        [HttpGet("study-group/{studyGroupId}")]
        public async Task<IActionResult> GetStudyGroupConversation(int studyGroupId)
        {
            var currentUserId = GetCurrentUserId();
            if (currentUserId == null) return Unauthorized();

            // Verify user is member of the study group
            var userGroups = await _groupMemberService.GetUserGroupsAsync(currentUserId.Value);
            var isMember = userGroups.Any(g => g.GroupId == studyGroupId && g.StatusId == 2); // Active member

            if (!isMember) return StatusCode(403, new { Error = "You must be a member of this study group to access its conversation" });

            try
            {
                var conversation = await _conversationService.GetOrCreateGroupConversationAsync(studyGroupId);
                if (conversation == null) return BadRequest("Failed to get or create group conversation");

                return Ok(new
                {
                    Conversation = conversation,
                    Message = "?? Study Group Chat",
                    StudyGroupId = studyGroupId
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = "Failed to get group conversation", Details = ex.Message });
            }
        }

        /// <summary>
        /// POST: api/conversations/{id}/participants
        /// Add a participant to a conversation (group chats only)
        /// </summary>
        [HttpPost("{id}/participants")]
        public async Task<IActionResult> AddParticipant(int id, [FromBody] AddParticipantDto addParticipantDto)
        {
            var currentUserId = GetCurrentUserId();
            if (currentUserId == null) return Unauthorized();

            var conversation = await _conversationService.GetConversationByIdAsync(id);
            if (conversation == null) return NotFound();

            // Only allow adding participants to group chats
            if (conversation.ConversationType != "GroupChat")
                return BadRequest("Can only add participants to group conversations");

            // Verify current user is participant
            var isParticipant = await _conversationService.IsUserParticipantAsync(id, currentUserId.Value);
            if (!isParticipant) return StatusCode(403, new { Error = "You are not a participant in this conversation" });

            var success = await _conversationService.AddParticipantAsync(id, addParticipantDto.UserId);
            if (!success) return BadRequest("Failed to add participant");

            // Notify all participants via SignalR
            await _hubContext.Clients.Group($"Conversation_{id}")
                .SendAsync("ParticipantAdded", new
                {
                    ConversationId = id,
                    UserId = addParticipantDto.UserId,
                    AddedBy = currentUserId.Value,
                    Type = "ParticipantAdded"
                });

            return Ok(new { Success = true, Message = "? Participant added successfully" });
        }

        /// <summary>
        /// DELETE: api/conversations/{id}/participants/{userId}
        /// Remove a participant from a conversation
        /// </summary>
        [HttpDelete("{id}/participants/{userId}")]
        public async Task<IActionResult> RemoveParticipant(int id, int userId)
        {
            var currentUserId = GetCurrentUserId();
            if (currentUserId == null) return Unauthorized();

            var conversation = await _conversationService.GetConversationByIdAsync(id);
            if (conversation == null) return NotFound();

            // Users can remove themselves, or study group creators can remove others
            bool canRemove = userId == currentUserId.Value;
            
            if (!canRemove && conversation.ConversationType == "GroupChat")
            {
                // Check if current user is the study group creator
                // Additional logic would be needed here based on your study group structure
                var isParticipant = await _conversationService.IsUserParticipantAsync(id, currentUserId.Value);
                canRemove = isParticipant; // For now, any participant can remove others in group chats
            }

            if (!canRemove) return StatusCode(403, new { Error = "You don't have permission to remove this participant" });

            var success = await _conversationService.RemoveParticipantAsync(id, userId);
            if (!success) return BadRequest("Failed to remove participant");

            // Notify all participants via SignalR
            await _hubContext.Clients.Group($"Conversation_{id}")
                .SendAsync("ParticipantRemoved", new
                {
                    ConversationId = id,
                    UserId = userId,
                    RemovedBy = currentUserId.Value,
                    Type = "ParticipantRemoved"
                });

            return Ok(new { Success = true, Message = "? Participant removed successfully" });
        }
    }
}