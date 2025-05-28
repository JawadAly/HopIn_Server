using HopIn_Server.Configurations;
using HopIn_Server.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace HopIn_Server.Services
{
	public class MessagingService
	{
		private readonly IMongoCollection<Message> _messagesCollection;

		public MessagingService(IOptions<DbSettings> databaseSettings) {
			var mongoClient = new MongoClient(databaseSettings.Value.connectionString);
			var dbName = mongoClient.GetDatabase(databaseSettings.Value.dbName);
			_messagesCollection = dbName.GetCollection<Message>(databaseSettings.Value.collectionNames["messagesColl"]);
		}
		//action methods
		public async Task<(bool success, string message)> AddMessage(Message message) {
			if (string.IsNullOrEmpty(message.senderId) ||
				string.IsNullOrEmpty(message.content) )
			{
				return (false,"Please provide required fields!");
			}
			//checking if message already exists
			var incomingMsg = await _messagesCollection.Find(x => x.msgId == message.msgId).FirstOrDefaultAsync();
			if (incomingMsg != null) {
				return (false,"This message already exists!");
			}
			await _messagesCollection.InsertOneAsync(message);
			return (true, "Message sent!");
		}

		public async Task<(bool success, string message)> DeleteMsg(string msgId ) {
			if (string.IsNullOrWhiteSpace(msgId)) {
				return (false,"Message ID cant be empty!");
			}
			//checking if message exists
			var incomingMessage = await _messagesCollection.Find(x => x.msgId == msgId).FirstOrDefaultAsync();
			if (incomingMessage == null) {
				return (false, "No such message exists!");
			}
			await _messagesCollection.DeleteOneAsync(x => x.msgId == msgId);
			return (true, "Message Deleted!");
		}

	}
}
