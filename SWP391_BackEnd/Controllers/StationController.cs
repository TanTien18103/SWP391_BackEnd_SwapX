using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Services.StationService;
using Microsoft.AspNetCore.Http;
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
    public async Task<IActionResult> GetStationByIdForAdmin([FromQuery] string? stationId)
    {
        var res = await _stationService.GetStationById(stationId);
        return StatusCode(res.StatusCode, res);
    }

    [HttpPut("delete_station_for_admin")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteStationForAdmin([FromQuery] string? stationId)
    {
        var res = await _stationService.DeleteStation(stationId);
        return StatusCode(res.StatusCode, res);
    }

    [HttpPost("add_staff_to_station_for_admin")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AddStaffToStationForAdmin([FromBody] AddStaffToStationRequest addStaffToStationRequest)
    {
        var res = await _stationService.AddStaffToStation(addStaffToStationRequest);
        return StatusCode(res.StatusCode, res);
    }

    [HttpGet("get_staffs_by_station_id_for_admin")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetStaffsByStationIdForAdmin([FromQuery] string? stationId)
    {
        var res = await _stationService.GetStaffsByStationId(stationId);
        return StatusCode(res.StatusCode, res);
    }
    
    [HttpDelete("remove_staff_from_station_for_admin")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> RemoveStaffFromStationForAdmin([FromQuery] string? stationId, [FromQuery] string? staffId)
    {
        var res = await _stationService.RemoveStaffFromStation(stationId, staffId);
        return StatusCode(res.StatusCode, res);
    }

    [HttpGet("get_station_by_staff_id_for_staff")]
    [Authorize(Roles = "Staff")]
    public async Task<IActionResult> GetStationByStaffIdForStaff([FromQuery] string? staffId)
    {
        var res = await _stationService.GetStationByStaffId(staffId);
        return StatusCode(res.StatusCode, res);
    }

    [HttpPut("update_station_status")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateStationStatus([FromForm] UpdateStationStatusRequest updateStationStatusRequest)
    {
        var res = await _stationService.UpdateStationStatus(updateStationStatusRequest);
        return StatusCode(res.StatusCode, res);
    }
    [HttpGet("get_all_station_of_customer")]
    [Authorize(Roles = "EvDriver")]
    public async Task<IActionResult> GetAllStationOfCustomer()
    {
        var res = await _stationService.GetAllStationOfCustomer();
        return StatusCode(res.StatusCode, res);
    }
}
