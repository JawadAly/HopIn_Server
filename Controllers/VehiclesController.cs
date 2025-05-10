using HopIn_Server.Models;
using HopIn_Server.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HopIn_Server.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class VehiclesController : ControllerBase
	{
		private readonly VehicleService _vehicleService;
		public VehiclesController(VehicleService vehicleService) => _vehicleService = vehicleService;

		private IActionResult ApiResponse(int statusCode, bool success, string? message = null,string? errorMsg = null, object? data = null)
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

		//[HttpGet("{id?}")]
		[HttpGet]
		public async Task<IActionResult> getVehicle(String? id) {
			try {
				var result = await _vehicleService.Get(id!);
				if (!result.success) { 
					return ApiResponse(400,result.success,result.message);
				}
				return ApiResponse(200, result.success, result.message,data:result.vehicle);
			}
			catch (Exception e) {
				return ApiResponse(500, false, "An unexpected error occurred.", errorMsg: e.Message);
			}
		}
		[HttpPost]
		public async Task<IActionResult> createVehicle(UserVehicle vehicle)
		{
			try
			{
				var result = await _vehicleService.Create(vehicle);
				if (!result.success) {
					return ApiResponse(400,result.success,result.message);
				}
				return ApiResponse(200, result.success, result.message);
			}
			catch (Exception e)
			{
				return ApiResponse(500, false, "An unexpected error occurred.", errorMsg: e.Message);
			}
		}
	}
}
