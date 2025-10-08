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
        public async Task<ResultModel> DeleteVehicle(string vehicleId)
        {
            try
            {

                var existingVehicle = await _vehicleRepo.GetVehicleById(vehicleId);
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
                existingVehicle.Status = VehicleStatusEnums.Inactive.ToString();
                existingVehicle.UpdateDate = TimeHepler.SystemTimeNow;
                await _vehicleRepo.UpdateVehicle(existingVehicle);
                return new ResultModel
                {
                    StatusCode = StatusCodes.Status200OK,
                    IsSuccess = true,
                    ResponseCode = ResponseCodeConstants.SUCCESS,
                    Message = ResponseMessageConstantsVehicle.DELETE_VEHICLE_SUCCESS,
                    Data = null
                };

            }
            catch (Exception ex)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    ResponseCode = ResponseCodeConstants.FAILED,
                    Message = ResponseMessageConstantsVehicle.DELETE_VEHICLE_FAILED,
                    Data = ex.Message,
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }

        }

        public async Task<ResultModel> GetVehicleByName(VehicleNameEnums vehicleName)
        {
            try
            {
                var vehicles = await _vehicleRepo.GetVehiclesByName(vehicleName);
                if (vehicles == null || vehicles.Count == 0)
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
                    Data = vehicles
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

        public async Task<ResultModel> GetPackageByVehicleName(VehicleNameEnums vehicleName)
        {
            try
            {

                var vehicles = await _vehicleRepo.GetVehiclesByName(vehicleName);
                if (vehicles == null || vehicles.Count == 0)
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
                var packages = vehicles.Where(v => v.Package != null).Select(v => v.Package).Distinct().ToList();
                if (packages == null || packages.Count == 0)
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
                return new ResultModel
                {
                    StatusCode = StatusCodes.Status200OK,
                    IsSuccess = true,
                    ResponseCode = ResponseCodeConstants.SUCCESS,
                    Message = ResponseMessageConstantsPackage.GET_PACKAGE_SUCCESS,
                    Data = packages
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
                // Tạo vehicle mới
                var vehicle = new BusinessObjects.Models.Vehicle
                {
                    Vin = linkVehicleRequest.VIN,
                    Status = VehicleStatusEnums.Active.ToString(),
                    VehicleName = linkVehicleRequest.VehicleName.ToString(),
                    CustomerId = evDriver.CustomerId,
                    StartDate = TimeHepler.SystemTimeNow,
                    UpdateDate = TimeHepler.SystemTimeNow
                };
                if (await _vehicleRepo.GetVehicleById(linkVehicleRequest.VIN) != null)
                {
                    return new ResultModel
                    {
                        StatusCode = StatusCodes.Status400BadRequest,
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.BAD_REQUEST,
                        Message = ResponseMessageConstantsVehicle.VEHICLE_ALREADY_EXISTS,
                        Data = null
                    };
                }
                // Kiểm tra battery
                if (vehicle.VehicleName == VehicleNameEnums.YADEA_I6_Lithium_Battery.ToString())
                {
                    var battery = new BusinessObjects.Models.Battery
                    {
                        BatteryId = _accountHelper.GenerateShortGuid(),
                        Status = BatteryStatusEnums.Available.ToString(),
                        Capacity = 100,
                        BatteryQuality = 100.00m,
                        BatteryType = BatteryTypeEnums.Lithium.ToString(),
                        BatteryName = "Battery",
                        Specification = BatterySpecificationEnums.V48_Ah13.ToString(),
                        StartDate = TimeHepler.SystemTimeNow,
                        UpdateDate = TimeHepler.SystemTimeNow
                    };
                    await _batteryRepo.AddBattery(battery);
                    vehicle.BatteryId = battery.BatteryId;
                    vehicle.VehicleType = VehicleTypeEnums.electric_bike.ToString();
                }
                if (vehicle.VehicleName == VehicleNameEnums.YADEA_I6_Accumulator.ToString())
                {
                    var battery = new BusinessObjects.Models.Battery
                    {
                        BatteryId = _accountHelper.GenerateShortGuid(),
                        Status = BatteryStatusEnums.Available.ToString(),
                        Capacity = 100,
                        BatteryQuality = 100.00m,
                        BatteryType = BatteryTypeEnums.Accumulator.ToString(),
                        BatteryName = "Battery",
                        Specification = BatterySpecificationEnums.V48_Ah13.ToString(),
                        StartDate = TimeHepler.SystemTimeNow,
                        UpdateDate = TimeHepler.SystemTimeNow
                    };
                    await _batteryRepo.AddBattery(battery);
                    vehicle.BatteryId = battery.BatteryId;
                    vehicle.VehicleType = VehicleTypeEnums.electric_bike.ToString();
                }
                if (vehicle.VehicleName == VehicleNameEnums.YADEA_I8_VINTAGE.ToString() || vehicle.VehicleName == VehicleNameEnums.YADEA_I8.ToString())
                {
                    var battery = new Battery
                    {
                        BatteryId = _accountHelper.GenerateShortGuid(),
                        Status = BatteryStatusEnums.Available.ToString(),
                        Capacity = 100,
                        BatteryQuality = 100.00m,
                        BatteryType = BatteryTypeEnums.Graphene_TTFAR_Accumulator.ToString(),
                        BatteryName = "Battery",
                        Specification = BatterySpecificationEnums.V48_Ah22.ToString(),
                        StartDate = TimeHepler.SystemTimeNow,
                        UpdateDate = TimeHepler.SystemTimeNow
                    };
                    await _batteryRepo.AddBattery(battery);
                    vehicle.BatteryId = battery.BatteryId;
                    vehicle.VehicleType = VehicleTypeEnums.electric_bike.ToString();
                }
                if (vehicle.VehicleName == VehicleNameEnums.YADEA_IFUN.ToString() || vehicle.VehicleName == VehicleNameEnums.YADEA_IGO.ToString())
                {
                    var battery = new Battery
                    {
                        BatteryId = _accountHelper.GenerateShortGuid(),
                        Status = BatteryStatusEnums.Available.ToString(),
                        Capacity = 100,
                        BatteryQuality = 100.00m,
                        BatteryType = BatteryTypeEnums.Lithium.ToString(),
                        BatteryName = "Battery",
                        Specification = BatterySpecificationEnums.V48_Ah12.ToString(),
                        StartDate = TimeHepler.SystemTimeNow,
                        UpdateDate = TimeHepler.SystemTimeNow
                    };
                    await _batteryRepo.AddBattery(battery);
                    vehicle.BatteryId = battery.BatteryId;
                    vehicle.VehicleType = VehicleTypeEnums.electric_bike.ToString();
                }
                if (vehicle.VehicleName == VehicleNameEnums.YADEA_VITO.ToString())
                {
                    var battery = new Battery
                    {
                        BatteryId = _accountHelper.GenerateShortGuid(),
                        Status = BatteryStatusEnums.Available.ToString(),
                        Capacity = 100,
                        BatteryQuality = 100.00m,
                        BatteryType = BatteryTypeEnums.Lithium.ToString(),
                        BatteryName = "Battery",
                        Specification = BatterySpecificationEnums.V36_Ah10_4.ToString(),
                        StartDate = TimeHepler.SystemTimeNow,
                        UpdateDate = TimeHepler.SystemTimeNow
                    };
                    await _batteryRepo.AddBattery(battery);
                    vehicle.BatteryId = battery.BatteryId;
                    vehicle.VehicleType = VehicleTypeEnums.electric_assist_bicycle.ToString();
                }
                if (vehicle.VehicleName == VehicleNameEnums.YADEA_FLIT.ToString())
                {
                    var battery = new Battery
                    {
                        BatteryId = _accountHelper.GenerateShortGuid(),
                        Status = BatteryStatusEnums.Available.ToString(),
                        Capacity = 100,
                        BatteryQuality = 100.00m,
                        BatteryType = BatteryTypeEnums.Lithium.ToString(),
                        BatteryName = "Battery",
                        Specification = BatterySpecificationEnums.V36_Ah7_8.ToString(),
                        StartDate = TimeHepler.SystemTimeNow,
                        UpdateDate = TimeHepler.SystemTimeNow
                    };
                    await _batteryRepo.AddBattery(battery);
                    vehicle.BatteryId = battery.BatteryId;
                    vehicle.VehicleType = VehicleTypeEnums.electric_assist_bicycle.ToString();
                }

                if (vehicle.VehicleName == VehicleNameEnums.YADEA_VELAX.ToString() || vehicle.VehicleName == VehicleNameEnums.YADEA_VELAX_SOOBIN.ToString())
                {
                    var battery = new Battery
                    {
                        BatteryId = _accountHelper.GenerateShortGuid(),
                        Status = BatteryStatusEnums.Available.ToString(),
                        Capacity = 100,
                        BatteryQuality = 100.00m,
                        BatteryType = BatteryTypeEnums.LFP.ToString(),
                        BatteryName = "Battery",
                        Specification = BatterySpecificationEnums.V72_Ah30.ToString(),
                        StartDate = TimeHepler.SystemTimeNow,
                        UpdateDate = TimeHepler.SystemTimeNow
                    };
                    await _batteryRepo.AddBattery(battery);
                    vehicle.BatteryId = battery.BatteryId;
                    vehicle.VehicleType = VehicleTypeEnums.electric_motorbike.ToString();
                }

                if (vehicle.VehicleName == VehicleNameEnums.YADEA_VOLTGUARD_U.ToString())
                {
                    var battery = new Battery
                    {
                        BatteryId = _accountHelper.GenerateShortGuid(),
                        Status = BatteryStatusEnums.Available.ToString(),
                        Capacity = 100,
                        BatteryQuality = 100.00m,
                        BatteryType = BatteryTypeEnums.LFP.ToString(),
                        BatteryName = "Battery",
                        Specification = BatterySpecificationEnums.V72_Ah50.ToString(),
                        StartDate = TimeHepler.SystemTimeNow,
                        UpdateDate = TimeHepler.SystemTimeNow
                    };
                    await _batteryRepo.AddBattery(battery);
                    vehicle.BatteryId = battery.BatteryId;
                    vehicle.VehicleType = VehicleTypeEnums.electric_motorbike.ToString();
                }

                if (vehicle.VehicleName == VehicleNameEnums.YADEA_VOLTGUARD_P.ToString())
                {
                    var battery = new Battery
                    {
                        BatteryId = _accountHelper.GenerateShortGuid(),
                        Status = BatteryStatusEnums.Available.ToString(),
                        Capacity = 100,
                        BatteryQuality = 100.00m,
                        BatteryType = BatteryTypeEnums.Graphene_TTFAR_Accumulator.ToString(),
                        BatteryName = "Battery",
                        Specification = BatterySpecificationEnums.V72_Ah38.ToString(),
                        StartDate = TimeHepler.SystemTimeNow,
                        UpdateDate = TimeHepler.SystemTimeNow
                    };
                    await _batteryRepo.AddBattery(battery);
                    vehicle.BatteryId = battery.BatteryId;
                    vehicle.VehicleType = VehicleTypeEnums.electric_motorbike.ToString();
                }

                if (vehicle.VehicleName == VehicleNameEnums.YADEA_ORLA_P.ToString() || vehicle.VehicleName == VehicleNameEnums.YADEA_OCEAN.ToString() || vehicle.VehicleName == VehicleNameEnums.YADEA_ODORA_S.ToString() || vehicle.VehicleName == VehicleNameEnums.YADEA_ODORA_S2.ToString()
                    || vehicle.VehicleName == VehicleNameEnums.YADEA_M6I.ToString() || vehicle.VehicleName == VehicleNameEnums.YADEA_VIGOR.ToString() || vehicle.VehicleName == VehicleNameEnums.YADEA_X_MEN_NEO.ToString())
                {
                    var battery = new Battery
                    {
                        BatteryId = _accountHelper.GenerateShortGuid(),
                        Status = BatteryStatusEnums.Available.ToString(),
                        Capacity = 100,
                        BatteryQuality = 100.00m,
                        BatteryType = BatteryTypeEnums.Graphene_TTFAR_Accumulator.ToString(),
                        BatteryName = "Battery",
                        Specification = BatterySpecificationEnums.V60_Ah22.ToString(),
                        StartDate = TimeHepler.SystemTimeNow,
                        UpdateDate = TimeHepler.SystemTimeNow
                    };
                    await _batteryRepo.AddBattery(battery);
                    vehicle.BatteryId = battery.BatteryId;
                    vehicle.VehicleType = VehicleTypeEnums.electric_motorbike.ToString();
                }

                if (vehicle.VehicleName == VehicleNameEnums.YADEA_ORIS.ToString() || vehicle.VehicleName == VehicleNameEnums.YADEA_ORIS_SOOBIN.ToString() || vehicle.VehicleName == VehicleNameEnums.YADEA_OSSY.ToString())
                {
                    var battery = new Battery
                    {
                        BatteryId = _accountHelper.GenerateShortGuid(),
                        Status = BatteryStatusEnums.Available.ToString(),
                        Capacity = 100,
                        BatteryQuality = 100.00m,
                        BatteryType = BatteryTypeEnums.Graphene_TTFAR_Accumulator.ToString(),
                        BatteryName = "Battery",
                        Specification = BatterySpecificationEnums.V72_Ah22.ToString(),
                        StartDate = TimeHepler.SystemTimeNow,
                        UpdateDate = TimeHepler.SystemTimeNow
                    };
                    await _batteryRepo.AddBattery(battery);
                    vehicle.BatteryId = battery.BatteryId;
                    vehicle.VehicleType = VehicleTypeEnums.electric_motorbike.ToString();
                }

                if (vehicle.VehicleName == VehicleNameEnums.YADEA_ICUTE.ToString() || vehicle.VehicleName == VehicleNameEnums.YADEA_X_ZONE.ToString() || vehicle.VehicleName == VehicleNameEnums.YADEA_VEKOO.ToString()
                    || vehicle.VehicleName == VehicleNameEnums.YADEA_VEKOO_SOOBIN.ToString() || vehicle.VehicleName == VehicleNameEnums.YADEA_X_SKY.ToString() || vehicle.VehicleName == VehicleNameEnums.YADEA_X_BULL.ToString())
                {
                    var battery = new Battery
                    {
                        BatteryId = _accountHelper.GenerateShortGuid(),
                        Status = BatteryStatusEnums.Available.ToString(),
                        Capacity = 100,
                        BatteryQuality = 100.00m,
                        BatteryType = BatteryTypeEnums.Graphene_TTFAR_Accumulator.ToString(),
                        BatteryName = "Battery",
                        Specification = BatterySpecificationEnums.V48_Ah22.ToString(),
                        StartDate = TimeHepler.SystemTimeNow,
                        UpdateDate = TimeHepler.SystemTimeNow
                    };
                    await _batteryRepo.AddBattery(battery);
                    vehicle.BatteryId = battery.BatteryId;
                    vehicle.VehicleType = VehicleTypeEnums.electric_motorbike.ToString();
                }


                await _vehicleRepo.AddVehicle(vehicle);




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

        public async Task<ResultModel> AddVehicleInPackage(AddVehicleInPackageRequest addVehicleInPackageRequest)
        {
            try
            {
                var vehicle = await _vehicleRepo.GetVehicleById(addVehicleInPackageRequest.Vin);
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
                var package = await _packageRepo.GetPackageById(addVehicleInPackageRequest.PackageId);
                if (package == null)
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
                // Kiểm tra trước khi gán
                if (vehicle.PackageId != null && vehicle.PackageId != "")
                {
                    return new ResultModel
                    {
                        StatusCode = StatusCodes.Status400BadRequest,
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.FAILED,
                        Message = ResponseMessageConstantsVehicle.VEHICLE_ALREADY_IN_PACKAGE,
                        Data = null
                    };
                }
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

                // Get Evdriver by accountId
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

                // Check if the vehicle belongs to the user
                if (vehicle.CustomerId != evDriver.CustomerId)
                {
                    return new ResultModel
                    {
                        StatusCode = StatusCodes.Status403Forbidden,
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.FORBIDDEN,
                        Message = "You do not own this vehicle.",
                        Data = null
                    };
                }
                vehicle.PackageId = addVehicleInPackageRequest.PackageId;
                vehicle.UpdateDate = TimeHepler.SystemTimeNow;
                await _vehicleRepo.UpdateVehicle(vehicle);
                return new ResultModel
                {
                    StatusCode = StatusCodes.Status200OK,
                    IsSuccess = true,
                    ResponseCode = ResponseCodeConstants.SUCCESS,
                    Message = ResponseMessageConstantsVehicle.ADD_VEHICLE_IN_PACKAGE_SUCCESS,
                    Data = vehicle
                };
            }
            catch (Exception ex)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    ResponseCode = ResponseCodeConstants.FAILED,
                    Message = ResponseMessageConstantsVehicle.ADD_VEHICLE_IN_PACKAGE_FAILED,
                    Data = ex.Message,
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }

        }

        public async Task<ResultModel> DeleteVehicleInPackage(string vehicleId)
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

                // Get Evdriver by accountId
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

                // Check if the vehicle belongs to the user
                if (vehicle.CustomerId != evDriver.CustomerId)
                {
                    return new ResultModel
                    {
                        StatusCode = StatusCodes.Status403Forbidden,
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.FORBIDDEN,
                        Message = "You do not own this vehicle.",
                        Data = null
                    };
                }
                vehicle.PackageId = null;
                vehicle.UpdateDate = TimeHepler.SystemTimeNow;
                await _vehicleRepo.UpdateVehicle(vehicle);
                return new ResultModel
                {
                    StatusCode = StatusCodes.Status200OK,
                    IsSuccess = true,
                    ResponseCode = ResponseCodeConstants.SUCCESS,
                    Message = ResponseMessageConstantsVehicle.DELETE_VEHICLE_IN_PACKAGE_SUCCESS,
                    Data = vehicle
                };

            }
            catch (Exception ex)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    ResponseCode = ResponseCodeConstants.FAILED,
                    Message = ResponseMessageConstantsVehicle.DELETE_VEHICLE_IN_PACKAGE_FAILED,
                    Data = ex.Message,
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }

        }

        public async Task<ResultModel> UnlinkVehicle(string vehicleId)
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

                // Get Evdriver by accountId
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

                // Check if the vehicle belongs to the user
                if (vehicle.CustomerId != evDriver.CustomerId)
                {
                    return new ResultModel
                    {
                        StatusCode = StatusCodes.Status403Forbidden,
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.FORBIDDEN,
                        Message = "You do not own this vehicle.",
                        Data = null
                    };
                }

                // Unlink logic here (for example, set CustomerId to null)
                vehicle.CustomerId = null;
                vehicle.UpdateDate = TimeHepler.SystemTimeNow;
                await _vehicleRepo.UpdateVehicle(vehicle);

                return new ResultModel
                {
                    StatusCode = StatusCodes.Status200OK,
                    IsSuccess = true,
                    ResponseCode = ResponseCodeConstants.SUCCESS,
                    Message = ResponseMessageConstantsVehicle.UNLINK_VEHICLE_SUCCESS,
                    Data = null
                };
            }
            catch (Exception ex)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    ResponseCode = ResponseCodeConstants.FAILED,
                    Message = ResponseMessageConstantsVehicle.UNLINK_VEHICLE_FAILED,
                    Data = ex.Message,
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }

        public async Task<ResultModel> GetAllVehicleByCustomerId()
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
                        Message = "Evdriver not found.",
                        Data = null
                    };
                }
                var vehicles = await _vehicleRepo.GetAllVehicleByCustomerId(evDriver.CustomerId);
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

        public async Task<ResultModel> GetPackageByVehicleId(string vehicleId)
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
                var package = await _vehicleRepo.GetPackageByVehicleId(vehicleId);
                if (package == null)
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
                return new ResultModel
                {
                    StatusCode = StatusCodes.Status200OK,
                    IsSuccess = true,
                    ResponseCode = ResponseCodeConstants.SUCCESS,
                    Message = ResponseMessageConstantsVehicle.GET_PACKAGE_BY_VEHICLE_ID_SUCCESS,
                    Data = package
                };
            }
            catch (Exception ex)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    ResponseCode = ResponseCodeConstants.FAILED,
                    Message = ResponseMessageConstantsVehicle.GET_PACKAGE_BY_VEHICLE_ID_FAILED,
                    Data = ex.Message,
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }
    }
}
