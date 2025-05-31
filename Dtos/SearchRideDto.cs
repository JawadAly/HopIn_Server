namespace HopIn_Server.Dtos
{
	public class SearchRideDto
	{
		public string startLocation { get; set; } = null!;
		public CoordinatesDto startCoordinates { get; set; } = null!;
		public string endLocation { get; set; } = null!;
		public CoordinatesDto endCoordinates { get; set; } = null!;
		public DateTime rideDate { get; set; }
		public int reqSeats { get; set; }
	}
}
