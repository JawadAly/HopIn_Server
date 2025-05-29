using HopIn_Server.Models;
using HopIn_Server.Dtos;

namespace HopIn_Server.Mappers
{
	public static class UserMapper
	{
		public static User ToUser(this CreateUserDto dto, string inboxId)
		{
			return new User
			{
				userId = dto.userId,
				userFirstName = dto.userFirstName,
				userLastName = dto.userLastName,
				userEmail = dto.userEmail,
				receiveEmailUpdates = dto.receiveEmailUpdates,
				inboxId = inboxId,
				userCreatedAt = DateTime.UtcNow
			};
		}
	}
}
