using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace HopIn_Server.Models
{
	public class Chat
	{
		[BsonId]
		[BsonRepresentation(BsonType.ObjectId)]
		public string? chatId { get; set; } = ObjectId.GenerateNewId().ToString();

        [BsonRepresentation(BsonType.String)]
		public string person1Id { get; set; } = null!;

		[BsonRepresentation(BsonType.String)]
		public string person2Id { get; set; } = null!;
		public List<Message> chatMessages { get; set; } = new List<Message>();
		public DateTime chatLastUpdated { get; set; } = DateTime.UtcNow;

    }
}
