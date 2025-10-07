using BusinessObjects.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ApiModels.Vehicle
{
    public class LinkVehicleRequest
    {
        [Required]
        public string VIN { get; set; }
        [Required]
        public VehicleNameEnums VehicleName { get; set; }


    }
}
