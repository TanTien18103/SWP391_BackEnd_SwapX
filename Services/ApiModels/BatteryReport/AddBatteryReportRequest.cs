using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ApiModels.BatteryReport
{
    public class AddBatteryReportRequest
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string Image { get; set; }
        [Required]
        public string AccountId { get; set; }
        [Required]
        public string StationId { get; set; }
        [Required]
        public string BatteryId { get; set; }
        [Required]
        public string ReportType { get; set; }
        [Required]
        [Range(0, 100, ErrorMessage = "Dung lượng pin cần nhập từ 0 đến 100")]
        public int Capacity { get; set; }
        [Required]
        [Range(0, 100, ErrorMessage = "Độ bền pin cần nhập từ 0 đến 100")]
        public decimal BatteryQuality { get; set; }
        [Required]
        public string ExchangeBatteryId { get; set; }
    }

}
