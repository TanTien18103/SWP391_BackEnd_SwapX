using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SWP391_BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VehicleController : ControllerBase
    {
        private readonly Services.Services.VehicleService.IVehicleService _vehicleService;
        public VehicleController(Services.Services.VehicleService.IVehicleService vehicleService)
        {
            _vehicleService = vehicleService;
        }
        [HttpPost("add_vehicle")]
        public async Task<IActionResult> AddVehicle([FromForm] Services.ApiModels.Vehicle.AddVehicleRequest addVehicleRequest)
        {
            var res = await _vehicleService.AddVehicle(addVehicleRequest);
            return StatusCode(res.StatusCode, res);
        }
        [HttpGet("get_vehicle_by_id")]
        public async Task<IActionResult> GetVehicleById([FromQuery] string? vehicleId)
        {
            var res = await _vehicleService.GetVehicleById(vehicleId);
            return StatusCode(res.StatusCode, res);
        }
        [HttpPut("delete_vehicle")]
        public async Task<IActionResult> DeleteVehicle([FromForm] string? vehicleId)
        {
            var res = await _vehicleService.DeleteVehicle(vehicleId);
            return StatusCode(res.StatusCode, res);
        }
        [HttpPut("update_vehicle")]
        public async Task<IActionResult> UpdateVehicle([FromForm] Services.ApiModels.Vehicle.UpdateVehicleRequest updateVehicleRequest)
        {
            var res = await _vehicleService.UpdateVehicle(updateVehicleRequest);
            return StatusCode(res.StatusCode, res);
        }
        [HttpGet("get_all_vehicles")]
        public async Task<IActionResult> GetAllVehicles()
        {
            var res = await _vehicleService.GetAllVehicles();
            return StatusCode(res.StatusCode, res);
        }
    }
}
