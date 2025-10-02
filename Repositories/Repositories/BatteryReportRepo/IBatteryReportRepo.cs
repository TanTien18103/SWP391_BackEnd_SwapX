using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects.Models;

namespace Repositories.Repositories.BatteryReportRepo
{
    public interface IBatteryReportRepo
    {
        Task<BatteryReport> GetBatteryReportById(string batteryReportId);
        Task<List<BatteryReport>> GetAllBatteryReports();
        Task<BatteryReport> AddBatteryReport(BatteryReport batteryReport);
        Task<BatteryReport> UpdateBatteryReport(BatteryReport batteryReport);
    }
}
