using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ApiModels.Station
{
    public class UpdateStationRequest
    {
        public string? Name { get; set; }

        public string? StationId { get; set; }

        public string? Location { get; set; }
    }
}
