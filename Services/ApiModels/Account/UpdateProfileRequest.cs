using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ApiModels.Account
{
    public class UpdateProfileRequest
    {
        [RegularExpression(@"^[\p{L} ]+$", ErrorMessage = "Tên không được chứa số và các ký tự đặc biệt")]
        [MinLength(6, ErrorMessage = "Mật khẩu mới phải có ít nhất 6 ký tự")]
        public string? Name { get; set; }

        public string? Phone { get; set; }

        public string? Address { get; set; }

        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Email không hợp lệ")]
        public string? Email { get; set; }
    }
}
