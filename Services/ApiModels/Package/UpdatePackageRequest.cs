using BusinessObjects.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ApiModels.Package
{
    public class UpdatePackageRequest
    {
        public string? PackageId { get; set; }
        public string? PackageName { get; set; }

        public decimal? Price { get; set; }

        public string? Description { get; set; }
        public BatterySpecificationEnums? BatteryType { get; set; }
    }
}
