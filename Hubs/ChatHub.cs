using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using StudyGroup.Service.DTOs;
using StudyGroup.Service.Interfaces;
using System.Security.Claims;

namespace StudyGroup.Api.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly IConversationService _conversationService;
        private readonly IMessageService _messageService;

        public ChatHub(IConversationService conversationService, IMessageService messageService)
        {
            _conversationService = conversationService;
            _messageService = messageService;
        }

        private int? GetCurrentUserId()
        {
            var userIdClaim = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out int userId) ? userId : null;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = GetCurrentUserId();
            if (userId.HasValue)
            {
                // Join user to their personal group for notifications
                await Groups.AddToGroupAsync(Context.ConnectionId, $"User_{userId}");
                
                // Join user to all their conversation groups
                var conversations = await _conversationService.GetUserConversationsAsync(userId.Value);
                foreach (var conversation in conversations)
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, $"Conversation_{conversation.ConversationId}");
                }
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = GetCurrentUserId();
            if (userId.HasValue)
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"User_{userId}");
            }

            await base.OnDisconnectedAsync(exception);
        }

        /// <summary>
        /// Join a specific conversation room for real-time updates
        /// </summary>
        public async Task JoinConversation(int conversationId)
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue) return;

            // Verify user is participant in this conversation
            var isParticipant = await _conversationService.IsUserParticipantAsync(conversationId, userId.Value);
            if (isParticipant)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"Conversation_{conversationId}");
            }
        }

        /// <summary>
        /// Leave a specific conversation room
        /// </summary>
        public async Task LeaveConversation(int conversationId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Conversation_{conversationId}");
        }

        /// <summary>
        /// Send a message through SignalR (alternative to REST API)
        /// </summary>
        public async Task SendMessage(int conversationId, string content, int? replyToMessageId = null)
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue) return;

            try
            {
                // Verify user is participant
                var isParticipant = await _conversationService.IsUserParticipantAsync(conversationId, userId.Value);
                if (!isParticipant) return;

                // Create and send message
                var messageId = await _messageService.SendTextMessageAsync(conversationId, userId.Value, content, replyToMessageId);
                var message = await _messageService.GetMessageByIdAsync(messageId);

                if (message != null)
                {
                    // Broadcast to all participants in the conversation
                    await Clients.Group($"Conversation_{conversationId}")
                        .SendAsync("ReceiveMessage", new
                        {
                            ConversationId = conversationId,
                            Message = message,
                            Type = "NewMessage"
                        });
                }
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("Error", new { Message = "Failed to send message", Details = ex.Message });
            }
        }

        /// <summary>
        /// Mark messages as read
        /// </summary>
        public async Task MarkAsRead(int conversationId)
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue) return;

            try
            {
                var success = await _messageService.MarkMessagesAsReadAsync(conversationId, userId.Value);
                if (success)
                {
                    // Notify other participants that user has read messages
                    await Clients.OthersInGroup($"Conversation_{conversationId}")
                        .SendAsync("UserReadMessages", new
                        {
                            ConversationId = conversationId,
                            UserId = userId.Value,
                            ReadAt = DateTime.UtcNow
                        });
                }
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("Error", new { Message = "Failed to mark as read", Details = ex.Message });
            }
        }

        /// <summary>
        /// Notify that user is typing
        /// </summary>
        public async Task StartTyping(int conversationId)
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue) return;

            var isParticipant = await _conversationService.IsUserParticipantAsync(conversationId, userId.Value);
            if (isParticipant)
            {
                await Clients.OthersInGroup($"Conversation_{conversationId}")
                    .SendAsync("UserTyping", new
                    {
                        ConversationId = conversationId,
                        UserId = userId.Value,
                        IsTyping = true
                    });
            }
        }

        /// <summary>
        /// Notify that user stopped typing
        /// </summary>
        public async Task StopTyping(int conversationId)
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue) return;

            var isParticipant = await _conversationService.IsUserParticipantAsync(conversationId, userId.Value);
            if (isParticipant)
            {
                await Clients.OthersInGroup($"Conversation_{conversationId}")
                    .SendAsync("UserTyping", new
                    {
                        ConversationId = conversationId,
                        UserId = userId.Value,
                        IsTyping = false
                    });
            }
        }
    }
}