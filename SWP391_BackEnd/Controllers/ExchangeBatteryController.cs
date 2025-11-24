using Microsoft.AspNetCore.Authorization;

namespace SWP391_BackEnd.Controllers;

using Services.ApiModels.ExchangeBattery;
using Microsoft.AspNetCore.Mvc;
using Services.Services.ExchangeBatteryService;

[ApiController]
[Route("api/[controller]")]
public class ExchangeBatteryController : ControllerBase
{
    private readonly IExchangeBatteryService _exchangeService;

    public ExchangeBatteryController(IExchangeBatteryService exchangeService)
    {
        _exchangeService = exchangeService;
    }


    [HttpPost("create_exchange_battery")]
    [Authorize]
    public async Task<IActionResult> CreateExchange([FromBody] CreateExchangeBatteryRequest request)
    {
        var result = await _exchangeService.CreateExchange(request);
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("get_all_exchange_battery")]
    [Authorize(Roles = "Bsstaff, Admin")]
    public async Task<IActionResult> GetAll()
    {
        var result = await _exchangeService.GetAllExchanges();
        return StatusCode(result.StatusCode, result);
    }


    [HttpGet("get_exchange_battery_by_exchange{id}")]
    [Authorize(Roles = "Bsstaff, Admin")]
    public async Task<IActionResult> GetById(string id)
    {
        var result = await _exchangeService.GetExchangeDetail(id);
        return StatusCode(result.StatusCode, result);
    }


    [HttpGet("get_exchange_by_station/{stationId}")]
    [Authorize(Roles = "Bsstaff, Admin")]
    public async Task<IActionResult> GetByStation(string stationId)
    {
        var result = await _exchangeService.GetExchangeByStation(stationId);
        return StatusCode(result.StatusCode, result);
    }


    [HttpGet("get_exchange_by_driver/{accountId}")]
    [Authorize(Roles = "Bsstaff, Admin")]
    public async Task<IActionResult> GetByDriver(string accountId)
    {
        var result = await _exchangeService.GetExchangeByDriver(accountId);
        return StatusCode(result.StatusCode, result);
    }


    [HttpPut("update_exchange{exchangeId}")]
    [Authorize(Roles = "Bsstaff, Admin")]
    public async Task<IActionResult> UpdateExchange(string exchangeId, [FromBody] UpdateExchangeBatteryRequest request)
    {
        var result = await _exchangeService.UpdateExchange(exchangeId, request);
        return StatusCode(result.StatusCode, result);
    }


    [HttpDelete("delete_exchange{exchangeId}")]
    [Authorize(Roles = "Bsstaff, Admin")]
    public async Task<IActionResult> DeleteExchange(string exchangeId)
    {
        var result = await _exchangeService.DeleteExchange(exchangeId);
        return StatusCode(result.StatusCode, result);
    }
    
    [HttpPut("update_exchange_battery_status")]
    [Authorize]
    public async Task<IActionResult> UpdateExchangeStatus([FromForm] UpdateExchangeStatusRequest request)
    {
        var result = await _exchangeService.UpdateExchangeStatus(request);
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("get_exchanges_by_schedule/{stationscheduleId}")]
    [Authorize(Roles = "Bsstaff, Admin")]
    public async Task<IActionResult> GetExchangesByScheduleId(string stationscheduleId)
    {
        var result = await _exchangeService.GetExchangesByScheduleId(stationscheduleId);
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("get_pending_exchange_by_VIN_and_AccountId")]
    [Authorize]
    public async Task<IActionResult> GetPendingExchangeByVINAndAccountID([FromQuery] string vin, [FromQuery] string accountId)
    {
        var result = await _exchangeService.GetPendingExchangeByVINAndAccountID(vin, accountId);
        return StatusCode(result.StatusCode, result);
    }
}
