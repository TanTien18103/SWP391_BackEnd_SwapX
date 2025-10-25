using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure;
using BusinessObjects.Enums;
using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;

namespace Repositories.Repositories.StationRepo
{
    public class StationRepo: IStationRepo
    {
        private readonly SwapXContext _context;
        public StationRepo(SwapXContext context)
        {
            _context = context;
        }

        public async Task AddStation(Station station)
        {
            _context.Stations.Add(station);
            await _context.SaveChangesAsync();
        }
        public async Task<List<Station>> GetAllStations()
        {
          await UpdateAllStationsAverageRating();
            return await _context.Stations
                .Include(b=>b.Batteries)
                .Include(s => s.BssStaffs)
                .ToListAsync();
        }

        public async Task<Station> GetStationById(string stationId)
        {
            await UpdateAllStationsAverageRating();
            return await _context.Stations.Include(b=>b.Batteries).FirstOrDefaultAsync(s => s.StationId == stationId);
        }

       

        public async Task<Station> UpdateStation(Station station)
        {
            _context.Stations.Update(station);
            await UpdateAllStationsAverageRating();
            await _context.SaveChangesAsync();
            return station;
        }
        
        public async Task UpdateAllStationsAverageRating()
        {
            var stations = await _context.Stations.ToListAsync();
            foreach (var station in stations)
            {
                var ratings = await _context.Ratings
                    .Where(r => r.StationId == station.StationId && r.Rating1 != null&&r.Status==RatingStatusEnums.Active.ToString())
                    .ToListAsync();

                decimal avg = 0m;
                if (ratings.Count > 0)
                {
                    avg = ratings.Average(r => r.Rating1.Value);
                }

                station.Rating = avg;
            }
            await _context.SaveChangesAsync();
        }

        public async Task<List<Station>> GetAllStationsOfCustomer()
        {
            await UpdateAllStationsAverageRating();
            return await _context.Stations
                .Where(s => s.Status == StationStatusEnum.Active.ToString())
                .Include(b => b.Batteries)
                .Include(s => s.BssStaffs)
                .ToListAsync();
        }

        public async Task<List<Station>> GetAllStationsOfCustomerSuitVehicle(string vehicleId)
        {
            await UpdateAllStationsAverageRating();
            var vehicle = await _context.Vehicles.FirstOrDefaultAsync(v => v.Vin == vehicleId);
            if(vehicle == null) return await GetAllStationsOfCustomer();
            var batteryOfVehicle = await _context.Batteries.FirstOrDefaultAsync(b => b.BatteryId == vehicle.BatteryId);
            return await _context.Stations
                .Where(s => s.Status == StationStatusEnum.Active.ToString())
                .Include(b => b.Batteries)
                .Include(s => s.BssStaffs)
                .Where(s => s.Batteries.Any(b => b.BatteryType == batteryOfVehicle.BatteryType && b.Status == BatteryStatusEnums.Available.ToString()&& b.Specification == batteryOfVehicle.Specification))
                .ToListAsync();
        }
    }
}
