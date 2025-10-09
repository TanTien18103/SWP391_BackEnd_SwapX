using BusinessObjects.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ApiModels.Package
{
    public class UpdatePackageStatusRequest
    {
        [Required]
        public string PackageId { get; set; }
        [Required]
        public PackageStatusEnums Status { get; set; }
    }
}
