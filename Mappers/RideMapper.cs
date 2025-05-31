using HopIn_Server.Models;
using HopIn_Server.Dtos;

namespace HopIn_Server.Mappers
{
    public static class RideMapper
    {
        public static Ride ToRide(this CreateRideDto dto)   
        {
            return new Ride
            {
                riderId = dto.riderId,
                startLocation = dto.startLocation,
                startCoordinates = new Coordinates
                {
                    Latitude = dto.startCoordinates.Latitude,
                    Longitude = dto.startCoordinates.Longitude
                },
                endLocation = dto.endLocation,
                endCoordinates = new Coordinates
                {
                    Latitude = dto.endCoordinates.Latitude,
                    Longitude = dto.endCoordinates.Longitude
                },
                rideDateTime = dto.rideDateTime,
                availableSeats = dto.availableSeats,
                bookedSeats = dto.bookedSeats,
                pricePerSeat = dto.pricePerSeat,
                rideCreatedAt = DateTime.UtcNow,
                rideUpdatedAt = DateTime.UtcNow,
                status = RideStatus.Active
            };
        }

		//public static Ride ToSearchRide(this SearchRideDto dto)
		//{
		//	return new Ride
		//	{
		//		startLocation = dto.startLocation,
		//		startCoordinates = new Coordinates
		//		{
		//			Latitude = dto.startCoordinates.Latitude,
		//			Longitude = dto.startCoordinates.Longitude
		//		},
		//		endLocation = dto.endLocation,
		//		endCoordinates = new Coordinates
		//		{
		//			Latitude = dto.endCoordinates.Latitude,
		//			Longitude = dto.endCoordinates.Longitude
		//		},
		//		rideDateTime = dto.rideDate,
		//		reqSeats = dto.reqSeats
		//	};
		//}
	}

}
