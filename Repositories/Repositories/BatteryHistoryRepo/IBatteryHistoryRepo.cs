using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repositories.BatteryHistoryRepo
{
    public interface IBatteryHistoryRepo
    {
        Task<BatteryHistory> AddBatteryHistory(BatteryHistory batteryHistory);
        Task<List<BatteryHistory>> GetBatteryHistoryByBatteryId(string batteryId);
        Task<List<BatteryHistory>> GetAllBatteryHistories();
        Task<List<BatteryHistory>>GetAllBatteryHistoryByStationId(string stationId);
    }
}
