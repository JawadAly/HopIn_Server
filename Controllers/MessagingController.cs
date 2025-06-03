using HopIn_Server.Models;
using HopIn_Server.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using HopIn_Server.Hubs;

namespace HopIn_Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessagingController : ControllerBase
    {
        private readonly MessagingService _messagingService;
        private readonly ChatService _chatService;
        private readonly IHubContext<MessagingHub> _hubContext;

        public MessagingController(
            MessagingService incomingMessagingService,
            ChatService incomingChatService,
            IHubContext<MessagingHub> hubContext)
        {
            _messagingService = incomingMessagingService;
            _chatService = incomingChatService;
            _hubContext = hubContext;
        }

        private IActionResult ApiResponse(int statusCode, bool success, string? message = null, string? errorMsg = null, object? data = null)
        {
            var response = new
            {
                success,
                message,
                errorMsg,
                data
            };
            return StatusCode(statusCode, response);
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage(Message message, string? chatId)
        {
            try
            {
                // 1. Validate chat existence
                var chatExistence = await _chatService.doesChatExists(chatId!);
                if (!chatExistence.success)
                {
                    return ApiResponse(400, chatExistence.success, chatExistence.message);
                }

                // 2. Save message to Chat and update inboxes directly in MessagingService
                var result = await _messagingService.AddMessageAndUpdateInboxes(message, chatId!);
                if (!result.success)
                {
                    return ApiResponse(400, result.success, result.message);
                }

                // 3. Real-time message broadcast
                await _hubContext.Clients.Group(chatId!).SendAsync("ReceiveMessage", message);

                return ApiResponse(200, true, "Message sent and inboxes updated.");
            }
            catch (Exception e)
            {
                return ApiResponse(500, false, "An unexpected error occurred.", errorMsg: e.Message);
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteMessage(string? msgId)
        {
            try
            {
                var result = await _messagingService.DeleteMsg(msgId!);
                if (!result.success)
                {
                    return ApiResponse(400, result.success, result.message);
                }
                return ApiResponse(200, result.success, result.message);
            }
            catch (Exception e)
            {
                return ApiResponse(500, false, "An unexpected error occurred.", errorMsg: e.Message);
            }
        }
    }
}
