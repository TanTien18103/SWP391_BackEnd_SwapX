using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using Repositories.Repositories.VehicleRepo;
using Services.ServicesHelpers;
using Services.ApiModels;
using Services.ApiModels.Vehicle;



namespace Services.Services.VehicleService
{
    public class VehicleService: IVehicleService
    {
        private readonly IVehicleRepo _vehicleRepo;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly AccountHelper _accountHelper;

        public VehicleService(IVehicleRepo vehicleRepo, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _vehicleRepo = vehicleRepo;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        public Task<ResultModel> AddVehicle(AddVehicleRequest addVehicleRequest)
        {
            throw new NotImplementedException();
        }

        public Task<ResultModel> GetAllVehicles()
        {
            throw new NotImplementedException();
        }

        public Task<ResultModel> GetVehicleById(string vehicleId)
        {
            throw new NotImplementedException();
        }

        public Task<ResultModel> UpdateVehicle(UpdateVehicleRequest updateVehicleRequest)
        {
            throw new NotImplementedException();
        }
    }
}
