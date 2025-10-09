using BusinessObjects.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ApiModels.Battery
{
    public class UpdateBatteryStatusRequest
    {
        [Required]
        public string BatteryId { get; set; }

        [Required]
        public BatteryStatusEnums Status { get; set; }
    }
}
