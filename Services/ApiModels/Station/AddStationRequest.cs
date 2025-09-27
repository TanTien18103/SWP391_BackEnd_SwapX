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
        public int? BatteryNumber { get; set; }
        [Required]
        public string Location { get; set; }
        [Required]
        public string Rating { get; set; }
    }
}
