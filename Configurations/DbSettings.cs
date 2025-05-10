namespace HopIn_Server.Configurations
{
	public class DbSettings
	{
		public string connecitonString { get; set; } = null!;
		public string dbName { get; set; } = null!;
		public Dictionary<string,string> collectionNames { get; set; } = new ();
	}
}
