using BusinessObjects.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ApiModels.Form
{
    public class UpdateFormStatusStaffRequest
    {
        [Required]
        public string FormId { get; set; }
        public StaffUpdateFormEnums Status { get; set; }
    }
}
