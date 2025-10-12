using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects.Enums;

namespace Repositories.Repositories.BatteryReportRepo
{
    public class BatteryReportRepo: IBatteryReportRepo
    {
        private readonly SwapXContext _context;
        public BatteryReportRepo(SwapXContext context)
        {
            _context = context;
        }
        public async Task<BatteryReport> AddBatteryReport(BatteryReport batteryReport)
        {
            _context.BatteryReports.Add(batteryReport);
            await _context.SaveChangesAsync();
            return batteryReport;
        }
        public async Task<List<BatteryReport>> GetAllBatteryReports()
        {
            return await _context.BatteryReports.Include(a=>a.Battery).Include(b=>b.Account).Include(c=>c.Station).ToListAsync();
        }
        public async Task<BatteryReport> GetBatteryReportById(string batteryReportId)
        {
            return await _context.BatteryReports.Include(a => a.Battery).Include(b => b.Account).Include(c => c.Station).FirstOrDefaultAsync(br => br.BatteryReportId == batteryReportId);
        }
        public async Task<BatteryReport> UpdateBatteryReport(BatteryReport batteryReport)
        {
            var trackedReport = _context.ChangeTracker.Entries<BatteryReport>()
                .FirstOrDefault(e => e.Entity.BatteryReportId == batteryReport.BatteryReportId);
            if (trackedReport != null)
                trackedReport.State = EntityState.Detached;

            // Detach tracked Battery entity if exists
            if (batteryReport.Battery != null)
            {
                var trackedBattery = _context.ChangeTracker.Entries<Battery>()
                    .FirstOrDefault(e => e.Entity.BatteryId == batteryReport.Battery.BatteryId);
                if (trackedBattery != null)
                    trackedBattery.State = EntityState.Detached;
            }

            _context.Entry(batteryReport).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return batteryReport;
        }


        public async Task<List<BatteryReport>> GetBatteryReportsByStation(string stationId)
        {
            return await _context.BatteryReports
                .Include(r => r.Battery)
                .Include(r => r.Account)
                .Where(r => r.StationId == stationId && r.Status != BatteryReportStatusEnums.Inactive.ToString())
                .OrderByDescending(r => r.UpdateDate)
                .ToListAsync();
        }
        public async Task<BatteryReport?> GetByBatteryId(string batteryId)
        {
            return await _context.BatteryReports
                .OrderByDescending(r => r.UpdateDate)
                .FirstOrDefaultAsync(r => r.BatteryId == batteryId);
        }
        public async Task<BatteryReport?> GetByExchangeBatteryId(string exchangeBatteryId)
        {
            return await _context.BatteryReports
                .OrderByDescending(r => r.UpdateDate)
                .FirstOrDefaultAsync(r => r.ExchangeBatteryId == exchangeBatteryId);
        }

        public async Task<BatteryReport> Update(BatteryReport batteryReport)
        {
            return await UpdateBatteryReport(batteryReport);
        }
    }
}
