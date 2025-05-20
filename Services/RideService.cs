using HopIn_Server.Configurations;
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

        public async Task<(bool success, string message)> AddPassenger(string rideId, string passengerId, int seats)
        {
            var ride = await GetAsync(rideId);
            if (ride == null)
                return (false, "Ride not found");

            if (ride.status != RideStatus.Active)
                return (false, "Ride is not available for booking");

            if (ride.bookedSeats + seats > ride.availableSeats)
                return (false, "Not enough seats available");

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
    }
}