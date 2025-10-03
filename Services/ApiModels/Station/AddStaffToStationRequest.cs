using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ApiModels.Station
{
    public class AddStaffToStationRequest
    {
        [Required]
        public string StaffId { get; set; }

        [Required]
        public string StationId { get; set; }
    }
}
