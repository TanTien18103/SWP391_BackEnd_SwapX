using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects.Enums;
using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;

namespace Repositories.Repositories.VehicleRepo
{
    public class VehicleRepo : IVehicleRepo
    {

        private readonly SwapXContext _context;
        public VehicleRepo(SwapXContext context)
        {
            _context = context;
        }
        public async Task<Vehicle> AddVehicle(Vehicle vehicle)
        {
            _context.Vehicles.Add(vehicle);
            await _context.SaveChangesAsync();
            return vehicle;
        }
        public async Task<List<Vehicle>> GetAllVehicles()
        {
            return await _context.Vehicles.Include(a => a.Battery).Include(b => b.Package).ToListAsync();
        }
        public async Task<Vehicle> GetVehicleById(string vehicleId)
        {
            return await _context.Vehicles.Include(a => a.Battery).Include(b => b.Package).FirstOrDefaultAsync(v => v.Vin == vehicleId);
        }
        public async Task<Vehicle> UpdateVehicle(Vehicle vehicle)
        {
            _context.Vehicles.Update(vehicle);
            await _context.SaveChangesAsync();
            return vehicle;
        }
        public async Task<List<Vehicle>> GetVehiclesByName(VehicleNameEnums vehicleName)
        {
            return await _context.Vehicles
                .Include(a => a.Battery)
                .Include(b => b.Package)
                .Where(v => v.VehicleName == vehicleName.ToString())
                .ToListAsync();
        }
        public async Task<List<Vehicle>> GetAllVehicleByCustomerId(string customerId)
        {
            return await _context.Vehicles
                .Include(a => a.Battery)
                .Include(b => b.Package)
                .Where(v => v.CustomerId == customerId)
                .ToListAsync();
        }

        public async Task<Package> GetPackageByVehicleId(string vehicleId)
        {
            return await _context.Vehicles
                 .Include(v => v.Package)
                 .Where(v => v.Vin == vehicleId)
                 .Select(v => v.Package)
                 .FirstOrDefaultAsync();
        }

        public async Task<List<Vehicle>> GetVehiclesByPackageId(string packageid)
        {
            return await _context.Vehicles
                .Include(a => a.Battery)
                .Include(b => b.Package)
                .Where(v => v.PackageId == packageid)
                .ToListAsync();
        }
        public async Task<Vehicle> GetVehicleByBatteryId(string batteryId)
        {
            return await _context.Vehicles
                .Include(a => a.Battery)
                .Include(b => b.Package)
                .FirstOrDefaultAsync(v => v.BatteryId == batteryId);
        }
    }
}
