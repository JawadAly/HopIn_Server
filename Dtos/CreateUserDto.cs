namespace HopIn_Server.Dtos
{
	public class CreateUserDto
	{
		public string userId { get; set; }
		public string userFirstName { get; set; }
		public string userLastName { get; set; }
		public string userEmail { get; set; }
		public bool receiveEmailUpdates { get; set; }




	}

}
