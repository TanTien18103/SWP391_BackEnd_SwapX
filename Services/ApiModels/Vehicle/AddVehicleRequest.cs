using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects.Enums;
using Microsoft.AspNetCore.Antiforgery;

namespace Services.ApiModels.Vehicle
{
    public class AddVehicleRequest
    {
        [Required]
        public VehicleNameEnums VehicleName { get; set; }
        [Required]
        public VehicleTypeEnums VehicleType { get; set; }
        [Required]
        public string BatteryId { get; set; }
        [Required]
        public string PackageId { get; set; }
    }
}
