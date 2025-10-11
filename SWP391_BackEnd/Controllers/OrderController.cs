using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Services;
using Services.ApiModels.Order;

namespace Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrderController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllOrders()
    {
        var result = await _orderService.GetAllOrders();
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("{orderId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetOrderById(string orderId)
    {
        var result = await _orderService.GetOrderById(orderId);
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("filter")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> FilterOrders([FromQuery] OrderFilterRequest request)
    {
        var result = await _orderService.FilterOrders(request);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
    {
        var result = await _orderService.CreateOrder(request);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPut("{orderId}/status")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateOrderStatus(string orderId, [FromBody] string newStatus)
    {
        var result = await _orderService.UpdateOrderStatus(orderId, newStatus);
        return StatusCode(result.StatusCode, result);
    }
}