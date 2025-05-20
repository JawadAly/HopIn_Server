using HopIn_Server.Models;
using HopIn_Server.Services;
using Microsoft.AspNetCore.Mvc;

namespace HopIn_Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly ChatService _chatService;
        private readonly RideService _rideService;

        public ChatController(ChatService chatService, RideService rideService)
        {
            _chatService = chatService;
            _rideService = rideService;
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

        [HttpPost("create")]
        public async Task<IActionResult> CreateChat([FromBody] CreateChatRequest request)
        {
            try
            {
                var result = await _chatService.CreateChat(request.RideId, request.PassengerId);
                if (!result.success)
                {
                    return ApiResponse(400, false, errorMsg: result.message);
                }
                return ApiResponse(200, true, "Chat created successfully", data: result.chat);
            }
            catch (Exception ex)
            {
                return ApiResponse(500, false, "An unexpected error occurred", errorMsg: ex.Message);
            }
        }

        [HttpPost("message")]
        public async Task<IActionResult> SendMessage([FromBody] SendMessageRequest request)
        {
            try
            {
                var result = await _chatService.SendMessage(request.ChatId, request.SenderId, request.Content);
                if (!result.success)
                {
                    return ApiResponse(400, false, errorMsg: result.statusMessage);
                }
                return ApiResponse(200, true, "Message sent successfully", data: result.message);
            }
            catch (Exception ex)
            {
                return ApiResponse(500, false, "An unexpected error occurred", errorMsg: ex.Message);
            }
        }

        [HttpGet("{chatId}")]
        public async Task<IActionResult> GetChatHistory(string chatId)
        {
            try
            {
                var result = await _chatService.GetChatHistory(chatId);
                if (!result.success)
                {
                    return ApiResponse(400, false, errorMsg: result.message);
                }
                return ApiResponse(200, true, "Chat history retrieved successfully", data: result.messages);
            }
            catch (Exception ex)
            {
                return ApiResponse(500, false, "An unexpected error occurred", errorMsg: ex.Message);
            }
        }

        [HttpGet("inbox/{userId}")]
        public async Task<IActionResult> GetUserInbox(string userId)
        {
            try
            {
                var result = await _chatService.GetUserInbox(userId);
                if (!result.success)
                {
                    return ApiResponse(400, false, errorMsg: result.message);
                }
                return ApiResponse(200, true, "Inbox retrieved successfully", data: result.inbox);
            }
            catch (Exception ex)
            {
                return ApiResponse(500, false, "An unexpected error occurred", errorMsg: ex.Message);
            }
        }
    }

    public class CreateChatRequest
    {
        public string RideId { get; set; } = null!;
        public string PassengerId { get; set; } = null!;
    }

    public class SendMessageRequest
    {
        public string ChatId { get; set; } = null!;
        public string SenderId { get; set; } = null!;
        public string Content { get; set; } = null!;
    }
}