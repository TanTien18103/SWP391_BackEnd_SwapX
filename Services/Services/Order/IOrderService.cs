using BusinessObjects.Models;

namespace Services.Services;

public interface IOrderService
{
    Task<Order> CreateOrderAsync(Order order);
    Task<Order> UpdateOrderStatusAsync(string orderId, string newStatus);
    Task<Order?> GetOrderByIdAsync(string orderId);
}