using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects.Models;
using BusinessObjects.Enums;

namespace Repositories.Repositories.PackageRepo
{
    public interface IPackageRepo
    {
        Task<Package> GetPackageById(string packageId);
        Task<List<Package>> GetAllPackages();
        Task<Package> AddPackage(BusinessObjects.Models.Package package);
        Task<Package> UpdatePackage(BusinessObjects.Models.Package package);
        Task<List<Package>> GetAllPackageByBatteryType(string batteryType);
    }
}
