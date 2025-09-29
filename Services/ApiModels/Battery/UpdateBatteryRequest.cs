using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ApiModels.Battery
{
    public class UpdateBatteryRequest
    {
        public string BatteryId { get; set; }

        public int? Capacity { get; set; }

        public string BatteryType { get; set; }

        public string Specification { get; set; }

        public decimal? BatteryQuality { get; set; }
    }
}
