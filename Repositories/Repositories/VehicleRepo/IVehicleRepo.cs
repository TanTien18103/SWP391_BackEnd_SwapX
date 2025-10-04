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
    public interface IVehicleRepo
    {
        Task<Vehicle> GetVehicleById(string vehicleId);
        Task<List<Vehicle>> GetAllVehicles();
        Task<Vehicle> AddVehicle(Vehicle vehicle);
        Task<Vehicle> UpdateVehicle(Vehicle vehicle);
        Task<Vehicle> GetVehicleByName(VehicleNameEnums vehicleName);   

    }
}
