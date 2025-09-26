using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;

namespace Repositories.Repositories.OrderRepo;

public class OrderRepository : IOrderRepository
{
    private readonly SwapXContext _context;

    public OrderRepository(SwapXContext context)
    {
        _context = context;
    }

    public async Task<Order> CreateOrderAsync(Order order)
    {
        order.StartDate = DateTime.UtcNow;
        order.UpdateDate = DateTime.UtcNow;
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();
        return order;
    }

    public async Task<Order> UpdateOrderStatusAsync(string orderId, string newStatus)
    {
        var order = await _context.Orders.FirstOrDefaultAsync(o => o.OrderId == orderId);
        if (order == null)
        {
            throw new KeyNotFoundException($"Order with ID {orderId} not found");
        }

        order.Status = newStatus;
        order.UpdateDate = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return order;
    }

    public async Task<Order?> GetOrderByIdAsync(string orderId)
    {
        return await _context.Orders
            .Include(o => o.Account)
            .Include(o => o.Battery)
            .FirstOrDefaultAsync(o => o.OrderId == orderId);
    }
}