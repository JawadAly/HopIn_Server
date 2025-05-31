using HopIn_Server.Configurations;
using HopIn_Server.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace HopIn_Server.Services
{
	public class UserService
	{
		private readonly IMongoCollection<User> _userCollection;
		private readonly InboxService _inboxService;

		public UserService(IOptions<DbSettings> databaseSettings, InboxService inboxService)
		{
			var mongoClient = new MongoClient(databaseSettings.Value.connectionString);
			var databaseName = mongoClient.GetDatabase(databaseSettings.Value.dbName);
			_userCollection = databaseName.GetCollection<User>(databaseSettings.Value.collectionNames["userColl"]);
			_inboxService = inboxService;
		}










		public async Task<List<User>> GetAllUsersAsync() =>
	await _userCollection.Find(_ => true).ToListAsync();





		public async Task<User?> GetUserByIdAsync(string id) =>
			await _userCollection.Find(u => u.userId == id).FirstOrDefaultAsync();





		public async Task<(bool success, string message)> CreateUserAsync(User newUser)
		{

			await _userCollection.InsertOneAsync(newUser);
			return (true, "User created successfully.");
		}
		public async Task<Inbox?> GetUserInbxById(string userId) {
			var incomingUser = await _userCollection.Find(x=>x.userId == userId).FirstOrDefaultAsync();
			if (string.IsNullOrEmpty(incomingUser.inboxId))
				return null;
			return await _inboxService.GetInboxByIdAsync(incomingUser.inboxId);
		}








		public async Task UpdateUserAsync(string id, User updatedUser) =>
			await _userCollection.ReplaceOneAsync(u => u.userId == id, updatedUser);









		public async Task DeleteUserAsync(string id) =>
			await _userCollection.DeleteOneAsync(u => u.userId == id);



	}
}
