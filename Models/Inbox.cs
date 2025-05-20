// Models/Inbox.cs
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace HopIn_Server.Models
{
    public class Inbox
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? inboxId { get; set; }

        [BsonRepresentation(BsonType.String)]
        public string userId { get; set; } = null!;

        public List<Chat> inbChats { get; set; } = new List<Chat>();
        public int unreadCount { get; set; } = 0;
        public Dictionary<string, int> unreadPerChat { get; set; } = new Dictionary<string, int>();
        public DateTime inbCreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime inbUpdatedAt { get; set; } = DateTime.UtcNow;
    }
}