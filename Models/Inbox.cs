using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace HopIn_Server.Models
{
	public class Inbox
	{
		[BsonId]
		[BsonRepresentation(BsonType.ObjectId)]
		public string? inboxId { get; set; } =  ObjectId.GenerateNewId().ToString();
		public List<Chat> inbChats { get; set; } = new List<Chat>();
		public DateTime inbCreatedAt { get; set; } = DateTime.UtcNow;
		public DateTime inbUpdatedAt { get; set; } = DateTime.UtcNow;
	}
}
