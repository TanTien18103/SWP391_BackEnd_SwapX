using BusinessObjects.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ApiModels.StationSchedule
{
    public class UpdateStatusStationScheduleRequest
    {
        [Required]
        public string StationScheduleId { get; set; }
        [Required]
        public StationScheduleStatusEnums Status { get; set; }
    }
}
