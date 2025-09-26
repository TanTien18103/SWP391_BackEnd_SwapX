using Services.ApiModels.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services.Account
{
    public interface IAccountService
    {
        Task<(string accessToken, string refreshToken)> Login(string username, string password);

        Task<string> Register(RegisterRequest registerRequest);
        Task<string> CreateStaff(RegisterRequest registerRequest);
        Task<string> UpdateStaff(UpdateStaffRequest updateStaffRequest);
        Task<List<BusinessObjects.Models.Account>> GetAllAccounts();
        Task<BusinessObjects.Models.Account> GetAccountById(string accountId);
        Task<List<BusinessObjects.Models.Account>> GetAllStaff();
        Task<List<BusinessObjects.Models.Account>> GetAllCustomer();
    }
}
