using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ApiModels.Account
{
    public class ForgotPasswordRequest
    {
        [Required(ErrorMessage = "Email không được để trống.")]
        public string Email { get; set; }
    }
}
