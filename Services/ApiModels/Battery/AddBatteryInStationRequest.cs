using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ApiModels.Battery
{
    public class AddBatteryInStationRequest
    {
        
        public string BatteryId { get; set; }
        [Required]
        public string StationId { get; set; }
    }
}
