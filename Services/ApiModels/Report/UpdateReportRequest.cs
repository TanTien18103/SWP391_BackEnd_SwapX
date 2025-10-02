using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ApiModels.Report
{
    public class UpdateReportRequest
    {
        public string? ReportId { get; set; }

        public string? Name { get; set; }

        public string? Description { get; set; }

        public string? Image { get; set; }

        public string? AccountId { get; set; }

        public string? StationId { get; set; }
    }
}
