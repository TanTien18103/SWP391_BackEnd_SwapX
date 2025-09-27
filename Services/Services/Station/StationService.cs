using AutoMapper;
using Microsoft.Extensions.Configuration;
using BusinessObjects.Constants;
using BusinessObjects.Enums;
using Microsoft.AspNetCore.Http;
using Repositories.Repositories.Station;
using Services.ApiModels;
using Services.ApiModels.Station;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects.TimeCoreHelper;

namespace Services.Services.Station
{
    public class StationService : IStationService
    {
        private readonly IStationRepo _stationRepository;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public StationService(IStationRepo stationRepository, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _stationRepository = stationRepository;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ResultModel> AddStation(AddStationRequest addStationRequest)
        {
            try
            {
                var station = new BusinessObjects.Models.Station
                {
                    StationId = Guid.NewGuid().ToString(),
                    Location = addStationRequest.Location,
                    Status = StationStatusEnum.Active.ToString(),
                    Rating = addStationRequest.Rating,
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
                var stations = _stationRepository.GetAllStations().Result;
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
                return new ResultModel
                {
                    IsSuccess = true,
                    ResponseCode = ResponseCodeConstants.SUCCESS,
                    Message = ResponseMessageConstantsStation.GET_ALL_STATION_SUCCESS,
                    Data = stations,
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
                return new ResultModel
                {
                    IsSuccess = true,
                    ResponseCode = ResponseCodeConstants.SUCCESS,
                    Message = ResponseMessageConstantsStation.GET_STATION_SUCCESS,
                    Data = station,
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
                if (!string.IsNullOrEmpty(updateStationRequest.Rating))
                {
                    existingStation.Rating = updateStationRequest.Rating;
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
