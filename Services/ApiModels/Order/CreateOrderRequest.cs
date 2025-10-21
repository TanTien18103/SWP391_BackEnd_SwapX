using BusinessObjects.Enums;
using System.ComponentModel.DataAnnotations;

namespace Services.ApiModels.Order;

public class CreateOrderRequest
{
    [Required]
    public string AccountId { get; set; }
    [Required]
    public decimal? Total { get; set; }
    [Required]
    public string BatteryId { get; set; }
    [Required]
    public string ServiceId { get; set; }
    [Required]
    public PaymentType ServiceType { get; set; }
    public string? ExchangeBatteryId { get; set; }
    public string? Vin { get; set; }

}