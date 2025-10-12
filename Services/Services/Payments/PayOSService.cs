using BusinessObjects.Constants;
using BusinessObjects.Dtos;
using BusinessObjects.Enums;
using Microsoft.Extensions.Configuration;
using Net.payOS;
using Net.payOS.Types;
using Repositories.Repositories.BatteryReportRepo;
using Repositories.Repositories.ExchangeBatteryRepo;
using Repositories.Repositories.OrderRepo;
using Services.ApiModels;
using Services.Helpers;

namespace Services.Payments;

public class PayOSService : IPayOSService
{
    private readonly PayOS _payOS;
    private readonly PayOSHelper _helper;
    private readonly IConfiguration _config;
    private readonly IOrderRepository _orderRepository;
    private readonly IExchangeBatteryRepo _exchangeBatteryRepo;
    private readonly IBatteryReportRepo _batteryRepo;
    public PayOSService(PayOS payOs, PayOSHelper helper, IConfiguration config, IOrderRepository orderRepository, IExchangeBatteryRepo exchangeBatteryRepo, IBatteryReportRepo batteryRepo)
    {
        _payOS = payOs;
        _helper = helper;
        _config = config;
        _orderRepository = orderRepository;
        _exchangeBatteryRepo = exchangeBatteryRepo;
        _batteryRepo = batteryRepo;
    }

    public async Task<ResultModel<PayOSWebhookResponseDto>> HandleWebhookAsync(PayOSWebhookRequestDto webhook)
    {
        if (!_helper.VerifyWebhook(webhook))
        {
            var orderDetail = await _orderRepository.GetOrderByOrderCodeAsync(webhook.Data.OrderCode);
            if (orderDetail != null)
            {
                var exchangeBattery = await _exchangeBatteryRepo.GetByOrderId(orderDetail.OrderId);
                if (exchangeBattery != null)
                {
                    exchangeBattery.Status = ExchangeStatusEnums.Cancelled.ToString();
                    exchangeBattery.UpdateDate = DateTime.Now;
                    await _exchangeBatteryRepo.Update(exchangeBattery);
                }
                var batteryReportCancel = await _batteryRepo.GetByExchangeBatteryId(exchangeBattery.ExchangeBatteryId);
                if (batteryReportCancel != null)
                {
                    batteryReportCancel.Status = BatteryReportStatusEnums.Cancelled.ToString();
                    batteryReportCancel.UpdateDate = DateTime.Now;
                    await _batteryRepo.UpdateBatteryReport(batteryReportCancel);
                }
            }
            return new ResultModel<PayOSWebhookResponseDto>
            {
                IsSuccess = false,
                StatusCode = 400,
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
                StatusCode = 404,
                Message = ExchangeBatteryMessages.NotFound,
                Data = new PayOSWebhookResponseDto { Success = false, Message = ExchangeBatteryMessages.NotFound }
            };
        }
    
        await _orderRepository.UpdateOrderStatusAsync(orderDetail2.OrderId, status.ToString());
        var exchangeBattery2 = await _exchangeBatteryRepo.GetByOrderId(orderDetail2.OrderId);
        if (exchangeBattery2 != null)
        {
            exchangeBattery2.Status = status == PaymentStatus.Paid ? ExchangeStatusEnums.Completed.ToString() : ExchangeStatusEnums.Cancelled.ToString();
            exchangeBattery2.UpdateDate = DateTime.Now;
            await _exchangeBatteryRepo.Update(exchangeBattery2);
        }
        
        var batteryReportComplete = await _batteryRepo.GetByExchangeBatteryId(exchangeBattery2.ExchangeBatteryId);
        if (batteryReportComplete != null && status == PaymentStatus.Paid)
        {
            batteryReportComplete.Status = BatteryReportStatusEnums.Completed.ToString();
            batteryReportComplete.UpdateDate = DateTime.Now;
            await _batteryRepo.UpdateBatteryReport(batteryReportComplete);
        }
        var response = new PayOSWebhookResponseDto
        {
            Success = status == PaymentStatus.Paid,
            Message = status == PaymentStatus.Paid ? ExchangeBatteryMessages.CreateSuccess : ExchangeBatteryMessages.CreateFailed
        };

        return new ResultModel<PayOSWebhookResponseDto>
        {
            IsSuccess = status == PaymentStatus.Paid,
            StatusCode = status == PaymentStatus.Paid ? 200 : 400,
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
        
        var description = $"Order {request.OrderId}"; 

        var paymentRequest = new PaymentData(
            orderCode: long.Parse(orderCode),
            amount: (int)request.Amount,
            description: description,
            items: new List<ItemData>
            {
                new(request.Description, 1, (int)request.Amount)
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
