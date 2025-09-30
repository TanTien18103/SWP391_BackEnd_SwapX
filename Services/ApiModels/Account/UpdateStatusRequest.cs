using BusinessObjects.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ApiModels.Account
{
    public class UpdateStatusRequest
    {
        public string AccountId { get; set; }
        public AccountStatusEnums Status { get; set; }
    }
}
