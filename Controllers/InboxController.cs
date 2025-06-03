using HopIn_Server.Dtos;
using HopIn_Server.Models;
using HopIn_Server.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HopIn_Server.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class InboxController : ControllerBase
	{
		private readonly InboxService _inboxService;
		private readonly UserService _userService;
		public InboxController(InboxService inboxService, UserService userService) {
			_inboxService = inboxService;
			_userService = userService;
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
		public async Task<IActionResult> createInbox(Inbox inbox)
		{
			try
			{
				var result = await _inboxService.createInbox(inbox);
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


		[HttpGet("{id}")]
		public async Task<IActionResult> getInbox(string id)
		{
			var inbox = await _inboxService.GetInboxByIdAsync(id);
			if (inbox == null) { return NotFound(); }
			return Ok(inbox);
		}

        [HttpGet("chats/{userId}")]
        public async Task<IActionResult> GetChatsByUserId(string userId)
        {
            try
            {
                var inbxUser = await _userService.GetUserByIdAsync(userId);
                if (inbxUser == null)
                    return ApiResponse(404, false, errorMsg: "No such user exists with inbox");

                var incomingChats = await _inboxService.fetchInboxChats(inbxUser);
                if (!incomingChats.success || incomingChats.chatsList == null)
                {
                    return ApiResponse(400, false, errorMsg: incomingChats.messsage);
                }

                //creating new chat list according to new dto
                var chatDtos = new List<ChatDto>();
                foreach (var chat in incomingChats.chatsList)
                {
                    var isUserPerson1 = chat.person1Id == userId;
                    var otherUserId = isUserPerson1 ? chat.person2Id : chat.person1Id;
                    var otherUser = await _userService.GetUserByIdAsync(otherUserId);
                    if (otherUser == null) continue;

                    chatDtos.Add(new ChatDto
                    {
                        chatId = chat.chatId,
                        person1 = inbxUser,
                        person2 = otherUser,
                        chatMessages = chat.chatMessages,
                        chatLastUpdated = chat.chatLastUpdated,
                    });
                }
                return ApiResponse(200, true, incomingChats.messsage, data: chatDtos);
                //return ApiResponse(200, true, incomingChats.messsage,data:incomingChats.chatsList);
            }
            catch (Exception e)
            {
                return ApiResponse(500, false, "An unexpected error occurred.", errorMsg: e.Message);
            }
        }

    }
}
