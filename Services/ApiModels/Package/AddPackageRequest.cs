using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects.Enums;    

namespace Services.ApiModels.Package
{
    public class AddPackageRequest
    {
        [Required]
        public decimal Price { get; set; }
        [Required]
        public string PackageName { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public BatterySpecificationEnums BatteryType { get; set; }

    }
}
