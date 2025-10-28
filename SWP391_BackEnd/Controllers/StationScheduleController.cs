using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.ApiModels.StationSchedule;
using Services.Services.StationScheduleService;

namespace SWP391_BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StationScheduleController : ControllerBase
    {
        private readonly IStationScheduleService _stationScheduleService;
        public StationScheduleController(IStationScheduleService stationScheduleService)
        {
            _stationScheduleService = stationScheduleService;
        }
        [HttpPost("add_station_schedule")]
        [Authorize(Roles = "Admin, Bsstaff")]
        public async Task<IActionResult> AddStationSchedule([FromForm] AddStationScheduleRequest addStationScheduleRequest)
        {
            var res = await _stationScheduleService.AddStationSchedule(addStationScheduleRequest);
            return StatusCode(res.StatusCode, res);
        }
        [HttpGet("get_station_schedule_by_id")]
        [Authorize(Roles = "Admin, Bsstaff")]
        public async Task<IActionResult> GetStationScheduleById([FromQuery] string? stationScheduleId)
        {
            var res = await _stationScheduleService.GetStationScheduleById(stationScheduleId);
            return StatusCode(res.StatusCode, res);
        }
        [HttpPut("delete_station_schedule")]
        [Authorize(Roles = "Admin, Bsstaff")]
        public async Task<IActionResult> DeleteStationSchedule([FromForm] string? stationScheduleId)
        {
            var res = await _stationScheduleService.DeleteStationSchedule(stationScheduleId);
            return StatusCode(res.StatusCode, res);
        }
        [HttpPut("update_station_schedule")]
        [Authorize(Roles = "Admin, Bsstaff")]
        public async Task<IActionResult> UpdateStationSchedule([FromForm] UpdateStationScheduleRequest updateStationScheduleRequest)
        {
            var res = await _stationScheduleService.UpdateStationSchedule(updateStationScheduleRequest);
            return StatusCode(res.StatusCode, res);
        }
        [HttpGet("get_all_station_schedules")]
        [Authorize(Roles = "Admin, Bsstaff")]
        public async Task<IActionResult> GetAllStationSchedules()
        {
            var res = await _stationScheduleService.GetAllStationSchedules();
            return StatusCode(res.StatusCode, res);
        }
        [HttpGet("get_station_schedules_by_station_id")]
        [Authorize(Roles = "Admin, Bsstaff")]
        public async Task<IActionResult> GetStationSchedulesByStationId([FromQuery] string? stationId)
        {
            var res = await _stationScheduleService.GetStationScheduleByStationId(stationId);
            return StatusCode(res.StatusCode, res);
        }
        [HttpPut("update_status_station_schedule")]
        [Authorize(Roles = "Admin, Bsstaff")]
        public async Task<IActionResult> UpdateStatusStationSchedule([FromForm] UpdateStatusStationScheduleRequest updateStatusStationScheduleRequest)
        {
            var res = await _stationScheduleService.UpdateStatusStationSchedule(updateStatusStationScheduleRequest);
            return StatusCode(res.StatusCode, res);
        }

        [HttpGet("get_station_schedules_by_account_id")]
        [Authorize]
        public async Task<IActionResult> GetStationSchedulesByAccountId([FromQuery] string accountId)
        {
            var res = await _stationScheduleService.GetStationSchedulesByAccountId(accountId);
            return StatusCode(res.StatusCode, res);
        }

    }
}
