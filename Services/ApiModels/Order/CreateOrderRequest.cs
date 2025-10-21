using BusinessObjects.Enums;
using System.ComponentModel.DataAnnotations;

namespace Services.ApiModels.Order;

public class CreateOrderRequest
{
    [Required(ErrorMessage = "AccountId is required.")]
    public string AccountId { get; set; }

    [Required(ErrorMessage = "Total is required.")]
    [Range(0, double.MaxValue, ErrorMessage = "Total must be a positive number.")]
    public decimal Total { get; set; }

    [Required(ErrorMessage = "BatteryId is required.")]
    public string BatteryId { get; set; }

    [Required(ErrorMessage = "ServiceId is required.")]
    public string ServiceId { get; set; }

    [Required(ErrorMessage = "ServiceType is required.")]
    [EnumDataType(typeof(PaymentType), ErrorMessage = "Invalid payment type.")]
    public PaymentType ServiceType { get; set; }

    /// <summary>
    /// Dành riêng cho thanh toán tại trạm (PaidAtStation)
    /// </summary>
    public string? ExchangeBatteryId { get; set; }

    /// <summary>
    /// VIN của xe - cần cho các loại thanh toán liên quan đến gói
    /// </summary>
    public string? Vin { get; set; }
}