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
using Services.Services.Email;
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
        private readonly IEmailService _emailService;
        private readonly AccountHelper _accountHelper;

        public AccountService(IAccountRepo accountRepository, IConfiguration configuration, IHttpContextAccessor httpContextAccessor, IEvDriverRepo evDriverRepo, AccountHelper accountHelper, IEmailService emailService)
        {
            _accountRepository = accountRepository;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _evDriverRepository = evDriverRepo;
            _accountHelper = accountHelper;
            _emailService = emailService;
        }

        public async Task<(string accessToken, string refreshToken)> Login(string username, string password)
        {
            var user = await _accountRepository.GetAccountByUserName(username);
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
            var existingUser = await _accountRepository.GetAccountByUserName(registerRequest.Username);
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
                Status = AccountStatusEnums.Active.ToString(),
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
                var existingUser = await _accountRepository.GetAccountByUserName(registerRequest.Username);
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

        public async Task<ResultModel> ForgotPassword(string email)
        {
            var res = new ResultModel();
            try
            {
                var user = await _accountRepository.GetAccountByEmail(email);
                if (user == null)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.Message = ResponseMessageIdentity.INCORRECT_EMAIL;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    return res;
                }
                var random = new Random();
                string otp = random.Next(100000, 999999).ToString();

                user.OtpCode = otp;
                user.OtpExpiredTime = TimeHepler.SystemTimeNow.AddMinutes(5);
                user.UpdateDate = TimeHepler.SystemTimeNow;
                await _accountRepository.UpdateAccount(user);

                string subject = EmailConstants.OtpSubject;
                string body = string.Format(EmailConstants.OtpBodyTemplate, otp);

                await _emailService.SendEmail(email, subject, body);

                res.IsSuccess = true;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.Message = ResponseMessageIdentitySuccess.FORGOT_PASSWORD_SUCCESS;
                res.StatusCode = StatusCodes.Status200OK;
                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.ResponseCode = ResponseCodeConstants.FAILED;
                res.Message = ex.Message;
                res.StatusCode = StatusCodes.Status500InternalServerError;
                return res;
            }
        }

        public async Task<ResultModel> ForgotPasswordVerifyOtp(string email, string otp)
        {
            var res = new ResultModel();
            try
            {
                var user = await _accountRepository.GetAccountByEmail(email);
                if (user == null)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.Message = ResponseMessageIdentity.INCORRECT_EMAIL;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    return res;
                }

                if (user.OtpExpiredTime < TimeHepler.SystemTimeNow)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.BAD_REQUEST;
                    res.Message = ResponseMessageIdentity.OTP_EXPIRED;
                    res.StatusCode = StatusCodes.Status400BadRequest;
                    return res;
                }

                if (user.OtpCode != otp)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.BAD_REQUEST;
                    res.Message = ResponseMessageIdentity.OTP_INVALID;
                    res.StatusCode = StatusCodes.Status400BadRequest;
                    return res;
                }

                string newPassword = _accountHelper.GenerateShortGuid();
                user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
                user.OtpExpiredTime = null;
                user.OtpCode = null;

                await _accountRepository.UpdateAccount(user);

                string subject = EmailConstants.NewPasswordSubject;
                string body = string.Format(EmailConstants.NewPasswordBodyTemplate, newPassword);

                await _emailService.SendEmail(email, subject, body);

                res.IsSuccess = true;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.Message = ResponseMessageIdentity.FORGOT_PASSWORD_SUCCESS;
                res.StatusCode = StatusCodes.Status200OK;

                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.ResponseCode = ResponseCodeConstants.FAILED;
                res.Message = ex.Message;
                res.StatusCode = StatusCodes.Status500InternalServerError;

                return res;
            }
        }

        public async Task<ResultModel> ChangePassword(ChangePasswordRequest request)
        {
            try
            {
                if (!_httpContextAccessor.HttpContext.Request.Headers.TryGetValue("Authorization", out var authHeader)
                    || string.IsNullOrEmpty(authHeader)
                    || !authHeader.ToString().StartsWith("Bearer "))
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.UNAUTHORIZED,
                        Message = ResponseMessageIdentity.TOKEN_NOT_SEND,
                        StatusCode = StatusCodes.Status401Unauthorized
                    };
                }

                var token = authHeader.ToString().Substring("Bearer ".Length);
                var accountId = await _accountRepository.GetAccountIdFromToken(token);
                if (string.IsNullOrEmpty(accountId))
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.UNAUTHORIZED,
                        Message = ResponseMessageIdentity.TOKEN_INVALID_OR_EXPIRED,
                        StatusCode = StatusCodes.Status401Unauthorized
                    };
                }

                var existingAccount = await _accountRepository.GetAccountById(accountId);
                if (existingAccount == null)
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.NOT_FOUND,
                        Message = ResponseMessageIdentity.ACCOUNT_NOT_FOUND,
                        StatusCode = StatusCodes.Status404NotFound
                    };
                }

                if (!_accountHelper.VerifyPassword(request.OldPassword, existingAccount.Password))
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.BAD_REQUEST,
                        Message = ResponseMessageIdentity.OLD_PASSWORD_WRONG,
                        StatusCode = StatusCodes.Status400BadRequest
                    };
                }

                if (_accountHelper.VerifyPassword(request.NewPassword, existingAccount.Password))
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.BAD_REQUEST,
                        Message = ResponseMessageIdentity.NEW_PASSWORD_CANNOT_MATCH,
                        StatusCode = StatusCodes.Status400BadRequest
                    };
                }

                existingAccount.Password = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
                await _accountRepository.UpdateAccount(existingAccount);

                return new ResultModel
                {
                    IsSuccess = true,
                    ResponseCode = ResponseCodeConstants.SUCCESS,
                    Message = ResponseMessageIdentitySuccess.CHANGE_PASSWORD_SUCCESS,
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

        public async Task<ResultModel> GetCurrentUser()
        {
            try
            {
                if (!_httpContextAccessor.HttpContext.Request.Headers.TryGetValue("Authorization", out var authHeader)
                    || string.IsNullOrEmpty(authHeader)
                    || !authHeader.ToString().StartsWith("Bearer "))
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.UNAUTHORIZED,
                        Message = ResponseMessageIdentity.TOKEN_NOT_SEND,
                        StatusCode = StatusCodes.Status401Unauthorized
                    };
                }

                var token = authHeader.ToString().Substring("Bearer ".Length);
                var accountId = await _accountRepository.GetAccountIdFromToken(token);
                if (string.IsNullOrEmpty(accountId))
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.UNAUTHORIZED,
                        Message = ResponseMessageIdentity.TOKEN_INVALID_OR_EXPIRED,
                        StatusCode = StatusCodes.Status401Unauthorized
                    };
                }

                var existingAccount = await _accountRepository.GetAccountById(accountId);
                if (existingAccount == null)
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.NOT_FOUND,
                        Message = ResponseMessageIdentity.ACCOUNT_NOT_FOUND,
                        StatusCode = StatusCodes.Status404NotFound
                    };
                }

                return new ResultModel
                {
                    IsSuccess = true,
                    ResponseCode = ResponseCodeConstants.SUCCESS,
                    Message = ResponseMessageConstantsUser.GET_USER_INFO_SUCCESS,
                    Data = existingAccount,
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
