using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace HopIn_Server.Models
{
	public class RideRequest
	{
		[BsonId]
		[BsonRepresentation(BsonType.ObjectId)]
		public string? requestId { get; set; }

		[BsonRepresentation(BsonType.ObjectId)]
		public string rideId { get; set; } = null!;

		[BsonRepresentation(BsonType.String)]
		public string passengerId { get; set; } = null!;

		public int seatsRequested { get; set; }

		/// Status values: "pending", "accepted", "declined"
		public string status { get; set; } = "pending";

		public DateTime requestedAt { get; set; } = DateTime.UtcNow;
	}
}
