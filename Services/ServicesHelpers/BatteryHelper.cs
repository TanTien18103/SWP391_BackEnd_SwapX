using BusinessObjects.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ServicesHelpers
{
    public class BatteryHelper
    {
        public string GenBatteryName(string type, string spec, string suffix)
        {
            string shortType = type switch
            {
                nameof(BatteryTypeEnums.Graphene_TTFAR_Accumulator) => "GTFAR",
                nameof(BatteryTypeEnums.Lithium) => "Li",
                nameof(BatteryTypeEnums.LFP) => "LFP",
                nameof(BatteryTypeEnums.Accumulator) => "ACC",
                _ => type
            };

            string shortSpec = spec.Replace("V", "").Replace("_Ah", "V");

            return $"{shortType}_{shortSpec}_{suffix}";
        }

    }
}
