// Services/ChatService.cs
using HopIn_Server.Configurations;
using HopIn_Server.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace HopIn_Server.Services
{
    public class ChatService
    {
        private readonly IMongoCollection<Chat> _chatCollection;
        private readonly IMongoCollection<Inbox> _inboxCollection;
        private readonly IMongoCollection<Ride> _rideCollection;

        public ChatService(IOptions<DbSettings> databaseSettings)
        {
            var mongoClient = new MongoClient(databaseSettings.Value.connectionString);
            var database = mongoClient.GetDatabase(databaseSettings.Value.dbName);
            _chatCollection = database.GetCollection<Chat>(databaseSettings.Value.collectionNames["chatscoll"]);
            _inboxCollection = database.GetCollection<Inbox>(databaseSettings.Value.collectionNames["inboxescoll"]);
            _rideCollection = database.GetCollection<Ride>(databaseSettings.Value.collectionNames["ridescoll"]);
        }

        public async Task<(bool success, string message, Chat? chat)> CreateChat(string rideId, string passengerId)
        {
            try
            {
                var ride = await _rideCollection.Find(r => r.rideId == rideId).FirstOrDefaultAsync();
                if (ride == null)
                    return (false, "Ride not found", null);

                if (ride.riderId == passengerId)
                    return (false, "Cannot create chat with yourself", null);

                var existingChat = await _chatCollection.Find(c =>
                    c.rideId == rideId &&
                    (c.userId1 == ride.riderId && c.userId2 == passengerId ||
                     c.userId1 == passengerId && c.userId2 == ride.riderId)
                ).FirstOrDefaultAsync();

                if (existingChat != null)
                    return (false, "Chat already exists", existingChat);

                var newChat = new Chat
                {
                    userId1 = ride.riderId,
                    userId2 = passengerId,
                    rideId = rideId,
                    rideStatus = RideStatus.Active,
                    chatCreatedAt = DateTime.UtcNow,
                    chatLastUpdated = DateTime.UtcNow,
                    isActive = true
                };

                await _chatCollection.InsertOneAsync(newChat);
                await UpdateInboxes(newChat);

                return (true, "Chat created successfully", newChat);
            }
            catch (Exception ex)
            {
                return (false, $"Error creating chat: {ex.Message}", null);
            }
        }

        public async Task<(bool success, string statusMessage, Message? message)> SendMessage(string chatId, string senderId, string content)
        {
            try
            {
                var chat = await _chatCollection.Find(c => c.chatId == chatId).FirstOrDefaultAsync();
                if (chat == null)
                    return (false, "Chat not found", null);

                if (chat.userId1 != senderId && chat.userId2 != senderId)
                    return (false, "User is not a participant in this chat", null);

                var newMessage = new Message
                {
                    senderId = senderId,
                    content = content,
                    type = MessageType.Text,
                    msgCreatedAt = DateTime.UtcNow
                };

                var update = Builders<Chat>.Update
                    .Push(c => c.chatMessages, newMessage)
                    .Set(c => c.chatLastUpdated, DateTime.UtcNow);

                await _chatCollection.UpdateOneAsync(c => c.chatId == chatId, update);
                await UpdateInboxUnreadCount(chat, senderId);

                return (true, "Message sent successfully", newMessage);
            }
            catch (Exception ex)
            {
                return (false, $"Error sending message: {ex.Message}", null);
            }
        }

        public async Task<(bool success, string message, List<Message>? messages)> GetChatHistory(string chatId)
        {
            try
            {
                var chat = await _chatCollection.Find(c => c.chatId == chatId).FirstOrDefaultAsync();
                if (chat == null)
                    return (false, "Chat not found", null);

                return (true, "Chat history retrieved successfully", chat.chatMessages);
            }
            catch (Exception ex)
            {
                return (false, $"Error retrieving chat history: {ex.Message}", null);
            }
        }

        public async Task<(bool success, string message, Inbox? inbox)> GetUserInbox(string userId)
        {
            try
            {
                var inbox = await _inboxCollection.Find(i => i.userId == userId).FirstOrDefaultAsync();
                if (inbox == null)
                    return (false, "Inbox not found", null);

                return (true, "Inbox retrieved successfully", inbox);
            }
            catch (Exception ex)
            {
                return (false, $"Error retrieving inbox: {ex.Message}", null);
            }
        }

        private async Task UpdateInboxes(Chat chat)
        {
            // Update rider's inbox
            var riderInbox = await _inboxCollection.Find(i => i.userId == chat.userId1).FirstOrDefaultAsync();
            if (riderInbox == null)
            {
                riderInbox = new Inbox
                {
                    userId = chat.userId1,
                    inbChats = new List<Chat> { chat },
                    inbCreatedAt = DateTime.UtcNow,
                    inbUpdatedAt = DateTime.UtcNow
                };
                await _inboxCollection.InsertOneAsync(riderInbox);
            }
            else
            {
                var update = Builders<Inbox>.Update
                    .Push(i => i.inbChats, chat)
                    .Set(i => i.inbUpdatedAt, DateTime.UtcNow);
                await _inboxCollection.UpdateOneAsync(i => i.userId == chat.userId1, update);
            }

            // Update passenger's inbox
            var passengerInbox = await _inboxCollection.Find(i => i.userId == chat.userId2).FirstOrDefaultAsync();
            if (passengerInbox == null)
            {
                passengerInbox = new Inbox
                {
                    userId = chat.userId2,
                    inbChats = new List<Chat> { chat },
                    inbCreatedAt = DateTime.UtcNow,
                    inbUpdatedAt = DateTime.UtcNow
                };
                await _inboxCollection.InsertOneAsync(passengerInbox);
            }
            else
            {
                var update = Builders<Inbox>.Update
                    .Push(i => i.inbChats, chat)
                    .Set(i => i.inbUpdatedAt, DateTime.UtcNow);
                await _inboxCollection.UpdateOneAsync(i => i.userId == chat.userId2, update);
            }
        }

        private async Task UpdateInboxUnreadCount(Chat chat, string senderId)
        {
            string receiverId = chat.userId1 == senderId ? chat.userId2 : chat.userId1;
            
            var update = Builders<Inbox>.Update
                .Inc(i => i.unreadCount, 1)
                .Set(i => i.inbUpdatedAt, DateTime.UtcNow);

            await _inboxCollection.UpdateOneAsync(i => i.userId == receiverId, update);
        }
    }
}