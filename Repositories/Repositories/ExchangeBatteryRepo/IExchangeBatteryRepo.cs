namespace Repositories.Repositories.ExchangeBatteryRepo;

using BusinessObjects.Models;

public interface IExchangeBatteryRepo
{
    Task Add(ExchangeBattery exchange);
    Task Update(ExchangeBattery exchange);
    Task Delete(ExchangeBattery exchange);
    Task<ExchangeBattery?> GetById(string exchangeId);
    Task<List<ExchangeBattery>> GetAll();
    Task<List<ExchangeBattery>> GetByStationId(string stationId);
    Task<List<ExchangeBattery>> GetByDriverId(string driverId);
}
