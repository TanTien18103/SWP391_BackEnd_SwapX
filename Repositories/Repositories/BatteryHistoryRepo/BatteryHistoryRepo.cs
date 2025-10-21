using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repositories.BatteryHistoryRepo
{
    public class BatteryHistoryRepo: IBatteryHistoryRepo
    {
        private readonly SwapXContext _context;
        public BatteryHistoryRepo(SwapXContext context)
        {
            _context = context;
        }

        public async Task<BatteryHistory> AddBatteryHistory(BatteryHistory batteryHistory)
        {
             _context.BatteryHistories.Add(batteryHistory);
            await _context.SaveChangesAsync();
            return batteryHistory;
        }

        public async Task<List<BatteryHistory>> GetAllBatteryHistories()
        {
            return await _context.BatteryHistories.ToListAsync();
        }
        public async Task<List<BatteryHistory>> GetBatteryHistoryByBatteryId(string batteryId)
        {
           return await _context.BatteryHistories.Where(bh => bh.BatteryId == batteryId).ToListAsync();
        }
        public async Task<List<BatteryHistory>> GetAllBatteryHistoryByStationId(string stationId)
        {
            return await _context.BatteryHistories.Where(bh => bh.StationId == stationId).ToListAsync();
        }

    }
}
