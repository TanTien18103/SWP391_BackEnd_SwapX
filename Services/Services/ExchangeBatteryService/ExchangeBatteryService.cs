using BusinessObjects.Models;
using BusinessObjects.Enums;
using BusinessObjects.TimeCoreHelper;
using BusinessObjects.Constants;
using Repositories.Repositories.BatteryRepo;
using Repositories.Repositories.BatteryReportRepo;
using Repositories.Repositories.ExchangeBatteryRepo;
using Repositories.Repositories.StationRepo;
using Repositories.Repositories.OrderRepo;
using Services.ApiModels;
using Services.ApiModels.BatteryReport;
using Services.ApiModels.ExchangeBattery;
using Services.Services.BatteryService;
using Services.ServicesHelpers;

namespace Services.Services.ExchangeBatteryService;

public class ExchangeBatteryService : IExchangeBatteryService
{
    private readonly IExchangeBatteryRepo _exchangeRepo;
    private readonly IBatteryRepo _batteryRepo;
    private readonly IStationRepo _stationRepo;
    private readonly IOrderRepository _orderRepo;
    private readonly IBatteryReportRepo _batteryReportRepository;

    public ExchangeBatteryService(
        IExchangeBatteryRepo exchangeRepo,
        IBatteryRepo batteryRepo,
        IStationRepo stationRepo,
        IOrderRepository orderRepo,
        IBatteryReportRepo batteryReportRepository )
    {
        _exchangeRepo = exchangeRepo;
        _batteryRepo = batteryRepo;
        _stationRepo = stationRepo;
        _orderRepo = orderRepo;
        _batteryReportRepository = batteryReportRepository;
    }

    private ExchangeBatteryResponse MapToResponse(ExchangeBattery entity)
    {
        return new ExchangeBatteryResponse
        {
            ExchangeBatteryId = entity.ExchangeBatteryId,
            Vin = entity.Vin,
            VehicleName = entity.VinNavigation?.VehicleName,
            OldBatteryId = entity.OldBatteryId,
            OldBatteryName = entity.OldBattery?.BatteryName,
            NewBatteryId = entity.NewBatteryId,
            NewBatteryName = entity.NewBattery?.BatteryName,
            StaffAccountId = entity.StaffAccountId,
            StaffAccountName = entity.StaffAccount?.Name,
            ScheduleId = entity.ScheduleId,
            ScheduleTime = entity.Schedule?.StartDate,
            OrderId = entity.OrderId,
            StationId = entity.StationId,
            StationName = entity.Station?.StationName,
            StationAddress = entity.Station?.Location,
            Status = entity.Status,
            Notes = entity.Notes,
            StartDate = entity.StartDate,
            UpdateDate = entity.UpdateDate
        };
    }

    public async Task<ResultModel> CreateExchange(CreateExchangeBatteryRequest req)
    {
        var station = await _stationRepo.GetStationById(req.StationId);
        if (station == null)
            return new ResultModel { StatusCode = 404, Message = ExchangeMessages.InvalidStation };

        var oldBattery = await _batteryRepo.GetBatteryById(req.OldBatteryId);
        var newBattery = await _batteryRepo.GetBatteryById(req.NewBatteryId);
        if (oldBattery == null || newBattery == null)
            return new ResultModel { StatusCode = 404, Message = ExchangeMessages.InvalidBattery };

        try
        {
            var order = new Order
            {
                OrderId = Guid.NewGuid().ToString(),
                AccountId = req.AccountId,
                BatteryId = req.NewBatteryId,
                Total = req.Amount,
                Status = PaymentStatus.Pending.ToString(),
                Date = TimeHepler.SystemTimeNow,
                StartDate = TimeHepler.SystemTimeNow,
                UpdateDate = TimeHepler.SystemTimeNow
            };
            await _orderRepo.CreateOrderAsync(order);

            var exchange = new ExchangeBattery
            {
                ExchangeBatteryId = Guid.NewGuid().ToString(),
                StationId = req.StationId,
                OldBatteryId = req.OldBatteryId,
                NewBatteryId = req.NewBatteryId,
                OrderId = order.OrderId,
                Status = ExchangeStatusEnums.Completed.ToString(),
                StartDate = TimeHepler.SystemTimeNow,
                UpdateDate = TimeHepler.SystemTimeNow,
                Notes = req.Notes
            };
            await _exchangeRepo.Add(exchange);

            oldBattery.Status = BatteryStatusEnums.InUse.ToString();
            newBattery.Status = BatteryStatusEnums.InUse.ToString();
            await _batteryRepo.UpdateBattery(oldBattery);
            await _batteryRepo.UpdateBattery(newBattery);

            await _batteryReportRepository.AddBatteryReport(new BatteryReport
            {
                BatteryReportId = Guid.NewGuid().ToString(),
                BatteryId = req.NewBatteryId, 
                StationId = req.StationId,
                ExchangeBatteryId = exchange.ExchangeBatteryId,
                Status = BatteryReportStatusEnums.Pending.ToString(),
                Description = $"Battery {req.NewBatteryId} exchanged at station {req.StationId}",
                AccountId = req.AccountId,
                StartDate = TimeHepler.SystemTimeNow,
            });

            // Lấy lại entity từ repo để đảm bảo navigation property đã được Include
            var exchangeFull = await _exchangeRepo.GetById(exchange.ExchangeBatteryId);
            var response = MapToResponse(exchangeFull);

            return new ResultModel
            {
                StatusCode = 201,
                IsSuccess = true,
                Message = ExchangeMessages.CreateSuccess,
                Data = response
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ExchangeBatteryService] Error: {ex.Message}");
            return new ResultModel
            {
                StatusCode = 500,
                IsSuccess = false,
                Message = ExchangeMessages.CreateFailed
            };
        }
    }


    public async Task<ResultModel> GetExchangeByStation(string stationId)
    {
        var list = await _exchangeRepo.GetByStationId(stationId);
        if (list == null || !list.Any())
            return new ResultModel { StatusCode = 204, Message = ExchangeMessages.ListEmpty };
        var response = list.Select(MapToResponse).ToList();
        return new ResultModel { StatusCode = 200, IsSuccess = true, Data = response };
    }

    public async Task<ResultModel> GetExchangeByDriver(string accountId)
    {
        var list = await _exchangeRepo.GetByDriverId(accountId);
        if (list == null || !list.Any())
            return new ResultModel { StatusCode = 204, Message = ExchangeMessages.ListEmpty };
        var response = list.Select(MapToResponse).ToList();
        return new ResultModel { StatusCode = 200, IsSuccess = true, Data = response };
    }

    public async Task<ResultModel> GetExchangeDetail(string exchangeId)
    {
        var ex = await _exchangeRepo.GetById(exchangeId);
        if (ex == null)
            return new ResultModel { StatusCode = 404, Message = ExchangeMessages.NotFound };
        var response = MapToResponse(ex);
        return new ResultModel { StatusCode = 200, IsSuccess = true, Data = response };
    }

    public async Task<ResultModel> GetAllExchanges()
    {
        var list = await _exchangeRepo.GetAll();
        if (list == null || !list.Any())
            return new ResultModel { StatusCode = 204, Message = ExchangeMessages.ListEmpty };

        var response = list.Select(MapToResponse).ToList();
        return new ResultModel { StatusCode = 200, IsSuccess = true, Data = response };
    }

    public async Task<ResultModel> UpdateExchange(string exchangeId, UpdateExchangeBatteryRequest req)
    {
        var exchange = await _exchangeRepo.GetById(exchangeId);
        if (exchange == null)
            return new ResultModel { StatusCode = 404, Message = ExchangeMessages.NotFound };

        try
        {
            if (!string.IsNullOrEmpty(req.NewBatteryId))
                exchange.NewBatteryId = req.NewBatteryId;
            if (!string.IsNullOrEmpty(req.Notes))
                exchange.Notes = req.Notes;

            exchange.UpdateDate = TimeHepler.SystemTimeNow;
            await _exchangeRepo.Update(exchange);

            var response = exchange.Map<ExchangeBatteryResponse>();

            return new ResultModel
            {
                StatusCode = 200,
                IsSuccess = true,
                Message = ExchangeMessages.UpdateSuccess,
                Data = response
            };
        }
        catch
        {
            return new ResultModel
            {
                StatusCode = 500,
                IsSuccess = false,
                Message = ExchangeMessages.UpdateFailed
            };
        }
    }

    public async Task<ResultModel> DeleteExchange(string exchangeId)
    {
        var exchange = await _exchangeRepo.GetById(exchangeId);
        if (exchange == null)
            return new ResultModel { StatusCode = 404, Message = ExchangeMessages.NotFound };

        try
        {
            await _exchangeRepo.Delete(exchange);
            return new ResultModel
            {
                StatusCode = 200,
                IsSuccess = true,
                Message = ExchangeMessages.DeleteSuccess
            };
        }
        catch
        {
            return new ResultModel
            {
                StatusCode = 500,
                IsSuccess = false,
                Message = ExchangeMessages.DeleteFailed
            };
        }
    }
}
