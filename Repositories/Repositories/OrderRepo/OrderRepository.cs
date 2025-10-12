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
    public async Task<List<Order>> GetAllOrdersAsync()
    {
        return await _context.Orders
            .Include(o => o.Account)
            .Include(o => o.Battery)
            .ToListAsync();
    }

    public async Task<List<Order>> FilterOrdersByStatusAsync(string status)
    {
        return await _context.Orders
            .Where(o => o.Status == status)
            .Include(o => o.Account)
            .Include(o => o.Battery)
            .ToListAsync();
    }

    public async Task<bool> DeleteOrderAsync(string orderId)
    {
        var order = await _context.Orders.FirstOrDefaultAsync(o => o.OrderId == orderId);
        if (order == null) return false;
        _context.Orders.Remove(order);
        await _context.SaveChangesAsync();
        return true;
    }

    public Task<Order> GetOrderByOrderCodeAsync(long orderCode)
    {
        return _context.Orders
            .Include(o => o.Account)
            .Include(o => o.Battery)
            .FirstOrDefaultAsync(o => o.OrderCode == orderCode);
    }

    public async Task<Order> UpdateOrderByOrderCodeAsync(string orderId, long orderCode)
    {
        var order = await _context.Orders.FirstOrDefaultAsync(o => o.OrderId == orderId);
        if (order == null)
        {
            throw new KeyNotFoundException($"Order with ID {orderId} not found");
        }
        order.OrderCode = orderCode;
        order.UpdateDate = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return order;
    }

    public async Task<(List<Order> Orders, int Total)> FilterOrdersAsync(
        string orderId, string status, string accountId, string serviceType, int page, int pageSize)
    {
        var query = _context.Orders
            .Include(o => o.Account)
            .Include(o => o.Battery)
            .AsQueryable();
    
        if (!string.IsNullOrWhiteSpace(orderId))
            query = query.Where(o => o.OrderId == orderId);
    
        if (!string.IsNullOrWhiteSpace(status))
            query = query.Where(o => o.Status == status);
    
        if (!string.IsNullOrWhiteSpace(accountId))
            query = query.Where(o => o.AccountId == accountId);
    
        if (!string.IsNullOrWhiteSpace(serviceType))
            query = query.Where(o => o.ServiceType == serviceType);
    
        // Ensure page and pageSize are valid
        page = page < 1 ? 1 : page;
        pageSize = pageSize < 1 ? 10 : pageSize;
    
        var total = await query.CountAsync();
    
        var orders = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    
        return (orders, total);
    }
}