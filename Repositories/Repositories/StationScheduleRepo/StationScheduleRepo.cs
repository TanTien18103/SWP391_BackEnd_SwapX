using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repositories.StationScheduleRepo
{
    public class StationScheduleRepo: IStationScheduleRepo
    {
        private readonly SwapXContext _context;
        public StationScheduleRepo(SwapXContext context)
        {
            _context = context;
        }
        public async Task<StationSchedule> AddStationSchedule(StationSchedule stationSchedule)
        {
            _context.StationSchedules.Add(stationSchedule);
            await _context.SaveChangesAsync();
            return stationSchedule;
        }
        public async Task<List<StationSchedule>> GetAllStationSchedules()
        {
            return await _context.StationSchedules
                .Include(ss=>ss.ExchangeBatteries)
                .ToListAsync();
        }
        public async Task<StationSchedule> GetStationScheduleById(string stationScheduleId)
        {
            return await _context.StationSchedules
                .Include(ss=>ss.ExchangeBatteries)
                .FirstOrDefaultAsync(ss => ss.StationScheduleId == stationScheduleId);
        }

        public async Task<List<StationSchedule>> GetStationSchedulesByAccountId(string accountId)
        {
            return await _context.StationSchedules
                .Include(ss => ss.ExchangeBatteries)
                .Include(ss => ss.Form)
                .Where(ss => ss.Form.AccountId == accountId)
                .ToListAsync();
        }


        public async Task<List<StationSchedule>> GetStationSchedulesByStationId(string stationId)
        {
            return await _context.StationSchedules
                .Include(ss => ss.ExchangeBatteries)
                .Where(ss => ss.StationId == stationId)
                .ToListAsync();
        }

        public async Task<StationSchedule> UpdateStationSchedule(StationSchedule stationSchedule)
        {
            _context.StationSchedules.Update(stationSchedule);
            await _context.SaveChangesAsync();
            return stationSchedule;
        }
    }
}
