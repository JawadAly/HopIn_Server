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
		private readonly ChatService _chatService;
		private readonly UserService _userService;
        private readonly InboxService _inboxService;


		public RidesController(RideService rideService,ChatService chatService, UserService userService, InboxService inboxService)
        {
            _rideService = rideService;
            _chatService = chatService;
            _userService = userService;
            _inboxService = inboxService;
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
                var ride = await _rideService.GetAsync(rideId);
                if (ride == null) 
					return ApiResponse(404, false, "Ride not found!");

				if (ride.status != RideStatus.Active)
				    return ApiResponse(400, false, "Ride is not available for booking");

				if (ride.bookedSeats + request.Seats > ride.availableSeats)
				    return ApiResponse(400, false, "Not enough seats available");

				if (ride.passengers.Any(p => p.passengerId == request.PassengerId))
					return ApiResponse(400, false, "Passenger has already requested this ride.");

                //initiating chat
				var response = await _chatService.InitiateChatAsync(request.PassengerId, ride.riderId);

				if (!response.success)
					return ApiResponse(400, false, "Error initiating chat between the users!");
                //adding chat to respected inboxes

                var riderInboxTask =  _userService.GetUserInbxById(ride.riderId);
				var passengerInboxTask =  _userService.GetUserInbxById(request.PassengerId);
                await Task.WhenAll(riderInboxTask, passengerInboxTask);

				if (riderInboxTask.Result == null)
					return ApiResponse(400, false, "Error getting riders inbox to initiate chat!");

				if (passengerInboxTask.Result == null)
					return ApiResponse(400, false, "Error getting passengers inbox to initiate chat!");


                var resp = await _inboxService.updateInboxChats(response.chat!, riderInboxTask.Result.inboxId);
                if (!resp.success)
					return ApiResponse(400, false, "Error adding chat to riders inbox to initiate chat!");
				var resp1 = await _inboxService.updateInboxChats(response.chat!, passengerInboxTask.Result.inboxId);
				if (!resp1.success)
					return ApiResponse(400, false, "Error adding chat to passengers inbox to initiate chat!");



				var result1 = await _rideService.RequestRide(rideId, request.PassengerId, request.Seats);
                if (!result1.success)
                {
                    return ApiResponse(400, false, errorMsg: result1.message);
                }
                return ApiResponse(200, true, result1.message);
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