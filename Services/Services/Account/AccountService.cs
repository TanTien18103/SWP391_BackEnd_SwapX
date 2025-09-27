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
using Services.ApiModels;
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

        public async Task<ResultModel> CreateStaff(RegisterRequest registerRequest)
        {
            try
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
                    StartDate = TimeHepler.SystemTimeNow,
                    UpdateDate = TimeHepler.SystemTimeNow,
                };

                var driver = new BssStaff
                {
                    StaffId = _accountHelper.GenerateShortGuid(),
                    AccountId = newUser.AccountId,
                    Account = newUser,
                    StartDate = TimeHepler.SystemTimeNow,
                    UpdateDate = TimeHepler.SystemTimeNow,
                };

                newUser.BssStaffs.Add(driver);

                await _accountRepository.AddAccount(newUser);
                var createdUser =
                await _accountRepository.GetAccountById(newUser.AccountId);
                return new ResultModel
                {
                    IsSuccess = true,
                    ResponseCode = ResponseCodeConstants.SUCCESS,
                    Message = ResponseMessageConstantsUser.CREATE_STAFF_SUCCESS,
                    Data = createdUser,
                    StatusCode = StatusCodes.Status201Created
                };
            }
            catch (Exception ex)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    ResponseCode = ResponseCodeConstants.FAILED,
                    Message = ex.Message,
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }
        public async Task<ResultModel> UpdateStaff(UpdateStaffRequest updateStaffRequest)
        {
            var res = new ResultModel();
            try
            {
                var existingUser = await _accountRepository.GetAccountById(updateStaffRequest.AccountId);
                if (existingUser == null)
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.NOT_FOUND,
                        Message = ResponseMessageConstantsUser.USER_NOT_FOUND,
                        StatusCode = StatusCodes.Status404NotFound
                    };
                }
                if (existingUser.Role != RoleEnums.Bsstaff.ToString())
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.BAD_REQUEST,
                        Message = ResponseMessageConstantsUser.USER_NOT_STAFF,
                        StatusCode = StatusCodes.Status400BadRequest
                    };
                }
                // Only update if the field is not null
                if (updateStaffRequest.Name != null)
                    existingUser.Name = updateStaffRequest.Name;
                if (updateStaffRequest.Phone != null)
                    existingUser.Phone = updateStaffRequest.Phone;
                if (updateStaffRequest.Address != null)
                    existingUser.Address = updateStaffRequest.Address;
                if (updateStaffRequest.Email != null)
                    existingUser.Email = updateStaffRequest.Email;

                existingUser.UpdateDate = TimeHepler.SystemTimeNow;
                await _accountRepository.UpdateAccount(existingUser);
                return new ResultModel
                {
                    IsSuccess = true,
                    ResponseCode = ResponseCodeConstants.SUCCESS,
                    Message = ResponseMessageConstantsUser.UPDATE_USER_SUCCESS,
                    Data = existingUser,
                    StatusCode = StatusCodes.Status200OK
                };

            }
            catch (Exception ex)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    ResponseCode = ResponseCodeConstants.FAILED,
                    Message = ex.Message,
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }
        public async Task<ResultModel> GetAllAccounts()
        {

            var res = new ResultModel();
            try
            {
                var accounts = await _accountRepository.GetAll();
                return new ResultModel
                {
                    IsSuccess = true,
                    ResponseCode = ResponseCodeConstants.SUCCESS,
                    Message = ResponseMessageConstantsUser.GET_USER_INFO_SUCCESS,
                    Data = accounts,
                    StatusCode = StatusCodes.Status200OK
                };

            }
            catch (Exception ex)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    ResponseCode = ResponseCodeConstants.FAILED,
                    Message = ex.Message,
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }
        public async Task<ResultModel> GetAccountById(string accountId)
        {
            try
            {
                var account = await _accountRepository.GetAccountById(accountId);
                if (account == null)
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.NOT_FOUND,
                        Message = ResponseMessageConstantsUser.USER_NOT_FOUND,
                        StatusCode = StatusCodes.Status404NotFound
                    };
                }
                return new ResultModel
                {
                    IsSuccess = true,
                    ResponseCode = ResponseCodeConstants.SUCCESS,
                    Message = ResponseMessageConstantsUser.GET_USER_INFO_SUCCESS,
                    Data = account,
                    StatusCode = StatusCodes.Status200OK
                };
            }
            catch (Exception ex)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    ResponseCode = ResponseCodeConstants.FAILED,
                    Message = ex.Message,
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }

        public async Task<ApiModels.ResultModel> GetAllStaff()
        {
            var res = new ResultModel();
            try
            {
                var staffs = await _accountRepository.GetAllStaff();
                return new ResultModel
                {
                    IsSuccess = true,
                    ResponseCode = ResponseCodeConstants.SUCCESS,
                    Message = ResponseMessageConstantsUser.GET_USER_INFO_SUCCESS,
                    Data = staffs,
                    StatusCode = StatusCodes.Status200OK
                };


            }
            catch (Exception ex)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    ResponseCode = ResponseCodeConstants.FAILED,
                    Message = ex.Message,
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }


        }
        public async Task<ApiModels.ResultModel> GetAllCustomer()
        {
            var res = new ResultModel();
            try
            {
                var customers = await _accountRepository.GetAllCustomer();
                return new ResultModel
                {
                    IsSuccess = true,
                    ResponseCode = ResponseCodeConstants.SUCCESS,
                    Message = ResponseMessageConstantsUser.GET_USER_INFO_SUCCESS,
                    Data = customers,
                    StatusCode = StatusCodes.Status200OK
                };
            }
            catch (Exception ex)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    ResponseCode = ResponseCodeConstants.FAILED,
                    Message = ex.Message,
                    StatusCode = StatusCodes.Status500InternalServerError
                };


            }
        }
        public async Task<ResultModel> DeleteStaff(string accountId)
        {
            try
            {
                var existingUser = await _accountRepository.GetAccountById(accountId);
                if (existingUser == null)
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.NOT_FOUND,
                        Message = ResponseMessageConstantsUser.USER_NOT_FOUND,
                        StatusCode = StatusCodes.Status404NotFound
                    };
                }
                if (existingUser.Role != RoleEnums.Bsstaff.ToString())
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.BAD_REQUEST,
                        Message = ResponseMessageConstantsUser.USER_NOT_STAFF,
                        StatusCode = StatusCodes.Status400BadRequest
                    };
                }
                // Set status to Inactive
                existingUser.Status = AccountStatusEnums.Inactive.ToString();
                await _accountRepository.UpdateAccount(existingUser);

                return new ResultModel
                {
                    IsSuccess = true,
                    ResponseCode = ResponseCodeConstants.SUCCESS,
                    Message = ResponseMessageConstantsUser.DELETE_USER_SUCCESS,
                    StatusCode = StatusCodes.Status200OK
                };
            }
            catch (Exception ex)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    ResponseCode = ResponseCodeConstants.FAILED,
                    Message = ex.Message,
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }
        public async Task<ResultModel> DeleteCustomer(string accountId)
        {
            try
            {
                var existingUser = await _accountRepository.GetAccountById(accountId);
                if (existingUser == null)
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.NOT_FOUND,
                        Message = ResponseMessageConstantsUser.USER_NOT_FOUND,
                        StatusCode = StatusCodes.Status404NotFound
                    };
                }
                if (existingUser.Role != RoleEnums.EvDriver.ToString())
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.BAD_REQUEST,
                        Message = ResponseMessageConstantsUser.USER_NOT_CUSTOMER,
                        StatusCode = StatusCodes.Status400BadRequest
                    };
                }
                // Set status to Inactive
                existingUser.Status = AccountStatusEnums.Inactive.ToString();
                await _accountRepository.UpdateAccount(existingUser);
                return new ResultModel
                {
                    IsSuccess = true,
                    ResponseCode = ResponseCodeConstants.SUCCESS,
                    Message = ResponseMessageConstantsUser.DELETE_USER_SUCCESS,
                    StatusCode = StatusCodes.Status200OK
                };
            }
            catch (Exception ex)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    ResponseCode = ResponseCodeConstants.FAILED,
                    Message = ex.Message,
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }

        }
        public async Task<ResultModel> Logout()
        {
            try
            {
                if (_httpContextAccessor.HttpContext?.Session != null)
                {
                    _httpContextAccessor.HttpContext?.Session.Clear();
                }
                return new ResultModel
                {
                    IsSuccess = true,
                    ResponseCode = ResponseCodeConstants.SUCCESS,
                    Message = ResponseMessageIdentity.LOGOUT_SUCCESS,
                    StatusCode = StatusCodes.Status200OK
                };
            }
            catch (Exception ex)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    ResponseCode = ResponseCodeConstants.FAILED,
                    Message = ex.Message,
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }
    }
}
