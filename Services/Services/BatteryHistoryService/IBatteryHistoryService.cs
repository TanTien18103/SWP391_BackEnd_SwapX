using Services.ApiModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services.BatteryHistoryService
{
    public interface IBatteryHistoryService
    {
        Task<ResultModel> GetAllBatteryHistory();
        Task<ResultModel> GetBatteryHistoryByBatteryId(string batteryId);
    }
}
