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
        Task<Account> GetAccountByUserName(string username);
        Task AddAccount(Account account);
        Task<Account> UpdateAccount(Account account);
        Task<Account> GetAccountById(string accountId);
        Task<List<Account>> GetAll();
        Task<List<Account>> GetAllStaff();
        Task<List<Account>> GetAllCustomer();
        Task<Account> GetAccountByEmail(string email);
        Task<string> GetAccountIdFromToken(string token);
    }
}
