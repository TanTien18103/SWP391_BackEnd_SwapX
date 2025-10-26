using BusinessObjects.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace SWP391_BackEnd.Controllers;
[AllowAnonymous]

[ApiController]
[Route("api/[controller]")]
public class PayOSController : ControllerBase
{
    private readonly IPayOSService _payOSService;

    public PayOSController(IPayOSService payOSService)
    {
        _payOSService = payOSService;
    }
    [AllowAnonymous]
    [HttpPost("webhook")]
    public async Task<IActionResult> Webhook([FromBody] PayOSWebhookRequestDto webhook)
    {
        var result = await _payOSService.HandleWebhookAsync(webhook);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPost("create-payment")]
    public async Task<IActionResult> CreatePayment([FromBody] PayOSPaymentRequestDto request)
    {
        var result = await _payOSService.CreatePaymentAsync(request);
        return StatusCode(result.StatusCode, result);
    }
}