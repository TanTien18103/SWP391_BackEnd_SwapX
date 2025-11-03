using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.ApiModels;
using Services.Services.Dashboard;
using System.Threading.Tasks;

namespace Controllers
{
    [ApiController]
    [Route("api/dashboard")]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;
        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet("summary")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetDashboardSummary()
        {
            var summary = await _dashboardService.GetDashboardSummaryAsync();
            return Ok(summary);
        }
        [HttpGet("total_exchange_battery")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetTotalExchangeBattery()
        {
            var summary = await _dashboardService.GetDashboardSummaryAsync();
            return Ok(summary.TotalExchangeBatteries);
        }
        [HttpGet("total_user")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetTotalUser()
        {
            var summary = await _dashboardService.GetDashboardSummaryAsync();
            return Ok(summary.TotalAccounts);
        }
        [HttpGet("total_revenue")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetTotalRevenue()
        {
            var summary = await _dashboardService.GetDashboardSummaryAsync();
            return Ok(summary.TotalOrders);
        }
        [HttpPost("show_dashboard")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ShowDashboard([FromForm] DashboardFilterRequest dashboardFilterRequest)
        {
            var res = await _dashboardService.ShowDashboard(dashboardFilterRequest);
            return StatusCode(res.StatusCode, res);
        }

    }
}

