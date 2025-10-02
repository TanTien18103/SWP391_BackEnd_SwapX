using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ApiModels.Report
{
    public class AddReportRequest
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

    }
}
