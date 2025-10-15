using BusinessObjects.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repositories.Repositories.AccountRepo;
using Services.ApiModels.Account;
using Services.Services.AccountService;

namespace Services.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                var (accessToken, refreshToken) = await _accountService.Login(request.Username, request.Password);
                return Ok(new { AccessToken = accessToken, RefreshToken = refreshToken });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.InnerException?.Message ?? ex.Message });
            }
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var res = await _accountService.Logout();
            return StatusCode(res.StatusCode, res);
        }
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest forgotPasswordRequest)
        {
            var res = await _accountService.ForgotPassword(forgotPasswordRequest.Email);
            return Ok(res);
        }

        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp(string email, [FromQuery] string otp)
        {
            var res = await _accountService.ForgotPasswordVerifyOtp(email, otp);
            return StatusCode(res.StatusCode, res);
        }

        [Authorize]
        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            var res = await _accountService.ChangePassword(request);
            return StatusCode(res.StatusCode, res);
        }

        [HttpGet("get-currrent")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUser()
        {
            var res = await _accountService.GetCurrentUser();
            return StatusCode(res.StatusCode, res);
        }


        [HttpPost("create_admin")]
        public async Task<IActionResult> CreateAdmin([FromForm] RegisterRequest registerRequest)
        {
            var res = await _accountService.CreateAdmin(registerRequest);
            return StatusCode(res.StatusCode, res);
        }

        [HttpPut("update_status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateStatus([FromForm] UpdateStatusRequest updateStatusRequest)
        {
            var res = await _accountService.UpdateStatus(updateStatusRequest);
            return StatusCode(res.StatusCode, res);
        }

        [HttpPost("create_staff_for_admin")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateStaffForAdmin([FromForm] RegisterRequest registerRequest)
        {
            var res = await _accountService.CreateStaff(registerRequest);
            return StatusCode(res.StatusCode, res);
        }

        [HttpPut("update_staff_for_admin")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateStaffForAdmin([FromForm] UpdateStaffRequest updateStaffByAdminRequest)
        {
            var res = await _accountService.UpdateStaff(updateStaffByAdminRequest);
            return StatusCode(res.StatusCode, res);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm] RegisterRequest registerRequest)
        {
            try
            {
                var accessToken = await _accountService.Register(registerRequest);
                return Ok(new { accessToken });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.InnerException?.Message ?? ex.Message });
            }
        }

        [HttpGet("get_all_account_for_admin")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllAccount()
        {
            var res = await _accountService.GetAllAccounts();
            return StatusCode(res.StatusCode, res);
        }

        [HttpGet("get_account_by_id/{accountId}_for_admin")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAccountById(string? accountId)
        {
            var res = await _accountService.GetAccountById(accountId);
            return StatusCode(res.StatusCode, res);
        }

        [HttpGet("get_all_staff_for_admin")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllStaff()
        {
            var res = await _accountService.GetAllStaff();
            return StatusCode(res.StatusCode, res);

        }

        [HttpGet("get_all_customer_for_admin")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllCustomer()
        {
            var res = await _accountService.GetAllCustomer();
            return StatusCode(res.StatusCode, res);
        }
        [HttpPut("delete_staff_for_admin/{accountId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteStaff(string? accountId)
        {
            var res = await _accountService.DeleteStaff(accountId);
            return StatusCode(res.StatusCode, res);
        }
        [HttpPut("delete_customer_for_admin/{accountId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCustomer(string? accountId)
        {
            var res = await _accountService.DeleteCustomer(accountId);
            return StatusCode(res.StatusCode, res);
        }
        [HttpPut("update_customer_for_admin")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateCustomerForAdmin([FromForm] UpdateCustomerRequest updateCustomerRequest)
        {
            var res = await _accountService.UpdateCustomer(updateCustomerRequest);
            return StatusCode(res.StatusCode, res);
        }

        [HttpPut("update_current_profile")]
        [Authorize]
        public async Task<IActionResult> UpdateCurrentProfile([FromForm] UpdateProfileRequest updateProfileRequest)
        {
            var res = await _accountService.UpdateCurrentProfile(updateProfileRequest);
            return StatusCode(res.StatusCode, res);
        }
        [HttpGet("get_customer_has_package")]
        [Authorize(Roles = "Admin, Bsstaff")]
        public async Task<IActionResult> GetCustomerHasPackage()
        {
            var res = await _accountService.GetAllCustomerHasPackage();
            return StatusCode(res.StatusCode, res);
        }

        [HttpGet("get_customers_status")]
        [Authorize]
        public async Task<IActionResult> GetCustomersStatus()
        {
            var res = await _accountService.GetCustomersStatus();
            return StatusCode(res.StatusCode, res);
        }
        [HttpGet("get_account_by_customer_id_for_staff")]
        [Authorize(Roles = "Admin, Staff")]
        public async Task<IActionResult> GetAccountByCustomerId([FromQuery] string? customerId)
        {
            var res = await _accountService.GetAccountByCustomerId(customerId);
            return StatusCode(res.StatusCode, res);
        }
    }
}
