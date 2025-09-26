using BusinessObjects.Models;

namespace Repositories.Repositories.OrderRepo;

public interface IOrderRepository
{
    Task<Order> CreateOrderAsync(Order order);
    Task<Order> UpdateOrderStatusAsync(string orderId, string newStatus);
    Task<Order?> GetOrderByIdAsync(string orderId);
}