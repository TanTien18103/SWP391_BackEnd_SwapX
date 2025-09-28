using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ApiModels.Account
{
    public class UpdateCustomerRequest
    {
        public string AccountId { get; set; }
        [RegularExpression(@"^[\p{L} ]+$", ErrorMessage = "Tên không được chứa số và các ký tự đặc biệt")]
        public string? Name { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Email không hợp lệ")]

        public string? Email { get; set; }
    }
}
