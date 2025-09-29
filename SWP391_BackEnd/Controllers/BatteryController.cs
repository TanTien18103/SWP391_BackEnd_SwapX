using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

    }
}
