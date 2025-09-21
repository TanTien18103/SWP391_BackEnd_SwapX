using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects.Models;

namespace Repositories.Repositories.Account
{
    public interface IAccountRepo
    {
        Task<BusinessObjects.Models.Account> GetAccountByUserNameDao(string username);
    }
}
