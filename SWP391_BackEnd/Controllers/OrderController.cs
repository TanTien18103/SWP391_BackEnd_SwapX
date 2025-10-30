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

    [HttpGet("get_all_orders")]
    [Authorize(Roles = "Admin, Bsstaff")]
    public async Task<IActionResult> GetAllOrders()
    {
        var result = await _orderService.GetAllOrders();
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("get_order_by_{orderId}")]
    [Authorize]
    public async Task<IActionResult> GetOrderById(string orderId)
    {
        var result = await _orderService.GetOrderById(orderId);
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("filter")]
    [Authorize(Roles = "Admin, Bsstaff")]
    public async Task<IActionResult> FilterOrders([FromForm] OrderFilterRequest request)
    {
        var result = await _orderService.FilterOrders(request);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPost("create_order")]
    [Authorize]
    public async Task<IActionResult> CreateOrder([FromForm] CreateOrderRequest request)
    {
        var result = await _orderService.CreateOrder(request);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPut("update_order_{orderId}/status")]
    [Authorize(Roles = "Admin, Bsstaff")]
    public async Task<IActionResult> UpdateOrderStatus(string orderId, string newStatus)
    {
        var result = await _orderService.UpdateOrderStatus(orderId, newStatus);
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("get_order_by_service_id")]
    [Authorize]
    public async Task<IActionResult> GetOrderByServiceId([FromQuery] string serviceId)
    {
        var result = await _orderService.GetOrderByServiceId(serviceId);
        return StatusCode(result.StatusCode, result);
    }
    [HttpGet("get_orders_by_account_id")]
    [Authorize]
    public async Task<IActionResult> GetOrdersByAccountId([FromQuery] string accountId)
    {
        var result = await _orderService.GetOrdersByAccountId(accountId);
        return StatusCode(result.StatusCode, result);
    }
    [HttpGet("get_order_by_order_code")]
    [Authorize]
    public async Task<IActionResult> GetOrderByOrderCode([FromQuery] long orderCode)
    {
        var result = await _orderService.GetOrderByOrderCode(orderCode);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPost("paid_in_cash_at_station")]
    [Authorize(Roles = "Bsstaff, Admin")]
    public async Task<IActionResult> PaidInCashAtStation([FromForm] PaidInCashRequest request)
    {
        var result = await _orderService.PaidInCash(request);
        return StatusCode(result.StatusCode, result);
    }
}