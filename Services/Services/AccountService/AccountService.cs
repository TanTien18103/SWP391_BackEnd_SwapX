using BusinessObjects.Constants;
using BusinessObjects.Enums;
using BusinessObjects.Exceptions;
using BusinessObjects.Models;
using BusinessObjects.TimeCoreHelper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Repositories.Repositories.AccountRepo;
using Repositories.Repositories.EvDriverRepo;
using Services.ApiModels;
using Services.ApiModels.Account;
using Services.Services.EmailService;
using Services.ServicesHelpers;

namespace Services.Services.AccountService
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepo _accountRepository;
        private readonly IEvDriverRepo _evDriverRepository;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IEmailService _emailService;
        private readonly AccountHelper _accountHelper;

        public AccountService(
            IAccountRepo accountRepository,
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor,
            IEvDriverRepo evDriverRepo,
            AccountHelper accountHelper,
            IEmailService emailService
            )
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
            var users = await _accountRepository.GetAccountsByUserName(username);
            if (users == null || !users.Any())
            {
                throw new AppException(ResponseCodeConstants.NOT_FOUND,
                    ResponseMessageConstantsUser.USER_NOT_FOUND,
                    StatusCodes.Status404NotFound);
            }

            // Nếu nhiều user trùng username -> lỗi
            if (users.Count(u => u.Status == AccountStatusEnums.Active.ToString()) > 1)
            {
                throw new AppException(ResponseCodeConstants.BAD_REQUEST,
                    ResponseMessageConstantsUser.USERNAME_DUPLICATED,
                    StatusCodes.Status400BadRequest);
            }

            var user = users.FirstOrDefault(u => u.Status == AccountStatusEnums.Active.ToString() && u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));

            if (user == null)
            {
                throw new AppException(ResponseCodeConstants.BAD_REQUEST,
                    ResponseMessageIdentity.ACCOUNT_INACTIVE_OR_NOT_VERIFIED,
                    StatusCodes.Status400BadRequest);
            }

            if (!_accountHelper.VerifyPassword(password, user.Password))
            {
                throw new AppException(ResponseCodeConstants.BAD_REQUEST,
                    ResponseMessageIdentity.PASSWORD_INVALID,
                    StatusCodes.Status400BadRequest);
            }
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
            // Check username (chỉ cấm khi có user Active)
            var usersWithSameUsername = await _accountRepository.GetAccountsByUserName(registerRequest.Username);
            if (usersWithSameUsername.Any(u => u.Status == AccountStatusEnums.Active.ToString()))
            {
                throw new AppException(ResponseCodeConstants.EXISTED,
                    ResponseMessageIdentity.EXISTED_USERNAME,
                    StatusCodes.Status400BadRequest);
            }

            // Check email (chỉ cấm khi có user Active)
            var usersWithSameEmail = await _accountRepository.GetAccountsByEmail(registerRequest.Email);
            if (usersWithSameEmail.Any(u => u.Status == AccountStatusEnums.Active.ToString()))
            {
                throw new AppException(ResponseCodeConstants.EXISTED,
                    ResponseMessageIdentity.EMAIL_IN_USE,
                    StatusCodes.Status400BadRequest);
            }
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(registerRequest.Password);

            string otp = _accountHelper.GenerateSecureOtp();
            DateTime otpExpiredTime = TimeHepler.SystemTimeNow.AddMinutes(30);

            var newUser = new Account
            {
                AccountId = _accountHelper.GenerateShortGuid(),
                Username = registerRequest.Username,
                Password = hashedPassword,
                Name = registerRequest.Name,
                Phone = registerRequest.Phone,
                Address = registerRequest.Address,
                Email = registerRequest.Email,
                Role = RoleEnums.EvDriver.ToString(),
                Status = AccountStatusEnums.NotVerified.ToString(),
                StartDate = TimeHepler.SystemTimeNow,
                UpdateDate = TimeHepler.SystemTimeNow,
                Avatar = registerRequest.Avatar,
                OtpCode = otp,
                OtpExpiredTime = otpExpiredTime
            };

            var driver = new Evdriver
            {
                CustomerId = _accountHelper.GenerateShortGuid(),
                AccountId = newUser.AccountId,
                Account = newUser,
                StartDate = TimeHepler.SystemTimeNow,
                UpdateDate = TimeHepler.SystemTimeNow,
            };

            newUser.Evdrivers.Add(driver);

            await _accountRepository.AddAccount(newUser);

            if (!string.IsNullOrEmpty(newUser.Email))
            {
                var subject = EmailConstants.REGISTER_OTP_SUBJECT;
                var body = string.Format(
                    EmailConstants.REGISTER_OTP_BODY,
                    newUser.Name,
                    otp
                );

                await _emailService.SendEmail(newUser.Email, subject, body);
            }

            return ResponseMessageConstantsUser.REGISTER_SUCCESS_NEED_VERIFY;
        }
        public async Task<ResultModel> RegisterVerifyOtp(string email, string otp)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(otp))
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.BAD_REQUEST,
                        Message = ResponseMessageIdentity.EMAIL_OTP_REQUIRED,
                        StatusCode = StatusCodes.Status400BadRequest
                    };
                }

                var accounts = await _accountRepository.GetAccountsByEmail(email);

                if (accounts == null || !accounts.Any())
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.NOT_FOUND,
                        Message = ResponseMessageIdentity.INCORRECT_EMAIL,
                        StatusCode = StatusCodes.Status404NotFound
                    };
                }

                var activeUser = accounts.FirstOrDefault(a => a.Status == AccountStatusEnums.Active.ToString());
                if (activeUser != null)
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.BAD_REQUEST,
                        Message = ResponseMessageIdentity.EMAIL_IN_USE,
                        StatusCode = StatusCodes.Status400BadRequest
                    };
                }

                var user = accounts.FirstOrDefault(a =>
                a.Status == AccountStatusEnums.NotVerified.ToString() &&
                a.OtpCode != null &&
                a.OtpExpiredTime != null);

                if (user == null)
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.NOT_FOUND,
                        Message = ResponseMessageIdentity.INCORRECT_EMAIL,
                        StatusCode = StatusCodes.Status404NotFound
                    };
                }

                if (user.OtpExpiredTime < TimeHepler.SystemTimeNow)
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.BAD_REQUEST,
                        Message = ResponseMessageIdentity.OTP_EXPIRED,
                        StatusCode = StatusCodes.Status400BadRequest
                    };
                }

                if (!_accountHelper.SecureStringCompare(user.OtpCode, otp))
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.BAD_REQUEST,
                        Message = ResponseMessageIdentity.OTP_INVALID,
                        StatusCode = StatusCodes.Status400BadRequest
                    };
                }

                user.Status = AccountStatusEnums.Active.ToString();
                user.OtpExpiredTime = null;
                user.OtpCode = null;
                user.UpdateDate = TimeHepler.SystemTimeNow;
                var updatedUser = await _accountRepository.UpdateAccount(user);
                var token = string.Empty;

                if (updatedUser != null)
                {
                    if (!string.IsNullOrEmpty(user.Email))
                    {
                        var subject = EmailConstants.REGISTER_SUCCESS_SUBJECT;
                        var body = string.Format(
                            EmailConstants.REGISTER_SUCCESS_BODY,
                            user.Name,
                            user.Email
                        );

                        await _emailService.SendEmail(user.Email, subject, body);
                    }

                    token = _accountHelper.CreateToken(user);
                }
                else
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.FAILED,
                        Message = ResponseMessageIdentity.REGISTER_VERIFY_OTP_FAILED,
                        StatusCode = StatusCodes.Status500InternalServerError
                    };
                }
                return new ResultModel
                {
                    IsSuccess = true,
                    ResponseCode = ResponseCodeConstants.SUCCESS,
                    Message = ResponseMessageIdentitySuccess.REGISTER_VERIFY_OTP_SUCCESS,
                    StatusCode = StatusCodes.Status200OK,
                    Data = new
                    {
                        AccessToken = token
                    }
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
        public async Task<ResultModel> ResendRegisterOtp(string email)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email))
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.BAD_REQUEST,
                        Message = ResponseMessageIdentity.EMAIL_REQUIRED,
                        StatusCode = StatusCodes.Status400BadRequest
                    };
                }
                var accounts = await _accountRepository.GetAccountsByEmail(email);

                if (accounts == null || !accounts.Any())
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.NOT_FOUND,
                        Message = ResponseMessageIdentity.INCORRECT_EMAIL,
                        StatusCode = StatusCodes.Status404NotFound
                    };
                }

                var activeUser = accounts.FirstOrDefault(a => a.Status == AccountStatusEnums.Active.ToString());
                if (activeUser != null)
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.BAD_REQUEST,
                        Message = ResponseMessageIdentity.EMAIL_IN_USE,
                        StatusCode = StatusCodes.Status400BadRequest
                    };
                }

                var user = accounts.FirstOrDefault(a =>
                a.Status == AccountStatusEnums.NotVerified.ToString() &&
                a.OtpCode != null &&
                a.OtpExpiredTime != null);

                if (user == null)
                {
                    var notVerifiedUser = accounts.FirstOrDefault(a => a.Status == AccountStatusEnums.NotVerified.ToString());

                    var message = notVerifiedUser != null
                                            ? ResponseMessageIdentity.REGISTER_VERIFY_OTP_FAILED
                                            : ResponseMessageIdentity.ACCOUNT_ALREADY_VERIFIED;

                    return new ResultModel
                    {
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.BAD_REQUEST,
                        Message = message,
                        StatusCode = StatusCodes.Status400BadRequest
                    };
                }

                string otp = _accountHelper.GenerateSecureOtp();
                DateTime otpExpiredTime = TimeHepler.SystemTimeNow.AddMinutes(30);

                user.OtpCode = otp;
                user.OtpExpiredTime = otpExpiredTime;
                user.UpdateDate = TimeHepler.SystemTimeNow;
                var updatedUser = await _accountRepository.UpdateAccount(user);
                if (updatedUser != null)
                {
                    var subject = EmailConstants.REGISTER_OTP_SUBJECT;
                    var body = string.Format(
                        EmailConstants.REGISTER_OTP_BODY,
                        user.Name,
                        otp
                    );

                    await _emailService.SendEmail(user.Email, subject, body);
                }
                else
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.FAILED,
                        Message = ResponseMessageIdentity.RESEND_OTP_FAILED,
                        StatusCode = StatusCodes.Status500InternalServerError
                    };
                }
                return new ResultModel
                {
                    IsSuccess = true,
                    ResponseCode = ResponseCodeConstants.SUCCESS,
                    Message = ResponseMessageIdentitySuccess.RESEND_OTP_SUCCESS,
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

        public async Task<ResultModel> CreateStaff(RegisterRequest registerRequest)
        {
            try
            {
                var usersWithSameUsername = await _accountRepository.GetAccountsByUserName(registerRequest.Username);
                if (usersWithSameUsername.Any(u => u.Status == AccountStatusEnums.Active.ToString()))
                {
                    throw new AppException(ResponseCodeConstants.EXISTED,
                        ResponseMessageIdentity.EXISTED_USERNAME,
                        StatusCodes.Status400BadRequest);
                }

                // Check email (chỉ cấm khi có user Active)
                var usersWithSameEmail = await _accountRepository.GetAccountsByEmail(registerRequest.Email);
                if (usersWithSameEmail.Any(u => u.Status == AccountStatusEnums.Active.ToString()))
                {
                    throw new AppException(ResponseCodeConstants.EXISTED,
                        ResponseMessageIdentity.EMAIL_IN_USE,
                        StatusCodes.Status400BadRequest);
                }

                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(registerRequest.Password);

                var newUser = new Account
                {
                    AccountId = _accountHelper.GenerateShortGuid(),
                    Username = registerRequest.Username,
                    Password = hashedPassword,
                    Name = registerRequest.Name,
                    Phone = registerRequest.Phone,
                    Address = registerRequest.Address,
                    Email = registerRequest.Email,
                    Role = RoleEnums.Bsstaff.ToString(),
                    Status = AccountStatusEnums.Active.ToString(),
                    StartDate = TimeHepler.SystemTimeNow,
                    UpdateDate = TimeHepler.SystemTimeNow,
                    Avatar = registerRequest.Avatar,
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

                if (!string.IsNullOrEmpty(newUser.Email))
                {
                    var subject = EmailConstants.STAFF_REGISTER_SUCCESS_SUBJECT;
                    var body = string.Format(
                        EmailConstants.STAFF_REGISTER_SUCCESS_BODY,
                        newUser.Name,
                        newUser.Username
                    );

                    await _emailService.SendEmail(newUser.Email, subject, body);
                }
                else
                {
                    throw new AppException(ResponseCodeConstants.BAD_REQUEST,
                        ResponseMessageIdentity.EMAIL_REQUIRED,
                        StatusCodes.Status400BadRequest);
                }

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
                var existingEmailUsers = await _accountRepository.GetAccountsByEmail(updateStaffRequest.Email);
                if (existingEmailUsers != null)
                {
                    foreach (var user in existingEmailUsers)
                    {
                        if (user.AccountId != existingUser.AccountId && user.Status == AccountStatusEnums.Active.ToString())
                        {
                            return new ResultModel
                            {
                                IsSuccess = false,
                                ResponseCode = ResponseCodeConstants.EXISTED,
                                Message = ResponseMessageIdentity.EMAIL_IN_USE,
                                StatusCode = StatusCodes.Status400BadRequest
                            };
                        }
                    }
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
                if (updateStaffRequest.Avatar != null)
                    existingUser.Avatar = updateStaffRequest.Avatar;

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

        public async Task<ResultModel> GetAllStaff()
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
        public async Task<ResultModel> GetAllCustomer()
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
                var evdriver = await _evDriverRepository.GetDriverByAccountId(existingUser.AccountId);
                existingUser.UpdateDate = TimeHepler.SystemTimeNow;
                evdriver.UpdateDate = TimeHepler.SystemTimeNow;
                await _accountRepository.UpdateAccount(existingUser);
                await _evDriverRepository.UpdateDriver(evdriver);
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
                var accounts = await _accountRepository.GetAccountsByEmail(email);

                if (accounts == null || !accounts.Any())
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.Message = ResponseMessageIdentity.INCORRECT_EMAIL;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    return res;
                }

                var activeUser = accounts.FirstOrDefault(a => a.Status == AccountStatusEnums.Active.ToString());

                if (activeUser == null)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.Message = ResponseMessageIdentity.INCORRECT_EMAIL;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    return res;
                }

                if (accounts.Count(a => a.Status == AccountStatusEnums.Active.ToString()) > 1)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.BAD_REQUEST;
                    res.Message = ResponseMessageIdentity.DUPLICATED_EMAIL;
                    res.StatusCode = StatusCodes.Status400BadRequest;
                    return res;
                }

                string otp = _accountHelper.GenerateSecureOtp();
                DateTime otpExpiredTime = TimeHepler.SystemTimeNow.AddMinutes(30);

                activeUser.OtpCode = otp;
                activeUser.OtpExpiredTime = otpExpiredTime;
                activeUser.UpdateDate = TimeHepler.SystemTimeNow;
                await _accountRepository.UpdateAccount(activeUser);

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
                if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(otp))
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.BAD_REQUEST;
                    res.Message = ResponseMessageIdentity.EMAIL_OTP_REQUIRED;
                    res.StatusCode = StatusCodes.Status400BadRequest;
                    return res;
                }
                var accounts = await _accountRepository.GetAccountsByEmail(email);

                if (accounts == null || !accounts.Any())
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.Message = ResponseMessageIdentity.INCORRECT_EMAIL;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    return res;
                }

                var activeUsers = accounts.Where(a => a.Status == AccountStatusEnums.Active.ToString()).ToList();

                if (!activeUsers.Any())
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.NOT_FOUND;
                    res.Message = ResponseMessageIdentity.INCORRECT_EMAIL;
                    res.StatusCode = StatusCodes.Status404NotFound;
                    return res;
                }

                if (activeUsers.Count > 1)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.BAD_REQUEST;
                    res.Message = ResponseMessageIdentity.DUPLICATED_EMAIL;
                    res.StatusCode = StatusCodes.Status400BadRequest;
                    return res;
                }

                var user = activeUsers.First();

                if (user.OtpExpiredTime < TimeHepler.SystemTimeNow)
                {
                    res.IsSuccess = false;
                    res.ResponseCode = ResponseCodeConstants.BAD_REQUEST;
                    res.Message = ResponseMessageIdentity.OTP_EXPIRED;
                    res.StatusCode = StatusCodes.Status400BadRequest;
                    return res;
                }

                if (!_accountHelper.SecureStringCompare(user.OtpCode, otp))
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

        public async Task<ResultModel> UpdateCustomer(UpdateCustomerRequest updateCustomerRequest)
        {
            try
            {
                var existingUser = await _accountRepository.GetAccountById(updateCustomerRequest.AccountId);
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

                var existingEmailUsers = await _accountRepository.GetAccountsByEmail(updateCustomerRequest.Email);
                if (existingEmailUsers != null)
                {
                    foreach (var user in existingEmailUsers)
                    {
                        if (user.AccountId != existingUser.AccountId && user.Status == AccountStatusEnums.Active.ToString())
                        {
                            return new ResultModel
                            {
                                IsSuccess = false,
                                ResponseCode = ResponseCodeConstants.EXISTED,
                                Message = ResponseMessageIdentity.EMAIL_IN_USE,
                                StatusCode = StatusCodes.Status400BadRequest
                            };
                        }
                    }
                }

                // Only update if the field is not null
                if (updateCustomerRequest.Name != null)
                    existingUser.Name = updateCustomerRequest.Name;
                if (updateCustomerRequest.Phone != null)
                    existingUser.Phone = updateCustomerRequest.Phone;
                if (updateCustomerRequest.Address != null)
                    existingUser.Address = updateCustomerRequest.Address;
                if (updateCustomerRequest.Email != null)
                    existingUser.Email = updateCustomerRequest.Email;
                if (updateCustomerRequest.Avatar != null)
                    existingUser.Avatar = updateCustomerRequest.Avatar;

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

        public async Task<ResultModel> UpdateCurrentProfile(UpdateProfileRequest updateProfileRequest)
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
                var existingEmailUsers = await _accountRepository.GetAccountsByEmail(updateProfileRequest.Email);
                if (existingEmailUsers != null)
                {
                    foreach (var user in existingEmailUsers)
                    {
                        if (user.AccountId != existingAccount.AccountId && user.Status == AccountStatusEnums.Active.ToString())
                        {
                            return new ResultModel
                            {
                                IsSuccess = false,
                                ResponseCode = ResponseCodeConstants.EXISTED,
                                Message = ResponseMessageIdentity.EMAIL_IN_USE,
                                StatusCode = StatusCodes.Status400BadRequest
                            };
                        }
                    }
                }
                // Only update if the field is not null
                if (updateProfileRequest.Name != null)
                    existingAccount.Name = updateProfileRequest.Name;
                if (updateProfileRequest.Phone != null)
                    existingAccount.Phone = updateProfileRequest.Phone;
                if (updateProfileRequest.Address != null)
                    existingAccount.Address = updateProfileRequest.Address;
                if (updateProfileRequest.Email != null)
                    existingAccount.Email = updateProfileRequest.Email;
                if (updateProfileRequest.Avatar != null)
                    existingAccount.Avatar = updateProfileRequest.Avatar;

                existingAccount.UpdateDate = TimeHepler.SystemTimeNow;
                await _accountRepository.UpdateAccount(existingAccount);
                return new ResultModel
                {
                    IsSuccess = true,
                    ResponseCode = ResponseCodeConstants.SUCCESS,
                    Message = ResponseMessageConstantsUser.UPDATE_USER_SUCCESS,
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

        public async Task<ResultModel> CreateAdmin(RegisterRequest registerRequest)
        {
            try
            {
                var usersByUsername = await _accountRepository.GetAccountsByUserName(registerRequest.Username);
                if (usersByUsername.Any(u => u.Status == AccountStatusEnums.Active.ToString()))
                {
                    throw new AppException(
                        ResponseCodeConstants.EXISTED,
                        ResponseMessageIdentity.EXISTED_USERNAME,
                        StatusCodes.Status400BadRequest
                    );
                }

                var usersByEmail = await _accountRepository.GetAccountsByEmail(registerRequest.Email);
                if (usersByEmail.Any(u => u.Status == AccountStatusEnums.Active.ToString()))
                {
                    throw new AppException(
                        ResponseCodeConstants.EXISTED,
                        ResponseMessageIdentity.EMAIL_IN_USE,
                        StatusCodes.Status400BadRequest
                    );
                }

                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(registerRequest.Password);

                var newUser = new Account
                {
                    AccountId = _accountHelper.GenerateShortGuid(),
                    Username = registerRequest.Username,
                    Password = hashedPassword,
                    Name = registerRequest.Name,
                    Phone = registerRequest.Phone,
                    Address = registerRequest.Address,
                    Email = registerRequest.Email,
                    Role = RoleEnums.Admin.ToString(),
                    Status = AccountStatusEnums.Active.ToString(),
                    StartDate = TimeHepler.SystemTimeNow,
                    UpdateDate = TimeHepler.SystemTimeNow,
                    Avatar = registerRequest.Avatar,
                };

                await _accountRepository.AddAccount(newUser);
                var createdUser =
                await _accountRepository.GetAccountById(newUser.AccountId);
                return new ResultModel
                {
                    IsSuccess = true,
                    ResponseCode = ResponseCodeConstants.SUCCESS,
                    Message = ResponseMessageConstantsUser.CREATE_ADMIN_SUCCESS,
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

        public async Task<ResultModel> UpdateStatus(UpdateStatusRequest updateStatusRequest)
        {
            try
            {
                var existingUser = await _accountRepository.GetAccountById(updateStatusRequest.AccountId);
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
                if (existingUser.Role == RoleEnums.Admin.ToString())
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.BAD_REQUEST,
                        Message = ResponseMessageConstantsUser.CANNOT_CHANGE_ADMIN_STATUS,
                        StatusCode = StatusCodes.Status400BadRequest
                    };
                }

                if (existingUser.Status == updateStatusRequest.Status.ToString())
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.BAD_REQUEST,
                        Message = ResponseMessageConstantsUser.STATUS_NOT_CHANGED,
                        StatusCode = StatusCodes.Status400BadRequest
                    };
                }

                if (!Enum.IsDefined(typeof(AccountStatusEnums), updateStatusRequest.Status))
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.BAD_REQUEST,
                        Message = ResponseMessageConstantsUser.INVALID_STATUS,
                        StatusCode = StatusCodes.Status400BadRequest
                    };
                }

                if (existingUser.Status == AccountStatusEnums.Inactive.ToString()
                    && updateStatusRequest.Status == AccountStatusEnums.Active)
                {
                    var usersByUsername = await _accountRepository.GetAccountsByUserName(existingUser.Username);
                    if (usersByUsername.Any(u => u.Status == AccountStatusEnums.Active.ToString()
                                                 && u.AccountId != existingUser.AccountId))
                    {
                        return new ResultModel
                        {
                            IsSuccess = false,
                            ResponseCode = ResponseCodeConstants.EXISTED,
                            Message = ResponseMessageIdentity.EXISTED_USERNAME,
                            StatusCode = StatusCodes.Status400BadRequest
                        };
                    }

                    var usersByEmail = await _accountRepository.GetAccountsByEmail(existingUser.Email);
                    if (usersByEmail.Any(u => u.Status == AccountStatusEnums.Active.ToString()
                                              && u.AccountId != existingUser.AccountId))
                    {
                        return new ResultModel
                        {
                            IsSuccess = false,
                            ResponseCode = ResponseCodeConstants.EXISTED,
                            Message = ResponseMessageIdentity.EMAIL_IN_USE,
                            StatusCode = StatusCodes.Status400BadRequest
                        };
                    }
                }

                existingUser.Status = updateStatusRequest.Status.ToString();
                existingUser.UpdateDate = TimeHepler.SystemTimeNow;
                await _accountRepository.UpdateAccount(existingUser);

                return new ResultModel
                {
                    IsSuccess = true,
                    ResponseCode = ResponseCodeConstants.SUCCESS,
                    Message = ResponseMessageConstantsUser.UPDATE_STATUS_SUCCESS,
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

        public async Task<ResultModel> GetAllCustomerHasPackage()
        {
            try
            {
                var customers = await _accountRepository.GetAllCustomerHasPackage();
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

        public async Task<ResultModel> GetCustomersStatus()
        {
            try
            {
                var customerStatuses = await _accountRepository.GetCustomersStatus();
                if (customerStatuses == null)
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
                    Data = customerStatuses,
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
        public async Task<ResultModel> GetCustomerByAccountId(string accountId)
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
                return new ResultModel
                {
                    IsSuccess = true,
                    ResponseCode = ResponseCodeConstants.SUCCESS,
                    Message = ResponseMessageConstantsUser.GET_USER_INFO_SUCCESS,
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
                    Message = ResponseCodeConstants.BAD_REQUEST,
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }

        public async Task<string> RegisterGoogleUser(string name, string email, string avatar)
        {
            var existingUser = await _accountRepository.GetAccountByEmail(email);
            if (existingUser == null)
            {
                var newUser = new Account
                {
                    AccountId = _accountHelper.GenerateShortGuid(),
                    Username = name,
                    Email = email,
                    Role = RoleEnums.EvDriver.ToString(),
                    Status = AccountStatusEnums.Active.ToString(),
                    Name = name,
                    Avatar = avatar,
                    StartDate = TimeHepler.SystemTimeNow,
                    UpdateDate = TimeHepler.SystemTimeNow,
                };

                await _accountRepository.AddAccount(newUser);
                existingUser = newUser;

                var evdriver = new Evdriver
                {
                    CustomerId = _accountHelper.GenerateShortGuid(),
                    AccountId = newUser.AccountId,
                    StartDate = TimeHepler.SystemTimeNow,
                    UpdateDate = TimeHepler.SystemTimeNow,
                    Status = AccountStatusEnums.Active.ToString()
                };
                await _evDriverRepository.AddDriver(evdriver);
            }
            return _accountHelper.CreateToken(existingUser);
        }
    }
}


