using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SWP391_BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StationScheduleController : ControllerBase
    {
        private readonly Services.Services.StationScheduleService.IStationScheduleService _stationScheduleService;
        public StationScheduleController(Services.Services.StationScheduleService.IStationScheduleService stationScheduleService)
        {
            _stationScheduleService = stationScheduleService;
        }
        [HttpPost("add_station_schedule")]
        public async Task<IActionResult> AddStationSchedule([FromForm] Services.ApiModels.StationSchedule.AddStationScheduleRequest addStationScheduleRequest)
        {
            var res = await _stationScheduleService.AddStationSchedule(addStationScheduleRequest);
            return StatusCode(res.StatusCode, res);
        }
        [HttpGet("get_station_schedule_by_id")]
        public async Task<IActionResult> GetStationScheduleById([FromQuery] string? stationScheduleId)
        {
            var res = await _stationScheduleService.GetStationScheduleById(stationScheduleId);
            return StatusCode(res.StatusCode, res);
        }
        [HttpPut("delete_station_schedule")]
        public async Task<IActionResult> DeleteStationSchedule([FromForm] string? stationScheduleId)
        {
            var res = await _stationScheduleService.DeleteStationSchedule(stationScheduleId);
            return StatusCode(res.StatusCode, res);
        }
        [HttpPut("update_station_schedule")]
        public async Task<IActionResult> UpdateStationSchedule([FromForm] Services.ApiModels.StationSchedule.UpdateStationScheduleRequest updateStationScheduleRequest)
        {
            var res = await _stationScheduleService.UpdateStationSchedule(updateStationScheduleRequest);
            return StatusCode(res.StatusCode, res);
        }
        [HttpGet("get_all_station_schedules")]
        public async Task<IActionResult> GetAllStationSchedules()
        {
            var res = await _stationScheduleService.GetAllStationSchedules();
            return StatusCode(res.StatusCode, res);
        }
    }
}
