using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.TimeCoreHelper
{
    public static class TimeHepler
    {
        public static DateTime SystemTimeNow
        {
            get
            {
                TimeZoneInfo vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");

                DateTime vietnamTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone);

                return vietnamTime;
            }
        }
    }
}
