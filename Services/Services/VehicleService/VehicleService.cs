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
using Repositories.Repositories.BatteryHistoryRepo;
using Services.Services.EmailService;




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
        private readonly IBatteryHistoryRepo _batteryHistoryRepo;
        private readonly IEmailService _emailService;

        public VehicleService(
            IVehicleRepo vehicleRepo, 
            IConfiguration configuration, 
            IHttpContextAccessor httpContextAccessor, 
            IBatteryRepo batteryRepo, 
            IPackageRepo packageRepo, 
            IEvDriverRepo evDriverRepo, 
            IAccountRepo accountRepo, 
            IBatteryHistoryRepo batteryHistoryRepo,
            IEmailService emailService
            )
        {
            _vehicleRepo = vehicleRepo;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _accountHelper = new AccountHelper(configuration);
            _batteryRepo = batteryRepo;
            _packageRepo = packageRepo;
            _evDriverRepo = evDriverRepo;
            _accountRepo = accountRepo;
            _batteryHistoryRepo = batteryHistoryRepo;
            _emailService = emailService;
        }
        public async Task<ResultModel> AddVehicle(AddVehicleRequest addVehicleRequest)
        {
            try
            {
                var vehicle = new Vehicle
                {
                    Vin = _accountHelper.GenerateShortGuid(),
                    Status = VehicleStatusEnums.linked.ToString(),
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
                existingVehicle.Status = VehicleStatusEnums.Unlinked.ToString();
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
                        Message = ResponseMessageConstantsUser.CUSTOMER_NOT_FOUND,
                    };
                }
                var account = await _accountRepo.GetAccountById(accountId);
                // Tạo vehicle mới
                var vehicle = new Vehicle
                {
                    Vin = linkVehicleRequest.VIN,
                    Status = VehicleStatusEnums.linked.ToString(),
                    VehicleName = linkVehicleRequest.VehicleName.ToString(),
                    CustomerId = evDriver.CustomerId,
                    StartDate = TimeHepler.SystemTimeNow,
                    UpdateDate = TimeHepler.SystemTimeNow
                };
                var Vin = await _vehicleRepo.GetVehicleById(linkVehicleRequest.VIN);
                if (Vin != null)
                {
                    if (Vin.Status == VehicleStatusEnums.linked.ToString() && Vin.CustomerId == evDriver.CustomerId)
                    {
                        return new ResultModel
                        {
                            StatusCode = StatusCodes.Status400BadRequest,
                            IsSuccess = false,
                            ResponseCode = ResponseCodeConstants.BAD_REQUEST,
                            Message = ResponseMessageConstantsVehicle.VEHICLE_ALREADY_LINKED,
                            Data = null
                        };
                    }
                    var user_of_vehicle = await _accountRepo.GetAccountByCustomerId(Vin.CustomerId);
                    if (Vin.Status == VehicleStatusEnums.Unlinked.ToString()||
                        user_of_vehicle.Status == AccountStatusEnums.Inactive.ToString())
                    {
                        Vin.Status = VehicleStatusEnums.linked.ToString();
                        Vin.CustomerId = evDriver.CustomerId;
                        Vin.UpdateDate = TimeHepler.SystemTimeNow;
                        await _vehicleRepo.UpdateVehicle(Vin);

                        return new ResultModel
                        {
                            StatusCode = StatusCodes.Status200OK,
                            IsSuccess = true,
                            ResponseCode = ResponseCodeConstants.SUCCESS,
                            Message = ResponseMessageConstantsVehicle.LINK_VEHICLE_SUCCESS,
                            Data = Vin
                        };
                    }
                  
                    // Nếu xe tồn tại và vẫn đang active, nghĩa là người khác đang dùng
                    return new ResultModel
                    {
                        StatusCode = StatusCodes.Status400BadRequest,
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.BAD_REQUEST,
                        Message = ResponseMessageConstantsVehicle.VEHICLE_ALREADY_EXISTS,
                        Data = null
                    };
                }
                if (linkVehicleRequest.VehicleName == null)
                {
                    return new ResultModel
                    {
                        StatusCode = StatusCodes.Status400BadRequest,
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.BAD_REQUEST,
                        Message = ResponseMessageConstantsVehicle.VEHICLE_NAME_REQUIRED,
                        Data = null
                    };
                }
                var battery = new Battery();
                // Kiểm tra battery
                switch (vehicle.VehicleName)
                {
                    case var name when name == VehicleNameEnums.YADEA_I6_Lithium_Battery.ToString():
                        battery = new Battery
                        {
                            BatteryId = _accountHelper.GenerateShortGuid(),
                            Status = BatteryStatusEnums.InUse.ToString(),
                            Capacity = 100,
                            BatteryQuality = 80.00m,
                            BatteryType = BatteryTypeEnums.Lithium.ToString(),
                            BatteryName = account.Username + vehicle.VehicleName + ResponseMessageConstantsBattery.DefaultBatterySuffix,
                            Specification = BatterySpecificationEnums.V48_Ah13.ToString(),
                            Image = ResponseMessageConstantsBattery.BATTERY_IMAGE_LINK,
                            StartDate = TimeHepler.SystemTimeNow,
                            UpdateDate = TimeHepler.SystemTimeNow
                        };
                        await _batteryRepo.AddBattery(battery);
                        vehicle.BatteryId = battery.BatteryId;
                        vehicle.VehicleType = VehicleTypeEnums.electric_bike.ToString();
                        break;

                    case var name when name == VehicleNameEnums.YADEA_I6_Accumulator.ToString():
                        battery = new Battery
                        {
                            BatteryId = _accountHelper.GenerateShortGuid(),
                            Status = BatteryStatusEnums.InUse.ToString(),
                            Capacity = 100,
                            BatteryQuality = 80.00m,
                            BatteryType = BatteryTypeEnums.Accumulator.ToString(),
                            BatteryName = account.Username + vehicle.VehicleName + ResponseMessageConstantsBattery.DefaultBatterySuffix,
                            Specification = BatterySpecificationEnums.V48_Ah13.ToString(),
                            Image = ResponseMessageConstantsBattery.BATTERY_IMAGE_LINK,
                            StartDate = TimeHepler.SystemTimeNow,
                            UpdateDate = TimeHepler.SystemTimeNow
                        };
                        await _batteryRepo.AddBattery(battery);
                        vehicle.BatteryId = battery.BatteryId;
                        vehicle.VehicleType = VehicleTypeEnums.electric_bike.ToString();
                        break;

                    case var name when name == VehicleNameEnums.YADEA_I8_VINTAGE.ToString()
                                    || name == VehicleNameEnums.YADEA_I8.ToString():
                        battery = new Battery
                        {
                            BatteryId = _accountHelper.GenerateShortGuid(),
                            Status = BatteryStatusEnums.InUse.ToString(),
                            Capacity = 100,
                            BatteryQuality = 80.00m,
                            BatteryType = BatteryTypeEnums.Graphene_TTFAR_Accumulator.ToString(),
                            BatteryName = account.Username + vehicle.VehicleName + ResponseMessageConstantsBattery.DefaultBatterySuffix,
                            Specification = BatterySpecificationEnums.V48_Ah22.ToString(),
                            Image = ResponseMessageConstantsBattery.BATTERY_IMAGE_LINK,
                            StartDate = TimeHepler.SystemTimeNow,
                            UpdateDate = TimeHepler.SystemTimeNow
                        };
                        await _batteryRepo.AddBattery(battery);
                        vehicle.BatteryId = battery.BatteryId;
                        vehicle.VehicleType = VehicleTypeEnums.electric_bike.ToString();
                        break;

                    case var name when name == VehicleNameEnums.YADEA_IFUN.ToString()
                                    || name == VehicleNameEnums.YADEA_IGO.ToString():
                        battery = new Battery
                        {
                            BatteryId = _accountHelper.GenerateShortGuid(),
                            Status = BatteryStatusEnums.InUse.ToString(),
                            Capacity = 100,
                            BatteryQuality = 80.00m,
                            BatteryType = BatteryTypeEnums.Lithium.ToString(),
                            BatteryName = account.Username + vehicle.VehicleName + ResponseMessageConstantsBattery.DefaultBatterySuffix,
                            Specification = BatterySpecificationEnums.V48_Ah12.ToString(),
                            Image = ResponseMessageConstantsBattery.BATTERY_IMAGE_LINK,
                            StartDate = TimeHepler.SystemTimeNow,
                            UpdateDate = TimeHepler.SystemTimeNow
                        };
                        await _batteryRepo.AddBattery(battery);
                        vehicle.BatteryId = battery.BatteryId;
                        vehicle.VehicleType = VehicleTypeEnums.electric_bike.ToString();
                        break;

                    case var name when name == VehicleNameEnums.YADEA_VITO.ToString():
                        battery = new Battery
                        {
                            BatteryId = _accountHelper.GenerateShortGuid(),
                            Status = BatteryStatusEnums.InUse.ToString(),
                            Capacity = 100,
                            BatteryQuality = 80.00m,
                            BatteryType = BatteryTypeEnums.Lithium.ToString(),
                            BatteryName = account.Username + vehicle.VehicleName + ResponseMessageConstantsBattery.DefaultBatterySuffix,
                            Specification = BatterySpecificationEnums.V36_Ah10_4.ToString(),
                            Image = ResponseMessageConstantsBattery.BATTERY_IMAGE_LINK,
                            StartDate = TimeHepler.SystemTimeNow,
                            UpdateDate = TimeHepler.SystemTimeNow
                        };
                        await _batteryRepo.AddBattery(battery);
                        vehicle.BatteryId = battery.BatteryId;
                        vehicle.VehicleType = VehicleTypeEnums.electric_assist_bicycle.ToString();
                        break;

                    case var name when name == VehicleNameEnums.YADEA_FLIT.ToString():
                        battery = new Battery
                        {
                            BatteryId = _accountHelper.GenerateShortGuid(),
                            Status = BatteryStatusEnums.InUse.ToString(),
                            Capacity = 100,
                            BatteryQuality = 80.00m,
                            BatteryType = BatteryTypeEnums.Lithium.ToString(),
                            BatteryName = account.Username + vehicle.VehicleName + ResponseMessageConstantsBattery.DefaultBatterySuffix,
                            Specification = BatterySpecificationEnums.V36_Ah7_8.ToString(),
                            Image = ResponseMessageConstantsBattery.BATTERY_IMAGE_LINK,
                            StartDate = TimeHepler.SystemTimeNow,
                            UpdateDate = TimeHepler.SystemTimeNow
                        };
                        await _batteryRepo.AddBattery(battery);
                        vehicle.BatteryId = battery.BatteryId;
                        vehicle.VehicleType = VehicleTypeEnums.electric_assist_bicycle.ToString();
                        break;

                    case var name when name == VehicleNameEnums.YADEA_VELAX.ToString()
                                    || name == VehicleNameEnums.YADEA_VELAX_SOOBIN.ToString():
                        battery = new Battery
                        {
                            BatteryId = _accountHelper.GenerateShortGuid(),
                            Status = BatteryStatusEnums.InUse.ToString(),
                            Capacity = 100,
                            BatteryQuality = 80.00m,
                            BatteryType = BatteryTypeEnums.LFP.ToString(),
                            BatteryName = account.Username + vehicle.VehicleName + ResponseMessageConstantsBattery.DefaultBatterySuffix,
                            Specification = BatterySpecificationEnums.V72_Ah30.ToString(),
                            Image = ResponseMessageConstantsBattery.BATTERY_IMAGE_LINK,
                            StartDate = TimeHepler.SystemTimeNow,
                            UpdateDate = TimeHepler.SystemTimeNow
                        };
                        await _batteryRepo.AddBattery(battery);
                        vehicle.BatteryId = battery.BatteryId;
                        vehicle.VehicleType = VehicleTypeEnums.electric_motorbike.ToString();
                        break;

                    case var name when name == VehicleNameEnums.YADEA_VOLTGUARD_U.ToString():
                        battery = new Battery
                        {
                            BatteryId = _accountHelper.GenerateShortGuid(),
                            Status = BatteryStatusEnums.InUse.ToString(),
                            Capacity = 100,
                            BatteryQuality = 80.00m,
                            BatteryType = BatteryTypeEnums.LFP.ToString(),
                            BatteryName = account.Username + vehicle.VehicleName + ResponseMessageConstantsBattery.DefaultBatterySuffix,
                            Specification = BatterySpecificationEnums.V72_Ah50.ToString(),
                            Image = ResponseMessageConstantsBattery.BATTERY_IMAGE_LINK,
                            StartDate = TimeHepler.SystemTimeNow,
                            UpdateDate = TimeHepler.SystemTimeNow
                        };
                        await _batteryRepo.AddBattery(battery);
                        vehicle.BatteryId = battery.BatteryId;
                        vehicle.VehicleType = VehicleTypeEnums.electric_motorbike.ToString();
                        break;

                    case var name when name == VehicleNameEnums.YADEA_VOLTGUARD_P.ToString():
                        battery = new Battery
                        {
                            BatteryId = _accountHelper.GenerateShortGuid(),
                            Status = BatteryStatusEnums.InUse.ToString(),
                            Capacity = 100,
                            BatteryQuality = 80.00m,
                            BatteryType = BatteryTypeEnums.Graphene_TTFAR_Accumulator.ToString(),
                            BatteryName = account.Username + vehicle.VehicleName + ResponseMessageConstantsBattery.DefaultBatterySuffix,
                            Specification = BatterySpecificationEnums.V72_Ah38.ToString(),
                            Image = ResponseMessageConstantsBattery.BATTERY_IMAGE_LINK,
                            StartDate = TimeHepler.SystemTimeNow,
                            UpdateDate = TimeHepler.SystemTimeNow
                        };
                        await _batteryRepo.AddBattery(battery);
                        vehicle.BatteryId = battery.BatteryId;
                        vehicle.VehicleType = VehicleTypeEnums.electric_motorbike.ToString();
                        break;

                    case var name when name == VehicleNameEnums.YADEA_ORLA_P.ToString()
                                    || name == VehicleNameEnums.YADEA_OCEAN.ToString()
                                    || name == VehicleNameEnums.YADEA_ODORA_S.ToString()
                                    || name == VehicleNameEnums.YADEA_ODORA_S2.ToString()
                                    || name == VehicleNameEnums.YADEA_M6I.ToString()
                                    || name == VehicleNameEnums.YADEA_VIGOR.ToString()
                                    || name == VehicleNameEnums.YADEA_X_MEN_NEO.ToString():
                        battery = new Battery
                        {
                            BatteryId = _accountHelper.GenerateShortGuid(),
                            Status = BatteryStatusEnums.InUse.ToString(),
                            Capacity = 100,
                            BatteryQuality = 80.00m,
                            BatteryType = BatteryTypeEnums.Graphene_TTFAR_Accumulator.ToString(),
                            BatteryName = account.Username + vehicle.VehicleName + ResponseMessageConstantsBattery.DefaultBatterySuffix,
                            Specification = BatterySpecificationEnums.V60_Ah22.ToString(),
                            Image = ResponseMessageConstantsBattery.BATTERY_IMAGE_LINK,
                            StartDate = TimeHepler.SystemTimeNow,
                            UpdateDate = TimeHepler.SystemTimeNow
                        };
                        await _batteryRepo.AddBattery(battery);
                        vehicle.BatteryId = battery.BatteryId;
                        vehicle.VehicleType = VehicleTypeEnums.electric_motorbike.ToString();
                        break;

                    case var name when name == VehicleNameEnums.YADEA_ORIS.ToString()
                                    || name == VehicleNameEnums.YADEA_ORIS_SOOBIN.ToString()
                                    || name == VehicleNameEnums.YADEA_OSSY.ToString():
                        battery = new Battery
                        {
                            BatteryId = _accountHelper.GenerateShortGuid(),
                            Status = BatteryStatusEnums.InUse.ToString(),
                            Capacity = 100,
                            BatteryQuality = 80.00m,
                            BatteryType = BatteryTypeEnums.Graphene_TTFAR_Accumulator.ToString(),
                            BatteryName = account.Username + vehicle.VehicleName + ResponseMessageConstantsBattery.DefaultBatterySuffix,
                            Specification = BatterySpecificationEnums.V72_Ah22.ToString(),
                            Image = ResponseMessageConstantsBattery.BATTERY_IMAGE_LINK,
                            StartDate = TimeHepler.SystemTimeNow,
                            UpdateDate = TimeHepler.SystemTimeNow
                        };
                        await _batteryRepo.AddBattery(battery);
                        vehicle.BatteryId = battery.BatteryId;
                        vehicle.VehicleType = VehicleTypeEnums.electric_motorbike.ToString();
                        break;

                    case var name when name == VehicleNameEnums.YADEA_ICUTE.ToString()
                                    || name == VehicleNameEnums.YADEA_X_ZONE.ToString()
                                    || name == VehicleNameEnums.YADEA_VEKOO.ToString()
                                    || name == VehicleNameEnums.YADEA_VEKOO_SOOBIN.ToString()
                                    || name == VehicleNameEnums.YADEA_X_SKY.ToString()
                                    || name == VehicleNameEnums.YADEA_X_BULL.ToString():
                        battery = new Battery
                        {
                            BatteryId = _accountHelper.GenerateShortGuid(),
                            Status = BatteryStatusEnums.InUse.ToString(),
                            Capacity = 100,
                            BatteryQuality = 80.00m,
                            BatteryType = BatteryTypeEnums.Graphene_TTFAR_Accumulator.ToString(),
                            BatteryName = account.Username + vehicle.VehicleName + ResponseMessageConstantsBattery.DefaultBatterySuffix,
                            Specification = BatterySpecificationEnums.V48_Ah22.ToString(),
                            Image = ResponseMessageConstantsBattery.BATTERY_IMAGE_LINK,
                            StartDate = TimeHepler.SystemTimeNow,
                            UpdateDate = TimeHepler.SystemTimeNow
                        };
                        await _batteryRepo.AddBattery(battery);
                        vehicle.BatteryId = battery.BatteryId;
                        vehicle.VehicleType = VehicleTypeEnums.electric_motorbike.ToString();
                        break;

                }
                await _vehicleRepo.AddVehicle(vehicle);

                if (!string.IsNullOrEmpty(account.Email))
                {
                    var subject = EmailConstants.VEHICLE_LINK_SUCCESS_SUBJECT;
                    var body = string.Format(
                        EmailConstants.VEHICLE_LINK_SUCCESS_BODY,
                        account.Name,
                        vehicle.VehicleName,
                        vehicle.Vin
                    );

                    await _emailService.SendEmail(account.Email, subject, body);
                }
                else
                {
                    return new ResultModel
                    {
                        StatusCode = StatusCodes.Status200OK,
                        IsSuccess = true,
                        ResponseCode = ResponseCodeConstants.SUCCESS,
                        Message = ResponseMessageConstantsVehicle.LINK_VEHICLE_SUCCESS_BUT_NO_EMAIL,
                        Data = vehicle
                    };
                }

                //record lại lịch sử pin
                var batteryHistory = new BatteryHistory
                {
                    BatteryHistoryId = _accountHelper.GenerateShortGuid(),
                    BatteryId = battery.BatteryId,
                    Notes = HistoryActionConstants.BATTERY_CREATED_BY_USER.ToString(),
                    ActionType = BatteryHistoryActionTypeEnums.Created.ToString(),
                    EnergyLevel = battery.Capacity.ToString(),
                    Status = BatteryHistoryStatusEnums.Active.ToString(),
                    Vin = vehicle.Vin,
                    ActionDate = TimeHepler.SystemTimeNow,
                    StartDate = TimeHepler.SystemTimeNow,
                    UpdateDate = TimeHepler.SystemTimeNow

                };
                await _batteryHistoryRepo.AddBatteryHistory(batteryHistory);
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
                        Message = ResponseMessageConstantsUser.EVDRIVER_NOT_FOUND,
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
                        Message = ResponseMessageConstantsVehicle.VEHICLE_NOT_OWNED,
                        Data = null
                    };
                }
                vehicle.PackageId = addVehicleInPackageRequest.PackageId;
                vehicle.UpdateDate = TimeHepler.SystemTimeNow;
                vehicle.PackageExpiredate = TimeHepler.SystemTimeNow.AddDays((double)package.ExpiredDate);
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
                        Message = ResponseMessageConstantsUser.EVDRIVER_NOT_FOUND,
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
                        Message = ResponseMessageConstantsVehicle.VEHICLE_NOT_OWNED,
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
                        Message = ResponseMessageConstantsUser.EVDRIVER_NOT_FOUND,
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
                        Message = ResponseMessageConstantsVehicle.VEHICLE_NOT_OWNED,
                        Data = null
                    };
                }
                if(vehicle.PackageId != null && vehicle.PackageExpiredate>TimeHepler.SystemTimeNow)
                {
                    return new ResultModel
                    {
                        StatusCode = StatusCodes.Status400BadRequest,
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.BAD_REQUEST,
                        Message = ResponseMessageConstantsVehicle.VEHICLE_IN_PACKAGE_CANNOT_UNLINK,
                        Data = null
                    };
                }

                // Unlink logic here (for example, set CustomerId to null)
                vehicle.Status = VehicleStatusEnums.Unlinked.ToString();
                vehicle.UpdateDate = TimeHepler.SystemTimeNow;
                await _vehicleRepo.UpdateVehicle(vehicle);

                var account = await _accountRepo.GetAccountById(accountId);
                if (account != null)
                {
                    var subject = EmailConstants.VEHICLE_UNLINK_SUCCESS_SUBJECT;
                    var body = string.Format(
                        EmailConstants.VEHICLE_UNLINK_SUCCESS_BODY,
                        account.Name,
                        vehicle.Vin + " - " + vehicle.VehicleName
                    );

                    await _emailService.SendEmail(account.Email, subject, body);
                }


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
                        Message = ResponseMessageConstantsUser.EVDRIVER_NOT_FOUND,
                        Data = null
                    };
                }
                var vehicles = await _vehicleRepo.GetAllVehicleByCustomerId(evDriver.CustomerId);
                var activeVehicles = vehicles.Where(v => v.Status == VehicleStatusEnums.linked.ToString()).ToList();
                if (activeVehicles == null || activeVehicles.Count == 0)
                {
                    return new ResultModel
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.NOT_FOUND,
                        Message = ResponseMessageConstantsVehicle.NO_VEHICLE_FOR_USER,
                        Data = null
                    };
                }
                return new ResultModel
                {
                    StatusCode = StatusCodes.Status200OK,
                    IsSuccess = true,
                    ResponseCode = ResponseCodeConstants.SUCCESS,
                    Message = ResponseMessageConstantsVehicle.GET_ALL_VEHICLE_SUCCESS,
                    Data = activeVehicles
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

        public async Task<ResultModel> GetBatteryByVehicleId(string vehicleId)
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
                var battery = await _batteryRepo.GetBatteryById(vehicle.BatteryId);
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
                return new ResultModel
                {
                    StatusCode = StatusCodes.Status200OK,
                    IsSuccess = true,
                    ResponseCode = ResponseCodeConstants.SUCCESS,
                    Message = ResponseMessageConstantsBattery.GET_BATTERY_DETAIL_SUCCESS,
                    Data = battery
                };

            }
            catch (Exception ex)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    ResponseCode = ResponseCodeConstants.FAILED,
                    Message = ResponseMessageConstantsBattery.GET_BATTERY_FAIL,
                    Data = ex.Message,
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }

        public async Task<ResultModel> GetVehicleOfUserByVehicleId(string vehicleId)
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
                        Message = ResponseMessageConstantsUser.EVDRIVER_NOT_FOUND,
                        Data = null
                    };
                }
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
                // Kiểm tra xem vehicle có thuộc về user hay không
                if (vehicle.CustomerId != evDriver.CustomerId)
                {
                    return new ResultModel
                    {
                        StatusCode = StatusCodes.Status403Forbidden,
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.FORBIDDEN,
                        Message = ResponseMessageConstantsVehicle.VEHICLE_NOT_OWNED,
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
    }
}
