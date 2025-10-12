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

        public async Task Add(ExchangeBattery exchange)
        {
            _context.ExchangeBatteries.Add(exchange);
            await _context.SaveChangesAsync();
        }

        public async Task Update(ExchangeBattery exchange)
        {
            var trackedExchange = await _context.ExchangeBatteries
                .FirstOrDefaultAsync(e => e.ExchangeBatteryId == exchange.ExchangeBatteryId);

            if (trackedExchange != null)
            {
                trackedExchange.Status = exchange.Status;
                trackedExchange.UpdateDate = exchange.UpdateDate;
                trackedExchange.Notes = exchange.Notes;
                trackedExchange.OrderId = exchange.OrderId;
                trackedExchange.NewBatteryId = exchange.NewBatteryId;
                trackedExchange.OldBatteryId = exchange.OldBatteryId;
                trackedExchange.ScheduleId = exchange.ScheduleId;
                trackedExchange.StaffAccountId = exchange.StaffAccountId;
                trackedExchange.StationId = exchange.StationId;
                trackedExchange.Vin = exchange.Vin;
                await _context.SaveChangesAsync();
            }
        }

        public async Task Delete(ExchangeBattery exchange)
        {
            _context.ExchangeBatteries.Remove(exchange);
            await _context.SaveChangesAsync();
        }

        public async Task<ExchangeBattery?> GetById(string exchangeId)
        {
            return await _context.ExchangeBatteries
                .AsNoTracking()
                .Include(e => e.Station)
                .Include(e => e.Order)
                .Include(e => e.NewBattery)
                .Include(e => e.OldBattery)
                .Include(e => e.StaffAccount)
                .Include(e => e.Schedule)
                .Include(e => e.VinNavigation)
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
                .Include(e => e.VinNavigation)
                .FirstOrDefaultAsync(e => e.OrderId == orderId);
        }
    }
}
