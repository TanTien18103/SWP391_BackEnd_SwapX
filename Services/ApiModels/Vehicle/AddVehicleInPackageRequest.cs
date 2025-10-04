using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ApiModels.Vehicle
{
    public class AddVehicleInPackageRequest
    {
        [Required]
        public string Vin { get; set; }
        [Required]
        public string PackageId { get; set; }
    }
}
