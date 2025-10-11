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
}