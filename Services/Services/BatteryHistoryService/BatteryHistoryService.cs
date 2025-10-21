using BusinessObjects.Constants;
using BusinessObjects.Models;
using Microsoft.AspNetCore.Http;
using Repositories.Repositories.BatteryHistoryRepo;
using Repositories.Repositories.BatteryRepo;
using Repositories.Repositories.StationRepo;
using Services.ApiModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services.BatteryHistoryService
{
    public class BatteryHistoryService : IBatteryHistoryService
    {
        private readonly IBatteryHistoryRepo _batteryHistoryRepo;
        private readonly IStationRepo _stationRepo;
        private readonly IBatteryRepo _batteryRepo;
        public BatteryHistoryService(IBatteryHistoryRepo batteryHistoryRepo, IStationRepo stationRepo, IBatteryRepo batteryRepo)
        {
            _batteryHistoryRepo = batteryHistoryRepo;
            _stationRepo = stationRepo;
            _batteryRepo = batteryRepo;
        }

        public async Task<ResultModel> GetAllBatteryHistory()
        {
            try
            {
                var batteryHistories = await _batteryHistoryRepo.GetAllBatteryHistories();
                if (batteryHistories == null || batteryHistories.Count == 0)
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.NOT_FOUND,
                        Message = ResponseMessageBatteryHistory.BATTERY_HISTORY_LIST_EMPTY,
                        Data = null,
                        StatusCode = StatusCodes.Status404NotFound
                    };
                }
                return new ResultModel
                {
                    IsSuccess = true,
                    ResponseCode = ResponseCodeConstants.SUCCESS,
                    Message = ResponseMessageBatteryHistory.GET_BATTERY_HISTORY_SUCCESS,
                    Data = batteryHistories,
                    StatusCode = StatusCodes.Status200OK
                };
            }
            catch (Exception ex)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    ResponseCode = ResponseCodeConstants.INTERNAL_SERVER_ERROR,
                    Message = ResponseMessageBatteryHistory.GET_BATTERY_HISTORY_FAIL,
                    Data = null,
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }


        public async Task<ResultModel> GetBatteryHistoryByBatteryId(string batteryId)
        {
            try
            {
                var batteryHistories = await _batteryHistoryRepo.GetBatteryHistoryByBatteryId(batteryId);

                if (await _batteryRepo.GetBatteryById(batteryId) == null)
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.NOT_FOUND,
                        Message = ResponseMessageConstantsBattery.BATTERY_NOT_EXIST,
                        Data = null,
                        StatusCode = StatusCodes.Status404NotFound
                    };
                }
                if (batteryHistories == null || batteryHistories.Count == 0)
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.NOT_FOUND,
                        Message = ResponseMessageBatteryHistory.BATTERY_HISTORY_LIST_EMPTY,
                        Data = null,
                        StatusCode = StatusCodes.Status404NotFound
                    };
                }
                return new ResultModel
                {
                    IsSuccess = true,
                    ResponseCode = ResponseCodeConstants.SUCCESS,
                    Message = ResponseMessageBatteryHistory.GET_BATTERY_HISTORY_SUCCESS,
                    Data = batteryHistories,
                    StatusCode = StatusCodes.Status200OK
                };

            }
            catch (Exception ex)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    ResponseCode = ResponseCodeConstants.INTERNAL_SERVER_ERROR,
                    Message = ResponseMessageBatteryHistory.GET_BATTERY_HISTORY_FAIL,
                    Data = null,
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }

        public async Task<ResultModel> GetBatteryHistoryByStationId(string stationId)
        {
            try
            {
                var batteryHistories = await _batteryHistoryRepo.GetAllBatteryHistoryByStationId(stationId);
                if (await _stationRepo.GetStationById(stationId)==null)
                {
                    return new ResultModel {
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.NOT_FOUND,
                        Message = ResponseMessageConstantsStation.STATION_NOT_EXIST,
                        Data = null,
                        StatusCode = StatusCodes.Status404NotFound
                    };
                }
                if (batteryHistories == null || batteryHistories.Count == 0)
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.NOT_FOUND,
                        Message = ResponseMessageBatteryHistory.BATTERY_HISTORY_LIST_EMPTY,
                        Data = null,
                        StatusCode = StatusCodes.Status404NotFound
                    };
                }
                return new ResultModel
                {
                    IsSuccess = true,
                    ResponseCode = ResponseCodeConstants.SUCCESS,
                    Message = ResponseMessageBatteryHistory.GET_BATTERY_HISTORY_SUCCESS,
                    Data = batteryHistories,
                    StatusCode = StatusCodes.Status200OK
                };

            }
            catch (Exception e)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    ResponseCode = ResponseCodeConstants.INTERNAL_SERVER_ERROR,
                    Message = ResponseMessageBatteryHistory.GET_BATTERY_HISTORY_FAIL,
                    Data = null,
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }
    }
}
