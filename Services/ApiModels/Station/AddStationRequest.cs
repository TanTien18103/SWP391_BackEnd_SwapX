using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ApiModels.Station
{
    public class AddStationRequest
    {
        [Required]
        [Range(0, 50, ErrorMessage = "BatteryNumber must be between 0 and 50.")]
        public string Name { get; set; }

        [Range(0, 30, ErrorMessage = "BatteryNumber must be between 0 and 30.")]
        public int? BatteryNumber { get; set; }
        [Required]
        public string Location { get; set; }
    }
}
