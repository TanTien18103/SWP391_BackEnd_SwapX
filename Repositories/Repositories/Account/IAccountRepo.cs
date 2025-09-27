using BusinessObjects.Models;
using CloudinaryDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repositories.Account
{
    public interface IAccountRepo
    {
        Task<BusinessObjects.Models.Account> GetAccountByUserName(string username);
        Task AddAccount(BusinessObjects.Models.Account account);
        Task<BusinessObjects.Models.Account> UpdateAccount(BusinessObjects.Models.Account account);
        Task<BusinessObjects.Models.Account> GetAccountById(string accountId);
        Task<List<BusinessObjects.Models.Account>> GetAll();
        Task<List<BusinessObjects.Models.Account>> GetAllStaff();
        Task<List<BusinessObjects.Models.Account>> GetAllCustomer();
        Task<BusinessObjects.Models.Account> GetAccountByEmail(string email);

    }
}
