using BusinessObjects.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ApiModels.Vehicle
{
    public class UpdateVehicleRequest
    {
        public string? Vin { get; set; }

        public VehicleNameEnums? VehicleName { get; set; }

        public VehicleTypeEnums? VehicleType { get; set; }

        public string? BatteryId { get; set; }

        public string? PackageId { get; set; }

    }
}
