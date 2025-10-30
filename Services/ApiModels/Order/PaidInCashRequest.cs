using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ApiModels.Order
{
    public class PaidInCashRequest
    {
        [Required]
        public string ExchangeBatteryId { get; set; }
        [Required]
        public string FormId { get; set; }
        [Required]
        public decimal Total { get; set; }
    }
}
