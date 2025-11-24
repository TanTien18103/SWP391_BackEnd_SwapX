namespace Repositories.Repositories.ExchangeBatteryRepo;

using BusinessObjects.Models;

public interface IExchangeBatteryRepo
{
    Task<ExchangeBattery> Add(ExchangeBattery exchange);
    Task<ExchangeBattery> Update(ExchangeBattery exchange);
    Task Delete(ExchangeBattery exchange);
    Task<ExchangeBattery?> GetById(string exchangeId);
    Task<List<ExchangeBattery>> GetAll();
    Task<ExchangeBattery> GetPendingByVin(string Vin);
    Task<List<ExchangeBattery>> GetByStationId(string stationId);
    Task<List<ExchangeBattery>> GetByDriverId(string driverId);
    Task<ExchangeBattery?> GetByOrderId(string orderId);
    Task<List<ExchangeBattery>> GetByScheduleId(string stationscheduleId);
    Task<ExchangeBattery> GetPendingByVinAndAccountId(string Vin, string accountId);

}
