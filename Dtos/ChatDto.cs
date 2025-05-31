using HopIn_Server.Models;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace HopIn_Server.Dtos
{
	public class ChatDto
	{
		public string? chatId { get; set; }
		public User person1 { get; set; } = null!;
		public User person2 { get; set; } = null!;
		public List<Message> chatMessages { get; set; } = new List<Message>();
		public DateTime chatLastUpdated { get; set; } = DateTime.UtcNow;
	}
}
