using Json.Schema;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.ApiModels.Battery;
using Services.ApiModels.Station;
using Services.Services.BatteryService;
using System.ComponentModel.DataAnnotations;

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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddBattery([FromForm] Services.ApiModels.Battery.AddBatteryRequest addBatteryRequest)
        {
            var res = await _batteryService.AddBattery(addBatteryRequest);
            return StatusCode(res.StatusCode, res);
        }
        [HttpGet("get-battery-by-id")]
        [Authorize]
        public async Task<IActionResult> GetBatteryById([FromQuery] string? batteryId)
        {
            var res = await _batteryService.GetBatteryById(batteryId);
            return StatusCode(res.StatusCode, res);
        }
        [HttpPut("delete-battery")]
        [Authorize(Roles = "Admin, Bsstaff")]
        public async Task<IActionResult> DeleteBattery([FromForm] string? batteryId)
        {
            var res = await _batteryService.DeleteBattery(batteryId);
            return StatusCode(res.StatusCode, res);
        }
        [HttpPut("update-battery")]
        [Authorize(Roles = "Admin, Bsstaff")]
        public async Task<IActionResult> UpdateBattery([FromForm] UpdateBatteryRequest updateBatteryRequest)
        {
            var res = await _batteryService.UpdateBattery(updateBatteryRequest);
            return StatusCode(res.StatusCode, res);
        }
        [HttpPut("add-battery-in-station")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> addBatteryInStation([FromForm] AddBatteryInStationRequest addBatteryInStationRequest)
        {
            var res = await _batteryService.AddBatteryInStation(addBatteryInStationRequest);
            return StatusCode(res.StatusCode, res);
        }
        [HttpGet("get-all-batteries")]
        [Authorize(Roles = "Admin, Bsstaff")]
        public async Task<IActionResult> GetAllBatteries()
        {
            var res = await _batteryService.GetAllBatteries();
            return StatusCode(res.StatusCode, res);
        }

        [HttpPut("update_battery_in_station_status")]
        [Authorize(Roles = "Admin, Bsstaff")]
        public async Task<IActionResult> UpdateBatteryStatus([FromForm] UpdateBatteryStatusRequest updateBatteryStatusRequest)
        {
            var res = await _batteryService.UpdateBatteryStatusInStation(updateBatteryStatusRequest);
            return StatusCode(res.StatusCode, res);
        }
        [HttpGet("get_batteries_suit_vehicle")]
        [Authorize]
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
        [HttpPut("delete_battery_in_station")]
        [Authorize(Roles = "Admin, Bsstaff")]
        public async Task<IActionResult> DeleteBatteryInStation([FromForm] string? batteryId)
        {
            var res = await _batteryService.DeleteBatteryInStation(batteryId);
            return StatusCode(res.StatusCode, res);
        }
        [HttpGet("get_all_page_batteries")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllPageBatteries([FromQuery, Range(1, int.MaxValue, ErrorMessage = "Số trang phải lớn hơn 0")]  int pageNum, [FromQuery, Range(1, int.MaxValue, ErrorMessage = "Kích thước trang phải lớn hơn 0")] int pageSize)
        {
            var res = await _batteryService.GetAllBatteriesPage(pageNum, pageSize);
            return StatusCode(res.StatusCode, res);
        }
    }
}
