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
        Task<BusinessObjects.Models.Account> GetAccountByUserNameDao(string username);
        Task AddAccount(BusinessObjects.Models.Account account);
        Task<BusinessObjects.Models.Account> UpdateAccount(BusinessObjects.Models.Account account);
        Task<BusinessObjects.Models.Account> GetAccountById(string accountId);
        Task<List<BusinessObjects.Models.Account>> GetAll(BusinessObjects.Models.Account account);

    }
}
