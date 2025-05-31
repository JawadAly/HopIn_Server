using HopIn_Server.Configurations;
using HopIn_Server.Dtos;
using HopIn_Server.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace HopIn_Server.Services
{
    public class RideService
    {
        private readonly IMongoCollection<Ride> _rideCollection;

        public RideService(IOptions<DbSettings> databaseSettings)
        {
            var mongoClient = new MongoClient(databaseSettings.Value.connectionString);
            var database = mongoClient.GetDatabase(databaseSettings.Value.dbName);
            _rideCollection = database.GetCollection<Ride>(databaseSettings.Value.collectionNames["ridescoll"]);
        }

        public async Task<List<Ride>> GetAsync() =>
            await _rideCollection.Find(_ => true).ToListAsync();

        public async Task<Ride?> GetAsync(string id) =>
            await _rideCollection.Find(x => x.rideId == id).FirstOrDefaultAsync();

        public async Task CreateAsync(Ride ride) =>
            await _rideCollection.InsertOneAsync(ride);

        public async Task UpdateAsync(string id, Ride ride) =>
            await _rideCollection.ReplaceOneAsync(x => x.rideId == id, ride);

        public async Task RemoveAsync(string id) =>
            await _rideCollection.DeleteOneAsync(x => x.rideId == id);
        //public async Task<(bool success, string? riderId)> GetRidesRider(string rideId) {
        //    var rider = await _rideCollection.Find(x => x.riderId == rideId).FirstOrDefaultAsync();

        //}
        public async Task<(bool success, string message)> RequestRide(string rideId, string passengerId, int seats)
        {
			var passengerInfo = new PassengerInfo
            {
                passengerId = passengerId,
                requestedSeats = seats,
                status = PassengerStatus.Pending,
                requestTime = DateTime.UtcNow
            };

            var update = Builders<Ride>.Update
                .Push(r => r.passengers, passengerInfo)
                .Set(r => r.rideUpdatedAt, DateTime.UtcNow);

			await _rideCollection.UpdateOneAsync(r => r.rideId == rideId, update);

            return (true, "Passenger request added successfully");
        }

        public async Task<List<Ride>> GetPublishedRidesAsync(string userId)
        {

            return await _rideCollection.Find(x => x.riderId == userId).ToListAsync();

        }

		public async Task<(bool success, string message, List<Ride>? searchedRidesList)> SearchRidesAsync(SearchRideDto dto)
		{
			try
			{
				var filterBuilder = Builders<Ride>.Filter;

				var rideDateUtc = dto.rideDate;
				var rideDateStartUtc = rideDateUtc.Date.ToUniversalTime(); // Ensure UTC
				var rideDateEndUtc = rideDateStartUtc.AddDays(1);         // Next day in UTC
				Console.WriteLine($"Received rideDate (UTC): {dto.rideDate}");

				var filter = filterBuilder.Eq(r => r.startLocation, dto.startLocation) &
							 filterBuilder.Eq(r => r.endLocation, dto.endLocation) &
							 filterBuilder.Gte(r => r.rideDateTime, rideDateStartUtc) &
							 filterBuilder.Lt(r => r.rideDateTime, rideDateEndUtc) &
							 filterBuilder.Gte(r => r.availableSeats, dto.reqSeats) &
							 filterBuilder.Eq(r => r.status, RideStatus.Active);

				Console.WriteLine($"Filtering rides from (UTC): {rideDateStartUtc} to {rideDateEndUtc}");

				var rides = await _rideCollection.Find(filter).ToListAsync();

				if (rides == null || !rides.Any())
				{
					return (false, "No rides found!", null);
				}

				return (true, "Rides found", rides);
			}
			catch (Exception ex)
			{
				return (false, $"Error: {ex.Message}", null);
			}
		}


	}
}