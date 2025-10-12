using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ApiModels.Battery
{
    public class GetAllBatterySuitVehicle
    {
        public string Vin { get; set; }
        public string StationId { get; set; }
    }
}
