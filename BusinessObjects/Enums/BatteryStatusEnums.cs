using Microsoft.AspNetCore.Http.Connections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Enums
{
    public enum BatteryStatusEnums
    {
        Available,
        InUse,
        Charging,
        Booked,
        Maintenance,
        Decommissioned
    }
}
