using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repositories.BatteryRepo
{
    public class BatteryRepo : IBatteryRepo
    {
        private readonly SwapXContext _context;
        public BatteryRepo(SwapXContext context)
        {
            _context = context;
        }
        public async Task<Battery> AddBattery(Battery battery)
        {
            _context.Batteries.Add(battery);
            await _context.SaveChangesAsync();
            return battery;
        }

        public async Task<List<Battery>> GetAllBatteries()
        {
           return await _context.Batteries.ToListAsync();

        }

        public async Task<Battery> GetBatteriesByStationId(string stationId)
        {
            return await _context.Batteries.FirstOrDefaultAsync(b => b.StationId == stationId);
        }

        public async Task<Battery> GetBatteryById(string batteryId)
        {
            return await _context.Batteries.FirstOrDefaultAsync(b => b.BatteryId == batteryId);
        }

        public async Task<Battery> UpdateBattery(Battery battery)
        {
            _context.Batteries.Update(battery);
            await _context.SaveChangesAsync();
            return battery;
        }
    }
}
