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
		public InboxController(InboxService inboxService) => _inboxService = inboxService;

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
			try {
				var result = await _inboxService.createInbox(inbox);
				if (!result.success) {
					return ApiResponse(400,result.success,result.message);
				}
				return ApiResponse(200, result.success, result.message);
			}
			catch (Exception e) {
				return ApiResponse(500, false, "An unexpected error occurred.", errorMsg: e.Message);
			}
		}

	}
}
