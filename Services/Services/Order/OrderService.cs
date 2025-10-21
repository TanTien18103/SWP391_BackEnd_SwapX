using BusinessObjects.Enums;
using BusinessObjects.Models;
using BusinessObjects.TimeCoreHelper;
using Repositories.Repositories.OrderRepo;
using Repositories.Repositories.ExchangeBatteryRepo;
using Services.ApiModels;
using Services.ApiModels.Order;
using BusinessObjects.Constants;
using Microsoft.AspNetCore.Http;
using Repositories.Repositories.FormRepo;
using Services.ServicesHelpers;
using Repositories.Repositories.PackageRepo;
using Repositories.Repositories.VehicleRepo;

namespace Services.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IExchangeBatteryRepo _exchangeBatteryRepo;
    private readonly IFormRepo _formRepo;
    private readonly AccountHelper _accountHelper;
    private readonly IPackageRepo _packageRepo;
    private readonly IVehicleRepo _vehicleRepo;

    public OrderService(
        IOrderRepository orderRepository,
        IExchangeBatteryRepo exchangeBatteryRepo,
        IFormRepo formRepo,
        AccountHelper accountHelper,
        IPackageRepo packageRepo,
        IVehicleRepo vehicleRepo
        )
    {
        _orderRepository = orderRepository;
        _exchangeBatteryRepo = exchangeBatteryRepo;
        _formRepo = formRepo;
        _accountHelper = accountHelper;
        _packageRepo = packageRepo;
        _vehicleRepo = vehicleRepo;
    }

    public async Task<ResultModel> CreateOrder(CreateOrderRequest request)
    {
        try
        {
            //Khởi tạo Order cơ bản
            var newOrder = new Order
            {
                OrderId = _accountHelper.GenerateShortGuid(),
                AccountId = request.AccountId,
                Total = request.Total,
                Date = TimeHepler.SystemTimeNow,
                StartDate = TimeHepler.SystemTimeNow,
                UpdateDate = TimeHepler.SystemTimeNow,
            };

            // 3️⃣ Logic theo loại thanh toán
            switch (request.ServiceType)
            {
                case PaymentType.Package:
                    var package = await _packageRepo.GetPackageById(request.ServiceId);
                    if (package == null)
                    {
                        return new ResultModel
                        {
                            IsSuccess = false,
                            ResponseCode = ResponseCodeConstants.NOT_FOUND,
                            Message = ResponseMessageConstantsPackage.PACKAGE_NOT_FOUND,
                            StatusCode = StatusCodes.Status404NotFound
                        };
                    }
                    var vehicle = await _vehicleRepo.GetVehicleById(request.Vin);

                    if(vehicle == null)
                    {
                        return new ResultModel
                        {
                            IsSuccess = false,
                            ResponseCode = ResponseCodeConstants.NOT_FOUND,
                            Message = ResponseMessageConstantsVehicle.VEHICLE_NOT_FOUND,
                            StatusCode = StatusCodes.Status404NotFound
                        };
                    }

                    newOrder.Status = PaymentStatus.Pending.ToString();
                    newOrder.ServiceType = PaymentType.Package.ToString();
                    newOrder.UpdateDate = TimeHepler.SystemTimeNow;
                    newOrder.ServiceId = request.ServiceId;

                    await _orderRepository.CreateOrderAsync(newOrder);

                    return new ResultModel
                    {
                        IsSuccess = true,
                        ResponseCode = ResponseCodeConstants.SUCCESS,
                        Message = ResponseMessageOrder.ORDER_CREATED_PENDING_PAYMENT,
                        StatusCode = StatusCodes.Status200OK,
                        Data = newOrder
                    };
                case PaymentType.PrePaid:
                    //Kiểm tra form tồn tại
                    var PrePaidform = await _formRepo.GetById(request.ServiceId);
                    if (PrePaidform == null)
                    {
                        return new ResultModel
                        {
                            IsSuccess = false,
                            ResponseCode = ResponseCodeConstants.NOT_FOUND,
                            Message = ResponseMessageConstantsForm.FORM_NOT_FOUND,
                            StatusCode = StatusCodes.Status404NotFound
                        };
                    }

                    newOrder.Status = PaymentStatus.Pending.ToString();
                    newOrder.ServiceType = PaymentType.PrePaid.ToString();
                    newOrder.ServiceId = PrePaidform.FormId; // ServiceId = FormId

                    //Order được tạo trước, sẽ gán vào Form khi Form được approve
                    await _orderRepository.CreateOrderAsync(newOrder);

                    return new ResultModel
                    {
                        IsSuccess = true,
                        ResponseCode = ResponseCodeConstants.SUCCESS,
                        Message = ResponseMessageOrder.ORDER_CREATED_PENDING_PAYMENT,
                        StatusCode = StatusCodes.Status200OK,
                        Data = newOrder
                    };

                case PaymentType.UsePackage:
                    //Kiểm tra form tồn tại
                    var usePackageForm = await _formRepo.GetById(request.ServiceId);
                    if (usePackageForm == null)
                    {
                        return new ResultModel
                        {
                            IsSuccess = false,
                            ResponseCode = ResponseCodeConstants.NOT_FOUND,
                            Message = ResponseMessageConstantsForm.FORM_NOT_FOUND,
                            StatusCode = StatusCodes.Status404NotFound
                        };
                    }

                    //Kiểm tra Vehicle tồn tại
                    var vehicle = await _vehicleRepo.GetVehicleById(usePackageForm.Vin);
                    if (vehicle == null)
                    {
                        return new ResultModel
                        {
                            IsSuccess = false,
                            ResponseCode = ResponseCodeConstants.NOT_FOUND,
                            Message = ResponseMessageConstantsVehicle.VEHICLE_NOT_FOUND,
                            StatusCode = StatusCodes.Status404NotFound
                        };
                    }

                    //Kiểm tra gói Package của Vehicle
                    var packageInVehicle = await _vehicleRepo.GetPackageByVehicleId(usePackageForm.Vin);
                    if (packageInVehicle == null)
                    {
                        return new ResultModel
                        {
                            IsSuccess = false,
                            ResponseCode = ResponseCodeConstants.FAILED,
                            Message = ResponseMessageConstantsPackage.PACKAGE_NOT_FOUND_FOR_VEHICLE,
                            StatusCode = StatusCodes.Status400BadRequest
                        };
                    }

                    //Kiểm tra Vehicle có gói không
                    if (vehicle.PackageId == null)
                    {
                        return new ResultModel
                        {
                            IsSuccess = false,
                            ResponseCode = ResponseCodeConstants.FAILED,
                            Message = ResponseMessageConstantsPackage.PACKAGE_NOT_FOUND_FOR_VEHICLE,
                            StatusCode = StatusCodes.Status400BadRequest
                        };
                    }
                    //Kiểm tra hạn sử dụng gói của xe
                    if (vehicle.PackageExpiredate < TimeHepler.SystemTimeNow)
                    {
                        return new ResultModel
                        {
                            IsSuccess = false,
                            ResponseCode = ResponseCodeConstants.FAILED,
                            Message = ResponseMessageConstantsPackage.PACKAGE_EXPIRED_OR_NOT_EXISTS_IN_VEHICLE,
                            StatusCode = StatusCodes.Status400BadRequest
                        };
                    }

                    //Nếu hợp lệ → tạo Order
                    newOrder.Status = PaymentStatus.Paid.ToString();
                    newOrder.ServiceType = PaymentType.UsePackage.ToString();
                    newOrder.ServiceId = usePackageForm.FormId; // ServiceId = FormId

                    await _orderRepository.CreateOrderAsync(newOrder);

                    return new ResultModel
                    {
                        IsSuccess = true,
                        ResponseCode = ResponseCodeConstants.SUCCESS,
                        Message = ResponseMessageOrder.ORDER_CREATED_AND_PAID,
                        StatusCode = StatusCodes.Status200OK,
                        Data = newOrder
                    };

                case PaymentType.PaidAtStation:
                    var exchangeBattery = await _exchangeBatteryRepo.GetById(request.ExchangeBatteryId);
                    if (exchangeBattery == null)
                    {
                        return new ResultModel
                        {
                            IsSuccess = false,
                            ResponseCode = ResponseCodeConstants.NOT_FOUND,
                            Message = ExchangeBatteryMessages.EXCHANGE_BATTERY_NOT_FOUND,
                            StatusCode = StatusCodes.Status404NotFound
                        };
                    }
                    var PaidAtStationform = await _formRepo.GetByStationScheduleId(exchangeBattery.ScheduleId);
                    if (PaidAtStationform == null)
                    {
                        return new ResultModel
                        {
                            IsSuccess = false,
                            ResponseCode = ResponseCodeConstants.NOT_FOUND,
                            Message = ResponseMessageConstantsForm.FORM_NOT_FOUND,
                            StatusCode = StatusCodes.Status404NotFound
                        };
                    }
                    newOrder.Status = PaymentStatus.Pending.ToString();
                    newOrder.ServiceType = PaymentType.PaidAtStation.ToString();
                    newOrder.ServiceId = PaidAtStationform.FormId; // ServiceId = FormId

                    await _orderRepository.CreateOrderAsync(newOrder);

                    exchangeBattery.OrderId = newOrder.OrderId;
                    await _exchangeBatteryRepo.Update(exchangeBattery);

                    return new ResultModel
                    {
                        IsSuccess = true,
                        ResponseCode = ResponseCodeConstants.SUCCESS,
                        Message = ResponseMessageOrder.ORDER_CREATED_PENDING_PAYMENT,
                        StatusCode = StatusCodes.Status200OK,
                        Data = newOrder
                    };
                default:
                    return new ResultModel
                    {
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.FAILED,
                        Message = ResponseMessageOrder.INVALID_PAYMENT_METHOD,
                        StatusCode = StatusCodes.Status400BadRequest
                    };
            }
        }
        catch (Exception ex)
        {
            return new ResultModel
            {
                IsSuccess = false,
                ResponseCode = ResponseCodeConstants.FAILED,
                Message = ResponseMessageOrder.ORDER_CREATE_FAILED,
                StatusCode = StatusCodes.Status500InternalServerError,
                Data = ex.InnerException?.Message ?? ex.Message
            };
        }
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