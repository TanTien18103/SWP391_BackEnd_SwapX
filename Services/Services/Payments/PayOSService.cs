using BusinessObjects.Constants;
using BusinessObjects.Dtos;
using BusinessObjects.Enums;
using BusinessObjects.Models;
using BusinessObjects.TimeCoreHelper;
using Humanizer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using Net.payOS;
using Net.payOS.Types;
using Repositories.Repositories.BatteryReportRepo;
using Repositories.Repositories.EvDriverRepo;
using Repositories.Repositories.ExchangeBatteryRepo;
using Repositories.Repositories.OrderRepo;
using Repositories.Repositories.PackageRepo;
using Repositories.Repositories.VehicleRepo;
using Services.ApiModels;
using Services.ApiModels.Vehicle;
using Services.Helpers;
using Services.Services.VehicleService;

namespace Services.Payments;

public class PayOSService : IPayOSService
{
    private readonly PayOS _payOS;
    private readonly PayOSHelper _helper;
    private readonly IConfiguration _config;
    private readonly IOrderRepository _orderRepository;
    private readonly IExchangeBatteryRepo _exchangeBatteryRepo;
    private readonly IBatteryReportRepo _batteryRepo;
    private readonly IVehicleRepo _vehicleRepo;
    private readonly IVehicleService _vehicleService;
    private readonly ILogger<PayOSService> _logger;
    private readonly IPackageRepo _packageRepo;
    private readonly IEvDriverRepo _evDriverRepo;

    public PayOSService(
        PayOS payOs,
        PayOSHelper helper,
        IConfiguration config,
        IOrderRepository orderRepository,
        IExchangeBatteryRepo exchangeBatteryRepo,
        IBatteryReportRepo batteryRepo,
        IVehicleRepo vehicleRepo,
        IVehicleService vehicleService,
        ILogger<PayOSService> logger,
        IPackageRepo packageRepo,
        IEvDriverRepo evDriverRepo
        )
    {
        _payOS = payOs;
        _helper = helper;
        _config = config;
        _orderRepository = orderRepository;
        _exchangeBatteryRepo = exchangeBatteryRepo;
        _batteryRepo = batteryRepo;
        _vehicleRepo = vehicleRepo;
        _vehicleService = vehicleService;
        _logger = logger;
        _packageRepo = packageRepo;
        _evDriverRepo = evDriverRepo;
    }

    public async Task<ResultModel<PayOSWebhookResponseDto>> HandleWebhookAsync(PayOSWebhookRequestDto webhook)
    {
        // If webhook verification fails, do not modify other entities. Only return failure.
        if (!_helper.VerifyWebhook(webhook))
        {
            return new ResultModel<PayOSWebhookResponseDto>
            {
                IsSuccess = false,
                StatusCode = 200,
                Message = ExchangeBatteryMessages.CreateFailed,
                Data = new PayOSWebhookResponseDto { Success = false, Message = ExchangeBatteryMessages.CreateFailed }
            };
        }

        var status = webhook.Code == "00" && webhook.Success
            ? PaymentStatus.Paid
            : PaymentStatus.Failed;

        var orderDetail2 = await _orderRepository.GetOrderByOrderCodeAsync(webhook.Data.OrderCode);
        if (orderDetail2 == null)
        {
            return new ResultModel<PayOSWebhookResponseDto>
            {
                IsSuccess = false,
                StatusCode = 200,
                Message = ExchangeBatteryMessages.NotFound,
                Data = new PayOSWebhookResponseDto { Success = false, Message = ExchangeBatteryMessages.NotFound }
            };
        }

        // Only update the order status. Do not touch exchange battery or battery report entities here.
        var updateorder = await _orderRepository.UpdateOrderStatusAsync(orderDetail2.OrderId, status.ToString());

        if (updateorder.ServiceType == PaymentType.Package.ToString() &&
            updateorder.Status == PaymentStatus.Paid.ToString())
        {
            try
            {
                var vehicle = await _vehicleRepo.GetVehicleById(orderDetail2.Vin);
                if (vehicle == null)
                {
                    _logger.LogWarning("Vehicle not found with VIN: {Vin}", orderDetail2.Vin);
                    return new ResultModel<PayOSWebhookResponseDto>
                    {
                        IsSuccess = false,
                        StatusCode = StatusCodes.Status404NotFound,
                        ResponseCode = ResponseCodeConstants.NOT_FOUND,
                        Message = $"Vehicle not found with VIN: {orderDetail2.Vin}",
                        Data = null
                    };
                }

                var package = await _packageRepo.GetPackageById(orderDetail2.ServiceId);
                if (package == null)
                {
                    _logger.LogWarning("Package not found with ID: {ServiceId}", orderDetail2.ServiceId);
                    return new ResultModel<PayOSWebhookResponseDto>
                    {
                        IsSuccess = false,
                        StatusCode = StatusCodes.Status404NotFound,
                        ResponseCode = ResponseCodeConstants.NOT_FOUND,
                        Message = $"Package not found with ID: {orderDetail2.ServiceId}",
                        Data = null
                    };
                }

                if (!string.IsNullOrEmpty(vehicle.PackageId))
                {
                    _logger.LogWarning("Vehicle {Vin} already has a package assigned: {PackageId}", vehicle.Vin, vehicle.PackageId);
                    return new ResultModel<PayOSWebhookResponseDto>
                    {
                        IsSuccess = false,
                        StatusCode = StatusCodes.Status400BadRequest,
                        ResponseCode = ResponseCodeConstants.FAILED,
                        Message = $"Vehicle already has package {vehicle.PackageId}",
                        Data = null
                    };
                }

                var evDriver = await _evDriverRepo.GetDriverByAccountId(orderDetail2.AccountId);
                if (evDriver == null)
                {
                    _logger.LogWarning("EV driver not found with AccountId: {AccountId}", orderDetail2.AccountId);
                    return new ResultModel<PayOSWebhookResponseDto>
                    {
                        IsSuccess = false,
                        StatusCode = StatusCodes.Status404NotFound,
                        ResponseCode = ResponseCodeConstants.NOT_FOUND,
                        Message = $"EV driver not found with AccountId: {orderDetail2.AccountId}",
                        Data = null
                    };
                }

                if (vehicle.CustomerId != evDriver.CustomerId)
                {
                    _logger.LogWarning("Vehicle {Vin} does not belong to customer of AccountId: {AccountId}", vehicle.Vin, orderDetail2.AccountId);
                    return new ResultModel<PayOSWebhookResponseDto>
                    {
                        IsSuccess = false,
                        StatusCode = StatusCodes.Status403Forbidden,
                        ResponseCode = ResponseCodeConstants.FORBIDDEN,
                        Message = $"Vehicle {vehicle.Vin} does not belong to this account",
                        Data = null
                    };
                }

                vehicle.PackageId = orderDetail2.ServiceId;
                vehicle.PackageExpiredate = TimeHepler.SystemTimeNow.AddDays((double)package.ExpiredDate);
                vehicle.UpdateDate = TimeHepler.SystemTimeNow;

                await _vehicleRepo.UpdateVehicle(vehicle);

                _logger.LogInformation("Vehicle {Vin} successfully assigned to package {PackageId}.", vehicle.Vin, package.PackageId);

                return new ResultModel<PayOSWebhookResponseDto>
                {
                    IsSuccess = true,
                    StatusCode = StatusCodes.Status200OK,
                    ResponseCode = ResponseCodeConstants.SUCCESS,
                    Message = "Vehicle assigned to package successfully",
                    Data = new PayOSWebhookResponseDto
                    {
                        Success = true,
                        Message = "Vehicle assigned to package successfully"
                    }
                };

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while adding vehicle to package after payment. OrderId: {OrderId}", updateorder.OrderId);

                return new ResultModel<PayOSWebhookResponseDto>
                {
                    IsSuccess = false,
                    StatusCode = StatusCodes.Status500InternalServerError,
                    ResponseCode = ResponseCodeConstants.FAILED,
                    Message = "Internal error while assigning vehicle to package",
                    Data = new PayOSWebhookResponseDto
                    {
                        Success = false,
                        Message = "Internal error while assigning vehicle to package"
                    }
                };
            }
        }

        var response = new PayOSWebhookResponseDto
        {
            Success = status == PaymentStatus.Paid,
            Message = status == PaymentStatus.Paid ? ExchangeBatteryMessages.CreateSuccess : ExchangeBatteryMessages.CreateFailed
        };

        return new ResultModel<PayOSWebhookResponseDto>
        {
            IsSuccess = status == PaymentStatus.Paid,
            StatusCode = 200,
            Message = status == PaymentStatus.Paid ? ExchangeBatteryMessages.CreateSuccess : ExchangeBatteryMessages.CreateFailed,
            Data = response
        };
    }

    public async Task<ResultModel<PayOSPaymentResponseDto>> CreatePaymentAsync(PayOSPaymentRequestDto request)
    {
        var returnUrl = _config["PayOS:ReturnUrl"];
        var cancelUrl = _config["PayOS:CancelUrl"];

        var orderDetail = await _orderRepository.GetOrderByIdAsync(request.OrderId);
        if (orderDetail == null)
        {
            return new ResultModel<PayOSPaymentResponseDto>
            {
                IsSuccess = false,
                StatusCode = 404,
                Message = PayOSMessages.OrderNotFound
            };
        }
        var orderCode = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
        orderDetail.OrderCode = long.Parse(orderCode);
        await _orderRepository.UpdateOrderByOrderCodeAsync(orderDetail.OrderId, long.Parse(orderCode));


        static string Truncate(string? s, int max) => string.IsNullOrEmpty(s) ? string.Empty : (s.Length <= max ? s : s.Substring(0, max));

        var description = Truncate(request.Description, 25);


        var paymentRequest = new PaymentData(
            orderCode: long.Parse(orderCode),
            amount: (int)orderDetail.Total,
            description: description,
            items: new List<ItemData>
            {
                new(Truncate(description, 25), 1, (int)orderDetail.Total)
            },
            returnUrl: returnUrl,
            cancelUrl: cancelUrl
        );

        var response = await _payOS.createPaymentLink(paymentRequest);

        var paymentResponse = new PayOSPaymentResponseDto
        {
            PaymentUrl = response.checkoutUrl,
            OrderId = request.OrderId,
            Status = PaymentStatus.Pending.ToString()
        };

        return new ResultModel<PayOSPaymentResponseDto>
        {
            IsSuccess = true,
            StatusCode = 200,
            Message = PayOSMessages.PaymentLinkCreated,
            Data = paymentResponse
        };
    }
}