// Models/Message.cs
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace HopIn_Server.Models
{
    public class Message
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? messageId { get; set; }

        [BsonRepresentation(BsonType.String)]
        public string senderId { get; set; } = null!;

        public string content { get; set; } = null!;
        public MessageType type { get; set; } = MessageType.Text;
        public string? attachmentUrl { get; set; }
        public bool isRead { get; set; } = false;
        public DateTime msgCreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? msgReadAt { get; set; }
    }

    public enum MessageType
    {
        Text,
        Image,
        Location,
        RideRequest,
        RideAccept,
        RideDecline,
        RideCancel,
        RideComplete,
        SeatUpdate
    }
}