using BusinessObjects.Constants;
using BusinessObjects.Enums;
using BusinessObjects.TimeCoreHelper;
using Microsoft.AspNetCore.Http;
using Repositories.Repositories.BatteryRepo;
using Services.ApiModels;
using Services.ApiModels.Battery;
using Services.ApiModels.Station;
using Services.ServicesHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects.Constants;
using Microsoft.Extensions.Configuration;
using Repositories.Repositories.StationRepo;
using Services.Services.StationService;

namespace Services.Services.BatteryService
{
    public class BatteryService : IBatteryService
    {
        private readonly IBatteryRepo _batteryRepo;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly AccountHelper _accountHelper;
        private readonly IStationRepo _stationRepo;
        //--------------------------------------------------------------
        public BatteryService(IBatteryRepo batteryRepo, IConfiguration configuration, IHttpContextAccessor httpContextAccessor, AccountHelper accountHelper, IStationRepo stationRepo)
        {
            _batteryRepo = batteryRepo;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _accountHelper = accountHelper;
            _stationRepo = stationRepo;
        }
        //--------------------------------------------------------------
        public async Task<ResultModel> AddBattery(AddBatteryRequest addBatteryRequest)
        {
            try
            {
                var battery = new BusinessObjects.Models.Battery
                {
                    BatteryId = _accountHelper.GenerateShortGuid(),
                    Capacity = addBatteryRequest.Capacity,
                    Status = BatteryStatusEnums.Available.ToString(),
                    Specification = addBatteryRequest.Specification.ToString(),
                    BatteryType = addBatteryRequest.BatteryType.ToString(),
                    BatteryQuality = addBatteryRequest.BatteryQuality,
                    StartDate = TimeHepler.SystemTimeNow,
                    UpdateDate = TimeHepler.SystemTimeNow,
                };

                await _batteryRepo.AddBattery(battery);

                return new ResultModel
                {
                    IsSuccess = true,
                    ResponseCode = ResponseCodeConstants.SUCCESS,
                    Message = ResponseMessageConstantsBattery.ADD_BATTERY_SUCCESS,
                    Data = battery,
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
                return new ResultModel
                {
                    IsSuccess = true,
                    ResponseCode = ResponseCodeConstants.SUCCESS,
                    Message = ResponseMessageConstantsBattery.GET_BATTERY_LIST_SUCCESS,
                    Data = batteries,
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
                var battery = await _batteryRepo.GetBatteryById(batteryId);
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

                if (updateBatteryRequest.Specification.HasValue)
                    existingBattery.Specification = updateBatteryRequest.Specification.Value.ToString();

                if (updateBatteryRequest.BatteryType.HasValue)
                    existingBattery.BatteryType = updateBatteryRequest.BatteryType.Value.ToString();

                if (updateBatteryRequest.BatteryQuality.HasValue)
                    existingBattery.BatteryQuality = updateBatteryRequest.BatteryQuality.Value;

                existingBattery.UpdateDate = TimeHepler.SystemTimeNow;
                var updatedBattery = await _batteryRepo.UpdateBattery(existingBattery);

                return new ResultModel
                {
                    IsSuccess = true,
                    ResponseCode = ResponseCodeConstants.SUCCESS,
                    Message = ResponseMessageConstantsBattery.UPDATE_BATTERY_SUCCESS,
                    Data = updatedBattery,
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
        public async Task<ResultModel> addBatteryInStation(AddBatteryInStationRequest addBatteryInStationRequest)
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
                existingBattery.StationId = addBatteryInStationRequest.StationId;
                existingBattery.UpdateDate = TimeHepler.SystemTimeNow;
                var updateStation = await _batteryRepo.UpdateBattery(existingBattery);
                return new ResultModel
                {
                    IsSuccess = true,
                    ResponseCode = ResponseCodeConstants.SUCCESS,
                    Message = ResponseMessageConstantsBattery.ADD_BATTERY_IN_STATION_SUCCESS,
                    Data = updateStation,
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
    }
}
