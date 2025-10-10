using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects.Models;
using Services.ApiModels.Package;
using Services.ApiModels;
using BusinessObjects.Enums;

namespace Services.Services.PackageService
{
    public interface IPackageService
    {
        Task<ResultModel> GetAllPackages();
        Task<ResultModel> GetPackageById(string packageId);
        Task<ResultModel> AddPackage(AddPackageRequest createPackageRequest);
        Task<ResultModel> UpdatePackage(UpdatePackageRequest updatePackageRequest);
        Task<ResultModel> DeletePackage(string packageId);
        Task<ResultModel> GetPackageByBatteryType(BatterySpecificationEnums batterySpecificationEnums);
        Task<ResultModel> UpdatePackageStatus(UpdatePackageStatusRequest updatePackageStatusRequest);
        

    }
}
