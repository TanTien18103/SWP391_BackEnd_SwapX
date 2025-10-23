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
using Microsoft.AspNetCore.Http;
using Repositories.Repositories.VehicleRepo;
using Repositories.Repositories.BatteryHistoryRepo;

namespace Services.Services.ExchangeBatteryService;

public class ExchangeBatteryService : IExchangeBatteryService
{
    private readonly IExchangeBatteryRepo _exchangeRepo;
    private readonly IBatteryRepo _batteryRepo;
    private readonly IStationRepo _stationRepo;
    private readonly IOrderRepository _orderRepo;
    private readonly IBatteryReportRepo _batteryReportRepository;
    private readonly IVehicleRepo _vehicleRepo;
    private readonly IBatteryHistoryRepo _batteryHistoryRepo;

    public ExchangeBatteryService(
        IExchangeBatteryRepo exchangeRepo,
        IBatteryRepo batteryRepo,
        IStationRepo stationRepo,
        IOrderRepository orderRepo,
        IBatteryReportRepo batteryReportRepository,
        IVehicleRepo vehicleRepo,
        IBatteryHistoryRepo batteryHistoryRepo
        )
    {
        _exchangeRepo = exchangeRepo;
        _batteryRepo = batteryRepo;
        _stationRepo = stationRepo;
        _orderRepo = orderRepo;
        _batteryReportRepository = batteryReportRepository;
        _vehicleRepo = vehicleRepo;
        _batteryHistoryRepo = batteryHistoryRepo;
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

    public async Task<ResultModel> UpdateExchangeStatus(UpdateExchangeStatusRequest request)
    {
        try
        {
            //Kiểm tra ExchangeBattery tồn tại
            var exchange = await _exchangeRepo.GetById(request.ExchangeBatteryId);
            if (exchange == null)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    ResponseCode = ResponseCodeConstants.NOT_FOUND,
                    Message = ExchangeBatteryMessages.EXCHANGE_BATTERY_NOT_FOUND,
                    StatusCode = StatusCodes.Status404NotFound
                };
            }

            //Kiểm tra Order tồn tại
            var order = await _orderRepo.GetOrderByIdAsync(exchange.OrderId);
            if (order == null)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    ResponseCode = ResponseCodeConstants.NOT_FOUND,
                    Message = ResponseMessageOrder.ORDER_NOT_FOUND,
                    StatusCode = StatusCodes.Status404NotFound
                };
            }

            //Kiểm tra Order đã thanh toán
            if (order.Status != PaymentStatus.Paid.ToString())
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    ResponseCode = ResponseCodeConstants.FAILED,
                    Message = ResponseMessageOrder.ORDER_NOT_PAID,
                    StatusCode = StatusCodes.Status400BadRequest
                };
            }

            //Kiểm tra BatteryReport tồn tại
            var report = await _batteryReportRepository.GetByExchangeBatteryId(request.ExchangeBatteryId);
            if (report == null)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    ResponseCode = ResponseCodeConstants.NOT_FOUND,
                    Message = ResponseMessageConstantsBatteryReport.BATTERY_REPORT_NOT_FOUND,
                    StatusCode = StatusCodes.Status404NotFound
                };
            }

            //Kiểm tra BatteryReport đã hoàn tất
            if (report.Status != BatteryReportStatusEnums.Completed.ToString())
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    ResponseCode = ResponseCodeConstants.FAILED,
                    Message = ResponseMessageConstantsBatteryReport.BATTERY_REPORT_NOT_COMPLETED,
                    StatusCode = StatusCodes.Status400BadRequest
                };
            }

            //Kiểm tra ExchangeBattery chưa bị finalize
            if (exchange.Status == ExchangeStatusEnums.Completed.ToString() ||
                exchange.Status == ExchangeStatusEnums.Cancelled.ToString())
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    ResponseCode = ResponseCodeConstants.FAILED,
                    Message = ExchangeBatteryMessages.EXCHANGE_BATTERY_ALREADY_FINALIZED,
                    StatusCode = StatusCodes.Status400BadRequest
                };
            }

            //Lấy thông tin pin
            var oldBattery = await _batteryRepo.GetBatteryById(exchange.OldBatteryId);
            var newBattery = await _batteryRepo.GetBatteryById(exchange.NewBatteryId);

            if (oldBattery == null || newBattery == null)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    ResponseCode = ResponseCodeConstants.NOT_FOUND,
                    Message = ResponseMessageConstantsBattery.BATTERY_NOT_FOUND,
                    StatusCode = StatusCodes.Status404NotFound
                };
            }

            //Kiểm tra phương tiện
            var vehicleOfExchange = await _vehicleRepo.GetVehicleById(exchange.Vin);
            if (vehicleOfExchange == null)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    ResponseCode = ResponseCodeConstants.NOT_FOUND,
                    Message = ResponseMessageConstantsVehicle.VEHICLE_NOT_FOUND,
                    StatusCode = StatusCodes.Status404NotFound
                };
            }

            //Check pin mới phải đang khả dụng
            if (newBattery.Status != BatteryStatusEnums.Available.ToString())
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    ResponseCode = ResponseCodeConstants.FAILED,
                    Message = ResponseMessageConstantsBattery.BATTERY_NOT_AVAILABLE,
                    StatusCode = StatusCodes.Status400BadRequest
                };
            }

            //Kiểm tra trạm tồn tại
            var station = await _stationRepo.GetStationById(exchange.StationId);
            if (station == null)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    ResponseCode = ResponseCodeConstants.NOT_FOUND,
                    Message = ResponseMessageConstantsStation.STATION_NOT_FOUND,
                    StatusCode = StatusCodes.Status404NotFound
                };
            }
            //Xử lý logic theo trạng thái yêu cầu
            switch (request.Status)
            {
                case ExchangeStatusEnums.Completed:
                    exchange.Status = ExchangeStatusEnums.Completed.ToString();
                    exchange.StaffAccountId = request.StaffId;
                    exchange.UpdateDate = TimeHepler.SystemTimeNow;

                    oldBattery.BatteryName = $"{oldBattery.BatteryType}_{oldBattery.Specification}_{ResponseMessageConstantsStation.DefaultBatterySuffix}";
                    oldBattery.StationId = exchange.StationId;
                    oldBattery.Status = BatteryStatusEnums.Maintenance.ToString();
                    oldBattery.UpdateDate = TimeHepler.SystemTimeNow;

                    newBattery.StationId = null;
                    newBattery.Status = BatteryStatusEnums.InUse.ToString();
                    newBattery.UpdateDate = TimeHepler.SystemTimeNow;

                    vehicleOfExchange.BatteryId = newBattery.BatteryId;
                    vehicleOfExchange.UpdateDate = TimeHepler.SystemTimeNow;

                    await _vehicleRepo.UpdateVehicle(vehicleOfExchange);
                    await _batteryRepo.UpdateBattery(oldBattery);
                    await _batteryRepo.UpdateBattery(newBattery);

                    var oldbatteryhistory = new BatteryHistory
                    {
                        BatteryHistoryId = Guid.NewGuid().ToString(),
                        BatteryId = oldBattery.BatteryId,
                        StationId = oldBattery.StationId,
                        ActionType = BatteryHistoryActionTypeEnums.ReturnedToStation.ToString(),
                        Notes = string.Format(HistoryActionConstants.BATTERY_RETURNED_TO_STATION_AFTER_EXCHANGE, station.StationName),
                        Status = BatteryHistoryStatusEnums.Active.ToString(),
                        ActionDate = TimeHepler.SystemTimeNow,
                        EnergyLevel = oldBattery.Capacity.ToString(),
                        StartDate = TimeHepler.SystemTimeNow,
                        UpdateDate = TimeHepler.SystemTimeNow
                    };
                    var newbatteryhistory = new BatteryHistory
                    {
                        BatteryHistoryId = Guid.NewGuid().ToString(),
                        BatteryId = newBattery.BatteryId,
                        StationId = null,
                        ActionType = BatteryHistoryActionTypeEnums.AssignedToVehicle.ToString(),
                        Notes = string.Format(HistoryActionConstants.BATTERY_ASSIGNED_TO_VEHICLE_AFTER_EXCHANGE, vehicleOfExchange.Vin, vehicleOfExchange.VehicleName),
                        Status = BatteryHistoryStatusEnums.Active.ToString(),
                        ActionDate = TimeHepler.SystemTimeNow,
                        EnergyLevel = newBattery.Capacity.ToString(),
                        StartDate = TimeHepler.SystemTimeNow,
                        UpdateDate = TimeHepler.SystemTimeNow
                    };

                    await _batteryHistoryRepo.AddBatteryHistory(oldbatteryhistory);
                    await _batteryHistoryRepo.AddBatteryHistory(newbatteryhistory);

                    break;

                case ExchangeStatusEnums.Cancelled:
                    exchange.Status = ExchangeStatusEnums.Cancelled.ToString();
                    exchange.StaffAccountId = request.StaffId;
                    exchange.UpdateDate = TimeHepler.SystemTimeNow;

                    newBattery.Status = BatteryStatusEnums.Available.ToString();
                    newBattery.UpdateDate = TimeHepler.SystemTimeNow;
                    await _batteryRepo.UpdateBattery(newBattery);
                    break;

                case ExchangeStatusEnums.Pending:
                    return new ResultModel
                    {
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.FAILED,
                        Message = ExchangeBatteryMessages.INVALID_STATUS_UPDATE,
                        StatusCode = StatusCodes.Status400BadRequest
                    };

                default:
                    return new ResultModel
                    {
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.FAILED,
                        Message = ExchangeBatteryMessages.INVALID_STATUS_TYPE,
                        StatusCode = StatusCodes.Status400BadRequest
                    };
            }

            // 🔟 Cập nhật ExchangeBattery
            var updatedExchange = await _exchangeRepo.Update(exchange);

            return new ResultModel
            {
                IsSuccess = true,
                ResponseCode = ResponseCodeConstants.SUCCESS,
                Message = ExchangeBatteryMessages.UPDATE_EXCHANGE_STATUS_SUCCESS,
                StatusCode = StatusCodes.Status200OK,
                Data = updatedExchange
            };
        }
        catch (Exception ex)
        {
            return new ResultModel
            {
                IsSuccess = false,
                ResponseCode = ResponseCodeConstants.FAILED,
                Message = ExchangeBatteryMessages.UPDATE_EXCHANGE_STATUS_FAILED,
                StatusCode = StatusCodes.Status500InternalServerError,
                Data = ex.InnerException?.Message ?? ex.Message
            };
        }
    }

}
