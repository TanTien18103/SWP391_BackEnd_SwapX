using BusinessObjects.Models;

namespace Repositories.Repositories.OrderRepo;

public interface IOrderRepository
{
    Task<Order> CreateOrderAsync(Order order);
    Task<Order> UpdateOrderStatusAsync(string orderId, string newStatus);
    Task<Order?> GetOrderByIdAsync(string orderId);
    Task<List<Order>> GetAllOrdersAsync();
    Task<List<Order>> FilterOrdersByStatusAsync(string status);
    Task<bool> DeleteOrderAsync(string orderId);
    Task<Order> GetOrderByOrderCodeAsync(long orderCode);

    public Task<(List<Order> Orders, int Total)> FilterOrdersAsync(string orderId, string status, string accountId,
        string serviceType, int page, int pageSize);
}