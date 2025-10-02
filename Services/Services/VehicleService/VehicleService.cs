using BusinessObjects.Constants;
using BusinessObjects.Enums;
using BusinessObjects.TimeCoreHelper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Repositories.Repositories.BatteryRepo;
using Repositories.Repositories.PackageRepo;
using Repositories.Repositories.VehicleRepo;
using Services.ApiModels;
using Services.ApiModels.Vehicle;
using Services.ServicesHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace Services.Services.VehicleService
{
    public class VehicleService : IVehicleService
    {
        private readonly IVehicleRepo _vehicleRepo;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly AccountHelper _accountHelper;
        private readonly IBatteryRepo _batteryRepo;
        private readonly IPackageRepo _packageRepo;

        public VehicleService(IVehicleRepo vehicleRepo, IConfiguration configuration, IHttpContextAccessor httpContextAccessor, AccountHelper accountHelper, IBatteryRepo batteryRepo, IPackageRepo packageRepo)
        {
            _vehicleRepo = vehicleRepo;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _accountHelper = accountHelper;
            _batteryRepo = batteryRepo;
            _packageRepo = packageRepo;
        }

        public async Task<ResultModel> AddVehicle(AddVehicleRequest addVehicleRequest)
        {
            try
            {
                var vehicle = new BusinessObjects.Models.Vehicle
                {
                    Vin = _accountHelper.GenerateShortGuid(),
                    Status = VehicleStatusEnums.Active.ToString(),
                    VehicleType = addVehicleRequest.VehicleType.ToString(),
                    VehicleName = addVehicleRequest.VehicleName.ToString(),
                    PackageId = addVehicleRequest.PackageId,
                    BatteryId = addVehicleRequest.BatteryId,
                    StartDate = TimeHepler.SystemTimeNow,
                    UpdateDate = TimeHepler.SystemTimeNow
                };
                if (await _batteryRepo.GetBatteryById(addVehicleRequest.BatteryId) == null)
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
                if (await _packageRepo.GetPackageById(addVehicleRequest.PackageId) == null)
                {
                    return new ResultModel
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.NOT_FOUND,
                        Message = ResponseMessageConstantsPackage.PACKAGE_NOT_FOUND,
                        Data = null
                    };
                }
                await _vehicleRepo.AddVehicle(vehicle);
                return new ResultModel
                {
                    StatusCode = StatusCodes.Status200OK,
                    IsSuccess = true,
                    ResponseCode = ResponseCodeConstants.SUCCESS,
                    Message = ResponseMessageConstantsVehicle.ADD_VEHICLE_SUCCESS,
                    Data = vehicle
                };
            }
            catch (Exception ex)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    ResponseCode = ResponseCodeConstants.FAILED,
                    Message = ResponseMessageConstantsVehicle.ADD_VEHICLE_FAIL,
                    Data = ex.Message,
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }
        public async Task<ResultModel> GetAllVehicles()
        {
            try
            {
                var vehicles = await _vehicleRepo.GetAllVehicles();
                return new ResultModel
                {
                    StatusCode = StatusCodes.Status200OK,
                    IsSuccess = true,
                    ResponseCode = ResponseCodeConstants.SUCCESS,
                    Message = ResponseMessageConstantsVehicle.GET_ALL_VEHICLE_SUCCESS,
                    Data = vehicles
                };
            }
            catch (Exception ex)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    ResponseCode = ResponseCodeConstants.FAILED,
                    Message = ResponseMessageConstantsVehicle.GET_ALL_VEHICLE_FAIL,
                    Data = ex.Message,
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }

        public async Task<ResultModel> GetVehicleById(string vehicleId)
        {
            try
            {
                var vehicle = await _vehicleRepo.GetVehicleById(vehicleId);
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
                return new ResultModel
                {
                    StatusCode = StatusCodes.Status200OK,
                    IsSuccess = true,
                    ResponseCode = ResponseCodeConstants.SUCCESS,
                    Message = ResponseMessageConstantsVehicle.GET_VEHICLE_SUCCESS,
                    Data = vehicle
                };
            }
            catch (Exception ex)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    ResponseCode = ResponseCodeConstants.FAILED,
                    Message = ResponseMessageConstantsVehicle.GET_VEHICLE_FAIL,
                    Data = ex.Message,
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }

        public async Task<ResultModel> UpdateVehicle(UpdateVehicleRequest updateVehicleRequest)
        {
            try
            {

                var existingVehicle = await _vehicleRepo.GetVehicleById(updateVehicleRequest.Vin);
                if (existingVehicle == null)
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

                if (updateVehicleRequest.VehicleType != null)
                {
                    existingVehicle.VehicleType = updateVehicleRequest.VehicleType.ToString();
                }
                if (updateVehicleRequest.VehicleName != null)
                {
                    existingVehicle.VehicleName = updateVehicleRequest.VehicleName.ToString();
                }
                existingVehicle.UpdateDate = TimeHepler.SystemTimeNow;
                await _vehicleRepo.UpdateVehicle(existingVehicle);
                return new ResultModel
                {
                    StatusCode = StatusCodes.Status200OK,
                    IsSuccess = true,
                    ResponseCode = ResponseCodeConstants.SUCCESS,
                    Message = ResponseMessageConstantsVehicle.UPDATE_VEHICLE_SUCCESS,
                    Data = existingVehicle
                };
            }
            catch (Exception ex)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    ResponseCode = ResponseCodeConstants.FAILED,
                    Message = ResponseMessageConstantsVehicle.UPDATE_VEHICLE_FAILED,
                    Data = ex.Message,
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }
        public Task<ResultModel> DeleteVehicle(string vehicleId)
        {
            try
            {
                var existingVehicle = _vehicleRepo.GetVehicleById(vehicleId).Result;
                if (existingVehicle == null)
                {
                    return Task.FromResult(new ResultModel
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.NOT_FOUND,
                        Message = ResponseMessageConstantsVehicle.VEHICLE_NOT_FOUND,
                        Data = null
                    });
                }
                existingVehicle.Status = VehicleStatusEnums.Inactive.ToString();
                existingVehicle.UpdateDate = TimeHepler.SystemTimeNow;
                _vehicleRepo.UpdateVehicle(existingVehicle);
                return Task.FromResult(new ResultModel
                {
                    StatusCode = StatusCodes.Status200OK,
                    IsSuccess = true,
                    ResponseCode = ResponseCodeConstants.SUCCESS,
                    Message = ResponseMessageConstantsVehicle.DELETE_VEHICLE_SUCCESS,
                    Data = existingVehicle
                });
            }
            catch (Exception ex)
            {
                return Task.FromResult(new ResultModel
                {
                    IsSuccess = false,
                    ResponseCode = ResponseCodeConstants.FAILED,
                    Message = ResponseMessageConstantsVehicle.DELETE_VEHICLE_FAILED,
                    Data = ex.Message,
                    StatusCode = StatusCodes.Status500InternalServerError
                });
            }
        }
    }
}
