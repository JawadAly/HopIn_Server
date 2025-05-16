using HopIn_Server.Models;
using HopIn_Server.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HopIn_Server.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ChatController : ControllerBase
	{
		private readonly ChatService _chatService;
		private readonly InboxService _inboxService;
		public ChatController(ChatService chatService,InboxService inboxService) {
			_chatService = chatService;
			_inboxService = inboxService;
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
		public async Task<IActionResult> createChat(Chat chat,string? inbId)
		{
			try {
				//checking inbox existance
				var result = await _inboxService.doesInbExists(inbId!);
				if (!result.success) {
					return ApiResponse(400,result.success,result.message);
				}

				var result1 = await _chatService.addChat(chat);
				if (!result1.success) {
					return ApiResponse(400,result1.success,result1.message);
				}
				//addition of chat to inbox
				var result2 = await _inboxService.updateInboxChats(chat,inbId!);
				if(!result2.success) {
					return ApiResponse(400, result2.success, result2.message);
				}

				return ApiResponse(200, result2.success, result2.message);
			}
			catch (Exception e) {
				return ApiResponse(500, false, "An unexpected error occurred.", errorMsg: e.Message);
			}
		}
	}
}
