using BusinessObjects.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ApiModels.Report
{
    public class UpdateReportStatusRequest
    {
        [Required]
        public string ReportId { get; set; }
        [Required]
        public ReportStatusEnums Status { get; set; }
    }
}
