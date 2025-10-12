using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.ApiModels.Battery;
using Services.ApiModels.Station;
using Services.Services.BatteryService;

namespace SWP391_BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BatteryController : ControllerBase
    {
        private readonly IBatteryService _batteryService;
        public BatteryController(IBatteryService batteryService)
        {
            _batteryService = batteryService;
        }
        [HttpPost("add-battery")]
        public async Task<IActionResult> AddBattery([FromForm] Services.ApiModels.Battery.AddBatteryRequest addBatteryRequest)
        {
            var res = await _batteryService.AddBattery(addBatteryRequest);
            return StatusCode(res.StatusCode, res);
        }
        [HttpGet("get-battery-by-id")]
        public async Task<IActionResult> GetBatteryById([FromQuery] string? batteryId)
        {
            var res = await _batteryService.GetBatteryById(batteryId);
            return StatusCode(res.StatusCode, res);
        }
        [HttpPut("delete-battery")]
        public async Task<IActionResult> DeleteBattery([FromForm] string? batteryId)
        {
            var res = await _batteryService.DeleteBattery(batteryId);
            return StatusCode(res.StatusCode, res);
        }
        [HttpPut("update-battery")]
        public async Task<IActionResult> UpdateBattery([FromForm] Services.ApiModels.Battery.UpdateBatteryRequest updateBatteryRequest)
        {
            var res = await _batteryService.UpdateBattery(updateBatteryRequest);
            return StatusCode(res.StatusCode, res);
        }
        [HttpPut("add-battery-in-station")]
        public async Task<IActionResult> addBatteryInStation([FromForm] Services.ApiModels.Battery.AddBatteryInStationRequest addBatteryInStationRequest)
        {
            var res = await _batteryService.AddBatteryInStation(addBatteryInStationRequest);
            return StatusCode(res.StatusCode, res);
        }
        [HttpGet("get-all-batteries")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllBatteries()
        {
            var res = await _batteryService.GetAllBatteries();
            return StatusCode(res.StatusCode, res);
        }

        [HttpPut("update_battery_in_station_status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateBatteryStatus([FromForm] UpdateBatteryStatusRequest updateBatteryStatusRequest)
        {
            var res = await _batteryService.UpdateBatteryStatusInStation(updateBatteryStatusRequest);
            return StatusCode(res.StatusCode, res);
        }
        [HttpGet("get_batteries_suit_vehicle")]
        [Authorize(Roles ="EvDriver")]
        public async Task<IActionResult> GetAllBatterySuitVehicle([FromQuery] GetAllBatterySuitVehicle getAllBatterySuitVehicle)
        {
            var res = await _batteryService.GetAllBatterySuitVehicle(getAllBatterySuitVehicle);
            return StatusCode(res.StatusCode, res);
        }
        [HttpPost("create_battery_by_vehicle_name")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateBatteryByVehicleName([FromForm] CreateBatteryByVehicleNameRequest createBatteryByVehicleName)
        {
            var res = await _batteryService.CreateBatteryByVehicleName(createBatteryByVehicleName);
            return StatusCode(res.StatusCode, res);
        }
    }
}
