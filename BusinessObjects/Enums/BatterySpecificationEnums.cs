using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Enums
{
    public enum BatterySpecificationEnums
    {
        [Description("48V-12Ah")]
        V48_Ah12,
        [Description("60V-22Ah")]
        V60_Ah22,
        [Description("72V-38Ah")]
        V72_Ah38,
        [Description("72V-50Ah")]
        V72_Ah50,
        [Description("48V-22Ah")]
        V48_Ah22,
        [Description("72V-30Ah")]
        V72_Ah30,
        [Description("72V-22Ah")]
        V72_Ah22,
        [Description("60V-20Ah")]
        V60_Ah20
    }
}
