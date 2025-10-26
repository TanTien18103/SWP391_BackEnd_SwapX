using Services.ApiModels;
using Services.ApiModels.ExchangeBattery;

namespace Services.Services.ExchangeBatteryService;

public interface IExchangeBatteryService
{
    Task<ResultModel> CreateExchange(CreateExchangeBatteryRequest req);
    Task<ResultModel> GetExchangeByStation(string stationId);
    Task<ResultModel> GetAllExchanges();
    Task<ResultModel> GetExchangeByDriver(string driverId);
    Task<ResultModel> GetExchangeDetail(string exchangeId);
    Task<ResultModel> UpdateExchange(string exchangeId, UpdateExchangeBatteryRequest req);
    Task<ResultModel> DeleteExchange(string exchangeId);
    Task<ResultModel> UpdateExchangeStatus(UpdateExchangeStatusRequest request);
    Task<ResultModel> GetExchangesByScheduleId(string stationscheduleId);
}