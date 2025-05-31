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

        public MessagingController(MessagingService incomingMessagingService,ChatService incomingChatService , IHubContext<MessagingHub> hubContext) {
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
		public async Task<IActionResult> SendMessage(Message message,string? chatId)
		{
			try {
				//checking chat existance
				var chatExistence = await _chatService.doesChatExists(chatId!);
				if (!chatExistence.success)
				{
					return ApiResponse(400, chatExistence.success, chatExistence.message);
				}
				var result = await _messagingService.AddMessage(message);
				if (!result.success) {
					return ApiResponse(400,result.success,result.message);
				}
				//updating chat
				var finalResult = await _chatService.updateChatList(message,chatId!);
				if (!finalResult.success) {
					return ApiResponse(400, finalResult.success, finalResult.message);
				}
                await _hubContext.Clients.Group(chatId!).SendAsync("ReceiveMessage", message);
                return ApiResponse(200, result.success, finalResult.message);
			}
			catch (Exception e) {	
				return ApiResponse(500, false, "An unexpected error occurred.", errorMsg: e.Message);
			}
		}

		[HttpDelete]

		public async Task<IActionResult> DeleteMessage(string? msgId)
		{
			try
			{
				var result = await _messagingService.DeleteMsg(msgId!);
				if (!result.success) {
					return ApiResponse(400,result.success,result.message);
				}
				return ApiResponse(200,result.success,result.message);
			}
			catch (Exception e) {
				return ApiResponse(500, false, "An unexpected error occurred.", errorMsg: e.Message);
			}
		}
	}
}
