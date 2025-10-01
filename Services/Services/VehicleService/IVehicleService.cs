using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Services.ApiModels;
using Services.ApiModels.Vehicle;

namespace Services.Services.VehicleService
{
    public interface IVehicleService
    {
        Task<ResultModel> GetVehicleById(string vehicleId);
        Task<ResultModel> GetAllVehicles();
        Task<ResultModel> AddVehicle(AddVehicleRequest addVehicleRequest);
        Task<ResultModel> UpdateVehicle(UpdateVehicleRequest updateVehicleRequest);
        Task<ResultModel> DeleteVehicle(string vehicleId);
    }
}
