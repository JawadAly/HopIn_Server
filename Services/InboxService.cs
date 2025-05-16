using HopIn_Server.Configurations;
using HopIn_Server.Models;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;

namespace HopIn_Server.Services
{
	public class InboxService
	{
		private readonly IMongoCollection<Inbox> _inboxCollection;
		public InboxService(IOptions<DbSettings> databaseSetings) {
			var mongoClient = new MongoClient(databaseSetings.Value.connectionString);
			var dbName = mongoClient.GetDatabase(databaseSetings.Value.dbName);
			_inboxCollection = dbName.GetCollection<Inbox>(databaseSetings.Value.collectionNames["inboxColl"]);
		}
		//action methods
		public async Task<(bool success, string message)> doesInbExists(string inbId) {
			if (string.IsNullOrWhiteSpace(inbId)) {
				return (false,"Inbox ID is required!");
			}
			var incomingInbox = await _inboxCollection.Find(x => x.inboxId == inbId).FirstOrDefaultAsync();
			if (incomingInbox == null)
			{
				return (false, "Inbox doesn't exists!");
			}
			return (true,"Inbox Exists!");
		}
		public async Task<(bool success, string message)> createInbox(Inbox inbox) {
			//checking for previous inbox existance
			var incomingInbox = await _inboxCollection.Find(x => x.inboxId == inbox.inboxId).FirstOrDefaultAsync();
			if (incomingInbox != null)
			{
				return (false, "Inbox already exists!");
			}
			await _inboxCollection.InsertOneAsync(inbox);
			return (true,"Inbox created successfuly!");
		}
		public async Task<(bool success, string message)> updateInboxChats(Chat incomingChat,string inbId)
		{
			var update = Builders<Inbox>.Update
				.Push(x => x.inbChats, incomingChat)
				.Set(x => x.inbUpdatedAt, DateTime.UtcNow);
			await _inboxCollection.UpdateOneAsync(x => x.inboxId == inbId, update);
			return (true, "Chat added to inbox!");
		}
	}
}
