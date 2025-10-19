using BusinessObjects.Enums;
using BusinessObjects.Models;
using BusinessObjects.TimeCoreHelper;
using Repositories.Repositories.OrderRepo;
using Repositories.Repositories.ExchangeBatteryRepo;
using Services.ApiModels;
using Services.ApiModels.Order;

namespace Services.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IExchangeBatteryRepo _exchangeBatteryRepo;

    public OrderService(IOrderRepository orderRepository, IExchangeBatteryRepo exchangeBatteryRepo)
    {
        _orderRepository = orderRepository;
        _exchangeBatteryRepo = exchangeBatteryRepo;
    }

    public async Task<ResultModel> CreateOrder(CreateOrderRequest request)
    {
        var order = new Order
        {
            OrderId = Guid.NewGuid().ToString(),
            AccountId = request.AccountId,
            BatteryId = request.BatteryId,
            Total = request.Total,
            Status = PaymentStatus.Pending.ToString(),
            ServiceId = request.ServiceId,
            ServiceType = request.ServiceType.ToString(),
            StartDate = TimeHepler.SystemTimeNow,
            UpdateDate = TimeHepler.SystemTimeNow
        };

        var createdOrder = await _orderRepository.CreateOrderAsync(order);

        // If an ExchangeBatteryId is provided, link the exchange record to this order
        if (!string.IsNullOrEmpty(request.ExchangeBatteryId))
        {
            var exchange = await _exchangeBatteryRepo.GetById(request.ExchangeBatteryId);
            if (exchange != null)
            {
                exchange.OrderId = createdOrder.OrderId;
                exchange.UpdateDate = TimeHepler.SystemTimeNow;
                await _exchangeBatteryRepo.Update(exchange);
            }
        }

        var response = new OrderResponse(createdOrder);
        return new ResultModel { StatusCode = 201, IsSuccess = true, Data = response };
    }

    public async Task<ResultModel> UpdateOrderStatus(string orderId, string newStatus)
    {
        if (string.IsNullOrEmpty(newStatus))
        {
            return new ResultModel { StatusCode = 400, IsSuccess = false, Message = "Status cannot be empty" };
        }

        var updatedOrder = await _orderRepository.UpdateOrderStatusAsync(orderId, newStatus);
        var response = new OrderResponse(updatedOrder);
        return new ResultModel { StatusCode = 200, IsSuccess = true, Data = response };
    }

    public async Task<ResultModel> GetOrderById(string orderId)
    {
        var order = await _orderRepository.GetOrderByIdAsync(orderId);
        if (order == null)
            return new ResultModel { StatusCode = 404, IsSuccess = false, Message = "Order not found" };

        var response = new OrderResponse(order);
        return new ResultModel { StatusCode = 200, IsSuccess = true, Data = response };
    }

    public async Task<ResultModel> GetAllOrders()
    {
        var orders = await _orderRepository.GetAllOrdersAsync();
        var response = orders.Select(order => new OrderResponse(order)).ToList();
        return new ResultModel { StatusCode = 200, IsSuccess = true, Data = response };
    }

    public async Task<ResultModel> FilterOrders(OrderFilterRequest filter)
    {
        var page = filter.Page < 1 ? 1 : filter.Page;
        var pageSize = filter.PageSize < 1 ? 10 : filter.PageSize;

        var (orders, total) = await _orderRepository.FilterOrdersAsync(
            filter.OrderId, filter.Status, filter.AccountId, filter.ServiceType, page, pageSize);

        var pagedOrders = orders.Select(order => new OrderResponse(order)).ToList();

        return new ResultModel
        {
            StatusCode = 200,
            IsSuccess = true,
            Data = new
            {
                Total = total,
                Page = page,
                PageSize = pageSize,
                Orders = pagedOrders
            }
        };
    }

    public async Task<ResultModel> DeleteOrder(string orderId)
    {
        var success = await _orderRepository.DeleteOrderAsync(orderId);
        if (!success)
            return new ResultModel { StatusCode = 404, IsSuccess = false, Message = "Order not found" };

        return new ResultModel { StatusCode = 200, IsSuccess = true, Message = "Order deleted" };
    }
}