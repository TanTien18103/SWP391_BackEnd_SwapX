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
                    BatteryNumber = addStationRequest.BatteryNumber,
                    StartDate = DateTime.UtcNow,
                    UpdateDate = DateTime.UtcNow,
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
        public Task<ResultModel> GetAllStations()
        {
            throw new NotImplementedException();
        }

        public Task<ResultModel> GetStationById(string stationId)
        {
            throw new NotImplementedException();
        }

        public Task<ResultModel> UpdateStation(UpdateStationRequest updateStationRequest)
        {
            throw new NotImplementedException();
        }
    }
}
