using HopIn_Server.Models;
using HopIn_Server.Services;
using Microsoft.AspNetCore.Mvc;

namespace HopIn_Server.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class UserController : ControllerBase
	{
		private readonly UserService _userService;

		public UserController(UserService userService)
		{
			_userService = userService;
		}

		[HttpGet]
		public async Task<ActionResult<List<User>>> GetAllUsers() =>
			Ok(await _userService.GetAllUsersAsync());

		
		[HttpGet("{id}")]
		public async Task<ActionResult<User>> GetUserById(string id)
		{
			var user = await _userService.GetUserByIdAsync(id);
			if (user == null) return NotFound();
			return Ok(user);
		}

		[HttpPost]
		public async Task<IActionResult> CreateUser([FromBody] User newUser)
		{
			var user = new User
			{
				userId = newUser.userId,
				userFirstName = newUser.userFirstName,
				userLastName= newUser.userLastName,
				userEmail= newUser.userEmail,
				receiveEmailUpdates= newUser.receiveEmailUpdates,
				inbox = new Inbox(),
				userCreatedAt = DateTime.UtcNow,
			};
			await _userService.CreateUserAsync(user);
			return CreatedAtAction(nameof(GetUserById), new { id = newUser.userId }, newUser);
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateUser(string id, [FromBody] User updatedUser)
		{
			var user = await _userService.GetUserByIdAsync(id);
			if (user == null) return NotFound();

			updatedUser.userId = id; // Ensure ID stays the same
			await _userService.UpdateUserAsync(id, updatedUser);
			return NoContent();
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteUser(string id)
		{
			var user = await _userService.GetUserByIdAsync(id);
			if (user == null) return NotFound();

			await _userService.DeleteUserAsync(id);
			return NoContent();
		}
	}
}
