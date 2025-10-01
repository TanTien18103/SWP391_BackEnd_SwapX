using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ApiModels.StationSchedule
{
    public class UpdateStationScheduleRequest
    {
        public string? StationScheduleId { get; set; }

        public DateTime? Date { get; set; }

        public string? Description { get; set; }

        public string? StationId { get; set; }

        public string? FormId { get; set; }

    }
}
