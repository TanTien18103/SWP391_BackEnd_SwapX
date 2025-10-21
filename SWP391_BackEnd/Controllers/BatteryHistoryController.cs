using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Services.BatteryHistoryService;

namespace SWP391_BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BatteryHistoryController : ControllerBase
    {
        private readonly IBatteryHistoryService _batteryHistoryService;
        public BatteryHistoryController(IBatteryHistoryService batteryHistoryService)
        {
            _batteryHistoryService = batteryHistoryService;
        }
        [HttpGet("get_battery_history_by_battery_id")]
        [Authorize(Roles = "Admin, Bsstaff")]
        public async Task<IActionResult> GetBatteryHistoryByBatteryId([FromQuery] string? batteryId)
        {
            var res = await _batteryHistoryService.GetBatteryHistoryByBatteryId(batteryId);
            return StatusCode(res.StatusCode, res);
        }
        [HttpGet("get_all_battery_histories")]
        [Authorize(Roles = "Admin, Bsstaff")]
        public async Task<IActionResult> GetAllBatteryHistories()
        {
            var res = await _batteryHistoryService.GetAllBatteryHistory();
            return StatusCode(res.StatusCode, res);
        }
        [HttpGet("get_all_battery_histories_by_stationId")]
        [Authorize(Roles = "Admin, Bsstaff")]
        public async Task<IActionResult> GetAllBatteryHistoriesByStationId([FromQuery] string? stationId)
        {
            var res = await _batteryHistoryService.GetBatteryHistoryByStationId(stationId);
            return StatusCode(res.StatusCode, res);
        }
    }
}
