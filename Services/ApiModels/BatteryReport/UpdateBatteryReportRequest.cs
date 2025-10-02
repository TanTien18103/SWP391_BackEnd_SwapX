using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ApiModels.BatteryReport
{
    public class UpdateBatteryReportRequest
    {
        public string? BatteryReportId { get; set; }

        public string? Name { get; set; }

        public string? Description { get; set; }

        public string? Image { get; set; }
        
        public string? BatteryId { get; set; }
    }
}
