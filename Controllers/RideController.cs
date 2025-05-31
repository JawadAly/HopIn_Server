using HopIn_Server.Dtos;
using HopIn_Server.Mappers;
using HopIn_Server.Models;
using HopIn_Server.Services;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

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
        public async Task<IActionResult> CreateRide([FromBody] CreateRideDto request)
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

                // Use your mapper extension method to convert DTO to domain model
                var ride = request.ToRide();

                // Make sure passengers list is initialized empty
                ride.passengers = new List<PassengerInfo>();

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














        [HttpGet("getpublishedRides/{userId}")]
        public async Task<IActionResult> GetPublishedRides(string userId)
        {
            try
            {
                var result = await _rideService.GetPublishedRidesAsync(userId);

                return ApiResponse(200, true, data: result);

            }
            catch (Exception ex)
            {
                return ApiResponse(500, false, "An unexpected error occurred", errorMsg: ex.Message);

            }
        }













        [HttpPost("search")]
        public async Task<IActionResult> SearchRides([FromBody] SearchRideDto request)
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

                var result = await _rideService.SearchRidesAsync(request);

                if (!result.success && result.message == "No rides found!")
                {
                    return ApiResponse(200, result.success, result.message, data: null);
                }

                return ApiResponse(200, result.success, result.message, data: result.searchedRidesList);
            }
            catch (Exception ex)
            {
                return ApiResponse(500, false, "An unexpected error occurred", errorMsg: ex.Message);
            }
        }








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