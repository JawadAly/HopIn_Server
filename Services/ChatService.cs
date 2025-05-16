using HopIn_Server.Configurations;
using HopIn_Server.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace HopIn_Server.Services
{
	public class ChatService
	{
		private readonly IMongoCollection<Chat> _chatCollection;
		public ChatService(IOptions<DbSettings> databaseSettings) {
			var mongoCLient = new MongoClient(databaseSettings.Value.connectionString);
			var dbName = mongoCLient.GetDatabase(databaseSettings.Value.dbName);
			_chatCollection = dbName.GetCollection<Chat>(databaseSettings.Value.collectionNames["chatsColl"]);
		}
		//action methods
		public async Task<(bool success,string? message)> doesChatExists(string chatid) {
			if (string.IsNullOrWhiteSpace(chatid))
			{
				return (false, "Chat ID cant be null!");
			}
			var incomingChat = await _chatCollection.Find(x => x.chatId == chatid).FirstOrDefaultAsync();
			if (incomingChat == null)
			{
				return (false, "No such chat exists!");
			}
			return (true,"Chat found!");
		}
		public async Task<(bool success, string message)> addChat(Chat chat) {
			if (string.IsNullOrEmpty(chat.person1Id) ||
				string.IsNullOrEmpty(chat.person2Id) ) 
			{
				return (false,"Please provide required fields!");
			}
			//checking if previously chat exists
			var incomingChat = await _chatCollection.Find(x => x.person1Id == chat.person1Id && x.person2Id == chat.person2Id).FirstOrDefaultAsync();
			if (incomingChat != null) {
				return (false,"Chat already exists between the specified persons!");
			}
			await _chatCollection.InsertOneAsync(chat);
			return (true,"Chat created successfully!");
		}
		public async Task<(bool success, string message)> updateChatList(Message message,string chatid) {
			//var isChatOkAndExists = await doesChatExists(chatid);
			//if (!isChatOkAndExists.success) {
			//	return (false, "No chat exists to add this message!");
			//}
			var update = Builders<Chat>.Update
				.Push(x => x.chatMessages, message)
				.Set(x => x.chatLastUpdated, DateTime.UtcNow);
			await _chatCollection.UpdateOneAsync(x => x.chatId == chatid,update);
			return (true, "Message Sent!");	

		}
	}
}
