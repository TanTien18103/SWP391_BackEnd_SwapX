using BusinessObjects.Dtos;
using Services.ApiModels;

namespace Services;

public interface IPayOSService
{
    Task<ResultModel<PayOSWebhookResponseDto>> HandleWebhookAsync(PayOSWebhookRequestDto webhook);
    Task<ResultModel<PayOSPaymentResponseDto>> CreatePaymentAsync(PayOSPaymentRequestDto request);
}