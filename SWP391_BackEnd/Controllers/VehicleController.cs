using Microsoft.AspNetCore.Authorization;
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
        [HttpPost("add_vehicle_for_package")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddVehicle([FromForm] Services.ApiModels.Vehicle.AddVehicleRequest addVehicleRequest)
        {
            var res = await _vehicleService.AddVehicle(addVehicleRequest);
            return StatusCode(res.StatusCode, res);
        }
        [HttpGet("get_vehicle_by_id")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetVehicleById([FromQuery] string? vehicleId)
        {
            var res = await _vehicleService.GetVehicleById(vehicleId);
            return StatusCode(res.StatusCode, res);
        }
        [HttpPut("delete_vehicle")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteVehicle([FromForm] string? vehicleId)
        {
            var res = await _vehicleService.DeleteVehicle(vehicleId);
            return StatusCode(res.StatusCode, res);
        }
        [HttpGet("get_all_vehicles")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllVehicles()
        {
            var res = await _vehicleService.GetAllVehicles();
            return StatusCode(res.StatusCode, res);
        }
        [HttpGet("get_vehicle_by_name")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetVehicleByName([FromQuery] BusinessObjects.Enums.VehicleNameEnums vehicleName)
        {
            var res = await _vehicleService.GetVehicleByName(vehicleName);
            return StatusCode(res.StatusCode, res);
        }
        [HttpGet("get_package_by_vehicle_name")]
        [Authorize]
        public async Task<IActionResult> GetPackageByVehicleName([FromQuery] BusinessObjects.Enums.VehicleNameEnums vehicleName)
        {
            var res = await _vehicleService.GetPackageByVehicleName(vehicleName);
            return StatusCode(res.StatusCode, res);
        }
        [HttpPost("link_vehicle")]
        [Authorize(Roles = "EvDriver, Admin")]
        public async Task<IActionResult> LinkVehicle([FromForm] Services.ApiModels.Vehicle.LinkVehicleRequest linkVehicleRequest)
        {
            var res = await _vehicleService.LinkVehicle(linkVehicleRequest);
            return StatusCode(res.StatusCode, res);
        }
        [HttpPut("add_vehicle_in_package")]
        [Authorize(Roles = "EvDriver, Admin")]
        public async Task<IActionResult> AddVehicleInPackage([FromForm] Services.ApiModels.Vehicle.AddVehicleInPackageRequest addVehicleInPackageRequest)
        {
            var res = await _vehicleService.AddVehicleInPackage(addVehicleInPackageRequest);
            return StatusCode(res.StatusCode, res);
        }
        [HttpPut("delete_vehicle_in_package")]
        [Authorize(Roles = "EvDriver")]
        public async Task<IActionResult> DeleteVehicleInPackage([FromForm] string? vehicleId)
        {
            var res = await _vehicleService.DeleteVehicleInPackage(vehicleId);
            return StatusCode(res.StatusCode, res);
        }
        [HttpPut("unlink_vehicle")]
        [Authorize(Roles = "EvDriver")]
        public async Task<IActionResult> UnlinkVehicle([FromForm] string? vehicleId)
        {
            var res = await _vehicleService.UnlinkVehicle(vehicleId);
            return StatusCode(res.StatusCode, res);
        }
        [HttpGet("get_all_vehicle_by_customer_id")]
        [Authorize(Roles = "EvDriver, Admin")]
        public async Task<IActionResult> GetAllVehicleByCustomerId()
        {
            var res = await _vehicleService.GetAllVehicleByCustomerId();
            return StatusCode(res.StatusCode, res);
        }
        [HttpGet("get_package_by_vehicle_id")]
        [Authorize(Roles = "EvDriver, Admin")]
        public async Task<IActionResult> GetPackageByVehicleId([FromQuery] string? vehicleId)
        {
            var res = await _vehicleService.GetPackageByVehicleId(vehicleId);
            return StatusCode(res.StatusCode, res);
        }
        [HttpGet("get_battery_by_vehicle_id")]
        [Authorize(Roles = "EvDriver, Admin")]
        public async Task<IActionResult> GetBatteryByVehicleId([FromQuery] string? vehicleId)
        {
            var res = await _vehicleService.GetBatteryByVehicleId(vehicleId);
            return StatusCode(res.StatusCode, res);
        }
        [HttpGet("get_vehicle_of_user_by_vehicle_id")]
        [Authorize(Roles = "EvDriver, Admin")]
        public async Task<IActionResult> GetVehicleOfUserByVehicleId([FromQuery] string? vehicleId)
        {
            var res = await _vehicleService.GetVehicleOfUserByVehicleId(vehicleId);
            return StatusCode(res.StatusCode, res);
        }
        [HttpPut("add_battery_in_vehicle")]
        [Authorize(Roles = "EvDriver, Admin")]
        public async Task<IActionResult> AddBatteryInVehicle([FromForm] Services.ApiModels.Vehicle.AddBatteryInVehicleRequest addBatteryInVehicleRequest)
        {
            var res = await _vehicleService.AddBatteryInVehicle(addBatteryInVehicleRequest);
            return StatusCode(res.StatusCode, res);
        }
    }

}
