using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects.Enums;

namespace Services.ApiModels.Battery
{
    public class AddBatteryRequest
    {
        [Required]
        [Range(0, 100, ErrorMessage = "Dung lượng pin cần nhập từ 0 đến 100")]
        public int? Capacity { get; set; }
        [Required]
        public BatteryTypeEnums BatteryType { get; set; }
        [Required]
        public BatterySpecificationEnums Specification { get; set; }
        [Required]
        [Range(0, 100, ErrorMessage = "Độ bền pin cần nhập từ 0 đến 100")]
        public decimal? BatteryQuality { get; set; }



    }
}
