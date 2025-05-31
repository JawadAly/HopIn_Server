using HopIn_Server.Models;

namespace HopIn_Server.Dtos
{
    public class RideWithUsernameDto
    {
        public Ride Ride { get; set; }
        public string Username { get; set; } = string.Empty;
    }

}
