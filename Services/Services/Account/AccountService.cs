using BusinessObjects.Constants;
using BusinessObjects.Enums;
using BusinessObjects.Exceptions;
using BusinessObjects.Models;
using BusinessObjects.TimeCoreHelper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Repositories.Repositories.Account;
using Repositories.Repositories.EvDriver;
using Services.ApiModels.Account;
using Services.ServicesHelpers;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services.Account
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepo _accountRepository;
        private readonly IEvDriverRepo _evDriverRepository;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly AccountHelper _accountHelper;

        public AccountService(IAccountRepo accountRepository, IConfiguration configuration, IHttpContextAccessor httpContextAccessor, IEvDriverRepo evDriverRepo, AccountHelper accountHelper)
        {
            _accountRepository = accountRepository;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _evDriverRepository = evDriverRepo;
            _accountHelper = accountHelper;
        }

  

        public async Task<(string accessToken, string refreshToken)> Login(string username, string password)
        {
            var user = await _accountRepository.GetAccountByUserNameDao(username);
            if (user == null)
                throw new AppException(ResponseCodeConstants.NOT_FOUND, ResponseMessageConstantsUser.USER_NOT_FOUND, StatusCodes.Status404NotFound);

            if (!_accountHelper.VerifyPassword(password, user.Password))
                throw new AppException(ResponseCodeConstants.BAD_REQUEST, ResponseMessageIdentity.PASSWORD_INVALID, StatusCodes.Status400BadRequest);
            string accessToken = _accountHelper.CreateToken(user);
            string refreshToken = _accountHelper.GenerateRefreshToken();

            if (_httpContextAccessor.HttpContext?.Session != null)
            {
                _httpContextAccessor.HttpContext.Session.SetString("RefreshToken", refreshToken);
                _httpContextAccessor.HttpContext.Session.SetString("Username", username);
            }

            return (accessToken, refreshToken);
        }

        public async Task<string> Register(RegisterRequest registerRequest)
        {
            var existingUser = await _accountRepository.GetAccountByUserNameDao(registerRequest.Username);
            if (existingUser != null)
                throw new AppException(ResponseCodeConstants.EXISTED, ResponseMessageIdentity.EXISTED_USERNAME, StatusCodes.Status400BadRequest);

            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(registerRequest.Password);

            var newUser = new BusinessObjects.Models.Account
            {
                AccountId = _accountHelper.GenerateShortGuid(),
                Username = registerRequest.Username,
                Password = hashedPassword,
                Name = registerRequest.Name,
                Phone = registerRequest.Phone,
                Address = registerRequest.Address,
                Email = registerRequest.Email,
                Role = RoleEnums.EvDriver.ToString(),
                StartDate = DateTime.UtcNow,
                UpdateDate = DateTime.UtcNow,
            };

            var driver = new Evdriver
            {
                CustomerId = _accountHelper.GenerateShortGuid(),
                AccountId = newUser.AccountId,
                Account = newUser,
                StartDate = DateTime.UtcNow,
                UpdateDate = DateTime.UtcNow,
            };

            newUser.Evdrivers.Add(driver);

            await _accountRepository.AddAccount(newUser);

            string accessToken = _accountHelper.CreateToken(newUser);
            string refreshToken = _accountHelper.GenerateRefreshToken();

            if (_httpContextAccessor.HttpContext?.Session != null)
            {
                _httpContextAccessor.HttpContext.Session.SetString("RefreshToken", refreshToken);
                _httpContextAccessor.HttpContext.Session.SetString("UserName", registerRequest.Username);
            }

            return accessToken;
        }

        public async Task<string> CreateStaff(RegisterRequest registerRequest)
        {
            var existingUser = await _accountRepository.GetAccountByUserNameDao(registerRequest.Username);
            if (existingUser != null)
                throw new AppException(ResponseCodeConstants.EXISTED, ResponseMessageIdentity.EXISTED_USERNAME, StatusCodes.Status400BadRequest);

            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(registerRequest.Password);

            var newUser = new BusinessObjects.Models.Account
            {
                AccountId = _accountHelper.GenerateShortGuid(),
                Username = registerRequest.Username,
                Password = hashedPassword,
                Name = registerRequest.Name,
                Phone = registerRequest.Phone,
                Address = registerRequest.Address,
                Email = registerRequest.Email,
                Role = RoleEnums.Bsstaff.ToString(),
                StartDate = DateTime.UtcNow,
                UpdateDate = DateTime.UtcNow,
            };

            var driver = new BssStaff
            {
                StaffId = _accountHelper.GenerateShortGuid(),
                AccountId = newUser.AccountId,
                Account = newUser,
                StartDate = DateTime.UtcNow,
                UpdateDate = DateTime.UtcNow,
            };

            newUser.BssStaffs.Add(driver);

            await _accountRepository.AddAccount(newUser);

            string accessToken = _accountHelper.CreateToken(newUser);
            string refreshToken = _accountHelper.GenerateRefreshToken();

            if (_httpContextAccessor.HttpContext?.Session != null)
            {
                _httpContextAccessor.HttpContext.Session.SetString("RefreshToken", refreshToken);
                _httpContextAccessor.HttpContext.Session.SetString("UserName", registerRequest.Username);
            }

            return accessToken;
        }
        public async Task<string> UpdateStaff(UpdateStaffRequest updateStaffRequest)
        {
            var existingUser = await _accountRepository.GetAccountById(updateStaffRequest.AccountId);
            if (existingUser == null)
                throw new AppException(ResponseCodeConstants.NOT_FOUND, ResponseMessageConstantsUser.USER_NOT_FOUND, StatusCodes.Status404NotFound);

            // Only update if the field is not null
            if (updateStaffRequest.Name != null)
                existingUser.Name = updateStaffRequest.Name;
            if (updateStaffRequest.Phone != null)
                existingUser.Phone = updateStaffRequest.Phone;
            if (updateStaffRequest.Address != null)
                existingUser.Address = updateStaffRequest.Address;
            if (updateStaffRequest.Email != null)
                existingUser.Email = updateStaffRequest.Email;

            existingUser.UpdateDate = DateTime.UtcNow;
            await _accountRepository.UpdateAccount(existingUser);
            string accessToken = _accountHelper.CreateToken(existingUser);
            return accessToken;
        }
    }
}
