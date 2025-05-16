using HopIn_Server.Configurations;
using HopIn_Server.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace HopIn_Server.Services
{
	public class VehicleService
	{
		private readonly IMongoCollection<UserVehicle> _vehicleCollection;
		public VehicleService(IOptions<DbSettings> databaseSettings) {
			var mongoClient = new MongoClient(databaseSettings.Value.connecitonString);
			var databaseName = mongoClient.GetDatabase(databaseSettings.Value.dbName);
			_vehicleCollection = databaseName.GetCollection<UserVehicle>(databaseSettings.Value.collectionNames["vehiclesColl"]);
		}
		public async Task<(bool success, string message, List<UserVehicle>? vehiclesList)> GetVs(string uId)
		{
			if (String.IsNullOrWhiteSpace(uId))
			{
				return (false, "User ID is required to fetch vehicles!", null);
			}
			var incomingVehicles = await _vehicleCollection.Find(x=>x.uId == uId).ToListAsync();
			if (incomingVehicles.Any())
			{
				return (true, "Vehicles found!", incomingVehicles);
			}
			return (false, "This user doesnt have any listed vehicles!",null);
		}
		public async Task<(bool success, string message, UserVehicle? vehicle)> Get(string id)
		{
			if (String.IsNullOrWhiteSpace(id))
			{
				return (false, "Vehicle ID is required!", null);
			}
			var incomingVehicle = await _vehicleCollection.Find(x => x.vehicleId == id).FirstOrDefaultAsync();
			if (incomingVehicle != null)
			{
				return (true, "Vehicle found!", incomingVehicle);
			}
			return (false, "No such vehicle found!", null);

		}
		public async Task<(bool success, string message)> Create(UserVehicle vehicle) {
			if (string.IsNullOrEmpty(vehicle.brand) ||
				string.IsNullOrEmpty(vehicle.uId) ||
				string.IsNullOrEmpty(vehicle.model) ||
				string.IsNullOrEmpty(vehicle.color)) {
				return (false,"Please provide the required fields!");
			}
			//there should also be a validation to check user exists/not!
			var isExisting = await _vehicleCollection.Find(x => x.vehicleId == vehicle.vehicleId).FirstOrDefaultAsync();
			if (isExisting != null) { 
				return (false, "This vehicle already exists!");
			}
			await _vehicleCollection.InsertOneAsync(vehicle);
			return (true, "Vehicle added successfully!");
		}
		//public async Task UpdateAsync(UserVehicle vehicle) => await _vehicleCollection.ReplaceOneAsync(x => x.vehicleId == vehicle.vehicleId,vehicle);
		public async Task DeleteAsync(string id) => await _vehicleCollection.DeleteOneAsync(x => x.vehicleId == id);
		public async Task<(bool success, string message)> DeleteVehicle(string id) {
			if (string.IsNullOrEmpty(id)) {
				return (false, "Vehicle ID is required!");
			}
			//checking if vehicles exists or not
			var incomingVehicle = await _vehicleCollection.Find(x => x.vehicleId == id).FirstOrDefaultAsync();
			if (incomingVehicle == null) {
				return (false,"No such vehicle exists!");
			}
			await _vehicleCollection.DeleteOneAsync(x => x.vehicleId == id);
			return (true,"Vehicle Removed Successfully!");
		}

	}
}
