using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace HopIn_Server.Models
{
	public class Message
	{
		[BsonId]
		[BsonRepresentation(BsonType.ObjectId)]
		public string? msgId { get; set; } = ObjectId.GenerateNewId().ToString();

        [BsonRepresentation(BsonType.String)]
		public string senderId { get; set; } = null!;
		public string content { get; set; } = null!;
		public DateTime msgCreatedAt { get; set; } = DateTime.UtcNow;
	}
}
