using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects.Enums;

namespace Services.ApiModels.Battery
{
    public class UpdateBatteryRequest
    {
        public string? BatteryId { get; set; }
        public string? BatteryName { get; set; }

        [Range(0, 100, ErrorMessage = "Dung lượng pin cần nhập từ 0 đến 100")]
        public int? Capacity { get; set; }

        [Range(0, 100, ErrorMessage = "Độ bền pin cần nhập từ 0 đến 100")]
        public decimal? BatteryQuality { get; set; }

        public string? Image { get; set; }
    }
}
