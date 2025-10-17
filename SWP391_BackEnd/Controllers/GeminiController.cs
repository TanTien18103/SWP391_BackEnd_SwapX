using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.ApiModels.GeminiAI;
using Services.Services.GeminiService;
namespace SWP391_BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GeminiController : ControllerBase
    {
        private readonly IGeminiService _geminiService;
        public GeminiController(IGeminiService geminiService, IConfiguration config)
        {
            _geminiService = geminiService;
            var apiKey = config["Gemini:ApiKey"];
        }
        [HttpPost("chat_gemini")]
        public async Task<IActionResult> ChatGemini([FromForm] GeminiAIRequest geminiAIRequest)
        {
            var res = await _geminiService.GetGeminiData(geminiAIRequest);
            return StatusCode(res.StatusCode, res);
        }
        [HttpGet("test_gemini")]
        public async Task<IActionResult> AnalyzeBatteryDataAsync()
        {
            var res = await _geminiService.AnalyzeBatteryDataAsync();
            return StatusCode(res.StatusCode, res);
        }
    }
}
