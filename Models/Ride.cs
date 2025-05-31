// Models/Ride.cs
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;
using System;
using System.ComponentModel.DataAnnotations;

namespace HopIn_Server.Models
{
	public class Ride
	{
		[BsonId]
		[BsonRepresentation(BsonType.ObjectId)]
		[BsonIgnoreIfDefault]
		public string? rideId { get; set; }

		[Required]
		[BsonRepresentation(BsonType.String)]
		public string riderId { get; set; } = null!;

		[Required]
		public string startLocation { get; set; } = null!;
		public Coordinates startCoordinates { get; set; }


		[Required]
		public string endLocation { get; set; } = null!;
		public Coordinates endCoordinates { get; set; }


		[Required]
		public DateTime rideDateTime { get; set; }

		[Required]
		[Range(1, int.MaxValue)]
		public int availableSeats { get; set; }

		public int bookedSeats { get; set; } = 0;

		[Required]
		[Range(0.01, double.MaxValue)]
		public decimal pricePerSeat { get; set; }

		public RideStatus status { get; set; } = RideStatus.Active;
		public List<PassengerInfo> passengers { get; set; } = new List<PassengerInfo>();
		public DateTime rideCreatedAt { get; set; } = DateTime.UtcNow;
		public DateTime rideUpdatedAt { get; set; } = DateTime.UtcNow;
		public int? reqSeats {get;set;}
	}



	public class Coordinates
	{
		public double Latitude { get; set; }
		public double Longitude { get; set; }
	}



	public class PassengerInfo
	{
		[BsonRepresentation(BsonType.String)]
		public string passengerId { get; set; } = null!;
		public int requestedSeats { get; set; }
		public PassengerStatus status { get; set; } = PassengerStatus.Pending;
		public DateTime requestTime { get; set; } = DateTime.UtcNow;
		public DateTime? acceptedTime { get; set; }
	}

	public enum RideStatus
	{
		Active,
		Full,
		InProgress,
		Completed,
		Cancelled
	}

	public enum PassengerStatus
	{
		Pending,
		Accepted,
		Declined,
		Cancelled
	}


}