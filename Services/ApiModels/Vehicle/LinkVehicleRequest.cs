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
        [StringLength(17, MinimumLength = 17, ErrorMessage = "VIN must be exactly 17 characters.")]
        public string VIN { get; set; }
        
        public VehicleNameEnums? VehicleName { get; set; }


    }
}
