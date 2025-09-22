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

    }
}
