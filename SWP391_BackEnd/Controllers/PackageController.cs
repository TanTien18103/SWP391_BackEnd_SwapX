using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.ApiModels.Package;
using Services.Services.PackageService;

namespace SWP391_BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PackageController : ControllerBase
    {
        private readonly IPackageService _packageService;
        public PackageController(IPackageService packageService)
        {
            _packageService = packageService;
        }
        [HttpPost("add_package")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddPackage([FromForm] Services.ApiModels.Package.AddPackageRequest addPackageRequest)
        {
            var res = await _packageService.AddPackage(addPackageRequest);
            return StatusCode(res.StatusCode, res);
        }
        [HttpGet("get_package_by_id")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetPackageById([FromQuery] string? packageId)
        {
            var res = await _packageService.GetPackageById(packageId);
            return StatusCode(res.StatusCode, res);
        }
        [HttpPut("delete_package")]
        [Authorize (Roles = "Admin")]
        public async Task<IActionResult> DeletePackage([FromForm] string? packageId)
        {
            var res = await _packageService.DeletePackage(packageId);
            return StatusCode(res.StatusCode, res);
        }
        [HttpPut("update_package")]
        [Authorize (Roles = "Admin")]
        public async Task<IActionResult> UpdatePackage([FromForm] Services.ApiModels.Package.UpdatePackageRequest updatePackageRequest)
        {
            var res = await _packageService.UpdatePackage(updatePackageRequest);
            return StatusCode(res.StatusCode, res);
        }
        [HttpGet("get_all_packages")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllPackages()
        {
            var res = await _packageService.GetAllPackages();
            return StatusCode(res.StatusCode, res);
        }
        [HttpGet("get_package_by_battery_type")]
        [Authorize]
        public async Task<IActionResult> GetPackageByBatteryType([FromQuery] BusinessObjects.Enums.BatterySpecificationEnums batterySpecificationEnums)
        {
            var res = await _packageService.GetPackageByBatteryType(batterySpecificationEnums);
            return StatusCode(res.StatusCode, res);
        }

        [HttpPut("update_package_status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdatePackageStatus([FromForm] UpdatePackageStatusRequest updatePackageStatusRequest)
        {
            var res = await _packageService.UpdatePackageStatus(updatePackageStatusRequest);
            return StatusCode(res.StatusCode, res);
        }
    }
}
