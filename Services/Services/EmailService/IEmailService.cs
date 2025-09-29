using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services.EmailService
{
    public interface IEmailService
    {
        Task SendEmail(string email, string subject, string body);
    }
}
