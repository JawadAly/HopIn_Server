// Models/Chat.cs
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace HopIn_Server.Models
{
    public class Chat
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? chatId { get; set; }

        [BsonRepresentation(BsonType.String)]
        public string userId1 { get; set; } = null!;  // Rider's ID

        [BsonRepresentation(BsonType.String)]
        public string userId2 { get; set; } = null!;  // Passenger's ID

        public string rideId { get; set; } = null!;
        public Models.RideStatus rideStatus { get; set; } = Models.RideStatus.Active;  // Using Active instead of Pending
        public int requestedSeats { get; set; } = 1;
        public List<Message> chatMessages { get; set; } = new List<Message>();
        public bool isActive { get; set; } = true;
        public DateTime chatCreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime chatLastUpdated { get; set; } = DateTime.UtcNow;
        public Dictionary<string, DateTime> lastReadBy { get; set; } = new Dictionary<string, DateTime>();
    }
}