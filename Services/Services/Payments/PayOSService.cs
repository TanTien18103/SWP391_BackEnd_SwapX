using BusinessObjects.Constants;
using BusinessObjects.Dtos;
using BusinessObjects.Enums;
using BusinessObjects.Models;
using Humanizer;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;
using Net.payOS;
using Net.payOS.Types;
using Repositories.Repositories.BatteryReportRepo;
using Repositories.Repositories.ExchangeBatteryRepo;
using Repositories.Repositories.OrderRepo;
using Repositories.Repositories.VehicleRepo;
using Services.ApiModels;
using Services.ApiModels.Vehicle;
using Services.Helpers;
using Services.Services.VehicleService;
using Repositories.Repositories.FormRepo;
using Repositories.Repositories.BatteryRepo;

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
    private readonly IFormRepo _formRepo;
    private readonly IBatteryRepo _batteryRepoRepo;
    public PayOSService(
        PayOS payOs,
        PayOSHelper helper,
        IConfiguration config,
        IOrderRepository orderRepository,
        IExchangeBatteryRepo exchangeBatteryRepo,
        IBatteryReportRepo batteryRepo,
        IVehicleRepo vehicleRepo,
        IVehicleService vehicleService,
        IFormRepo formRepo,
        IBatteryRepo batteryRepoRepo
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
        _formRepo = formRepo;
        _batteryRepoRepo = batteryRepoRepo;
    }

    public async Task<ResultModel<PayOSWebhookResponseDto>> HandleWebhookAsync(PayOSWebhookRequestDto webhook)
    {
       
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

        
        var updateorder = await _orderRepository.UpdateOrderStatusAsync(orderDetail2.OrderId, status.ToString());

        
        try
        {
            if (status == PaymentStatus.Failed &&
                (updateorder.ServiceType == PaymentType.PrePaid.ToString() || updateorder.ServiceType == PaymentType.PaidAtStation.ToString()))
            {
                
                var form = await _formRepo.GetById(updateorder.ServiceId);
                if (form != null)
                {
                    
                    form.Status = BusinessObjects.Enums.FormStatusEnums.Deleted.ToString();
                    form.UpdateDate = DateTime.UtcNow;
                    await _formRepo.Update(form);

                    
                    if (!string.IsNullOrEmpty(form.BatteryId))
                    {
                        var battery = await _batteryRepoRepo.GetBatteryById(form.BatteryId);
                        if (battery != null && battery.Status == BusinessObjects.Enums.BatteryStatusEnums.Booked.ToString())
                        {
                            battery.Status = BusinessObjects.Enums.BatteryStatusEnums.Available.ToString();
                            battery.UpdateDate = DateTime.UtcNow;
                            await _batteryRepoRepo.UpdateBattery(battery);
                        }
                    }
                }
            }
        }
        catch
        {
           
        }

        if (updateorder.ServiceType == PaymentType.Package.ToString() && updateorder.Status == PaymentStatus.Paid.ToString())
        {
            var addVehicleInPackageRequest = new AddVehicleInPackageRequest
            {
                Vin = updateorder.Vin,
                PackageId = updateorder.ServiceId
            };

            var assignResult = await _vehicleService.AddVehicleInPackage(addVehicleInPackageRequest);

            if (!assignResult.IsSuccess)
            {
                await _orderRepository.UpdateOrderStatusAsync(updateorder.OrderId, PaymentStatus.Failed.ToString());
                return new ResultModel<PayOSWebhookResponseDto>
                {
                    IsSuccess = false,
                    StatusCode = 500,
                    Message = assignResult.Message ?? ExchangeBatteryMessages.CreatePackageFailed,
                    Data = new PayOSWebhookResponseDto
                    {
                        Success = false,
                        Message = assignResult.Message ?? ExchangeBatteryMessages.CreatePackageFailed
                    }
                };
            }

            return new ResultModel<PayOSWebhookResponseDto>
            {
                IsSuccess = assignResult.IsSuccess,
                StatusCode = assignResult.IsSuccess ? 200 : 500,
                Message = assignResult.IsSuccess ? ExchangeBatteryMessages.CreateSuccess : assignResult.Message,
                Data = new PayOSWebhookResponseDto
                {
                    Success = status == PaymentStatus.Paid,
                    Message = assignResult.IsSuccess ? ExchangeBatteryMessages.CreatePackageSuccess : assignResult.Message ?? ExchangeBatteryMessages.CreateFailed,
                }
            };
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