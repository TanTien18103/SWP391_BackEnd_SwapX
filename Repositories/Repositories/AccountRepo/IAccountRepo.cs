using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repositories.AccountRepo
{
    public interface IAccountRepo
    {
        Task<List<Account>> GetAccountsByUserName(string username);
        Task AddAccount(Account account);
        Task<Account> UpdateAccount(Account account);
        Task<Account> GetAccountById(string accountId);
        Task<List<Account>> GetAll();
        Task<List<Account>> GetAllStaff();
        Task<List<Account>> GetAllCustomer();
        Task<List<Account>> GetAccountsByEmail(string email);
        Task<Account> GetAccountByStaffId(string staffId);
        Task<string> GetAccountIdFromToken(string token);
        Task<List<Account>> GetAllCustomerHasPackage();
        Task<List<object>> GetCustomersStatus();
        Task<Account>GetAccountByCustomerId(string customerId);

    }
}
