using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repositories.Battery
{
    public class BatteryRepo : IBatteryRepo
    {
        private readonly SwapXContext _context;
        public BatteryRepo(SwapXContext context)
        {
            _context = context;
        }
        public async Task<BusinessObjects.Models.Battery> AddBattery(BusinessObjects.Models.Battery battery)
        {
            _context.Batteries.Add(battery);
            await _context.SaveChangesAsync();
            return battery;
        }

        public async Task<List<BusinessObjects.Models.Battery>> GetAllBatteries()
        {
           return await _context.Batteries.ToListAsync();

        }

        public async Task<BusinessObjects.Models.Battery> GetBatteriesByStationId(string stationId)
        {
            return await _context.Batteries.FirstOrDefaultAsync(b => b.StationId == stationId);
        }

        public async Task<BusinessObjects.Models.Battery> GetBatteryById(string batteryId)
        {
            return await _context.Batteries.FirstOrDefaultAsync(b => b.BatteryId == batteryId);
        }

        public async Task<BusinessObjects.Models.Battery> UpdateBattery(BusinessObjects.Models.Battery battery)
        {
            _context.Batteries.Update(battery);
            await _context.SaveChangesAsync();
            return battery;
        }
    }
}
