namespace Services.ApiModels.ExchangeBattery;

public class CreateExchangeBatteryRequest
{
    public string StationId { get; set; }
    public string AccountId { get; set; }
    public string OldBatteryId { get; set; }
    public string NewBatteryId { get; set; }
    public string Notes { get; set; }
    public decimal? Amount { get; set; }
}
