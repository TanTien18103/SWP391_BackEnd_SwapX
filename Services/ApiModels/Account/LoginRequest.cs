using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ApiModels.Account
{
    public class LoginRequest
    {
        [Required(ErrorMessage = "Username là bắt buộc")]
        [EmailAddress(ErrorMessage = "Username không hợp lệ")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
        public string Password { get; set; }
    }
}
