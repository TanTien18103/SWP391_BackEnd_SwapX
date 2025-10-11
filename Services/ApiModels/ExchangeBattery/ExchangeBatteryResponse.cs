using System;

namespace Services.ApiModels.ExchangeBattery;

public class ExchangeBatteryResponse
{
    public string ExchangeBatteryId { get; set; }
    public string Vin { get; set; }
    public string VehicleName { get; set; }
    public string OldBatteryId { get; set; }
    public string OldBatteryName { get; set; }
    public string NewBatteryId { get; set; }
    public string NewBatteryName { get; set; }
    public string StaffAccountId { get; set; }
    public string StaffAccountName { get; set; }
    public string ScheduleId { get; set; }
    public DateTime? ScheduleTime { get; set; }
    public string OrderId { get; set; }
    public string StationId { get; set; }
    public string StationName { get; set; }
    public string StationAddress { get; set; }
    public string Status { get; set; }
    public string Notes { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? UpdateDate { get; set; }
}