using BusinessObjects.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ApiModels.Form
{
    public class AddFormRequest
    {
        [Required]
        public string AccountId { get; set; }
        
        [RegularExpression(@"^.{3,100}$", ErrorMessage = "Lời nhắn phải từ 3 đến 100 ký tự")]
        public string? Title { get; set; }

        [RegularExpression(@"^.{10,500}$", ErrorMessage = "Chi tiết lời nhắn phải từ 10 đến 500 ký tự")]
        public string? Description { get; set; }
        
        [Required]
        public DateTime? Date { get; set; }

        [Required]
        public string StationId { get; set; }
        [Required]
        public string Vin { get; set; }
        [Required]
        public string BatteryId { get; set; }

    }
}
