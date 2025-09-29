using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Services.ApiModels.Battery;
using Services.ApiModels;

namespace Services.Services.Battery
{
    public interface IBatteryService
    {
        Task<ResultModel> GetBatteryById(string batteryId);
        Task<ResultModel> AddBattery(AddBatteryRequest addBatteryRequest);
        Task<ResultModel> UpdateBattery(UpdateBatteryRequest updateBatteryRequest);
        Task<ResultModel> GetAllBatteries();
        Task<ResultModel> GetBatteriesByStationId(string stationId);
    }
}
