using BusinessObjects.Enums;
using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;

namespace Repositories.Repositories.ExchangeBatteryRepo
{
    public class ExchangeBatteryRepo : IExchangeBatteryRepo
    {
        private readonly SwapXContext _context;

        public ExchangeBatteryRepo(SwapXContext context)
        {
            _context = context;
        }

        public async Task<ExchangeBattery> Add(ExchangeBattery exchange)
        {
            _context.ExchangeBatteries.Add(exchange);
            await _context.SaveChangesAsync();
            return exchange;
        }

        public async Task<ExchangeBattery> Update(ExchangeBattery exchange)
        {
            var trackedEntity = await _context.ExchangeBatteries
                .FirstOrDefaultAsync(e => e.ExchangeBatteryId == exchange.ExchangeBatteryId);

            if (trackedEntity != null)
            {
                _context.Entry(trackedEntity).CurrentValues.SetValues(exchange);
            }
            else
            {
                _context.ExchangeBatteries.Update(exchange);
            }

            await _context.SaveChangesAsync();
            return exchange;
        }



        public async Task Delete(ExchangeBattery exchange)
        {
            _context.ExchangeBatteries.Remove(exchange);
            await _context.SaveChangesAsync();
        }

        public async Task<ExchangeBattery?> GetById(string exchangeId)
        {
            return await _context.ExchangeBatteries
                .Include(sc => sc.Schedule)
                                .ThenInclude(s => s.Form)
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.ExchangeBatteryId == exchangeId);
        }

        public async Task<List<ExchangeBattery>> GetAll()
        {
            return await _context.ExchangeBatteries
                .AsNoTracking()
                .Include(e => e.Station)
                .Include(e => e.Order)
                .Include(e => e.NewBattery)
                .Include(e => e.OldBattery)
                .Include(e => e.StaffAccount)
                .Include(e => e.Schedule)
                .ThenInclude(s => s.Form)
                .Include(e => e.VinNavigation)
                .ToListAsync();
        }

        public async Task<List<ExchangeBattery>> GetByStationId(string stationId)
        {
            return await _context.ExchangeBatteries
                .AsNoTracking()
                .Include(e => e.Station)
                .Include(e => e.Order)
                .Include(e => e.NewBattery)
                .Include(e => e.OldBattery)
                .Include(e => e.StaffAccount)
                .Include(e => e.Schedule)
                .ThenInclude(s => s.Form)
                .Include(e => e.VinNavigation)
                .Where(e => e.StationId == stationId)
                .ToListAsync();
        }

        public async Task<List<ExchangeBattery>> GetByDriverId(string driverId)
        {
            return await _context.ExchangeBatteries
                .AsNoTracking()
                .Include(e => e.Station)
                .Include(e => e.Order)
                .Include(e => e.NewBattery)
                .Include(e => e.OldBattery)
                .Include(e => e.StaffAccount)
                .Include(e => e.Schedule)
                .ThenInclude(s => s.Form)
                .Include(e => e.VinNavigation)
                .Where(e => e.Vin == driverId)
                .ToListAsync();
        }

        public async Task<ExchangeBattery?> GetByOrderId(string orderId)
        {
            return await _context.ExchangeBatteries
                .AsNoTracking()
                .Include(e => e.Station)
                .Include(e => e.Order)
                .Include(e => e.NewBattery)
                .Include(e => e.OldBattery)
                .Include(e => e.StaffAccount)
                .Include(e => e.Schedule)
                        .ThenInclude(s => s.Form)
                .Include(e => e.VinNavigation)
                .FirstOrDefaultAsync(e => e.OrderId == orderId);
        }

        public async Task<List<ExchangeBattery>> GetByScheduleId(string stationscheduleId)
        {
            return await _context.ExchangeBatteries
                .AsNoTracking()
                .Include(e => e.Station)
                .Include(e => e.Order)
                .Include(e => e.NewBattery)
                .Include(e => e.OldBattery)
                .Include(e => e.StaffAccount)
                .Include(e => e.Schedule)
                        .ThenInclude(s => s.Form)
                .Include(e => e.VinNavigation)
                .Where(e => e.ScheduleId == stationscheduleId)
                .ToListAsync();
        }

        public async Task<ExchangeBattery> GetPendingByVin(string vin)
        {
            if (string.IsNullOrWhiteSpace(vin))
                return null;

            var pendingStatus = ExchangeStatusEnums.Pending.ToString();

            return await _context.ExchangeBatteries
                .FirstOrDefaultAsync(e => e.Vin == vin && e.Status == pendingStatus);
        }

        public async Task<ExchangeBattery?> GetPendingByVinAndAccountId(string vin, string accountId)
        {
            var pendingStatus = ExchangeStatusEnums.Pending.ToString();

            return await _context.ExchangeBatteries
                .Include(e => e.Station)
                .Include(e => e.Order)
                .Include(e => e.NewBattery)
                .Include(e => e.OldBattery)
                .Include(e => e.StaffAccount)
                .Include(e => e.Schedule)
                    .ThenInclude(s => s.Form)
                .Include(e => e.VinNavigation)
                .FirstOrDefaultAsync(e =>
                    e.Vin == vin &&
                    e.Status == pendingStatus &&
                    e.Schedule != null &&
                    e.Schedule.Form != null &&
                    e.Schedule.Form.AccountId == accountId);
        }
    }
}
