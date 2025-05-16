using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace HopIn_Server.Models
{
	public class Ride
	{
		[BsonId]
		[BsonRepresentation(BsonType.ObjectId)]
		public string? rideId { get; set; }

		[BsonRepresentation(BsonType.String)]
		public string driverId { get; set; } = null!;

		public string pickupLocation { get; set; } = null!;
		public string dropoffLocation { get; set; } = null!;

		public DateTime departureTime { get; set; }
		public int totalSeats { get; set; } = 4;
		public int availableSeats { get; set; }
		public decimal pricePerSeat { get; set; }

		[BsonRepresentation(BsonType.String)]
		public List<string> acceptedPassengerIds { get; set; } = new List<string>();

		public bool isCompleted { get; set; } = false;
		public DateTime createdAt { get; set; } = DateTime.UtcNow;
	}
}
