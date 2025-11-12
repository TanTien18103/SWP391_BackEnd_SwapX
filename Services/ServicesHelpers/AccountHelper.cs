using BusinessObjects.TimeCoreHelper;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Services.ServicesHelpers
{
    public class AccountHelper
    {
        private readonly IConfiguration _configuration;

        public AccountHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public bool VerifyPassword(string inputPassword, string storedPasswordHash)
        {
            return BCrypt.Net.BCrypt.Verify(inputPassword, storedPasswordHash);
        }


        public string CreateToken(BusinessObjects.Models.Account user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.AccountId.ToString()),
            new Claim(ClaimTypes.UserData, user.Username),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: TimeHepler.SystemTimeNow.AddHours(3),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            return Convert.ToBase64String(Guid.NewGuid().ToByteArray());
        }

        public string GenerateShortGuid()
        {
            Guid guid = Guid.NewGuid();
            string base64 = Convert.ToBase64String(guid.ToByteArray());
            return base64.Replace("/", "_").Replace("+", "-").Substring(0, 20);
        }

        private static readonly Random _random = new Random();
        public long GenerateOrderCode()
        {
            lock (_random) 
            {
                int randomNumber = _random.Next(100, 999); 
                string datePart = DateTime.Now.ToString("yyMMddHHmmssfff"); 
                string orderCodeString = $"{datePart}{randomNumber}";

                return long.TryParse(orderCodeString, out long result)
                    ? result
                    : DateTimeOffset.Now.ToUnixTimeMilliseconds();
            }
        }

        public string GenerateSecureOtp()
        {
            using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
            var bytes = new byte[4];
            rng.GetBytes(bytes);
            var randomNumber = BitConverter.ToUInt32(bytes, 0);
            return (randomNumber % 900000 + 100000).ToString();
        }

        public bool SecureStringCompare(string a, string b)
        {
            if (a == null || b == null || a.Length != b.Length)
                return false;

            int result = 0;
            for (int i = 0; i < a.Length; i++)
            {
                result |= a[i] ^ b[i];
            }
            return result == 0;
        }
        public string BuildRedirectUrl(string baseUrl, string? error = null, string? token = null)
        {
            if (!string.IsNullOrEmpty(error))
                return $"{baseUrl}?error={Uri.EscapeDataString(error)}";

            if (!string.IsNullOrEmpty(token))
                return $"{baseUrl}?token={Uri.EscapeDataString(token)}";

            return baseUrl;
        }
    }
}
