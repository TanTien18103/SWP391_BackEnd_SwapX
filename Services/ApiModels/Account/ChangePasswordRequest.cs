using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ApiModels.Account
{
    public class ChangePasswordRequest
    {
        [Required(ErrorMessage = "Mật khẩu cũ không được để trống")]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "Mật khẩu mới không được để trống")]
        [MinLength(6, ErrorMessage = "Mật khẩu mới phải có ít nhất 6 ký tự")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*\d)(?=.*[@#$%^&+=!]).{6,}$",
            ErrorMessage = "Mật khẩu mới phải chứa ít nhất một chữ cái viết hoa, một số và một ký tự đặc biệt (@#$%^&+=!)")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Mật khẩu xác nhận không được để trống")]
        [Compare("NewPassword", ErrorMessage = "Mật khẩu xác nhận không khớp")]
        public string ConfirmPassword { get; set; }
    }
}
