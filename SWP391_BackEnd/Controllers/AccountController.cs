using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repositories.Repositories.Account;
using Services.ApiModels.Account;
using Services.Services.Account;

namespace SWP391_BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly IAccountRepo _accountRepo;

        public AccountController(IAccountService accountService, IAccountRepo accountRepo)
        {
            _accountService = accountService;
            _accountRepo = accountRepo;
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

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm] Services.ApiModels.Account.RegisterRequest registerRequest)
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
    }
}
