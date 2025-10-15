using Services.ApiModels;
using Services.ApiModels.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services.AccountService
{
    public interface IAccountService
    {
        Task<(string accessToken, string refreshToken)> Login(string username, string password);
        Task<ResultModel> Logout();
        Task<ResultModel> ForgotPassword(string email);
        Task<ResultModel> ForgotPasswordVerifyOtp(string email, string otp);
        Task<ResultModel> ChangePassword(ChangePasswordRequest request);
        Task<string> Register(RegisterRequest registerRequest);
        Task<ResultModel> CreateAdmin(RegisterRequest registerRequest);
        Task<ResultModel> CreateStaff(RegisterRequest registerRequest);
        Task<ResultModel> UpdateStaff(UpdateStaffRequest updateStaffRequest);
        Task<ResultModel> GetAllAccounts();
        Task<ResultModel> GetAccountById(string accountId);
        Task<ResultModel> GetAllStaff();
        Task<ResultModel> GetAllCustomer();
        Task<ResultModel> DeleteStaff(string accountId);
        Task<ResultModel> DeleteCustomer(string accountId);
        Task<ResultModel> GetCurrentUser();
        Task<ResultModel> UpdateCurrentProfile(UpdateProfileRequest updateProfileRequest);
        Task<ResultModel> UpdateCustomer(UpdateCustomerRequest updateCustomerRequest);
        Task<ResultModel> UpdateStatus(UpdateStatusRequest updateStatusRequest);
        Task<ResultModel> GetAllCustomerHasPackage();
        Task<ResultModel> GetCustomersStatus();
    }
}
