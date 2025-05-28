namespace HopIn_Server.Dtos
{
    public class CoordinatesDto
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

    public class CreateRideDto
    {
        public string riderId { get; set; } = null!;
        public string startLocation { get; set; } = null!;
        public CoordinatesDto startCoordinates { get; set; } = null!;
        public string endLocation { get; set; } = null!;
        public CoordinatesDto endCoordinates { get; set; } = null!;
        public DateTime rideDateTime { get; set; }
        public int availableSeats { get; set; }
        public decimal pricePerSeat { get; set; }
        public int bookedSeats { get; set; } = 0;
    }

}
