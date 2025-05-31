using HopIn_Server.Configurations;
using HopIn_Server.Models;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Options;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using System;

namespace HopIn_Server.Services
{
	public class InboxService
	{
		private readonly IMongoCollection<Inbox> _inboxCollection;
		public InboxService(IOptions<DbSettings> databaseSetings)
		{
			var mongoClient = new MongoClient(databaseSetings.Value.connectionString);
			var dbName = mongoClient.GetDatabase(databaseSetings.Value.dbName);
			_inboxCollection = dbName.GetCollection<Inbox>(databaseSetings.Value.collectionNames["inboxColl"]);
		}
		//action methods
		public async Task<(bool success, string message)> doesInbExists(string inbId)
		{
			if (string.IsNullOrWhiteSpace(inbId))
			{
				return (false, "Inbox ID is required!");
			}
			var incomingInbox = await _inboxCollection.Find(x => x.inboxId == inbId).FirstOrDefaultAsync();
			if (incomingInbox == null)
			{
				return (false, "Inbox doesn't exists!");
			}
			return (true, "Inbox Exists!");

		}
		public async Task<(bool success, string message)> createInbox(Inbox inbox)
		{
			//checking for previous inbox existance
			var incomingInbox = await _inboxCollection.Find(x => x.inboxId == inbox.inboxId).FirstOrDefaultAsync();
			if (incomingInbox != null)
			{
				return (false, "Inbox already exists!");
			}
			await _inboxCollection.InsertOneAsync(inbox);
			return (true, "Inbox created successfully!");
		}
		public async Task<(bool success, string message)> updateInboxChats(Chat incomingChat, string inbId)
		{
			var update = Builders<Inbox>.Update
				.Push(x => x.inbChats, incomingChat)
				.Set(x => x.inbUpdatedAt, DateTime.UtcNow);
			await _inboxCollection.UpdateOneAsync(x => x.inboxId == inbId, update);
			return (true, "Chat added to inbox!");
		}


		public async Task<Inbox?> GetInboxByIdAsync(string id) => await _inboxCollection.Find(u => u.inboxId == id).FirstOrDefaultAsync();

		public async Task<(bool success,string messsage,List<Chat>? chatsList)> fetchInboxChats(User inbxUser) {
			var userInbox = await _inboxCollection.Find(x => x.inboxId == inbxUser.inboxId).FirstOrDefaultAsync();
			if (userInbox == null) {
				return (false, "No inbox exists!", null);
			}
			if (userInbox.inbChats == null || !userInbox.inbChats.Any())
				return (false, "Inbox found, but no chats exist.", null);

			return (true,"Inbox found with chats!",userInbox.inbChats);
		}
		
	}
}