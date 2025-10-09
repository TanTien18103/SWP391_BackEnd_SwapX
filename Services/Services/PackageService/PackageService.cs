using BusinessObjects.Constants;
using BusinessObjects.Enums;
using BusinessObjects.TimeCoreHelper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Repositories.Repositories.BatteryRepo;
using Repositories.Repositories.PackageRepo;
using Repositories.Repositories.VehicleRepo;
using Services.ApiModels;
using Services.ApiModels.Package;
using Services.ServicesHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services.PackageService
{
    public class PackageService : IPackageService
    {
        private readonly IPackageRepo _packageRepo;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IBatteryRepo _batteryRepo;
        private readonly IVehicleRepo _vehicleRepo;
        private readonly AccountHelper _accountHelper;

        public PackageService(IPackageRepo packageRepo, IConfiguration configuration, IHttpContextAccessor httpContextAccessor, AccountHelper accountHelper, IBatteryRepo batteryRepo, IVehicleRepo vehicleRepo)
        {
            _packageRepo = packageRepo;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _accountHelper = accountHelper;
            _batteryRepo = batteryRepo;
            _vehicleRepo = vehicleRepo;
        }

        public async Task<ResultModel> AddPackage(AddPackageRequest createPackageRequest)
        {
            try
            {
                var package = new BusinessObjects.Models.Package
                {
                    PackageId = _accountHelper.GenerateShortGuid(),
                    Description = createPackageRequest.Description,
                    Price = createPackageRequest.Price,
                    Status = PackageStatusEnums.Active.ToString(),
                    PackageName = createPackageRequest.PackageName,
                    BatteryType = createPackageRequest.BatteryType.ToString(),
                    StartDate = TimeHepler.SystemTimeNow,
                    UpdateDate = TimeHepler.SystemTimeNow
                };

                await _packageRepo.AddPackage(package);
                return new ResultModel
                {
                    StatusCode = StatusCodes.Status200OK,
                    IsSuccess = true,
                    ResponseCode = ResponseCodeConstants.SUCCESS,
                    Message = ResponseMessageConstantsPackage.ADD_PACKAGE_SUCCESS,
                    Data = package
                };

            }
            catch (Exception ex)
            {
                return new ResultModel
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    IsSuccess = false,
                    ResponseCode = ResponseCodeConstants.FAILED,
                    Message = ResponseMessageConstantsPackage.ADD_PACKAGE_FAIL,
                    Data = ex.Message
                };
            }
        }

        public async Task<ResultModel> DeletePackage(string packageId)
        {
            try
            {
                var package = await _packageRepo.GetPackageById(packageId);
                if (package == null)
                {
                    return new ResultModel
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.FAILED,
                        Message = ResponseMessageConstantsPackage.PACKAGE_NOT_FOUND,

                    };
                }
                package.Status = BatteryStatusEnums.Decommissioned.ToString();
                package.UpdateDate = TimeHepler.SystemTimeNow;
                var deletedPackage = await _packageRepo.UpdatePackage(package);
                return new ResultModel
                {
                    StatusCode = StatusCodes.Status200OK,
                    IsSuccess = true,
                    ResponseCode = ResponseCodeConstants.SUCCESS,
                    Message = ResponseMessageConstantsPackage.DELETE_PACKAGE_SUCCESS,

                };

            }
            catch (Exception ex)
            {
                return new ResultModel
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    IsSuccess = false,
                    ResponseCode = ResponseCodeConstants.FAILED,
                    Message = ResponseMessageConstantsPackage.DELETE_PACKAGE_FAILED,
                };
            }
        }

        public async Task<ResultModel> GetAllPackages()
        {
            try
            {

                var packages = await _packageRepo.GetAllPackages();
                return new ResultModel
                {
                    StatusCode = StatusCodes.Status200OK,
                    IsSuccess = true,
                    ResponseCode = ResponseCodeConstants.SUCCESS,
                    Message = ResponseMessageConstantsPackage.GET_ALL_PACKAGE_SUCCESS,
                    Data = packages
                };

            }
            catch (Exception ex)
            {
                return new ResultModel
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    IsSuccess = false,
                    ResponseCode = ResponseCodeConstants.FAILED,
                    Message = ResponseMessageConstantsPackage.GET_ALL_PACKAGE_FAIL,
                    Data = ex.Message
                };
            }
        }



        public async Task<ResultModel> GetPackageById(string packageId)
        {
            try
            {
                var package = await _packageRepo.GetPackageById(packageId);
                if (package == null)
                {
                    return new ResultModel
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.FAILED,
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
                    Data = package
                };


            }
            catch (Exception ex)
            {
                return new ResultModel
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    IsSuccess = false,
                    ResponseCode = ResponseCodeConstants.FAILED,
                    Message = ResponseMessageConstantsPackage.GET_PACKAGE_FAIL,
                    Data = ex.Message
                };
            }
        }



        public async Task<ResultModel> UpdatePackage(UpdatePackageRequest updatePackageRequest)
        {
            try
            {
                var package = await _packageRepo.GetPackageById(updatePackageRequest.PackageId);
                if (package == null)
                {
                    return new ResultModel
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.FAILED,
                        Message = ResponseMessageConstantsPackage.PACKAGE_NOT_FOUND,
                        Data = null
                    };
                }
                if (updatePackageRequest.Description != null)
                {
                    package.Description = updatePackageRequest.Description;
                }
                if (updatePackageRequest.Price != null)
                {
                    package.Price = updatePackageRequest.Price;
                }
                if (updatePackageRequest.PackageName != null)
                {
                    package.PackageName = updatePackageRequest.PackageName;
                }
                if (updatePackageRequest.BatteryType != null)
                {
                    package.BatteryType = updatePackageRequest.BatteryType.ToString();
                }

                package.UpdateDate = TimeHepler.SystemTimeNow;
                var updatedPackage = await _packageRepo.UpdatePackage(package);
                return new ResultModel
                {
                    StatusCode = StatusCodes.Status200OK,
                    IsSuccess = true,
                    ResponseCode = ResponseCodeConstants.SUCCESS,
                    Message = ResponseMessageConstantsPackage.UPDATE_PACKAGE_SUCCESS,
                    Data = updatedPackage
                };
            }
            catch (Exception ex)
            {
                return new ResultModel
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    IsSuccess = false,
                    ResponseCode = ResponseCodeConstants.FAILED,
                    Message = ResponseMessageConstantsPackage.UPDATE_PACKAGE_FAILED,
                    Data = ex.Message
                };
            }

        }
        public async Task<ResultModel> GetPackageByBatteryType(BatterySpecificationEnums batterySpecificationEnums)
        {
            try
            {
                var packages = await _packageRepo.GetAllPackageByBatteryType(batterySpecificationEnums.ToString());
                if (packages == null || !packages.Any())
                {
                    return new ResultModel
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.FAILED,
                        Message = ResponseMessageConstantsPackage.PACKAGE_NOT_FOUND,
                        Data = null
                    };
                }
                return new ResultModel
                {
                    StatusCode = StatusCodes.Status200OK,
                    IsSuccess = true,
                    ResponseCode = ResponseCodeConstants.SUCCESS,
                    Message = ResponseMessageConstantsPackage.GET_PACKAGE_BY_BATTERY_TYPE_SUCCESS,
                    Data = packages
                };

            }
            catch (Exception ex)
            {
                return new ResultModel
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    IsSuccess = false,
                    ResponseCode = ResponseCodeConstants.FAILED,
                    Message = ResponseMessageConstantsPackage.GET_PACKAGE_BY_BATTERY_TYPE_FAIL,
                    Data = ex.Message
                };
            }
        }

        public async Task<ResultModel> UpdatePackageStatus(UpdatePackageStatusRequest updatePackageStatusRequest)
        {
            try
            {
                var package = await _packageRepo.GetPackageById(updatePackageStatusRequest.PackageId);
                if (package == null)
                {
                    return new ResultModel
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.FAILED,
                        Message = ResponseMessageConstantsPackage.PACKAGE_NOT_FOUND,
                        Data = null
                    };
                }
                if (package.Status == updatePackageStatusRequest.Status.ToString())
                {
                    return new ResultModel
                    {
                        StatusCode = StatusCodes.Status400BadRequest,
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.FAILED,
                        Message = ResponseMessageConstantsPackage.PACKAGE_STATUS_SAME,
                        Data = null
                    };
                }

                if (updatePackageStatusRequest.Status == PackageStatusEnums.Inactive)
                {
                    var vehicles = await _vehicleRepo.GetVehiclesByPackageId(package.PackageId);
                    if (vehicles != null)
                    {
                        return new ResultModel
                        {
                            StatusCode = StatusCodes.Status400BadRequest,
                            IsSuccess = false,
                            ResponseCode = ResponseCodeConstants.FAILED,
                            Message = ResponseMessageConstantsPackage.PACKAGE_IN_USE_CANNOT_INACTIVE,
                            Data = null
                        };
                    }
                }

                package.Status = updatePackageStatusRequest.Status.ToString();
                package.UpdateDate = TimeHepler.SystemTimeNow;
                var updatedPackage = await _packageRepo.UpdatePackage(package);
                
                return new ResultModel
                {
                    StatusCode = StatusCodes.Status200OK,
                    IsSuccess = true,
                    ResponseCode = ResponseCodeConstants.SUCCESS,
                    Message = ResponseMessageConstantsPackage.UPDATE_PACKAGE_STATUS_SUCCESS,
                    Data = updatedPackage
                };
            }
            catch (Exception ex)
            {
                return new ResultModel
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    IsSuccess = false,
                    ResponseCode = ResponseCodeConstants.FAILED,
                    Message = ResponseMessageConstantsPackage.UPDATE_PACKAGE_STATUS_FAILED,
                    Data = ex.Message
                };
            }
        }
    }
}

