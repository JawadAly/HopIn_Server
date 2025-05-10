using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace HopIn_Server.Models
{
	public class User
	{
		[BsonId]
		[BsonRepresentation(BsonType.String)]
		public string? userId { get; set; }
		public string userName { get; set; } = null!;
		public string userEmail { get; set; } = null!;
		public string userPhone { get; set; } = null!;
		public DateTime userCreaetedAt { get; set; } = DateTime.UtcNow;

	}
}
