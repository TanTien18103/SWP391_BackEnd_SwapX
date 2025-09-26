using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ApiModels
{
    public class ResultModel<T>
    {
        public bool IsSuccess { get; set; }
        public string? ResponseCode { get; set; }
        public int StatusCode { get; set; }
        public T? Data { get; set; }
        public string? Message { get; set; }
    }

    public class ResultModel
    {
        public bool IsSuccess { get; set; }
        public string? ResponseCode { get; set; }
        public int StatusCode { get; set; }
        public object? Data { get; set; }
        public string? Message { get; set; }
    }
}
