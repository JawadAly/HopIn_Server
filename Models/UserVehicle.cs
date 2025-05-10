using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace HopIn_Server.Models
{
	public class UserVehicle
	{
		[BsonId]
		[BsonRepresentation(BsonType.ObjectId)]
		public string? vehicleId { get; set; }

		[BsonRepresentation(BsonType.String)]
		public string uId { get; set; } = null!;
		public string brand { get; set; } = null!;
		public string model { get; set; } = null!;
		public string color { get; set; } = null!;
		public DateTime vehicleCreaetedAt { get; set; } = DateTime.UtcNow;
	}
}
