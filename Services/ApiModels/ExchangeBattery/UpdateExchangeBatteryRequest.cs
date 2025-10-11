namespace Services.ApiModels.ExchangeBattery;

public class UpdateExchangeBatteryRequest
{
    public string? NewBatteryId { get; set; }
    public decimal? PaymentAmount { get; set; }
    public string? Notes { get; set; }
}