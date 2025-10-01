using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ApiModels.StationSchedule
{
    public class AddStationScheduleRequest
    {
        [Required]
        public DateTime Date { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string StationId { get; set; }
        [Required]
        public string FormId { get; set; }

    }
}
