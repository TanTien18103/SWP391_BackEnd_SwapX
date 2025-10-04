using BusinessObjects.Constants;
using BusinessObjects.Enums;
using BusinessObjects.Models;
using BusinessObjects.TimeCoreHelper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Repositories.Repositories.AccountRepo;
using Repositories.Repositories.BatteryRepo;
using Repositories.Repositories.EvDriverRepo;
using Repositories.Repositories.PackageRepo;
using Repositories.Repositories.VehicleRepo;
using Services.ApiModels;
using Services.ApiModels.Vehicle;
using Services.ServicesHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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
        private readonly IEvDriverRepo _evDriverRepo;
        private readonly IAccountRepo _accountRepo;

        public VehicleService(IVehicleRepo vehicleRepo, IConfiguration configuration, IHttpContextAccessor httpContextAccessor, IBatteryRepo batteryRepo, IPackageRepo packageRepo, IEvDriverRepo evDriverRepo, IAccountRepo accountRepo)
        {
            _vehicleRepo = vehicleRepo;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _accountHelper = new AccountHelper(configuration);
            _batteryRepo = batteryRepo;
            _packageRepo = packageRepo;
            _evDriverRepo = evDriverRepo;
            _accountRepo = accountRepo;
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

        public Task<ResultModel> GetVehicleByName(VehicleNameEnums vehicleName)
        {
            try
            {

                var vehicles = _vehicleRepo.GetVehiclesByName(vehicleName).Result;
                if (vehicles == null || vehicles.Count == 0)
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
                return Task.FromResult(new ResultModel
                {
                    StatusCode = StatusCodes.Status200OK,
                    IsSuccess = true,
                    ResponseCode = ResponseCodeConstants.SUCCESS,
                    Message = ResponseMessageConstantsVehicle.GET_VEHICLE_SUCCESS,
                    Data = vehicles
                });
            }
            catch (Exception ex)
            {
                return Task.FromResult(new ResultModel
                {
                    IsSuccess = false,
                    ResponseCode = ResponseCodeConstants.FAILED,
                    Message = ResponseMessageConstantsVehicle.GET_VEHICLE_FAIL,
                    Data = ex.Message,
                    StatusCode = StatusCodes.Status500InternalServerError
                });
            }
        }

        public Task<ResultModel> GetPackageByVehicleName(VehicleNameEnums vehicleName)
        {
            try
            {

                var vehicles = _vehicleRepo.GetVehiclesByName(vehicleName).Result;
                if (vehicles == null || vehicles.Count == 0)
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
                var packages = vehicles.Select(v => v.Package).Distinct().ToList();
                return Task.FromResult(new ResultModel
                {
                    StatusCode = StatusCodes.Status200OK,
                    IsSuccess = true,
                    ResponseCode = ResponseCodeConstants.SUCCESS,
                    Message = ResponseMessageConstantsVehicle.GET_VEHICLE_SUCCESS,
                    Data = packages
                });

            }
            catch (Exception ex)
            {
                return Task.FromResult(new ResultModel
                {
                    IsSuccess = false,
                    ResponseCode = ResponseCodeConstants.FAILED,
                    Message = ResponseMessageConstantsVehicle.GET_VEHICLE_FAIL,
                    Data = ex.Message,
                    StatusCode = StatusCodes.Status500InternalServerError
                });
            }
        }

        public async Task<ResultModel> LinkVehicle(LinkVehicleRequest linkVehicleRequest)
        {
            try
            {
                // Lấy accountId từ claims của người dùng đang đăng nhập
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
                // Kiểm tra battery
                var battery = await _batteryRepo.GetBatteryById(linkVehicleRequest.BatteryId);
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

                // Tạo vehicle mới
                var vehicle = new BusinessObjects.Models.Vehicle
                {
                    Vin = linkVehicleRequest.VIN,
                    Status = VehicleStatusEnums.Active.ToString(),
                    VehicleType = linkVehicleRequest.VehicleType.ToString(),
                    VehicleName = linkVehicleRequest.VehicleName.ToString(),
                    BatteryId = linkVehicleRequest.BatteryId,
                    StartDate = TimeHepler.SystemTimeNow,
                    UpdateDate = TimeHepler.SystemTimeNow
                };

                await _vehicleRepo.AddVehicle(vehicle);

                // Lấy Evdriver theo accountId
                var evDriver = await _evDriverRepo.GetDriverByAccountId(accountId);
                if (evDriver == null)
                {
                    return new ResultModel
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.NOT_FOUND,
                        Message = "Evdriver not found.",
                        Data = null
                    };
                }

                // Gán VIN cho Evdriver
                evDriver.Vin = vehicle.Vin;
                evDriver.UpdateDate = TimeHepler.SystemTimeNow;
                await _evDriverRepo.UpdateDriver(evDriver);

                return new ResultModel
                {
                    StatusCode = StatusCodes.Status200OK,
                    IsSuccess = true,
                    ResponseCode = ResponseCodeConstants.SUCCESS,
                    Message = ResponseMessageConstantsVehicle.LINK_VEHICLE_SUCCESS,
                    Data = vehicle
                };
            }
            catch (Exception ex)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    ResponseCode = ResponseCodeConstants.FAILED,
                    Message = ResponseMessageConstantsVehicle.LINK_VEHICLE_FAILED,
                    Data = ex.Message,
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }

        public Task<ResultModel> AddVehicleInPackage(AddVehicleInPackageRequest addVehicleInPackageRequest)
        {
            try
            {
                var vehicle = _vehicleRepo.GetVehicleById(addVehicleInPackageRequest.Vin).Result;
                if (vehicle == null)
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
                var package = _packageRepo.GetPackageById(addVehicleInPackageRequest.PackageId).Result;
                if (package == null)
                {
                    return Task.FromResult(new ResultModel
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.NOT_FOUND,
                        Message = ResponseMessageConstantsPackage.PACKAGE_NOT_FOUND,
                        Data = null
                    });
                }
                vehicle.PackageId = addVehicleInPackageRequest.PackageId;
                vehicle.UpdateDate = TimeHepler.SystemTimeNow;
                _vehicleRepo.UpdateVehicle(vehicle);
                return Task.FromResult(new ResultModel
                {
                    StatusCode = StatusCodes.Status200OK,
                    IsSuccess = true,
                    ResponseCode = ResponseCodeConstants.SUCCESS,
                    Message = ResponseMessageConstantsVehicle.ADD_VEHICLE_IN_PACKAGE_SUCCESS,
                    Data = vehicle
                });

            }
            catch (Exception ex)
            {
                return Task.FromResult(new ResultModel
                {
                    IsSuccess = false,
                    ResponseCode = ResponseCodeConstants.FAILED,
                    Message = ResponseMessageConstantsVehicle.ADD_VEHICLE_IN_PACKAGE_FAILED,
                    Data = ex.Message,
                    StatusCode = StatusCodes.Status500InternalServerError
                });
            }
        }

        public Task<ResultModel> DeleteVehicleInPackage(string vehicleId)
        {
            try
            {
                var vehicle = _vehicleRepo.GetVehicleById(vehicleId).Result;
                if (vehicle == null)
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
                vehicle.PackageId = null;
                vehicle.UpdateDate = TimeHepler.SystemTimeNow;
                _vehicleRepo.UpdateVehicle(vehicle);
                return Task.FromResult(new ResultModel
                {
                    StatusCode = StatusCodes.Status200OK,
                    IsSuccess = true,
                    ResponseCode = ResponseCodeConstants.SUCCESS,
                    Message = ResponseMessageConstantsVehicle.DELETE_VEHICLE_IN_PACKAGE_SUCCESS,
                    Data = null
                });

            }
            catch (Exception ex)
            {
                return Task.FromResult(new ResultModel
                {
                    IsSuccess = false,
                    ResponseCode = ResponseCodeConstants.FAILED,
                    Message = ResponseMessageConstantsVehicle.DELETE_VEHICLE_IN_PACKAGE_FAILED,
                    Data = ex.Message,
                    StatusCode = StatusCodes.Status500InternalServerError
                });
            }
        }
    }
}
