using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repositories.Repositories.Station;
using Services.Services.Station;

namespace SWP391_BackEnd.Controllers;

[Route("api/[controller]")]
[ApiController]
public class StationController : ControllerBase
{
    private readonly IStationRepo _stationRepo;
    private readonly IStationService _stationService;
    public StationController(IStationRepo stationRepo, IStationService stationService)
    {
        _stationRepo = stationRepo;
        _stationService = stationService;
    }
    [HttpPost("add_station_for_admin")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AddStationForAdmin([FromBody] Services.ApiModels.Station.AddStationRequest addStationRequest)
    {
        var res = await _stationService.AddStation(addStationRequest);
        return StatusCode(res.StatusCode, res);
    }

}
