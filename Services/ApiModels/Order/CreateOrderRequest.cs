using BusinessObjects.Enums;

namespace Services.ApiModels.Order;

public class CreateOrderRequest
{
    public string AccountId { get; set; }
    public decimal? Total { get; set; }
    public string BatteryId { get; set; }
    public string ServiceId { get; set; }
    public PaymentType ServiceType { get; set; }
}