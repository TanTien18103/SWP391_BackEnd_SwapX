using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects.Models;

namespace Repositories.Repositories.Battery
{
    public interface IBatteryRepo
    {
        Task<BusinessObjects.Models.Battery> GetBatteryById(string batteryId);
        Task<BusinessObjects.Models.Battery> AddBattery(BusinessObjects.Models.Battery battery);
        Task<BusinessObjects.Models.Battery> UpdateBattery(BusinessObjects.Models.Battery battery);
        Task<List<BusinessObjects.Models.Battery>> GetAllBatteries();
        Task<BusinessObjects.Models.Battery> GetBatteriesByStationId(string stationId);
    }
}
