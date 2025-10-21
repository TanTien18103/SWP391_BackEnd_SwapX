using BusinessObjects.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ApiModels.ExchangeBattery
{
    public class UpdateExchangeStatusRequest
    {
        [Required]
        public string ExchangeBatteryId { get; set; }
        [Required]
        public string StaffId { get; set; }
        [Required]
        public ExchangeStatusEnums Status { get; set; }
    }
}
