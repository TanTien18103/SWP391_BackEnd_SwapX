using BusinessObjects.Constants;
using BusinessObjects.Enums;
using BusinessObjects.TimeCoreHelper;
using Microsoft.AspNetCore.Http;
using Repositories.Repositories.Battery;
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

namespace Services.Services.Battery
{
    public class BatteryService : IBatteryService
    {
        private readonly IBatteryRepo _batteryRepo;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly AccountHelper _accountHelper;

        public BatteryService(IBatteryRepo batteryRepo, IConfiguration configuration, IHttpContextAccessor httpContextAccessor, AccountHelper accountHelper)
        {
            _batteryRepo = batteryRepo;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _accountHelper = accountHelper;
        }

        public async Task<ResultModel> AddBattery(AddBatteryRequest addBatteryRequest)
        {
            try
            {
                var battery = new BusinessObjects.Models.Battery
                {
                    BatteryId = _accountHelper.GenerateShortGuid(),
                    Capacity = addBatteryRequest.Capacity,
                    Status = BatteryStatusEnums.Available.ToString(),
                    Specification= addBatteryRequest.Specification.ToString(),
                    BatteryType=addBatteryRequest.BatteryType.ToString(),
                    BatteryQuality= addBatteryRequest.BatteryQuality,
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

        public Task<ResultModel> GetAllBatteries()
        {
            throw new NotImplementedException();
        }

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

        public Task<ResultModel> GetBatteryById(string batteryId)
        {
            throw new NotImplementedException();
        }

        public Task<ResultModel> UpdateBattery(UpdateBatteryRequest updateBatteryRequest)
        {
            throw new NotImplementedException();
        }
    }
}
