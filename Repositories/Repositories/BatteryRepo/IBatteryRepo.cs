using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects.Models;

namespace Repositories.Repositories.BatteryRepo
{
    public interface IBatteryRepo
    {
        Task<Battery> GetBatteryById(string batteryId);
        Task<Battery> AddBattery(Battery battery);
        Task<Battery> UpdateBattery(Battery battery);
        Task<List<Battery>> GetAllBatteries();
        Task<List<Battery>> GetBatteriesByStationId(string stationId);
        Task<List<Battery>> GetBatteriesByStationIdAndSpecification(string stationId, string specification, string batteryType);
    }
}
