using BusinessObjects.Constants;
using BusinessObjects.Enums;
using BusinessObjects.Models;
using BusinessObjects.TimeCoreHelper;
using CloudinaryDotNet;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Repositories.Repositories.AccountRepo;
using Repositories.Repositories.BatteryHistoryRepo;
using Repositories.Repositories.BatteryRepo;
using Repositories.Repositories.EvDriverRepo;
using Repositories.Repositories.StationRepo;
using Repositories.Repositories.VehicleRepo;
using Services.ApiModels;
using Services.ApiModels.Battery;
using Services.ApiModels.Station;
using Services.Services.StationService;
using Services.ServicesHelpers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Services.Services.BatteryService
{
    public class BatteryService : IBatteryService
    {
        private readonly IBatteryRepo _batteryRepo;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly AccountHelper _accountHelper;
        private readonly IStationRepo _stationRepo;
        private readonly IVehicleRepo _vehicleRepo;
        private readonly IBatteryHistoryRepo _batteryHistoryRepo;
        private readonly IEvDriverRepo _evDriverRepo;
        private readonly IAccountRepo _accountRepo;

        //--------------------------------------------------------------
        public BatteryService(IBatteryRepo batteryRepo, IConfiguration configuration, IHttpContextAccessor httpContextAccessor, AccountHelper accountHelper, IStationRepo stationRepo, IVehicleRepo vehicleRepo, IBatteryHistoryRepo batteryHistoryRepo, IEvDriverRepo evDriverRepo, IAccountRepo accountRepo)
        {
            _batteryRepo = batteryRepo;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _accountHelper = accountHelper;
            _stationRepo = stationRepo;
            _vehicleRepo = vehicleRepo;
            _batteryHistoryRepo = batteryHistoryRepo;
            _evDriverRepo = evDriverRepo;
            _accountRepo = accountRepo;
        }
        //--------------------------------------------------------------
        public async Task<ResultModel> AddBattery(AddBatteryRequest addBatteryRequest)
        {
            try
            {
                var battery = new Battery
                {
                    BatteryId = _accountHelper.GenerateShortGuid(),
                    Capacity = addBatteryRequest.Capacity,
                    BatteryName = addBatteryRequest.BatteryName,
                    Status = BatteryStatusEnums.Available.ToString(),
                    Specification = addBatteryRequest.Specification.ToString(),
                    BatteryType = addBatteryRequest.BatteryType.ToString(),
                    BatteryQuality = addBatteryRequest.BatteryQuality,
                    StartDate = TimeHepler.SystemTimeNow,
                    UpdateDate = TimeHepler.SystemTimeNow,
                    Image = addBatteryRequest.Image
                };

                await _batteryRepo.AddBattery(battery);
                //record lại lịch sử pin
                var batteryHistory = new BatteryHistory
                {
                    BatteryHistoryId = _accountHelper.GenerateShortGuid(),
                    BatteryId = battery.BatteryId,
                    Notes = HistoryActionConstants.BATTERY_CREATED_BY_ADMIN.ToString(),
                    ActionType = BatteryHistoryActionTypeEnums.Created.ToString(),
                    EnergyLevel = battery.Capacity.ToString(),
                    Status = BatteryHistoryStatusEnums.Active.ToString(),
                    ActionDate = TimeHepler.SystemTimeNow,
                    StartDate = TimeHepler.SystemTimeNow,
                    UpdateDate = TimeHepler.SystemTimeNow

                };
                await _batteryHistoryRepo.AddBatteryHistory(batteryHistory);
                var response = new
                {
                    BatteryId = battery.BatteryId,
                    BatteryName = battery.BatteryName,
                    Status = battery.Status,
                    Capacity = battery.Capacity,
                    BatteryType = battery.BatteryType,
                    Specification = battery.Specification,
                    BatteryQuality = battery.BatteryQuality,
                    StartDate = battery.StartDate,
                    UpdateDate = battery.UpdateDate,
                    Image = battery.Image,
                    Station = battery.Station == null ? null : new
                    {
                        StationId = battery.Station.StationId,
                        StationName = battery.Station.StationName,
                        Location = battery.Station.Location,
                        Status = battery.Station.Status,
                        Rating = battery.Station.Rating,
                        BatteryNumber = battery.Station.BatteryNumber,
                        StartDate = battery.Station.StartDate,
                        UpdateDate = battery.Station.UpdateDate,
                        Image = battery.Station.Image,
                        // KHÔNG có trường Batteries ở đây!
                    },
                    BatteryHistory = batteryHistory == null ? null : new
                    {
                        BatteryHistoryId = batteryHistory.BatteryHistoryId,
                        BatteryId = batteryHistory.BatteryId,
                        ExchangeBatteryId = batteryHistory.ExchangeBatteryId,
                        Vin = batteryHistory.Vin,
                        StationId = batteryHistory.StationId,
                        ActionType = batteryHistory.ActionType,
                        EnergyLevel = batteryHistory.EnergyLevel,
                        ActionDate = batteryHistory.ActionDate,
                        Notes = batteryHistory.Notes,
                        Status = batteryHistory.Status,
                        StartDate = batteryHistory.StartDate,
                        UpdateDate = batteryHistory.UpdateDate
                    }
                };
                return new ResultModel
                {
                    IsSuccess = true,
                    ResponseCode = ResponseCodeConstants.SUCCESS,
                    Message = ResponseMessageConstantsBattery.ADD_BATTERY_SUCCESS,
                    Data = response,
                    StatusCode = StatusCodes.Status201Created
                };
            }
            catch (Exception ex)
            {
                return new ResultModel
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    IsSuccess = false,
                    ResponseCode = ResponseCodeConstants.FAILED,
                    Message = ResponseMessageConstantsBattery.ADD_BATTERY_FAIL,
                    Data = ex.Message
                };
            }

        }
        //--------------------------------------------------------------
        public async Task<ResultModel> GetAllBatteries()
        {
            try
            {
                var batteries = await _batteryRepo.GetAllBatteries();
                var response = new List<object>();

                foreach (var b in batteries)
                {
                    var vehicle = await _vehicleRepo.GetVehicleByBatteryId(b.BatteryId);

                    response.Add(new
                    {
                        BatteryId = b.BatteryId,
                        BatteryName = b.BatteryName,
                        Status = b.Status,
                        Capacity = b.Capacity,
                        BatteryType = b.BatteryType,
                        Specification = b.Specification,
                        BatteryQuality = b.BatteryQuality,
                        StartDate = b.StartDate,
                        UpdateDate = b.UpdateDate,
                        Image = b.Image,

                        Station = b.Station == null ? null : new
                        {
                            StationId = b.Station.StationId,
                            StationName = b.Station.StationName,
                            Location = b.Station.Location,
                            Status = b.Station.Status,
                            Rating = b.Station.Rating,
                            BatteryNumber = b.Station.BatteryNumber,
                            StartDate = b.Station.StartDate,
                            UpdateDate = b.Station.UpdateDate,
                            Image = b.Station.Image,
                        },
                        Vehicle = vehicle == null ? null : new
                        {
                            Vin = vehicle.Vin,
                            VehicleName = vehicle.VehicleName,
                            CustomerId = vehicle.CustomerId
                        }
                    });
                }

                return new ResultModel
                {
                    IsSuccess = true,
                    ResponseCode = ResponseCodeConstants.SUCCESS,
                    Message = ResponseMessageConstantsBattery.GET_BATTERY_LIST_SUCCESS,
                    Data = response,
                    StatusCode = StatusCodes.Status200OK
                };
            }
            catch (Exception ex)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    ResponseCode = ResponseCodeConstants.INTERNAL_SERVER_ERROR,
                    Message = ResponseMessageConstantsBattery.GET_ALL_BATTERY_FAIL,
                    Data = null,
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }
        //--------------------------------------------------------------
        public async Task<ResultModel> GetBatteriesByStationId(string stationId)
        {
            try
            {

                var battery = await _batteryRepo.GetBatteriesByStationId(stationId);
                if (battery == null)
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.NOT_FOUND,
                        Message = ResponseMessageConstantsBattery.BATTERY_NOT_FOUND,
                        Data = null,
                        StatusCode = StatusCodes.Status404NotFound
                    };
                }
                return new ResultModel
                {
                    IsSuccess = true,
                    ResponseCode = ResponseCodeConstants.SUCCESS,
                    Message = ResponseMessageConstantsBattery.GET_BATTERY_SUCCESS,
                    Data = battery,
                    StatusCode = StatusCodes.Status200OK
                };


            }
            catch (Exception ex)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    ResponseCode = ResponseCodeConstants.INTERNAL_SERVER_ERROR,
                    Message = ResponseMessageConstantsBattery.GET_BATTERY_FAIL,
                    Data = null,
                    StatusCode = StatusCodes.Status500InternalServerError
                };

            }
        }
        //--------------------------------------------------------------
        public async Task<ResultModel> GetBatteryById(string batteryId)
        {
            try
            {
                var b = await _batteryRepo.GetBatteryById(batteryId);
                if (b == null)
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.NOT_FOUND,
                        Message = ResponseMessageConstantsBattery.BATTERY_NOT_FOUND,
                        Data = null,
                        StatusCode = StatusCodes.Status404NotFound
                    };
                }
                var vehicle = await _vehicleRepo.GetVehicleByBatteryId(b.BatteryId);

                // Map sang object mới, station chỉ chứa thông tin cơ bản, không có batteries
                // Before creating the response object
                var stationObj = b.Station == null ? null : new
                {
                    StationId = b.Station.StationId,
                    StationName = b.Station.StationName,
                    Location = b.Station.Location,
                    Status = b.Station.Status,
                    Rating = b.Station.Rating,
                    BatteryNumber = b.Station.BatteryNumber,
                    StartDate = b.Station.StartDate,
                    UpdateDate = b.Station.UpdateDate,
                    Image = b.Station.Image,
                    // No Batteries field here!
                };

                var response = new
                {
                    BatteryId = b.BatteryId,
                    BatteryName = b.BatteryName,
                    Status = b.Status,
                    Capacity = b.Capacity,
                    BatteryType = b.BatteryType,
                    Specification = b.Specification,
                    BatteryQuality = b.BatteryQuality,
                    StartDate = b.StartDate,
                    UpdateDate = b.UpdateDate,
                    Image = b.Image,
                    Station = stationObj,
                    Vehicle = b.Vehicles == null ? null : new
                    {
                        Vin = vehicle?.Vin,
                        VehicleName = vehicle?.VehicleName,
                        CustomerId = vehicle?.CustomerId,
                    }
                };


                return new ResultModel
                {
                    IsSuccess = true,
                    ResponseCode = ResponseCodeConstants.SUCCESS,
                    Message = ResponseMessageConstantsBattery.GET_BATTERY_SUCCESS,
                    Data = response,
                    StatusCode = StatusCodes.Status200OK
                };
            }
            catch (Exception ex)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    ResponseCode = ResponseCodeConstants.INTERNAL_SERVER_ERROR,
                    Message = ResponseMessageConstantsBattery.GET_BATTERY_FAIL,
                    Data = null,
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }
        //--------------------------------------------------------------
        public async Task<ResultModel> UpdateBattery(UpdateBatteryRequest updateBatteryRequest)
        {
            try
            {

                var existingBattery = await _batteryRepo.GetBatteryById(updateBatteryRequest.BatteryId);
                if (existingBattery == null)
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.NOT_FOUND,
                        Message = ResponseMessageConstantsBattery.BATTERY_NOT_FOUND,
                        Data = null,
                        StatusCode = StatusCodes.Status404NotFound
                    };
                }

                // Only update if a new value is provided
                if (updateBatteryRequest.Capacity.HasValue)
                    existingBattery.Capacity = updateBatteryRequest.Capacity.Value;
                if (updateBatteryRequest.BatteryQuality.HasValue)
                    existingBattery.BatteryQuality = updateBatteryRequest.BatteryQuality.Value;
                if (!string.IsNullOrEmpty(updateBatteryRequest.BatteryName))
                    existingBattery.BatteryName = updateBatteryRequest.BatteryName;
                if (!string.IsNullOrEmpty(updateBatteryRequest.Image))
                    existingBattery.Image = updateBatteryRequest.Image;
                existingBattery.UpdateDate = TimeHepler.SystemTimeNow;
                var updatedBattery = await _batteryRepo.UpdateBattery(existingBattery);
                //record lại lịch sử pin
                var batteryHistory = new BatteryHistory
                {
                    BatteryHistoryId = _accountHelper.GenerateShortGuid(),
                    BatteryId = updatedBattery.BatteryId,
                    Notes = HistoryActionConstants.BATTERY_UPDATED.ToString(),
                    ActionType = BatteryHistoryActionTypeEnums.Updated.ToString(),
                    EnergyLevel = updatedBattery.Capacity.ToString(),
                    UpdateDate = TimeHepler.SystemTimeNow

                };
                await _batteryHistoryRepo.AddBatteryHistory(batteryHistory);
                var response = new
                {
                    BatteryId = updatedBattery.BatteryId,
                    BatteryName = updatedBattery.BatteryName,
                    Status = updatedBattery.Status,
                    Capacity = updatedBattery.Capacity,
                    BatteryType = updatedBattery.BatteryType,
                    Specification = updatedBattery.Specification,
                    BatteryQuality = updatedBattery.BatteryQuality,
                    StartDate = updatedBattery.StartDate,
                    UpdateDate = updatedBattery.UpdateDate,
                    Image = updatedBattery.Image,
                    Station = updatedBattery.Station == null ? null : new
                    {
                        StationId = updatedBattery.Station.StationId,
                        StationName = updatedBattery.Station.StationName,
                        Location = updatedBattery.Station.Location,
                        Status = updatedBattery.Station.Status,
                        Rating = updatedBattery.Station.Rating,
                        BatteryNumber = updatedBattery.Station.BatteryNumber,
                        StartDate = updatedBattery.Station.StartDate,
                        UpdateDate = updatedBattery.Station.UpdateDate,
                        Image = updatedBattery.Station.Image,
                        // KHÔNG có trường Batteries ở đây!
                    },
                    BatteryHistory = batteryHistory == null ? null : new
                    {
                        BatteryHistoryId = batteryHistory.BatteryHistoryId,
                        BatteryId = batteryHistory.BatteryId,
                        ExchangeBatteryId = batteryHistory.ExchangeBatteryId,
                        Vin = batteryHistory.Vin,
                        StationId = batteryHistory.StationId,
                        ActionType = batteryHistory.ActionType,
                        EnergyLevel = batteryHistory.EnergyLevel,
                        ActionDate = batteryHistory.ActionDate,
                        Notes = batteryHistory.Notes,
                        Status = batteryHistory.Status,
                        StartDate = batteryHistory.StartDate,
                        UpdateDate = batteryHistory.UpdateDate
                    }
                };

                return new ResultModel
                {
                    IsSuccess = true,
                    ResponseCode = ResponseCodeConstants.SUCCESS,
                    Message = ResponseMessageConstantsBattery.UPDATE_BATTERY_SUCCESS,
                    Data = response,
                    StatusCode = StatusCodes.Status200OK
                };

            }
            catch (Exception ex)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    ResponseCode = ResponseCodeConstants.FAILED,
                    Message = ResponseMessageConstantsBattery.UPDATE_BATTERY_FAILED,
                    Data = ex.Message,
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }
        //--------------------------------------------------------------
        public async Task<ResultModel> DeleteBattery(string batteryId)
        {
            try
            {

                var existingBattery = await _batteryRepo.GetBatteryById(batteryId);
                if (existingBattery == null)
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.NOT_FOUND,
                        Message = ResponseMessageConstantsBattery.BATTERY_NOT_FOUND,
                        StatusCode = StatusCodes.Status404NotFound
                    };
                }
                existingBattery.Status = BatteryStatusEnums.Decommissioned.ToString();
                existingBattery.UpdateDate = TimeHepler.SystemTimeNow;
                var deletedBattery = await _batteryRepo.UpdateBattery(existingBattery);
                //record lại lịch sử pin
                var batteryHistory = new BatteryHistory
                {
                    BatteryHistoryId = _accountHelper.GenerateShortGuid(),
                    BatteryId = deletedBattery.BatteryId,
                    Notes = HistoryActionConstants.BATTERY_DELETED.ToString(),
                    ActionType = BatteryHistoryActionTypeEnums.Deleted.ToString(),
                    EnergyLevel = deletedBattery.Capacity.ToString(),
                    Status = BatteryHistoryStatusEnums.Active.ToString(),
                    ActionDate = TimeHepler.SystemTimeNow,
                    StartDate = TimeHepler.SystemTimeNow,
                    UpdateDate = TimeHepler.SystemTimeNow

                };
                await _batteryHistoryRepo.AddBatteryHistory(batteryHistory);
                var response = new
                {
                    BatteryId = deletedBattery.BatteryId,
                    BatteryName = deletedBattery.BatteryName,
                    Status = deletedBattery.Status,
                    Capacity = deletedBattery.Capacity,
                    BatteryType = deletedBattery.BatteryType,
                    Specification = deletedBattery.Specification,
                    BatteryQuality = deletedBattery.BatteryQuality,
                    StartDate = deletedBattery.StartDate,
                    UpdateDate = deletedBattery.UpdateDate,
                    Image = deletedBattery.Image,
                    Station = deletedBattery.Station == null ? null : new
                    {
                        StationId = deletedBattery.Station.StationId,
                        StationName = deletedBattery.Station.StationName,
                        Location = deletedBattery.Station.Location,
                        Status = deletedBattery.Station.Status,
                        Rating = deletedBattery.Station.Rating,
                        BatteryNumber = deletedBattery.Station.BatteryNumber,
                        StartDate = deletedBattery.Station.StartDate,
                        UpdateDate = deletedBattery.Station.UpdateDate,
                        Image = deletedBattery.Station.Image
                        // KHÔNG có trường Batteries ở đây!
                    },
                    BatteryHistory = batteryHistory == null ? null : new
                    {
                        BatteryHistoryId = batteryHistory.BatteryHistoryId,
                        BatteryId = batteryHistory.BatteryId,
                        ExchangeBatteryId = batteryHistory.ExchangeBatteryId,
                        Vin = batteryHistory.Vin,
                        StationId = batteryHistory.StationId,
                        ActionType = batteryHistory.ActionType,
                        EnergyLevel = batteryHistory.EnergyLevel,
                        ActionDate = batteryHistory.ActionDate,
                        Notes = batteryHistory.Notes,
                        Status = batteryHistory.Status,
                        StartDate = batteryHistory.StartDate,
                        UpdateDate = batteryHistory.UpdateDate
                    }
                };
                return new ResultModel
                {
                    IsSuccess = true,
                    ResponseCode = ResponseCodeConstants.SUCCESS,
                    Message = ResponseMessageConstantsBattery.DELETE_BATTERY_SUCCESS,
                    StatusCode = StatusCodes.Status200OK
                };

            }
            catch (Exception ex)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    ResponseCode = ResponseCodeConstants.FAILED,
                    Message = ResponseMessageConstantsBattery.DELETE_BATTERY_FAILED,
                    Data = ex.Message,
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }
        //--------------------------------------------------------------
        public async Task<ResultModel> AddBatteryInStation(AddBatteryInStationRequest addBatteryInStationRequest)
        {
            try
            {
                var existingBattery = await _batteryRepo.GetBatteryById(addBatteryInStationRequest.BatteryId);
                if (existingBattery == null)
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.NOT_FOUND,
                        Message = ResponseMessageConstantsBattery.BATTERY_NOT_FOUND,
                        StatusCode = StatusCodes.Status404NotFound
                    };
                }
                var stationResult = await _stationRepo.GetStationById(addBatteryInStationRequest.StationId);
                if (stationResult == null)
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.NOT_FOUND,
                        Message = ResponseMessageConstantsStation.STATION_NOT_FOUND,
                        StatusCode = StatusCodes.Status404NotFound
                    };
                }
                if (existingBattery.Status == BatteryStatusEnums.Decommissioned.ToString() || existingBattery.Status == BatteryStatusEnums.InUse.ToString())
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.FAILED,
                        Message = ResponseMessageConstantsBattery.BATTERY_DECOMMISSIONED_INUSE_CANNOT_ADD_TO_STATION,
                        StatusCode = StatusCodes.Status400BadRequest
                    };
                }
                existingBattery.StationId = addBatteryInStationRequest.StationId;
                existingBattery.UpdateDate = TimeHepler.SystemTimeNow;
                var updatedBattery = await _batteryRepo.UpdateBattery(existingBattery);

                var batteryInStation = await _batteryRepo.GetBatteriesByStationId(addBatteryInStationRequest.StationId);
                if (batteryInStation.Count > 30)
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.FAILED,
                        Message = ResponseMessageConstantsStation.STATION_BATTERY_LIMIT,
                        StatusCode = StatusCodes.Status400BadRequest
                    };
                }
                stationResult.BatteryNumber = batteryInStation.Count;
                await _stationRepo.UpdateStation(stationResult);

                // Lấy lại battery đã cập nhật, include station
                var batteryDetail = await _batteryRepo.GetBatteryById(updatedBattery.BatteryId);
                //record lại lịch sử pin
                var batteryHistory = new BatteryHistory
                {
                    BatteryHistoryId = _accountHelper.GenerateShortGuid(),
                    BatteryId = batteryDetail.BatteryId,
                    Notes = HistoryActionConstants.BATTERY_ADDED_TO_STATION.ToString()+" "+ batteryDetail.Station.StationName,
                    ActionType = BatteryHistoryActionTypeEnums.Moved.ToString(),
                    StationId = batteryDetail.StationId,
                    EnergyLevel = batteryDetail.Capacity.ToString(),
                    Status = BatteryHistoryStatusEnums.Active.ToString(),
                    ActionDate = TimeHepler.SystemTimeNow,
                    StartDate = TimeHepler.SystemTimeNow,
                    UpdateDate = TimeHepler.SystemTimeNow

                };
                await _batteryHistoryRepo.AddBatteryHistory(batteryHistory);
                var response = new
                {
                    BatteryId = batteryDetail.BatteryId,
                    BatteryName = batteryDetail.BatteryName,
                    Status = batteryDetail.Status,
                    Capacity = batteryDetail.Capacity,
                    BatteryType = batteryDetail.BatteryType,
                    Specification = batteryDetail.Specification,
                    BatteryQuality = batteryDetail.BatteryQuality,
                    StartDate = batteryDetail.StartDate,
                    UpdateDate = batteryDetail.UpdateDate,
                    Image = batteryDetail.Image,
                    Station = batteryDetail.Station == null ? null : new
                    {
                        StationId = batteryDetail.Station.StationId,
                        StationName = batteryDetail.Station.StationName,
                        Location = batteryDetail.Station.Location,
                        Status = batteryDetail.Station.Status,
                        Rating = batteryDetail.Station.Rating,
                        BatteryNumber = batteryDetail.Station.BatteryNumber,
                        StartDate = batteryDetail.Station.StartDate,
                        UpdateDate = batteryDetail.Station.UpdateDate,
                        Image = batteryDetail.Station.Image,
                        // KHÔNG có trường Batteries ở đây!
                    },
                    BatteryHistory = batteryHistory == null ? null : new
                    {
                        BatteryHistoryId = batteryHistory.BatteryHistoryId,
                        BatteryId = batteryHistory.BatteryId,
                        ExchangeBatteryId = batteryHistory.ExchangeBatteryId,
                        Vin = batteryHistory.Vin,
                        StationId = batteryHistory.StationId,
                        ActionType = batteryHistory.ActionType,
                        EnergyLevel = batteryHistory.EnergyLevel,
                        ActionDate = batteryHistory.ActionDate,
                        Notes = batteryHistory.Notes,
                        Status = batteryHistory.Status,
                        StartDate = batteryHistory.StartDate,
                        UpdateDate = batteryHistory.UpdateDate
                    }
                };

                return new ResultModel
                {
                    IsSuccess = true,
                    ResponseCode = ResponseCodeConstants.SUCCESS,
                    Message = ResponseMessageConstantsBattery.ADD_BATTERY_IN_STATION_SUCCESS,
                    Data = response,
                    StatusCode = StatusCodes.Status200OK
                };
            }
            catch (Exception ex)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    ResponseCode = ResponseCodeConstants.FAILED,
                    Message = ResponseMessageConstantsBattery.ADD_BATTERY_IN_STATION_FAILED,
                    Data = ex.Message,
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }

        public async Task<ResultModel> UpdateBatteryStatusInStation(UpdateBatteryStatusRequest updateBatteryStatusRequest)
        {
            try
            {
                var existingBattery = await _batteryRepo.GetBatteryById(updateBatteryStatusRequest.BatteryId);
                if (existingBattery == null)
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.NOT_FOUND,
                        Message = ResponseMessageConstantsBattery.BATTERY_NOT_FOUND,
                        StatusCode = StatusCodes.Status404NotFound
                    };
                }
                if (existingBattery.Status == updateBatteryStatusRequest.Status.ToString())
                {
                    return new ResultModel
                    {
                        StatusCode = StatusCodes.Status400BadRequest,
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.FAILED,
                        Message = ResponseMessageConstantsBattery.BATTERY_STATUS_ALREADY_EXISTS,
                        Data = null
                    };
                }
                if (existingBattery.Status == BatteryStatusEnums.Decommissioned.ToString())
                {
                    return new ResultModel
                    {
                        StatusCode = StatusCodes.Status400BadRequest,
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.FAILED,
                        Message = ResponseMessageConstantsBattery.BATTERY_DECOMMISSIONED_CANNOT_UPDATE_STATUS,
                        Data = null
                    };
                }

                existingBattery.Status = updateBatteryStatusRequest.Status.ToString();
                existingBattery.UpdateDate = TimeHepler.SystemTimeNow;
                var updatedBattery = await _batteryRepo.UpdateBattery(existingBattery);

                var batteryDetail = await _batteryRepo.GetBatteryById(updatedBattery.BatteryId);

                var response = new
                {
                    BatteryId = batteryDetail.BatteryId,
                    BatteryName = batteryDetail.BatteryName,
                    Status = batteryDetail.Status,
                    Capacity = batteryDetail.Capacity,
                    BatteryType = batteryDetail.BatteryType,
                    Specification = batteryDetail.Specification,
                    BatteryQuality = batteryDetail.BatteryQuality,
                    StartDate = batteryDetail.StartDate,
                    UpdateDate = batteryDetail.UpdateDate,
                    Image = batteryDetail.Image,
                    Station = batteryDetail.Station == null ? null : new
                    {
                        StationId = batteryDetail.Station.StationId,
                        StationName = batteryDetail.Station.StationName,
                        Location = batteryDetail.Station.Location,
                        Status = batteryDetail.Station.Status,
                        Rating = batteryDetail.Station.Rating,
                        BatteryNumber = batteryDetail.Station.BatteryNumber,
                        StartDate = batteryDetail.Station.StartDate,
                        UpdateDate = batteryDetail.Station.UpdateDate,
                        Image = batteryDetail.Station.Image,
                    }
                };

                return new ResultModel
                {
                    IsSuccess = true,
                    ResponseCode = ResponseCodeConstants.SUCCESS,
                    Message = ResponseMessageConstantsBattery.UPDATE_BATTERY_STATUS_IN_STATION_SUCCESS,
                    Data = response,
                    StatusCode = StatusCodes.Status200OK
                };
            }
            catch (Exception ex)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    ResponseCode = ResponseCodeConstants.FAILED,
                    Message = ResponseMessageConstantsBattery.UPDATE_BATTERY_STATUS_IN_STATION_FAILED,
                    Data = ex.Message,
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }

        public async Task<ResultModel> GetAllBatterySuitVehicle(GetAllBatterySuitVehicle getAllBatterySuitVehicle)
        {
            try
            {
                if (!_httpContextAccessor.HttpContext.Request.Headers.TryGetValue("Authorization", out var authHeader)
             || string.IsNullOrEmpty(authHeader)
             || !authHeader.ToString().StartsWith("Bearer "))
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.UNAUTHORIZED,
                        Message = ResponseMessageIdentity.TOKEN_NOT_SEND,
                        StatusCode = StatusCodes.Status401Unauthorized
                    };
                }

                var token = authHeader.ToString().Substring("Bearer ".Length);
                var accountId = await _accountRepo.GetAccountIdFromToken(token);
                if (string.IsNullOrEmpty(accountId))
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.UNAUTHORIZED,
                        Message = ResponseMessageIdentity.TOKEN_INVALID_OR_EXPIRED,
                        StatusCode = StatusCodes.Status401Unauthorized
                    };
                }
                var evDriver = await _evDriverRepo.GetDriverByAccountId(accountId);
                if (evDriver == null)
                {
                    return new ResultModel
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.NOT_FOUND,
                        Message = ResponseMessageConstantsUser.EVDRIVER_NOT_FOUND,
                        Data = null
                    };
                }
                var vehicle = await _vehicleRepo.GetVehicleById(getAllBatterySuitVehicle.Vin);
                if (vehicle == null)
                {
                    return new ResultModel
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.NOT_FOUND,
                        Message = ResponseMessageConstantsVehicle.VEHICLE_NOT_FOUND,
                        Data = null
                    };
                }
                // kiểm tra xem vehicle có thuộc về customer không
                if (vehicle.CustomerId != evDriver.CustomerId)
                {
                    return new ResultModel
                    {
                        StatusCode = StatusCodes.Status403Forbidden,
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.FORBIDDEN,
                        Message = ResponseMessageConstantsVehicle.VEHICLE_NOT_OWNED,
                        Data = null
                    };
                }
                var station = await _stationRepo.GetStationById(getAllBatterySuitVehicle.StationId);
                if (station == null)
                {
                    return new ResultModel
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.NOT_FOUND,
                        Message = ResponseMessageConstantsStation.STATION_NOT_FOUND,
                        Data = null
                    };
                }
                var battery = await _batteryRepo.GetBatteryById(vehicle.BatteryId);
                var batteries = await _batteryRepo.GetBatteriesByStationIdAndSpecification(getAllBatterySuitVehicle.StationId, battery.Specification, battery.BatteryType);
                if (batteries == null || batteries.Count==0)
                {
                    return new ResultModel
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.NOT_FOUND,
                        Message = ResponseMessageConstantsBattery.BATTERY_NOT_FOUND,
                        Data = null
                    };
                }


                if (battery == null)
                {
                    return new ResultModel
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.NOT_FOUND,
                        Message = ResponseMessageConstantsBattery.BATTERY_NOT_FOUND,
                        Data = null
                    };
                }
                var response = new List<object>();

                foreach (var b in batteries)
                {
                    response.Add(new
                    {
                        BatteryId = b.BatteryId,
                        BatteryName = b.BatteryName,
                        Status = b.Status,
                        Capacity = b.Capacity,
                        BatteryType = b.BatteryType,
                        Specification = b.Specification,
                        BatteryQuality = b.BatteryQuality,
                        StartDate = b.StartDate,
                        UpdateDate = b.UpdateDate,
                        Image = b.Image,
                        Station = b.Station == null ? null : new
                        {
                            StationId = b.Station.StationId,
                            StationName = b.Station.StationName,
                            Location = b.Station.Location,
                            Status = b.Station.Status,
                            Rating = b.Station.Rating,
                            BatteryNumber = b.Station.BatteryNumber,
                            StartDate = b.Station.StartDate,
                            UpdateDate = b.Station.UpdateDate,
                            Image = b.Station.Image,
                        },

                    });
                }
                return new ResultModel
                {
                    IsSuccess = true,
                    ResponseCode = ResponseCodeConstants.SUCCESS,
                    Message = ResponseMessageConstantsBattery.GET_BATTERY_LIST_SUCCESS,
                    Data = response,
                    StatusCode = StatusCodes.Status200OK
                };

            }
            catch (Exception ex)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    ResponseCode = ResponseCodeConstants.INTERNAL_SERVER_ERROR,
                    Message = ResponseMessageConstantsBattery.GET_BATTERY_FAIL,
                    Data = null,
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }

        public async Task<ResultModel> CreateBatteryByVehicleName(CreateBatteryByVehicleNameRequest createBatteryByVehicleNameRequest)
        {
            try
            {
                var battery = new Battery();
                // Kiểm tra battery
                if (createBatteryByVehicleNameRequest.VehicleName.ToString() == VehicleNameEnums.YADEA_I6_Lithium_Battery.ToString())
                {
                    battery = new Battery
                    {
                        BatteryId = _accountHelper.GenerateShortGuid(),
                        Status = BatteryStatusEnums.Available.ToString(),
                        Capacity = 100,
                        BatteryQuality = 100.00m,
                        BatteryType = BatteryTypeEnums.Lithium.ToString(),
                        BatteryName = createBatteryByVehicleNameRequest.BatteryName,
                        Specification = BatterySpecificationEnums.V48_Ah13.ToString(),
                        StartDate = TimeHepler.SystemTimeNow,
                        UpdateDate = TimeHepler.SystemTimeNow,
                        Image = createBatteryByVehicleNameRequest.Image
                    };
                    await _batteryRepo.AddBattery(battery);

                }
                else if (createBatteryByVehicleNameRequest.VehicleName.ToString() == VehicleNameEnums.YADEA_I6_Accumulator.ToString())
                {
                    battery = new Battery
                    {
                        BatteryId = _accountHelper.GenerateShortGuid(),
                        Status = BatteryStatusEnums.Available.ToString(),
                        Capacity = 100,
                        BatteryQuality = 100.00m,
                        BatteryType = BatteryTypeEnums.Accumulator.ToString(),
                        BatteryName = createBatteryByVehicleNameRequest.BatteryName,
                        Specification = BatterySpecificationEnums.V48_Ah13.ToString(),
                        StartDate = TimeHepler.SystemTimeNow,
                        UpdateDate = TimeHepler.SystemTimeNow,
                        Image = createBatteryByVehicleNameRequest.Image,
                    };
                    await _batteryRepo.AddBattery(battery);


                }
                else if (createBatteryByVehicleNameRequest.VehicleName.ToString() == VehicleNameEnums.YADEA_I8_VINTAGE.ToString() || createBatteryByVehicleNameRequest.VehicleName.ToString() == VehicleNameEnums.YADEA_I8.ToString())
                {
                    battery = new Battery
                    {
                        BatteryId = _accountHelper.GenerateShortGuid(),
                        Status = BatteryStatusEnums.Available.ToString(),
                        Capacity = 100,
                        BatteryQuality = 100.00m,
                        BatteryType = BatteryTypeEnums.Graphene_TTFAR_Accumulator.ToString(),
                        BatteryName = createBatteryByVehicleNameRequest.BatteryName,
                        Specification = BatterySpecificationEnums.V48_Ah22.ToString(),
                        StartDate = TimeHepler.SystemTimeNow,
                        UpdateDate = TimeHepler.SystemTimeNow,
                        Image = createBatteryByVehicleNameRequest.Image,
                    };
                    await _batteryRepo.AddBattery(battery);

                }
                else if (createBatteryByVehicleNameRequest.VehicleName.ToString() == VehicleNameEnums.YADEA_IFUN.ToString() || createBatteryByVehicleNameRequest.VehicleName.ToString() == VehicleNameEnums.YADEA_IGO.ToString())
                {
                    battery = new Battery
                    {
                        BatteryId = _accountHelper.GenerateShortGuid(),
                        Status = BatteryStatusEnums.Available.ToString(),
                        Capacity = 100,
                        BatteryQuality = 100.00m,
                        BatteryType = BatteryTypeEnums.Lithium.ToString(),
                        BatteryName = createBatteryByVehicleNameRequest.BatteryName,
                        Specification = BatterySpecificationEnums.V48_Ah12.ToString(),
                        StartDate = TimeHepler.SystemTimeNow,
                        UpdateDate = TimeHepler.SystemTimeNow,
                        Image = createBatteryByVehicleNameRequest.Image,
                    };
                    await _batteryRepo.AddBattery(battery);

                }
                else if (createBatteryByVehicleNameRequest.VehicleName.ToString() == VehicleNameEnums.YADEA_VITO.ToString())
                {
                    battery = new Battery
                    {
                        BatteryId = _accountHelper.GenerateShortGuid(),
                        Status = BatteryStatusEnums.Available.ToString(),
                        Capacity = 100,
                        BatteryQuality = 100.00m,
                        BatteryType = BatteryTypeEnums.Lithium.ToString(),
                        BatteryName = createBatteryByVehicleNameRequest.BatteryName,
                        Specification = BatterySpecificationEnums.V36_Ah10_4.ToString(),
                        StartDate = TimeHepler.SystemTimeNow,
                        UpdateDate = TimeHepler.SystemTimeNow,
                        Image = createBatteryByVehicleNameRequest.Image,
                    };
                    await _batteryRepo.AddBattery(battery);

                }
                else if (createBatteryByVehicleNameRequest.VehicleName.ToString() == VehicleNameEnums.YADEA_FLIT.ToString())
                {
                    battery = new Battery
                    {
                        BatteryId = _accountHelper.GenerateShortGuid(),
                        Status = BatteryStatusEnums.Available.ToString(),
                        Capacity = 100,
                        BatteryQuality = 100.00m,
                        BatteryType = BatteryTypeEnums.Lithium.ToString(),
                        BatteryName = createBatteryByVehicleNameRequest.BatteryName,
                        Specification = BatterySpecificationEnums.V36_Ah7_8.ToString(),
                        StartDate = TimeHepler.SystemTimeNow,
                        UpdateDate = TimeHepler.SystemTimeNow,
                        Image = createBatteryByVehicleNameRequest.Image,
                    };
                    await _batteryRepo.AddBattery(battery);

                }

                else if (createBatteryByVehicleNameRequest.VehicleName.ToString() == VehicleNameEnums.YADEA_VELAX.ToString() || createBatteryByVehicleNameRequest.VehicleName.ToString() == VehicleNameEnums.YADEA_VELAX_SOOBIN.ToString())
                {
                    battery = new Battery
                    {
                        BatteryId = _accountHelper.GenerateShortGuid(),
                        Status = BatteryStatusEnums.Available.ToString(),
                        Capacity = 100,
                        BatteryQuality = 100.00m,
                        BatteryType = BatteryTypeEnums.LFP.ToString(),
                        BatteryName = createBatteryByVehicleNameRequest.BatteryName,
                        Specification = BatterySpecificationEnums.V72_Ah30.ToString(),
                        StartDate = TimeHepler.SystemTimeNow,
                        UpdateDate = TimeHepler.SystemTimeNow,
                        Image = createBatteryByVehicleNameRequest.Image,
                    };
                    await _batteryRepo.AddBattery(battery);

                }

                else if (createBatteryByVehicleNameRequest.VehicleName.ToString() == VehicleNameEnums.YADEA_VOLTGUARD_U.ToString())
                {
                    battery = new Battery
                    {
                        BatteryId = _accountHelper.GenerateShortGuid(),
                        Status = BatteryStatusEnums.Available.ToString(),
                        Capacity = 100,
                        BatteryQuality = 100.00m,
                        BatteryType = BatteryTypeEnums.LFP.ToString(),
                        BatteryName = createBatteryByVehicleNameRequest.BatteryName,
                        Specification = BatterySpecificationEnums.V72_Ah50.ToString(),
                        StartDate = TimeHepler.SystemTimeNow,
                        UpdateDate = TimeHepler.SystemTimeNow,
                        Image = createBatteryByVehicleNameRequest.Image,
                    };
                    await _batteryRepo.AddBattery(battery);

                }

                else if (createBatteryByVehicleNameRequest.VehicleName.ToString() == VehicleNameEnums.YADEA_VOLTGUARD_P.ToString())
                {
                    battery = new Battery
                    {
                        BatteryId = _accountHelper.GenerateShortGuid(),
                        Status = BatteryStatusEnums.Available.ToString(),
                        Capacity = 100,
                        BatteryQuality = 100.00m,
                        BatteryType = BatteryTypeEnums.Graphene_TTFAR_Accumulator.ToString(),
                        BatteryName = createBatteryByVehicleNameRequest.BatteryName,
                        Specification = BatterySpecificationEnums.V72_Ah38.ToString(),
                        StartDate = TimeHepler.SystemTimeNow,
                        UpdateDate = TimeHepler.SystemTimeNow,
                        Image = createBatteryByVehicleNameRequest.Image,
                    };
                    await _batteryRepo.AddBattery(battery);

                }

                else if (createBatteryByVehicleNameRequest.VehicleName.ToString() == VehicleNameEnums.YADEA_ORLA_P.ToString() || createBatteryByVehicleNameRequest.VehicleName.ToString() == VehicleNameEnums.YADEA_OCEAN.ToString() || createBatteryByVehicleNameRequest.VehicleName.ToString() == VehicleNameEnums.YADEA_ODORA_S.ToString() || createBatteryByVehicleNameRequest.VehicleName.ToString() == VehicleNameEnums.YADEA_ODORA_S2.ToString()
                    || createBatteryByVehicleNameRequest.VehicleName.ToString() == VehicleNameEnums.YADEA_M6I.ToString() || createBatteryByVehicleNameRequest.VehicleName.ToString() == VehicleNameEnums.YADEA_VIGOR.ToString() || createBatteryByVehicleNameRequest.VehicleName.ToString() == VehicleNameEnums.YADEA_X_MEN_NEO.ToString())
                {
                    battery = new Battery
                    {
                        BatteryId = _accountHelper.GenerateShortGuid(),
                        Status = BatteryStatusEnums.Available.ToString(),
                        Capacity = 100,
                        BatteryQuality = 100.00m,
                        BatteryType = BatteryTypeEnums.Graphene_TTFAR_Accumulator.ToString(),
                        BatteryName = createBatteryByVehicleNameRequest.BatteryName,
                        Specification = BatterySpecificationEnums.V60_Ah22.ToString(),
                        StartDate = TimeHepler.SystemTimeNow,
                        UpdateDate = TimeHepler.SystemTimeNow,
                        Image = createBatteryByVehicleNameRequest.Image,
                    };
                    await _batteryRepo.AddBattery(battery);

                }

                else if (createBatteryByVehicleNameRequest.VehicleName.ToString() == VehicleNameEnums.YADEA_ORIS.ToString() || createBatteryByVehicleNameRequest.VehicleName.ToString() == VehicleNameEnums.YADEA_ORIS_SOOBIN.ToString() || createBatteryByVehicleNameRequest.VehicleName.ToString() == VehicleNameEnums.YADEA_OSSY.ToString())
                {
                    battery = new Battery
                    {
                        BatteryId = _accountHelper.GenerateShortGuid(),
                        Status = BatteryStatusEnums.Available.ToString(),
                        Capacity = 100,
                        BatteryQuality = 100.00m,
                        BatteryType = BatteryTypeEnums.Graphene_TTFAR_Accumulator.ToString(),
                        BatteryName = createBatteryByVehicleNameRequest.BatteryName,
                        Specification = BatterySpecificationEnums.V72_Ah22.ToString(),
                        StartDate = TimeHepler.SystemTimeNow,
                        UpdateDate = TimeHepler.SystemTimeNow,
                        Image = createBatteryByVehicleNameRequest.Image,
                    };
                    await _batteryRepo.AddBattery(battery);

                }

                else if (createBatteryByVehicleNameRequest.VehicleName.ToString() == VehicleNameEnums.YADEA_ICUTE.ToString() || createBatteryByVehicleNameRequest.VehicleName.ToString() == VehicleNameEnums.YADEA_X_ZONE.ToString() || createBatteryByVehicleNameRequest.VehicleName.ToString() == VehicleNameEnums.YADEA_VEKOO.ToString()
                     || createBatteryByVehicleNameRequest.VehicleName.ToString() == VehicleNameEnums.YADEA_VEKOO_SOOBIN.ToString() || createBatteryByVehicleNameRequest.VehicleName.ToString() == VehicleNameEnums.YADEA_X_SKY.ToString() || createBatteryByVehicleNameRequest.VehicleName.ToString() == VehicleNameEnums.YADEA_X_BULL.ToString())
                {
                    battery = new Battery
                    {
                        BatteryId = _accountHelper.GenerateShortGuid(),
                        Status = BatteryStatusEnums.Available.ToString(),
                        Capacity = 100,
                        BatteryQuality = 100.00m,
                        BatteryType = BatteryTypeEnums.Graphene_TTFAR_Accumulator.ToString(),
                        BatteryName = createBatteryByVehicleNameRequest.BatteryName,
                        Specification = BatterySpecificationEnums.V48_Ah22.ToString(),
                        StartDate = TimeHepler.SystemTimeNow,
                        UpdateDate = TimeHepler.SystemTimeNow,
                        Image = createBatteryByVehicleNameRequest.Image,
                    };
                    await _batteryRepo.AddBattery(battery);

                }

                //record lại lịch sử pin
                var batteryHistory = new BatteryHistory
                {
                    BatteryHistoryId = _accountHelper.GenerateShortGuid(),
                    BatteryId = battery.BatteryId,
                    Notes = HistoryActionConstants.BATTERY_CREATED_BY_ADMIN.ToString(),
                    ActionType = BatteryHistoryActionTypeEnums.Created.ToString(),
                    EnergyLevel = battery.Capacity.ToString(),
                    Status = BatteryHistoryStatusEnums.Active.ToString(),
                    ActionDate = TimeHepler.SystemTimeNow,
                    StartDate = TimeHepler.SystemTimeNow,
                    UpdateDate = TimeHepler.SystemTimeNow

                };
                await _batteryHistoryRepo.AddBatteryHistory(batteryHistory);
                var response = new
                {
                    BatteryId = battery.BatteryId,
                    BatteryName = battery.BatteryName,
                    Status = battery.Status,
                    Capacity = battery.Capacity,
                    BatteryType = battery.BatteryType,
                    Specification = battery.Specification,
                    BatteryQuality = battery.BatteryQuality,
                    StartDate = battery.StartDate,
                    UpdateDate = battery.UpdateDate,
                    Image = battery.Image,
                    Station = battery.Station == null ? null : new
                    {
                        StationId = battery.Station.StationId,
                        StationName = battery.Station.StationName,
                        Location = battery.Station.Location,
                        Status = battery.Station.Status,
                        Rating = battery.Station.Rating,
                        BatteryNumber = battery.Station.BatteryNumber,
                        StartDate = battery.Station.StartDate,
                        UpdateDate = battery.Station.UpdateDate,
                        Image = battery.Station.Image,
                        // KHÔNG có trường Batteries ở đây!
                    },
                    BatteryHistory = batteryHistory == null ? null : new
                    {
                        BatteryHistoryId = batteryHistory.BatteryHistoryId,
                        BatteryId = batteryHistory.BatteryId,
                        ExchangeBatteryId = batteryHistory.ExchangeBatteryId,
                        Vin = batteryHistory.Vin,
                        StationId = batteryHistory.StationId,
                        ActionType = batteryHistory.ActionType,
                        EnergyLevel = batteryHistory.EnergyLevel,
                        ActionDate = batteryHistory.ActionDate,
                        Notes = batteryHistory.Notes,
                        Status = batteryHistory.Status,
                        StartDate = batteryHistory.StartDate,
                        UpdateDate = batteryHistory.UpdateDate
                    }
                };
                return new ResultModel
                {
                    IsSuccess = true,
                    ResponseCode = ResponseCodeConstants.SUCCESS,
                    Message = ResponseMessageConstantsBattery.ADD_BATTERY_SUCCESS,
                    Data = response,
                    StatusCode = StatusCodes.Status201Created
                };

            }
            catch (Exception ex)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    ResponseCode = ResponseCodeConstants.FAILED,
                    Message = ResponseMessageConstantsBattery.ADD_BATTERY_FAIL,
                    Data = ex.Message,
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }
    }
}
