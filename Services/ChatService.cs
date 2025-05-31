using HopIn_Server.Configurations;
using HopIn_Server.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace HopIn_Server.Services
{
	public class ChatService
	{
		private readonly IMongoCollection<Chat> _chatCollection;
		public ChatService(IOptions<DbSettings> databaseSettings)
		{
			var mongoCLient = new MongoClient(databaseSettings.Value.connectionString);
			var dbName = mongoCLient.GetDatabase(databaseSettings.Value.dbName);
			_chatCollection = dbName.GetCollection<Chat>(databaseSettings.Value.collectionNames["chatsColl"]);
		}
		//action methods
		public async Task<(bool success, string? message)> doesChatExists(string chatid)
		{
			if (string.IsNullOrWhiteSpace(chatid))
			{
				return (false, "Chat ID cant be null!");
			}
			var incomingChat = await _chatCollection.Find(x => x.chatId == chatid).FirstOrDefaultAsync();
			if (incomingChat == null)
			{
				return (false, "No such chat exists!");
			}
			return (true, "Chat found!");
		}
		public async Task<(bool success, string message)> addChat(Chat chat)
		{
			if (string.IsNullOrEmpty(chat.person1Id) ||
				string.IsNullOrEmpty(chat.person2Id))
			{
				return (false, "Please provide required fields!");
			}
			//checking if previously chat exists
			var incomingChat = await _chatCollection.Find(x => x.person1Id == chat.person1Id && x.person2Id == chat.person2Id).FirstOrDefaultAsync();
			if (incomingChat != null)
			{
				return (false, "Chat already exists between the specified persons!");
			}
			await _chatCollection.InsertOneAsync(chat);
			return (true, "Chat created successfully!");
		}
		public async Task<(bool success, string message)> updateChatList(Message message, string chatid)
		{
			//var isChatOkAndExists = await doesChatExists(chatid);
			//if (!isChatOkAndExists.success) {
			//	return (false, "No chat exists to add this message!");
			//}
			var update = Builders<Chat>.Update
			.Push(x => x.chatMessages, message)
			.Set(x => x.chatLastUpdated, DateTime.UtcNow);
			await _chatCollection.UpdateOneAsync(x => x.chatId == chatid, update);
			return (true, "Message Sent!");

		}


		public async Task<(bool success, string message,Chat? chat)> InitiateChatAsync(string senderId, string receiverId)
		{
			// Check if chat already exists
			var existingChat = await _chatCollection
				.Find(c => (c.person1Id == senderId && c.person2Id == receiverId) ||
						   (c.person1Id == receiverId && c.person2Id == senderId))
				.FirstOrDefaultAsync();

			var psngrMsg = new Message
			{
				senderId = senderId,
				content = "Hello there! I need a ride ASAP.",
				msgCreatedAt = DateTime.UtcNow
			};

			if (existingChat != null)
			{
				// Chat already exists – add message to chat
				var update = Builders<Chat>.Update
					.Push(c => c.chatMessages, psngrMsg)
					.Set(c => c.chatLastUpdated, DateTime.UtcNow);

				await _chatCollection.UpdateOneAsync(
					c => c.chatId == existingChat.chatId,
					update);

				return (true, "Message added to existing chat",existingChat);
			}
			else
			{
				// Create new chat
				var newChat = new Chat
				{
					person1Id = senderId,
					person2Id = receiverId,
					chatMessages = new List<Message> { psngrMsg },
					chatLastUpdated = DateTime.UtcNow
				};

				await _chatCollection.InsertOneAsync(newChat);
				return (true, "Chat initiated",newChat);
			}
		}

	}
}
