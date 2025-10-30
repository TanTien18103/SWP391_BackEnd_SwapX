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
           return await _context.Batteries
                .Include(v=>v.Vehicles)
                .Include(rp=>rp.BatteryReports)
                .Include(c=>c.BatteryHistories)
                .Include(b=>b.Station).ToListAsync();

        }

        public async Task<List<Battery>> GetBatteriesByStationId(string stationId)
        {
            return await _context.Batteries
                .Include(v => v.Vehicles)
                .Include(c => c.BatteryHistories)
                .Include(rp => rp.BatteryReports)
                .Include(b => b.Station)
                .Include(b => b.Slots)
                .Where(b => b.StationId == stationId)
                .ToListAsync();
        }



        public async Task<Battery> GetBatteryById(string batteryId)
        {
            return await _context.Batteries
                .Include(b=>b.Station)
                .Include(c=>c.BatteryHistories)
                .Include(rp => rp.BatteryReports)
                .Include(v=>v.Vehicles).FirstOrDefaultAsync(b => b.BatteryId == batteryId);
        }

        public async Task<Battery> UpdateBattery(Battery battery)
        {
            _context.Batteries.Update(battery);
            await _context.SaveChangesAsync();
            return battery;
        }
        public async Task<List<Battery>> GetBatteriesByStationIdAndSpecification(string stationId, string specification, string batteryType)
        {
            return await _context.Batteries.Include(v => v.Vehicles)
                .Include(b => b.Station)
                .Include(rp => rp.BatteryReports)
                .Include(c => c.BatteryHistories)
                .Where(b => b.StationId == stationId && b.Specification == specification&& b.BatteryType== batteryType)
                .ToListAsync();
        }

    }
}
