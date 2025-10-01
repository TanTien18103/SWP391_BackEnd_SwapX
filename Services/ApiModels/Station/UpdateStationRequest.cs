using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ApiModels.Station
{
    public class UpdateStationRequest
    {
        [Range(0, 50, ErrorMessage = "BatteryNumber must be between 0 and 50.")]
        public string Name { get; set; }

        public string? StationId { get; set; }
        [Range(0, 30, ErrorMessage = "BatteryNumber must be between 0 and 30.")]

        public int? BatteryNumber { get; set; }

        public string? Location { get; set; }
    }
}
