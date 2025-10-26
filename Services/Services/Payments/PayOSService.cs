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
using Repositories.Repositories.BatteryRepo;
using Repositories.Repositories.BatteryReportRepo;
using Repositories.Repositories.EvDriverRepo;
using Repositories.Repositories.ExchangeBatteryRepo;
using Repositories.Repositories.FormRepo;
using Repositories.Repositories.OrderRepo;
using Repositories.Repositories.PackageRepo;
using Repositories.Repositories.VehicleRepo;
using Services.ApiModels;
using Services.ApiModels.Vehicle;
using Services.Helpers;
using Services.Services.VehicleService;
using System.Text.Json;

namespace Services.Payments;

public class PayOSService : IPayOSService
{
    private readonly PayOS _payOS;
    private readonly PayOSHelper _helper;
    private readonly IConfiguration _config;
    private readonly IOrderRepository _orderRepository;
    private readonly IExchangeBatteryRepo _exchangeBatteryRepo;
    private readonly IBatteryReportRepo _batteryReportRepo;
    private readonly IVehicleRepo _vehicleRepo;
    private readonly IVehicleService _vehicleService;
    private readonly ILogger<PayOSService> _logger;
    private readonly IPackageRepo _packageRepo;
    private readonly IEvDriverRepo _evDriverRepo;
    private readonly IFormRepo _formRepo;
    private readonly IBatteryRepo _batteryRepo;

    public PayOSService(
        PayOS payOs,
        PayOSHelper helper,
        IConfiguration config,
        IOrderRepository orderRepository,
        IExchangeBatteryRepo exchangeBatteryRepo,
        IBatteryReportRepo batteryReportRepo,
        IVehicleRepo vehicleRepo,
        IVehicleService vehicleService,
        ILogger<PayOSService> logger,
        IPackageRepo packageRepo,
        IEvDriverRepo evDriverRepo,
        IFormRepo formRepo,
        IBatteryRepo batteryRepo
        )
    {
        _payOS = payOs;
        _helper = helper;
        _config = config;
        _orderRepository = orderRepository;
        _exchangeBatteryRepo = exchangeBatteryRepo;
        _batteryReportRepo = batteryReportRepo;
        _vehicleRepo = vehicleRepo;
        _vehicleService = vehicleService;
        _logger = logger;
        _packageRepo = packageRepo;
        _evDriverRepo = evDriverRepo;
        _formRepo = formRepo;
        _batteryRepo = batteryRepo;
    }

    public async Task<ResultModel<PayOSWebhookResponseDto>> HandleWebhookAsync(PayOSWebhookRequestDto webhook)
    {
        // Log incoming webhook for diagnostics (safe to log in non-sensitive environments).
        try
        {
            _logger.LogInformation("Received PayOS webhook: {Webhook}", JsonSerializer.Serialize(webhook));
        }
        catch
        {
            // Ignore logging errors
        }

        // If webhook verification fails, do not modify other entities. Only return failure.
        if (!_helper.VerifyWebhook(webhook))
        {
            _logger.LogWarning("PayOS webhook verification failed for payload: {Payload}", JsonSerializer.Serialize(webhook));
            return new ResultModel<PayOSWebhookResponseDto>
            {
                IsSuccess = false,
                StatusCode = 200,
                Message = ExchangeBatteryMessages.CreateFailed,
                Data = new PayOSWebhookResponseDto { Success = false, Message = ExchangeBatteryMessages.CreateFailed }
            };
        }

        // Determine success using multiple potential fields/values (robust against provider variations)
        var code = webhook.Data?.Code ?? webhook.Code;
        var desc = webhook.Data?.Desc ?? webhook.Desc;
        var rawCode = code?.Trim();

        var isSuccessCode = rawCode is not null && (
            string.Equals(rawCode, "00", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(rawCode, "0", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(rawCode, "PAYMENT_SUCCESS", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(rawCode, "SUCCESS", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(rawCode, "200", StringComparison.OrdinalIgnoreCase)
        );

        var isSuccessDesc = !string.IsNullOrEmpty(desc) && (
            string.Equals(desc, "Giao dịch thành công", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(desc, "Success", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(desc, "Payment successful", StringComparison.OrdinalIgnoreCase)
        );

        var status = (webhook.Success || isSuccessCode || isSuccessDesc)
            ? PaymentStatus.Paid
            : PaymentStatus.Failed;

        _logger.LogInformation("Computed payment status from webhook. Code='{Code}', Desc='{Desc}', SuccessFlag={SuccessFlag} => Status={Status}", rawCode, desc, webhook.Success, status);

        var orderCode = webhook.Data?.OrderCode ?? 0;
        _logger.LogInformation("Looking up order by orderCode: {OrderCode}", orderCode);

        var orderDetail2 = await _orderRepository.GetOrderByOrderCodeAsync(webhook.Data.OrderCode);

        if (orderDetail2 == null)
        {
            _logger.LogWarning("Order not found for orderCode: {OrderCode}", orderCode);
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

        _logger.LogInformation("Order {OrderId} status updated to {StatusInDb}", updateorder.OrderId, updateorder.Status);

        if (updateorder.Status == PaymentStatus.Failed.ToString() &&
        (updateorder.ServiceType == PaymentType.PrePaid.ToString() ||
        updateorder.ServiceType == PaymentType.UsePackage.ToString()
        ))
        {
            var form = await _formRepo.GetById(updateorder.ServiceId);
            if (form != null)
            {
                form.Status = FormStatusEnums.Deleted.ToString();
                form.UpdateDate = TimeHepler.SystemTimeNow;
                await _formRepo.Update(form);

                if (!string.IsNullOrEmpty(form.BatteryId))
                {
                    var battery = await _batteryRepo.GetBatteryById(form.BatteryId);
                    if (battery != null && battery.Status == BatteryStatusEnums.Booked.ToString())
                    {
                        battery.Status = BatteryStatusEnums.Available.ToString();
                        battery.UpdateDate = DateTime.UtcNow;
                        await _batteryRepo.UpdateBattery(battery);
                    }
                }
            }
        }

        if (updateorder.ServiceType == PaymentType.Package.ToString() &&
        updateorder.Status == PaymentStatus.Paid.ToString())
        {
            try
            {
                var vehicle = await _vehicleRepo.GetVehicleById(orderDetail2.Vin);
                var package = await _packageRepo.GetPackageById(orderDetail2.ServiceId);

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

        _logger.LogInformation("Finished handling PayOS webhook for order {OrderId}. Success={Success}", orderDetail2.OrderId, response.Success);

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