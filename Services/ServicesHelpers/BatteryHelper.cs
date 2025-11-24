using BusinessObjects.Enums;
using BusinessObjects.Models;
using BusinessObjects.TimeCoreHelper;
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


      
        public static decimal CalculateBatteryQuality(Battery battery)
        {
            if (battery == null || !battery.BatteryQuality.HasValue || !battery.UpdateDate.HasValue)
                return 0;

            var now = TimeHepler.SystemTimeNow;
            var daysSinceUpdate = (now - battery.UpdateDate.Value).TotalDays;

            decimal dailyDegradation = 0.05M;

            decimal newQuality = battery.BatteryQuality.Value - (decimal)daysSinceUpdate * dailyDegradation;

            newQuality = Math.Clamp(newQuality, 0, 100);

            return newQuality;
        }
     
    }
}
