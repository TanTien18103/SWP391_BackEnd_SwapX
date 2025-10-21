using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.ApiModels.BatteryReport;
using Services.Services.BatteryReportService;

namespace SWP391_BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BatteryReportController : ControllerBase
    {
        private readonly IBatteryReportService _batteryReportService;
        public BatteryReportController(IBatteryReportService batteryReportService)
        {
            _batteryReportService = batteryReportService;
        }
        [HttpPost("add_battery_report")]
        [Authorize]
        public async Task<IActionResult> AddBatteryReport([FromForm] AddBatteryReportRequest addBatteryReportRequest)
        {
            var res = await _batteryReportService.AddBatteryReport(addBatteryReportRequest);
            return StatusCode(res.StatusCode, res);
        }
        [HttpPut("delete_battery_report")]
        [Authorize]
        public async Task<IActionResult> DeleteBatteryReport([FromForm] string? batteryReportId)
        {
            var res = await _batteryReportService.DeleteBatteryReport(batteryReportId);
            return StatusCode(res.StatusCode, res);
        }
        [HttpGet("get_all_battery_reports")]
        [Authorize]

        public async Task<IActionResult> GetAllBatteryReports()
        {
            var res = await _batteryReportService.GetAllBatteryReports();
            return StatusCode(res.StatusCode, res);
        }
        [HttpGet("get_battery_report_by_id")]
        [Authorize(Roles ="Admin, Bsstaff")]
        public async Task<IActionResult> GetBatteryReportById([FromQuery] string? batteryReportId)
        {
            var res = await _batteryReportService.GetBatteryReportById(batteryReportId);
            return StatusCode(res.StatusCode, res);
        }
        [HttpPut("update_battery_report")]
        [Authorize("Bsstaff, Admin")]
        public async Task<IActionResult> UpdateBatteryReport([FromForm] UpdateBatteryReportRequest updateBatteryReportRequest)
        {
            var res = await _batteryReportService.UpdateBatteryReport(updateBatteryReportRequest);
            return StatusCode(res.StatusCode, res);
        }
        [Authorize(Roles = "Bsstaff, Admin")]
        [HttpGet("get_battery_reports_by_station")]
        public async Task<IActionResult> GetBatteryReportsByStation([FromQuery] string stationId)
        {
            var res = await _batteryReportService.GetBatteryReportsByStation(stationId);
            return StatusCode(res.StatusCode, res);
        }
        [Authorize(Roles = "Bsstaff, Admin")]
        [HttpGet("get_battery_reports_by_battery_id")]
        public async Task<IActionResult> GetBatteryReportsByBatteryId([FromQuery] string batteryId)
        {
            var res = await _batteryReportService.GetBatteryReportsByBatteryId(batteryId);
            return StatusCode(res.StatusCode, res);
        }
    }
}
