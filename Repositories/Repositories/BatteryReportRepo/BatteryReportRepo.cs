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
            _context.BatteryReports.Update(batteryReport);
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
    }
}
