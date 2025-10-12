using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ApiModels.Rating
{
    public class AddRatingRequest
    {
        [Required]
        [Range(0, 5, ErrorMessage = "Đánh giá chỉ từ 0.0 đến 5.0!!!")]
        public decimal? Rating1 { get; set; }
        public string? Description { get; set; }
        [Required]
        public string StationId { get; set; }
        [Required]
        public string AccountId { get; set; }
    }
}
