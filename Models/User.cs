using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace HopIn_Server.Models
{
	public class User
	{
		[BsonId]
		[BsonRepresentation(BsonType.String)]
		public string userId { get; set; }
		public string userFirstName { get; set; } = null!;
		public string userLastName { get; set; } = null!;
		public string userEmail { get; set; } = null!;

		public bool receiveEmailUpdates { get; set; } = false;

		[BsonRepresentation(BsonType.ObjectId)]
		public string inboxId { get; set; } = null!; // reference only
		public DateTime userCreatedAt { get; set; } = DateTime.UtcNow;

	}
}
