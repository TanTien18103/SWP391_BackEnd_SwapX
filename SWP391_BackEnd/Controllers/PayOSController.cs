using System.Security.Cryptography;
using System.Text;
using BusinessObjects.Dtos;
using BusinessObjects.Enums;
using Microsoft.AspNetCore.Mvc;
using Net.payOS;
using Net.payOS.Types;
using Services.Services;
using SWP391_BackEnd.Helpers;

namespace SWP391_BackEnd.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PayOSController : ControllerBase
    {
        private readonly PayOS _payOS;
        private readonly PayOSHelper _helper;
        private readonly IConfiguration _config;
        private readonly IOrderService _orderService;

        public PayOSController(PayOS payOs, PayOSHelper helper, IConfiguration config, IOrderService orderService)
        {
            _payOS = payOs;
            _helper = helper;
            _config = config;
            _orderService = orderService;
        }

        /// <summary>
        /// Webhook: PayOS will call this endpoint after payment attempt
        /// </summary>
        [HttpPost("webhook")]
        public async Task<IActionResult> Webhook([FromBody] PayOSWebhookRequestDto webhook)
        {
            try
            {
                // 1. Verify signature
                if (!_helper.VerifyWebhook(webhook))
                {
                    return Ok(new PayOSWebhookResponseDto
                    {
                        Success = false,
                        Message = "Invalid signature"
                    });
                }

                // 2. Determine status
                var status = webhook.Code == "00" && webhook.Success
                    ? PaymentStatus.Paid
                    : PaymentStatus.Failed;

                var orderCode = webhook.Data.OrderCode;
                var amount = webhook.Data.Amount;

                // 3. Extract OrderId from orderCode
                var orderId = ExtractOrderIdFromOrderCode(orderCode);

                // TODO: update DB (find order by orderId), check amount, update status...
                var orderDetail = await _orderService.GetOrderByIdAsync(orderId.ToString());
                if (orderDetail == null)
                {
                    return BadRequest("Order not found");
                }
                else
                {
                    await _orderService.UpdateOrderStatusAsync(orderDetail.OrderId, PaymentStatus.Paid.ToString());
                }

                return Ok(new PayOSWebhookResponseDto
                {
                    Success = true,
                    Message = $"Webhook OK: OrderId {orderId}, OrderCode {orderCode}, Amount {amount}, Status {status}"
                });
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// Create a payment link using PayOS SDK
        /// </summary>
        [HttpPost("create-payment")]
        public async Task<IActionResult> CreatePayment([FromBody] PayOSPaymentRequestDto request)
        {
            var returnUrl = _config["PayOS:ReturnUrl"];
            var cancelUrl = _config["PayOS:CancelUrl"];

            if (!long.TryParse(request.OrderId, out var orderId))
                return BadRequest("Invalid OrderId");
            
            var orderDetail = await _orderService.GetOrderByIdAsync(orderId.ToString());
            if (orderDetail == null)
            {
                return BadRequest("Order not found");
            }
            
            var orderCode = GenerateOrderCode(orderId);
            
            // description ngắn gọn (PayOS giới hạn 25 ký tự)
            var description = $"Order {orderId}";

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

            return Ok(new PayOSPaymentResponseDto
            {
                PaymentUrl = response.checkoutUrl,
                OrderId = request.OrderId,
                Status = PaymentStatus.Pending.ToString()
            });
        }

        #region Private Methods

        /// <summary>
        /// Generate orderCode from timestamp + orderId
        /// </summary>
        private long GenerateOrderCode(long orderId)
        {
            // yyyyMMddHHmmss (14 digits) + orderId
            var prefix = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            var orderCodeStr = prefix + orderId.ToString();
            return long.Parse(orderCodeStr);
        }

        /// <summary>
        /// Extract OrderId from orderCode (remove first 14 chars of timestamp)
        /// </summary>
        private long ExtractOrderIdFromOrderCode(long orderCode)
        {
            var codeStr = orderCode.ToString();
            if (codeStr.Length <= 14) return 0;
            var orderIdPart = codeStr.Substring(14);
            return long.Parse(orderIdPart);
        }

        #endregion
    }
}
