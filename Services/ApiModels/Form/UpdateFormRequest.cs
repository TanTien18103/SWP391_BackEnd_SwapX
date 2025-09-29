using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ApiModels.Form
{
    public class UpdateFormRequest
    {
        public string FormId { get; set; }  
        public string AccountId { get; set; }

        [RegularExpression(@"^.{3,100}$", ErrorMessage = "Title phải từ 3 đến 100 ký tự")]
        public string Title { get; set; }

        [RegularExpression(@"^.{10,500}$", ErrorMessage = "Description phải từ 10 đến 500 ký tự")]
        public string Description { get; set; }

        public DateTime? Date { get; set; }

        public string StationId { get; set; }
    }
}
