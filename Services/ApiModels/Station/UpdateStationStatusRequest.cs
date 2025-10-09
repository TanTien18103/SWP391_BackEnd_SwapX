using BusinessObjects.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ApiModels.Station
{
    public class UpdateStationStatusRequest
    {
        [Required]
        public string StationId { get; set; }

        [Required]
        public StationStatusEnum Status { get; set; }
    }
}
