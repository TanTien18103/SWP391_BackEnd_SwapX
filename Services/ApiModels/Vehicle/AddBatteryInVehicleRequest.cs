using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ApiModels.Vehicle
{
    public class AddBatteryInVehicleRequest
    {
        [Required]
        public string AccountId { get; set; }
        [Required]
        public string VehicleId { get; set; }
        [Required]
        public string BatteryId { get; set; }
    }
}
