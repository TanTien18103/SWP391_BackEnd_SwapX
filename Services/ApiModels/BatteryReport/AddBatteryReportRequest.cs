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
    }

}
