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
using Repositories.Repositories.StationScheduleRepo;
using Services.ApiModels.StationSchedule;
using Repositories.Repositories.FormRepo;
using Repositories.Repositories.AccountRepo;
using Repositories.Repositories.SlotRepo;
using Services.Services.EmailService;

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
    private readonly IStationScheduleRepo _stationScheduleRepo;
    private readonly IFormRepo _formRepo;
    private readonly IAccountRepo _accountRepo;
    private readonly BatteryHelper _batteryHelper;
    private readonly ISlotRepo _slotRepo;
    private readonly IEmailService _emailService;

    public ExchangeBatteryService(
        IExchangeBatteryRepo exchangeRepo,
        IBatteryRepo batteryRepo,
        IStationRepo stationRepo,
        IOrderRepository orderRepo,
        IBatteryReportRepo batteryReportRepository,
        IVehicleRepo vehicleRepo,
        IBatteryHistoryRepo batteryHistoryRepo,
        IStationScheduleRepo stationScheduleRepo,
        IFormRepo formRepo,
        IAccountRepo accountRepo,
        BatteryHelper batteryHelper,
        ISlotRepo slotRepo,
        IEmailService emailService
        )
    {
        _exchangeRepo = exchangeRepo;
        _batteryRepo = batteryRepo;
        _stationRepo = stationRepo;
        _orderRepo = orderRepo;
        _batteryReportRepository = batteryReportRepository;
        _vehicleRepo = vehicleRepo;
        _batteryHistoryRepo = batteryHistoryRepo;
        _stationScheduleRepo = stationScheduleRepo;
        _formRepo = formRepo;
        _accountRepo = accountRepo;
        _batteryHelper = batteryHelper;
        _slotRepo = slotRepo;
        _emailService = emailService;
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
            ScheduleTime = entity.Schedule.Form.StartDate,
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
            if (!string.IsNullOrEmpty(request.StaffId))
            {
                //Lấy thông tin của staff
                var staff = await _accountRepo.GetAccountById(request.StaffId);
                if (staff == null)
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.NOT_FOUND,
                        Message = ResponseMessageConstantsUser.USER_NOT_FOUND,
                        StatusCode = StatusCodes.Status404NotFound
                    };
                }

                if (staff.Role != RoleEnums.Bsstaff.ToString())
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.FAILED,
                        Message = ResponseMessageConstantsUser.USER_NOT_STAFF,
                        StatusCode = StatusCodes.Status400BadRequest
                    };
                }
            }

            //Lấy thông tin order liên quan
            var order = await _orderRepo.GetOrderByIdAsync(exchange.OrderId);
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

            //Check form trong station schedule xem có phải id pin trong form giống với của id pin new trong exchange, pin new phải ở trạng thái Booked
            var formInSchdeule = await _formRepo.GetByStationScheduleId(exchange.ScheduleId);
            if (formInSchdeule == null)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    ResponseCode = ResponseCodeConstants.NOT_FOUND,
                    Message = ResponseMessageConstantsForm.FORM_NOT_FOUND,
                    StatusCode = StatusCodes.Status404NotFound
                };
            }
            if (formInSchdeule.BatteryId != newBattery.BatteryId)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    ResponseCode = ResponseCodeConstants.FAILED,
                    Message = ExchangeBatteryMessages.NEW_BATTERY_ID_NOT_MATCHED_WITH_FORM_BATTERY_ID,
                    StatusCode = StatusCodes.Status400BadRequest
                };
            }
            if (newBattery.Status != BatteryStatusEnums.Booked.ToString())
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    ResponseCode = ResponseCodeConstants.FAILED,
                    Message = ExchangeBatteryMessages.NEW_BATTERY_NOT_IN_BOOKED_STATUS,
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
            //kiểm tra newbattery có trong trạm không
            if (newBattery.StationId != station.StationId)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    ResponseCode = ResponseCodeConstants.FAILED,
                    Message = ExchangeBatteryMessages.NEW_BATTERY_NOT_IN_STATION,
                    StatusCode = StatusCodes.Status400BadRequest
                };
            }
            //Lấy lịch trình liên quan
            var schedule = await _stationScheduleRepo.GetStationScheduleById(exchange.ScheduleId);
            if (schedule == null)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    ResponseCode = ResponseCodeConstants.NOT_FOUND,
                    Message = ResponseMessageConstantsStationSchedule.STATION_SCHEDULE_NOT_FOUND,
                    StatusCode = StatusCodes.Status404NotFound
                };
            }
            //Kiểm tra lịch trình chưa hoàn thành hoặc hủy
            if (schedule.Date.HasValue &&
                schedule.Date!.Value.Date > TimeHepler.SystemTimeNow.Date)
            {
                return new ResultModel
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    IsSuccess = false,
                    ResponseCode = ResponseCodeConstants.FAILED,
                    Message = ResponseMessageConstantsStationSchedule.CANNOT_COMPLETE_BEFORE_DATE,
                    Data = null
                };
            }
            //Xử lý logic theo trạng thái yêu cầu
            switch (request.Status)
            {
                case ExchangeStatusEnums.Completed:

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

                    // Kiểm tra Order đã thanh toán
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

                    // Lấy slot chứa newBattery và cập nhật
                    var slotWithNewBattery = await _slotRepo.GetByBatteryId(newBattery.BatteryId);
                    if (slotWithNewBattery != null)
                    {
                        slotWithNewBattery.BatteryId = null;
                        slotWithNewBattery.Status = SlotStatusEnum.Empty.ToString();
                        slotWithNewBattery.UpdateDate = TimeHepler.SystemTimeNow;

                        await _slotRepo.UpdateSlot(slotWithNewBattery);
                    }

                    // Nếu có oldBattery thì đặt vào slot trống
                    if (oldBattery != null)
                    {
                        var emptySlot = await _slotRepo.GetFirstAvailableSlot(exchange.StationId);
                        if (emptySlot == null)
                        {
                            return new ResultModel
                            {
                                IsSuccess = false,
                                ResponseCode = ResponseCodeConstants.FAILED,
                                Message = ExchangeBatteryMessages.NO_EMPTY_SLOT_FOR_OLD_BATTERY,
                                StatusCode = StatusCodes.Status400BadRequest
                            };
                        }
                        emptySlot.BatteryId = oldBattery.BatteryId;
                        emptySlot.Status = SlotStatusEnum.Occupied.ToString();
                        emptySlot.UpdateDate = TimeHepler.SystemTimeNow;
                        await _slotRepo.UpdateSlot(emptySlot);

                        // Cập nhật thông tin oldBattery
                        oldBattery.BatteryName = _batteryHelper.GenBatteryName(oldBattery.BatteryType, oldBattery.Specification, "EX");
                        oldBattery.StationId = exchange.StationId;
                        oldBattery.Status = BatteryStatusEnums.Maintenance.ToString();
                        oldBattery.UpdateDate = TimeHepler.SystemTimeNow;

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
                        await _batteryHistoryRepo.AddBatteryHistory(oldbatteryhistory);
                    }

                    // Cập nhật newBattery
                    newBattery.BatteryName = _batteryHelper.GenBatteryName(newBattery.BatteryType, newBattery.Specification, "USE");
                    newBattery.StationId = null;
                    newBattery.Status = BatteryStatusEnums.InUse.ToString();
                    newBattery.UpdateDate = TimeHepler.SystemTimeNow;

                    // Gắn newBattery cho xe
                    vehicleOfExchange.BatteryId = newBattery.BatteryId;
                    vehicleOfExchange.UpdateDate = TimeHepler.SystemTimeNow;

                    // Cập nhật lịch trình
                    schedule.Status = StationScheduleStatusEnums.Completed.ToString();
                    schedule.UpdateDate = TimeHepler.SystemTimeNow;

                    // Cập nhật exchange
                    exchange.Status = ExchangeStatusEnums.Completed.ToString();
                    exchange.StaffAccountId = request.StaffId;
                    exchange.Notes = request.Note;
                    exchange.UpdateDate = TimeHepler.SystemTimeNow;

                    // Tạo history cho newBattery
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

                    await _vehicleRepo.UpdateVehicle(vehicleOfExchange);
                    await _batteryRepo.UpdateBattery(newBattery);
                    await _stationScheduleRepo.UpdateStationSchedule(schedule);
                    await _exchangeRepo.Update(exchange);
                    await _batteryHistoryRepo.AddBatteryHistory(newbatteryhistory);

                    // Gửi email cho khách
                    var Completedform = await _formRepo.GetById(formInSchdeule.FormId);
                    if (Completedform != null)
                    {
                        var account = await _accountRepo.GetAccountById(Completedform.AccountId);
                        if (account != null && !string.IsNullOrEmpty(account.Email))
                        {
                            var subject = EmailConstants.EXCHANGE_COMPLETED_SUBJECT;
                            var body = string.Format(
                                EmailConstants.EXCHANGE_COMPLETED_BODY,
                                account.Username,
                                station.StationName,
                                TimeHepler.SystemTimeNow.ToString("dd/MM/yyyy HH:mm"),
                                newBattery.BatteryId
                            );
                            await _emailService.SendEmail(account.Email, subject, body);
                        }
                    }

                    break;

                case ExchangeStatusEnums.Cancelled:
                    exchange.Status = ExchangeStatusEnums.Cancelled.ToString();
                    exchange.StaffAccountId = request.StaffId;
                    exchange.UpdateDate = TimeHepler.SystemTimeNow;
                    exchange.Notes = request.Note;

                    schedule.Status = StationScheduleStatusEnums.Cancelled.ToString();
                    schedule.UpdateDate = TimeHepler.SystemTimeNow;

                    newBattery.Status = BatteryStatusEnums.Available.ToString();
                    newBattery.UpdateDate = TimeHepler.SystemTimeNow;

                    if (order != null)
                    {
                        order.Status = PaymentStatus.Failed.ToString();
                        order.UpdateDate = TimeHepler.SystemTimeNow;
                        await _orderRepo.UpdateOrderStatusAsync(order.OrderId, order.Status);
                    }

                    await _batteryRepo.UpdateBattery(newBattery);
                    await _stationScheduleRepo.UpdateStationSchedule(schedule);
                    await _exchangeRepo.Update(exchange);

                    var Cancelform = await _formRepo.GetById(formInSchdeule.FormId);
                    if (Cancelform != null)
                    {
                        var account = await _accountRepo.GetAccountById(Cancelform.AccountId);
                        if (account != null && !string.IsNullOrEmpty(account.Email))
                        {
                            var subject = EmailConstants.EXCHANGE_CANCELLED_SUBJECT;
                            var body = string.Format(
                                EmailConstants.EXCHANGE_CANCELLED_BODY,
                                account.Username,
                                station.StationName,
                                TimeHepler.SystemTimeNow.ToString("dd/MM/yyyy HH:mm"),
                                request.Note ?? "(No additional notes)"
                            );
                            await _emailService.SendEmail(account.Email, subject, body);
                        }
                    }

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



            return new ResultModel
            {
                IsSuccess = true,
                ResponseCode = ResponseCodeConstants.SUCCESS,
                Message = ExchangeBatteryMessages.UPDATE_EXCHANGE_STATUS_SUCCESS,
                StatusCode = StatusCodes.Status200OK,
                Data = exchange
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

    public async Task<ResultModel> GetExchangesByScheduleId(string stationscheduleId)
    {
        var list = await _exchangeRepo.GetByScheduleId(stationscheduleId);
        if (list == null || !list.Any())
            return new ResultModel { StatusCode = 204, Message = ExchangeMessages.ListEmpty };
        var response = list.Select(MapToResponse).ToList();
        return new ResultModel { StatusCode = 200, IsSuccess = true, Data = response };
    }

    public async Task<ResultModel> GetPendingExchangeByVINAndAccountID(string vin, string accountId)
    {
        try
        {
            var exchange = await _exchangeRepo.GetPendingByVinAndAccountId(vin, accountId);
            if (exchange == null)
            {
                return new ResultModel()
                {
                    IsSuccess = false,
                    ResponseCode = ResponseCodeConstants.NOT_FOUND,
                    Message = ExchangeBatteryMessages.EXCHANGE_BATTERY_NOT_FOUND,
                    StatusCode = StatusCodes.Status404NotFound
                };
            }
            var response = MapToResponse(exchange);
            return new ResultModel()
            {
                IsSuccess = true,
                ResponseCode = ResponseCodeConstants.SUCCESS,
                Message = ExchangeBatteryMessages.GET_PENDING_EXCHANGE_SUCCESS,
                StatusCode = StatusCodes.Status200OK,
                Data = response
            };
        }
        catch (Exception ex)
        {
            return new ResultModel()
            {
                IsSuccess = false,
                ResponseCode = ResponseCodeConstants.FAILED,
                Message = ExchangeBatteryMessages.GET_PENDING_EXCHANGE_FAILED,
                StatusCode = StatusCodes.Status500InternalServerError,
                Data = ex.InnerException?.Message ?? ex.Message
            };
        }
    }
}
