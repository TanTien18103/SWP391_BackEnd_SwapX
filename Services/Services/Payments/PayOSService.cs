using BusinessObjects.Dtos;
using BusinessObjects.Enums;
using Microsoft.Extensions.Configuration;
using Net.payOS;
using Net.payOS.Types;
using Repositories.Repositories.OrderRepo;
using Services.ApiModels;
using Services.Services;
using Services.Helpers;

namespace Services;

public class PayOSService : IPayOSService
{
    private readonly PayOS _payOS;
    private readonly PayOSHelper _helper;
    private readonly IConfiguration _config;
    private readonly IOrderRepository _orderRepository;
    
    public PayOSService(PayOS payOs, PayOSHelper helper, IConfiguration config, IOrderRepository orderRepository)
    {
        _payOS = payOs;
        _helper = helper;
        _config = config;
        _orderRepository = orderRepository;
    }

    public async Task<ResultModel<PayOSWebhookResponseDto>> HandleWebhookAsync(PayOSWebhookRequestDto webhook)
    {
        if (!_helper.VerifyWebhook(webhook))
        {
            return new ResultModel<PayOSWebhookResponseDto>
            {
                IsSuccess = false,
                StatusCode = 400,
                Message = "Invalid signature",
                Data = new PayOSWebhookResponseDto { Success = false, Message = "Invalid signature" }
            };
        }

        var status = webhook.Code == "00" && webhook.Success
            ? PaymentStatus.Paid
            : PaymentStatus.Failed;

        var orderId = ExtractOrderIdFromOrderCode(webhook.Data.OrderCode);

        var orderDetail = await _orderRepository.GetOrderByIdAsync(orderId.ToString());
        if (orderDetail == null)
        {
            return new ResultModel<PayOSWebhookResponseDto>
            {
                IsSuccess = false,
                StatusCode = 404,
                Message = "Order not found",
                Data = new PayOSWebhookResponseDto { Success = false, Message = "Order not found" }
            };
        }

        await _orderRepository.UpdateOrderStatusAsync(orderDetail.OrderId, status.ToString());

        var response = new PayOSWebhookResponseDto
        {
            Success = true,
            Message = $"Webhook OK: OrderId {orderId}, OrderCode {webhook.Data.OrderCode}, Amount {webhook.Data.Amount}, Status {status}"
        };

        return new ResultModel<PayOSWebhookResponseDto>
        {
            IsSuccess = true,
            StatusCode = 200,
            Message = "Webhook handled successfully",
            Data = response
        };
    }

    public async Task<ResultModel<PayOSPaymentResponseDto>> CreatePaymentAsync(PayOSPaymentRequestDto request)
    {
        var returnUrl = _config["PayOS:ReturnUrl"];
        var cancelUrl = _config["PayOS:CancelUrl"];

        if (!long.TryParse(request.OrderId, out var orderId))
        {
            return new ResultModel<PayOSPaymentResponseDto>
            {
                IsSuccess = false,
                StatusCode = 400,
                Message = "Invalid OrderId"
            };
        }

        var orderDetail = await _orderRepository.GetOrderByIdAsync(orderId.ToString());
        if (orderDetail == null)
        {
            return new ResultModel<PayOSPaymentResponseDto>
            {
                IsSuccess = false,
                StatusCode = 404,
                Message = "Order not found"
            };
        }

        var orderCode = GenerateOrderCode(orderId);
        var description = $"Order {orderId}"; // PayOS limit 25 chars

        var paymentRequest = new PaymentData(
            orderCode: orderCode,
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
            Message = "Payment link created successfully",
            Data = paymentResponse
        };
    }

    #region Helpers
    private long GenerateOrderCode(long orderId)
    {
        var prefix = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
        return long.Parse(prefix + orderId.ToString());
    }

    private long ExtractOrderIdFromOrderCode(long orderCode)
    {
        var codeStr = orderCode.ToString();
        if (codeStr.Length <= 14) return 0;
        return long.Parse(codeStr.Substring(14));
    }
    #endregion
}
