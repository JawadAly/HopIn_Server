using HopIn_Server.Models;
using HopIn_Server.Services;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace HopIn_Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RidesController : ControllerBase
    {
        private readonly RideService _rideService;

        public RidesController(RideService rideService)
        {
            _rideService = rideService;
        }

        private IActionResult ApiResponse(int statusCode, bool success, string? message = null, string? errorMsg = null, object? data = null)
        {
            var response = new
            {
                success,
                message,
                errorMsg,
                data
            };
            return StatusCode(statusCode, response);
        }

        [HttpPost]
        public async Task<IActionResult> CreateRide([FromBody] CreateRideRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return ApiResponse(400, false, "Invalid request data", 
                        errorMsg: string.Join(", ", ModelState.Values
                            .SelectMany(v => v.Errors)
                            .Select(e => e.ErrorMessage)));
                }

                var ride = new Ride
                {
                    riderId = request.RiderId,
                    startLocation = request.StartLocation,
                    endLocation = request.EndLocation,
                    rideDateTime = request.RideDateTime,
                    availableSeats = request.AvailableSeats,
                    pricePerSeat = request.PricePerSeat,
                    status = RideStatus.Active,
                    bookedSeats = 0,
                    passengers = new List<PassengerInfo>(),
                    rideCreatedAt = DateTime.UtcNow,
                    rideUpdatedAt = DateTime.UtcNow
                };

                await _rideService.CreateAsync(ride);
                return ApiResponse(200, true, "Ride created successfully", data: ride);
            }
            catch (Exception ex)
            {
                return ApiResponse(500, false, "An unexpected error occurred", errorMsg: ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRide(string id)
        {
            try
            {
                var ride = await _rideService.GetAsync(id);
                if (ride == null)
                {
                    return ApiResponse(404, false, "Ride not found");
                }
                return ApiResponse(200, true, "Ride retrieved successfully", data: ride);
            }
            catch (Exception ex)
            {
                return ApiResponse(500, false, "An unexpected error occurred", errorMsg: ex.Message);
            }
        }

        [HttpPost("{rideId}/passenger")]
        public async Task<IActionResult> AddPassenger(string rideId, [FromBody] AddPassengerRequest request)
        {
            try
            {
                var result = await _rideService.AddPassenger(rideId, request.PassengerId, request.Seats);
                if (!result.success)
                {
                    return ApiResponse(400, false, errorMsg: result.message);
                }
                return ApiResponse(200, true, result.message);
            }
            catch (Exception ex)
            {
                return ApiResponse(500, false, "An unexpected error occurred", errorMsg: ex.Message);
            }
        }
    }

    public class CreateRideRequest
    {
        [Required]
        public string RiderId { get; set; } = null!;

        [Required]
        public string StartLocation { get; set; } = null!;

        [Required]
        public string EndLocation { get; set; } = null!;

        [Required]
        public DateTime RideDateTime { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int AvailableSeats { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal PricePerSeat { get; set; }
    }

    public class AddPassengerRequest
    {
        [Required]
        public string PassengerId { get; set; } = null!;

        [Required]
        [Range(1, int.MaxValue)]
        public int Seats { get; set; }
    }
} 