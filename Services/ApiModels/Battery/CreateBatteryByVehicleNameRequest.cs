using BusinessObjects.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ApiModels.Battery
{
    public class CreateBatteryByVehicleNameRequest
    {
        [Required]
        public string BatteryName { get; set; }

        [Required]
        [Range(0, 100, ErrorMessage = "Dung lượng pin cần nhập từ 0 đến 100")]
        public int? Capacity { get; set; }
        [Required]
        public VehicleNameEnums VehicleName { get; set; }
        [Required]
        [Range(0, 100, ErrorMessage = "Độ bền pin cần nhập từ 0 đến 100")]
        public decimal? BatteryQuality { get; set; }
        [Required]
        public string Image { get; set; }
    }
}
