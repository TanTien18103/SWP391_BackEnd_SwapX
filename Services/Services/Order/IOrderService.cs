using BusinessObjects.Models;
using Services.ApiModels;
using Services.ApiModels.Order;

namespace Services.Services;
public interface IOrderService
{
    Task<ResultModel> CreateOrder(CreateOrderRequest request);
    Task<ResultModel> UpdateOrderStatus(string orderId, string newStatus);
    Task<ResultModel> GetOrderById(string orderId);
    Task<ResultModel> GetAllOrders();
    Task<ResultModel> FilterOrders(OrderFilterRequest filter);
    Task<ResultModel> DeleteOrder(string orderId);
    Task<ResultModel> GetOrderByServiceId(string serviceId);
    Task<ResultModel> GetOrderByOrderCode(long orderCode);
    Task<ResultModel> PaidInCash(PaidInCashRequest paidInCashRequest);
    Task<ResultModel> GetOrdersByAccountId(string accountId);
}