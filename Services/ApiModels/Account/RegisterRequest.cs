using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ApiModels.Account
{
    public class RegisterRequest
    {
        [Required]
        public string Username { get; set; }
        
        [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Mật khẩu phải có ít nhất 3 ký tự")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Mật khẩu xác nhận là bắt buộc")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Mật khẩu xác nhận không khớp")]
        public string ConfirmedPassword { get; set; }
        
        [Required]
        [RegularExpression(@"^[\p{L} ]+$", ErrorMessage = "Tên không được chứa số và các ký tự đặc biệt")]
        public string Name { get; set; }
        
        [Required]
        public string Phone { get; set; }
        
        [Required]
        public string Address { get; set; }
        public string? Avatar { get; set; }

        [Required]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; }
    }
}
