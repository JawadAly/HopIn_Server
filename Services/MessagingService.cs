using HopIn_Server.Configurations;
using HopIn_Server.Models;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace HopIn_Server.Services
{
    public class MessagingService
    {
        private readonly IMongoCollection<Message> _messagesCollection;
        private readonly IMongoCollection<Chat> _chatsCollection;
        private readonly IMongoCollection<Inbox> _inboxesCollection;
        private readonly IMongoCollection<User> _userCollection;

        public MessagingService(IOptions<DbSettings> databaseSettings)
        {
            var mongoClient = new MongoClient(databaseSettings.Value.connectionString);
            var dbName = mongoClient.GetDatabase(databaseSettings.Value.dbName);

            _messagesCollection = dbName.GetCollection<Message>(databaseSettings.Value.collectionNames["messagesColl"]);
            _chatsCollection = dbName.GetCollection<Chat>(databaseSettings.Value.collectionNames["chatsColl"]);
            _inboxesCollection = dbName.GetCollection<Inbox>(databaseSettings.Value.collectionNames["inboxColl"]);
            _userCollection = dbName.GetCollection<User>(databaseSettings.Value.collectionNames["userColl"]); // NEW
        }


        public async Task<(bool success, string message)> AddMessageAndUpdateInboxes(Message message, string chatId)
        {
            if (string.IsNullOrEmpty(message.senderId) || string.IsNullOrEmpty(message.content))
                return (false, "Please provide required fields!");

            var existingMessage = await _messagesCollection.Find(x => x.msgId == message.msgId).FirstOrDefaultAsync();
            if (existingMessage != null)
                return (false, "This message already exists!");

            // Add message
            await _messagesCollection.InsertOneAsync(message);

            // Add to chat
            var chat = await _chatsCollection.Find(c => c.chatId == chatId).FirstOrDefaultAsync();
            if (chat == null)
                return (false, "Chat not found!");

            chat.chatMessages.Add(message);
            chat.chatLastUpdated = DateTime.UtcNow;

            await _chatsCollection.ReplaceOneAsync(c => c.chatId == chatId, chat);

            // Update inboxes for both users
            await UpdateInbox(chat.person1Id, chat);
            await UpdateInbox(chat.person2Id, chat);

            return (true, "Message added and inboxes updated.");
        }

        private async Task UpdateInbox(string userId, Chat updatedChat)
        {
            var user = await _userCollection.Find(x => x.userId == userId).FirstOrDefaultAsync();
            if (user == null || string.IsNullOrEmpty(user.inboxId))
                return;

            var inboxObjectId = user.inboxId;
            var inbox = await _inboxesCollection.Find(x => x.inboxId == inboxObjectId).FirstOrDefaultAsync(); // x.Id is the [BsonId]

            if (inbox == null)
            {
                inbox = new Inbox
                {
                    inboxId = inboxObjectId, // Ensure this matches
                    inbChats = new List<Chat> { updatedChat },
                    inbCreatedAt = DateTime.UtcNow,
                    inbUpdatedAt = DateTime.UtcNow
                };
                await _inboxesCollection.InsertOneAsync(inbox);
            }
            else
            {
                var index = inbox.inbChats.FindIndex(c => c.chatId == updatedChat.chatId);
                if (index >= 0)
                    inbox.inbChats[index] = updatedChat;
                else
                    inbox.inbChats.Add(updatedChat);

                inbox.inbUpdatedAt = DateTime.UtcNow;
                await _inboxesCollection.ReplaceOneAsync(x => x.inboxId == inboxObjectId, inbox);
            }
        }


        public async Task<(bool success, string message)> DeleteMsg(string msgId)
        {
            if (string.IsNullOrWhiteSpace(msgId))
                return (false, "Message ID can't be empty!");

            var incomingMessage = await _messagesCollection.Find(x => x.msgId == msgId).FirstOrDefaultAsync();
            if (incomingMessage == null)
                return (false, "No such message exists!");

            await _messagesCollection.DeleteOneAsync(x => x.msgId == msgId);
            return (true, "Message Deleted!");
        }
    }
}
