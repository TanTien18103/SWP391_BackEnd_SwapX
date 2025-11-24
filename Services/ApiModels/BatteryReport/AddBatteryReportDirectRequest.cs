using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ApiModels.BatteryReport
{
    public class AddBatteryReportDirectRequest
    {
        [Required]
        public string AccountId { get; set; }
        [Required]
        public string VIN { get; set; }
    }
}
