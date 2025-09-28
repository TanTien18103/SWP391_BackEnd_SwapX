using BusinessObjects.Enums;
using BusinessObjects.Models;
using BusinessObjects.TimeCoreHelper;
using Repositories.Repositories.OrderRepo;

namespace Services.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;

    public OrderService(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<Order> CreateOrderAsync(Order order)
    {
        order.Status = PaymentStatus.Pending.ToString();
        order.StartDate = TimeHepler.SystemTimeNow;
        order.UpdateDate = TimeHepler.SystemTimeNow;

        return await _orderRepository.CreateOrderAsync(order);
    }

    public async Task<Order> UpdateOrderStatusAsync(string orderId, string newStatus)
    {
        if (string.IsNullOrEmpty(newStatus))
        {
            throw new ArgumentException("Status cannot be empty");
        }

        return await _orderRepository.UpdateOrderStatusAsync(orderId, newStatus);
    }

    public async Task<Order?> GetOrderByIdAsync(string orderId)
    {
        return await _orderRepository.GetOrderByIdAsync(orderId);
    }
}