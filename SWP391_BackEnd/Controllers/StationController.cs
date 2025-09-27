using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repositories.Repositories.Station;
using Services.Services.Station;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.ApiModels.Station;

namespace SWP391_BackEnd.Controllers;

[Route("api/[controller]")]
[ApiController]
public class StationController : ControllerBase
{
    private readonly IStationService _stationService;
    public StationController(IStationService stationService)
    {
        _stationService = stationService;
    }

    [HttpPost("add_station_for_admin")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AddStationForAdmin([FromForm]AddStationRequest addStationRequest)
    {
        var res = await _stationService.AddStation(addStationRequest);
        return StatusCode(res.StatusCode, res);
    }

    [HttpGet("get_all_stations")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllStations()
    {
        var res = await _stationService.GetAllStations();
        return StatusCode(res.StatusCode, res);
    }

    [HttpPut("update_station_for_admin")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateStationForAdmin([FromForm] UpdateStationRequest updateStationRequest)
    {
        var res = await _stationService.UpdateStation(updateStationRequest);
        return StatusCode(res.StatusCode, res);
    }

    [HttpGet("get_station_by_id_for_admin")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetStationByIdForAdmin([FromQuery] string stationId)
    {
        var res = await _stationService.GetStationById(stationId);
        return StatusCode(res.StatusCode, res);
    }

    [HttpPut("delete_station_for_admin")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteStationForAdmin([FromQuery] string stationId)
    {
        var res = await _stationService.DeleteStation(stationId);
        return StatusCode(res.StatusCode, res);
    }
}
