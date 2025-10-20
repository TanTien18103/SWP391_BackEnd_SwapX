using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Services.ReportService;
namespace SWP391_BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly IReportService _reportService;
        public ReportController(IReportService reportService)
        {
            _reportService = reportService;
        }
        [HttpGet("get_report_by_id")]
        [Authorize]
        public async Task<IActionResult> GetReportById([FromQuery] string? reportId)
        {
            var res = await _reportService.GetReportById(reportId);
            return StatusCode(res.StatusCode, res);
        }
        [HttpGet("get_all_reports")]
        [Authorize]
        public async Task<IActionResult> GetAllReports()
        {
            var res = await _reportService.GetAllReports();
            return StatusCode(res.StatusCode, res);
        }
        [HttpPut("delete_report")]
        [Authorize]
        public async Task<IActionResult> DeleteReport([FromForm] string? reportId)
        {
            var res = await _reportService.DeleteReport(reportId);
            return StatusCode(res.StatusCode, res);
        }
        [HttpPost("add_report")]
        [Authorize]
        public async Task<IActionResult> AddReport([FromForm] Services.ApiModels.Report.AddReportRequest addReportRequest)
        {
            var res = await _reportService.AddReport(addReportRequest);
            return StatusCode(res.StatusCode, res);
        }
        [HttpPut("update_report")]
        [Authorize]
        public async Task<IActionResult> UpdateReport([FromForm] Services.ApiModels.Report.UpdateReportRequest updateReportRequest)
        {
            var res = await _reportService.UpdateReport(updateReportRequest);
            return StatusCode(res.StatusCode, res);
        }

    }
}
