﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repositories.Repositories.BatteryReportRepo;
using Repositories.Repositories.BatteryRepo;
using Repositories.Repositories.AccountRepo;
using Repositories.Repositories.StationRepo;
using Microsoft.AspNetCore.Http;
using Services.ServicesHelpers;
using Microsoft.Extensions.Configuration;
using Services.ApiModels;
using Services.ApiModels.BatteryReport;
using BusinessObjects.Constants;
using BusinessObjects.Enums;
using BusinessObjects.TimeCoreHelper;
using Repositories.Repositories.ExchangeBatteryRepo;

namespace Services.Services.BatteryReportService
{
    public class BatteryReportService : IBatteryReportService
    {
        private readonly IBatteryReportRepo _batteryReportRepository;
        private readonly IBatteryRepo _batteryRepo;
        private readonly IAccountRepo _accountRepo;
        private readonly IStationRepo _stationRepo;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly AccountHelper _accountHelper;
        private readonly IConfiguration _configuration;
        private readonly IExchangeBatteryRepo _exchangeBatteryRepo;
        public BatteryReportService(IBatteryReportRepo batteryReportRepository, IBatteryRepo batteryRepo, IAccountRepo accountRepo, IStationRepo stationRepo, IHttpContextAccessor httpContextAccessor, AccountHelper accountHelper, IConfiguration configuration, IExchangeBatteryRepo exchangeBatteryRepo)
        {
            _batteryReportRepository = batteryReportRepository;
            _batteryRepo = batteryRepo;
            _accountRepo = accountRepo;
            _stationRepo = stationRepo;
            _httpContextAccessor = httpContextAccessor;
            _accountHelper = accountHelper;
            _configuration = configuration;
            _exchangeBatteryRepo = exchangeBatteryRepo;
        }

        public async Task<ResultModel> AddBatteryReport(AddBatteryReportRequest addBatteryReportRequest)
        {
            try
            {

                var battery = await _batteryRepo.GetBatteryById(addBatteryReportRequest.BatteryId);
                if (battery == null)
                {
                    return new ResultModel
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.FAILED,
                        Message = ResponseMessageConstantsBattery.BATTERY_NOT_FOUND,
                        Data = null
                    };
                }
                var account = await _accountRepo.GetAccountById(addBatteryReportRequest.AccountId);
                if (account == null)
                {
                    return new ResultModel
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.FAILED,
                        Message = ResponseMessageConstantsUser.USER_NOT_FOUND,
                        Data = null
                    };
                }
                var station = await _stationRepo.GetStationById(addBatteryReportRequest.StationId);
                if (station == null)
                {
                    return new ResultModel
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.FAILED,
                        Message = ResponseMessageConstantsStation.STATION_NOT_FOUND,
                        Data = null
                    };
                }
                var batteryReport = new BusinessObjects.Models.BatteryReport
                {
                    BatteryReportId = _accountHelper.GenerateShortGuid(),
                    BatteryId = addBatteryReportRequest.BatteryId,
                    Name = addBatteryReportRequest.Name,
                    Image = addBatteryReportRequest.Image,
                    AccountId = addBatteryReportRequest.AccountId,
                    StationId = addBatteryReportRequest.StationId,
                    Description = addBatteryReportRequest.Description,
                    StartDate = TimeHepler.SystemTimeNow,
                    UpdateDate = TimeHepler.SystemTimeNow,
                    Status = BatteryReportStatusEnums.Pending.ToString(),
                };

                // If ExchangeBatteryId provided, link it
                if (!string.IsNullOrEmpty(addBatteryReportRequest.ExchangeBatteryId))
                {
                    var exchange = await _exchangeBatteryRepo.GetById(addBatteryReportRequest.ExchangeBatteryId);
                    if (exchange != null)
                    {
                        batteryReport.ExchangeBatteryId = exchange.ExchangeBatteryId;
                        // Optionally update exchange record update date
                        exchange.UpdateDate = TimeHepler.SystemTimeNow;
                        await _exchangeBatteryRepo.Update(exchange);
                    }
                }

                await _batteryReportRepository.AddBatteryReport(batteryReport);
                return new ResultModel
                {
                    StatusCode = StatusCodes.Status200OK,
                    IsSuccess = true,
                    ResponseCode = ResponseCodeConstants.SUCCESS,
                    Message = ResponseMessageConstantsBatteryReport.ADD_BATTERY_REPORT_SUCCESS,
                    Data = batteryReport
                };

            }
            catch (Exception ex)
            {
                return new ResultModel
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    IsSuccess = false,
                    ResponseCode = ResponseCodeConstants.FAILED,
                    Message = ResponseMessageConstantsBatteryReport.ADD_BATTERY_REPORT_FAIL,
                    Data = ex.Message
                };
            }
        }

        public async Task<ResultModel> DeleteBatteryReport(string batteryReportId)
        {
            try
            {

                var batteryReport = await _batteryReportRepository.GetBatteryReportById(batteryReportId);
                if (batteryReport == null)
                {
                    return new ResultModel
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.FAILED,
                        Message = ResponseMessageConstantsBatteryReport.BATTERY_REPORT_NOT_FOUND,
                        Data = null
                    };
                }
                batteryReport.Status = BatteryReportStatusEnums.Inactive.ToString();
                batteryReport.UpdateDate = TimeHepler.SystemTimeNow;
                await _batteryReportRepository.UpdateBatteryReport(batteryReport);
                return new ResultModel
                {
                    StatusCode = StatusCodes.Status200OK,
                    IsSuccess = true,
                    ResponseCode = ResponseCodeConstants.SUCCESS,
                    Message = ResponseMessageConstantsBatteryReport.DELETE_BATTERY_REPORT_SUCCESS,
                    Data = null
                };

            }
            catch (Exception ex)
            {
                return new ResultModel
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    IsSuccess = false,
                    ResponseCode = ResponseCodeConstants.FAILED,
                    Message = ResponseMessageConstantsBatteryReport.DELETE_BATTERY_REPORT_FAILED,
                    Data = ex.Message
                };
            }
        }

        public async Task<ResultModel> GetBatteryReportsByStation(string stationId)
        {
            try
            {
                var station = await _stationRepo.GetStationById(stationId);
                if (station == null)
                {
                    return new ResultModel
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.FAILED,
                        Message = ResponseMessageConstantsStation.STATION_NOT_FOUND,
                        Data = null
                    };
                }

                var reports = await _batteryReportRepository.GetBatteryReportsByStation(stationId);
                if (reports == null || !reports.Any())
                {
                    return new ResultModel
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.FAILED,
                        Message = ResponseMessageConstantsBatteryReport.BATTERY_REPORT_LIST_EMPTY,
                        Data = null
                    };
                }

                return new ResultModel
                {
                    StatusCode = StatusCodes.Status200OK,
                    IsSuccess = true,
                    ResponseCode = ResponseCodeConstants.SUCCESS,
                    Message = ResponseMessageConstantsBatteryReport.GET_ALL_BATTERY_REPORT_SUCCESS,
                    Data = reports
                };
            }
            catch (Exception ex)
            {
                return new ResultModel
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    IsSuccess = false,
                    ResponseCode = ResponseCodeConstants.FAILED,
                    Message = ResponseMessageConstantsBatteryReport.GET_ALL_BATTERY_REPORT_FAIL,
                    Data = ex.Message
                };
            }
        }
        public async Task<ResultModel> GetAllBatteryReports()
        {
            try
            {
                var batteryReports = await _batteryReportRepository.GetAllBatteryReports();
                if (batteryReports == null || !batteryReports.Any())
                {
                    return new ResultModel
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.FAILED,
                        Message = ResponseMessageConstantsBatteryReport.BATTERY_REPORT_LIST_EMPTY,
                        Data = null
                    };
                }
                return new ResultModel
                {
                    StatusCode = StatusCodes.Status200OK,
                    IsSuccess = true,
                    ResponseCode = ResponseCodeConstants.SUCCESS,
                    Message = ResponseMessageConstantsBatteryReport.GET_ALL_BATTERY_REPORT_SUCCESS,
                    Data = batteryReports
                };

            }
            catch (Exception ex)
            {
                return new ResultModel
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    IsSuccess = false,
                    ResponseCode = ResponseCodeConstants.FAILED,
                    Message = ResponseMessageConstantsBatteryReport.GET_ALL_BATTERY_REPORT_FAIL,
                    Data = ex.Message
                };
            }
        }

        public async Task<ResultModel> GetBatteryReportById(string batteryReportId)
        {
            try
            {
                var batteryReport = await _batteryReportRepository.GetBatteryReportById(batteryReportId);
                if (batteryReport == null)
                {
                    return new ResultModel
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.FAILED,
                        Message = ResponseMessageConstantsBatteryReport.BATTERY_REPORT_NOT_FOUND,
                        Data = null
                    };
                }
                return new ResultModel
                {
                    StatusCode = StatusCodes.Status200OK,
                    IsSuccess = true,
                    ResponseCode = ResponseCodeConstants.SUCCESS,
                    Message = ResponseMessageConstantsBatteryReport.GET_BATTERY_REPORT_SUCCESS,
                    Data = batteryReport
                };

            }
            catch (Exception ex)
            {
                return new ResultModel
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    IsSuccess = false,
                    ResponseCode = ResponseCodeConstants.FAILED,
                    Message = ResponseMessageConstantsBatteryReport.GET_BATTERY_REPORT_FAIL,
                    Data = ex.Message
                };
            }
        }

        public async Task<ResultModel> UpdateBatteryReport(UpdateBatteryReportRequest updateBatteryReportRequest)
        {
            try
            {
                var batteryReport = await _batteryReportRepository.GetBatteryReportById(updateBatteryReportRequest.BatteryReportId);
                if (batteryReport == null)
                {
                    return new ResultModel
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.FAILED,
                        Message = ResponseMessageConstantsBatteryReport.BATTERY_REPORT_NOT_FOUND,
                        Data = null
                    };
                }
                if (!string.IsNullOrEmpty(updateBatteryReportRequest.Name))
                {
                    batteryReport.Name = updateBatteryReportRequest.Name;
                }
                if (!string.IsNullOrEmpty(updateBatteryReportRequest.Image))
                {
                    batteryReport.Image = updateBatteryReportRequest.Image;
                }
                if (!string.IsNullOrEmpty(updateBatteryReportRequest.Description))
                {
                    batteryReport.Description = updateBatteryReportRequest.Description;
                }
                if (!string.IsNullOrEmpty(updateBatteryReportRequest.BatteryId))
                {
                    var battery = await _batteryRepo.GetBatteryById(updateBatteryReportRequest.BatteryId);
                    if (battery == null)
                    {
                        return new ResultModel
                        {
                            StatusCode = StatusCodes.Status404NotFound,
                            IsSuccess = false,
                            ResponseCode = ResponseCodeConstants.FAILED,
                            Message = ResponseMessageConstantsBattery.BATTERY_NOT_FOUND,
                            Data = null
                        };
                    }
                    batteryReport.BatteryId = updateBatteryReportRequest.BatteryId;
                }
                batteryReport.UpdateDate = TimeHepler.SystemTimeNow;
                await _batteryReportRepository.UpdateBatteryReport(batteryReport);
                return new ResultModel
                {
                    StatusCode = StatusCodes.Status200OK,
                    IsSuccess = true,
                    ResponseCode = ResponseCodeConstants.SUCCESS,
                    Message = ResponseMessageConstantsBatteryReport.UPDATE_BATTERY_REPORT_SUCCESS,
                    Data = batteryReport
                };

            }
            catch (Exception ex)
            {
                return new ResultModel
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    IsSuccess = false,
                    ResponseCode = ResponseCodeConstants.FAILED,
                    Message = ResponseMessageConstantsBatteryReport.UPDATE_BATTERY_REPORT_FAILED,
                    Data = ex.Message
                };
            }
        }

        public async Task<ResultModel> GetBatteryReportsByBatteryId(string batteryId)
        {
            try
            {
                var batteryReports = await _batteryReportRepository.GetBatteryReportByBatteryId(batteryId);
                if (batteryReports == null || !batteryReports.Any())
                {
                    return new ResultModel
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.FAILED,
                        Message = ResponseMessageConstantsBatteryReport.BATTERY_REPORT_LIST_EMPTY,
                        Data = null
                    };
                }
                return new ResultModel
                {
                    StatusCode = StatusCodes.Status200OK,
                    IsSuccess = true,
                    ResponseCode = ResponseCodeConstants.SUCCESS,
                    Message = ResponseMessageConstantsBatteryReport.GET_BATTERY_REPORT_SUCCESS,
                    Data = batteryReports
                };


                return new ResultModel
                {
                    StatusCode = StatusCodes.Status200OK,
                    IsSuccess = true,
                    ResponseCode = ResponseCodeConstants.SUCCESS,
                    Message = ResponseMessageConstantsBatteryReport.GET_BATTERY_REPORT_SUCCESS,
                    Data = batteryReports
                };

            }
            catch (Exception ex)
            {
                return new ResultModel
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    IsSuccess = false,
                    ResponseCode = ResponseCodeConstants.FAILED,
                    Message = ResponseMessageConstantsBatteryReport.GET_BATTERY_REPORT_FAIL,
                    Data = ex.Message
                };
            }
        }
    }
}
