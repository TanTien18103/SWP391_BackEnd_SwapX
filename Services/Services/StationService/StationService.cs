using AutoMapper;
using BusinessObjects.Constants;
using BusinessObjects.Enums;
using BusinessObjects.Models;
using BusinessObjects.TimeCoreHelper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Repositories.Repositories.StationRepo;
using Services.ApiModels;
using Services.ApiModels.Station;
using Services.ServicesHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services.StationService
{
    public class StationService : IStationService
    {
        private readonly IStationRepo _stationRepository;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly AccountHelper _accountHelper;


        public StationService(IStationRepo stationRepository, IConfiguration configuration, IHttpContextAccessor httpContextAccessor, AccountHelper accountHelper)
        {
            _stationRepository = stationRepository;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _accountHelper = accountHelper;
        }

        public async Task<ResultModel> AddStation(AddStationRequest addStationRequest)
        {
            try
            {
                var station = new Station
                {
                    StationId = _accountHelper.GenerateShortGuid(),
                    StationName = addStationRequest.Name,
                    Location = addStationRequest.Location,
                    Status = StationStatusEnum.Active.ToString(),
                    Rating = 0m,
                    BatteryNumber = addStationRequest.BatteryNumber ?? 0, // Set to 0 if null
                    StartDate = TimeHepler.SystemTimeNow,
                    UpdateDate = TimeHepler.SystemTimeNow,
                };

                await _stationRepository.AddStation(station);
                var createdStation = await _stationRepository.GetStationById(station.StationId);
                return new ResultModel
                {
                    IsSuccess = true,
                    ResponseCode = ResponseCodeConstants.SUCCESS,
                    Message = ResponseMessageConstantsStation.ADD_STATION_SUCCESS,
                    Data = createdStation,
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
                    Message = ResponseMessageConstantsStation.ADD_STATION_FAIL,
                    Data = ex.Message
                };
            }
        }
        public async Task<ResultModel> GetAllStations()
        {
            try
            {
                var stations = await _stationRepository.GetAllStations();
                if (stations == null || stations.Count == 0)
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.NOT_FOUND,
                        Message = ResponseMessageConstantsStation.STATION_LIST_EMPTY,
                        Data = null,
                        StatusCode = StatusCodes.Status404NotFound
                    };
                }

                // Map sang object mới, ẩn trường station trong từng battery
                var response = stations.Select(station => new
                {
                    StationId = station.StationId,
                    StationName = station.StationName,
                    Location = station.Location,
                    Status = station.Status,
                    Rating = station.Rating,
                    BatteryNumber = station.BatteryNumber,
                    StartDate = station.StartDate,
                    UpdateDate = station.UpdateDate,
                    Batteries = station.Batteries.Select(b => new
                    {
                        BatteryId = b.BatteryId,
                        BatteryName = b.BatteryName,
                        Status = b.Status,
                        Capacity = b.Capacity,
                        BatteryType = b.BatteryType,
                        Specification = b.Specification,
                        BatteryQuality = b.BatteryQuality,
                        StartDate = b.StartDate,
                        UpdateDate = b.UpdateDate
                        // KHÔNG có trường station ở đây!
                    }).ToList()
                }).ToList();

                return new ResultModel
                {
                    IsSuccess = true,
                    ResponseCode = ResponseCodeConstants.SUCCESS,
                    Message = ResponseMessageConstantsStation.GET_ALL_STATION_SUCCESS,
                    Data = response,
                    StatusCode = StatusCodes.Status200OK
                };

            }
            catch (Exception ex)
            {
                return new ResultModel
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    IsSuccess = false,
                    ResponseCode = ResponseCodeConstants.FAILED,
                    Message = ResponseMessageConstantsStation.GET_ALL_STATION_FAIL,
                    Data = ex.Message
                };
            }
        }

        public async Task<ResultModel> GetStationById(string stationId)
        {
            try
            {
                var station = await _stationRepository.GetStationById(stationId);
                if (station == null)
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.NOT_FOUND,
                        Message = ResponseMessageConstantsStation.STATION_NOT_FOUND,
                        Data = null,
                        StatusCode = StatusCodes.Status404NotFound
                    };
                }

                // Map sang object mới, ẩn trường station trong từng battery
                var response = new
                {
                    StationId = station.StationId,
                    StationName = station.StationName,
                    Location = station.Location,
                    Status = station.Status,
                    Rating = station.Rating,
                    BatteryNumber = station.BatteryNumber,
                    StartDate = station.StartDate,
                    UpdateDate = station.UpdateDate,
                    Batteries = station.Batteries.Select(b => new
                    {
                        BatteryId = b.BatteryId,
                        BatteryName = b.BatteryName,
                        Status = b.Status,
                        Capacity = b.Capacity,
                        BatteryType = b.BatteryType,
                        Specification = b.Specification,
                        BatteryQuality = b.BatteryQuality,
                        StartDate = b.StartDate,
                        UpdateDate = b.UpdateDate
                        // KHÔNG có trường station ở đây!
                    }).ToList()
                };

                return new ResultModel
                {
                    IsSuccess = true,
                    ResponseCode = ResponseCodeConstants.SUCCESS,
                    Message = ResponseMessageConstantsStation.GET_STATION_SUCCESS,
                    Data = response,
                    StatusCode = StatusCodes.Status200OK
                };
            }
            catch (Exception ex)
            {
                return new ResultModel
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    IsSuccess = false,
                    ResponseCode = ResponseCodeConstants.FAILED,
                    Message = ResponseMessageConstantsStation.GET_STATION_FAIL,
                    Data = ex.Message
                };
            }
        }

        public async Task<ResultModel> UpdateStation(UpdateStationRequest updateStationRequest)
        {
            try
            {
                var existingStation = await _stationRepository.GetStationById(updateStationRequest.StationId);
                if (existingStation == null)
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.NOT_FOUND,
                        Message = ResponseMessageConstantsStation.STATION_NOT_FOUND,
                        Data = null,
                        StatusCode = StatusCodes.Status404NotFound
                    };
                }

                if (updateStationRequest.BatteryNumber.HasValue)
                {
                    existingStation.BatteryNumber = updateStationRequest.BatteryNumber.Value;
                }
                else
                {
                    existingStation.BatteryNumber = 0;
                }
                if (!string.IsNullOrEmpty(updateStationRequest.Location))
                {
                    existingStation.Location = updateStationRequest.Location;
                }
                if (!string.IsNullOrEmpty(updateStationRequest.Name))
                {
                    existingStation.StationName = updateStationRequest.Name;
                }
                existingStation.UpdateDate = TimeHepler.SystemTimeNow;
                var updatedStation = await _stationRepository.UpdateStation(existingStation);
                return new ResultModel
                {
                    IsSuccess = true,
                    ResponseCode = ResponseCodeConstants.SUCCESS,
                    Message = ResponseMessageConstantsStation.UPDATE_STATION_SUCCESS,
                    Data = updatedStation,
                    StatusCode = StatusCodes.Status200OK
                };
            }
            catch (Exception ex)
            {
                return new ResultModel
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    IsSuccess = false,
                    ResponseCode = ResponseCodeConstants.FAILED,
                    Message = ResponseMessageConstantsStation.UPDATE_STATION_FAILED,
                    Data = ex.Message
                };
            }
        }
        public async Task<ResultModel> DeleteStation(string stationId)
        {
            try
            {
                var existingStation = await _stationRepository.GetStationById(stationId);
                if (existingStation == null)
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.NOT_FOUND,
                        Message = ResponseMessageConstantsStation.STATION_NOT_FOUND,
                        Data = null,
                        StatusCode = StatusCodes.Status404NotFound
                    };
                }
                existingStation.Status = StationStatusEnum.Inactive.ToString();
                existingStation.UpdateDate = TimeHepler.SystemTimeNow;
                var updatedStation = await _stationRepository.UpdateStation(existingStation);
                return new ResultModel
                {
                    IsSuccess = true,
                    ResponseCode = ResponseCodeConstants.SUCCESS,
                    Message = ResponseMessageConstantsStation.DELETE_STATION_SUCCESS,
                    StatusCode = StatusCodes.Status200OK
                };

            }
            catch (Exception ex)
            {
                return new ResultModel
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    IsSuccess = false,
                    ResponseCode = ResponseCodeConstants.FAILED,
                    Message = ResponseMessageConstantsStation.DELETE_STATION_FAILED,
                    Data = ex.Message
                };
            }
        }
    }
}
