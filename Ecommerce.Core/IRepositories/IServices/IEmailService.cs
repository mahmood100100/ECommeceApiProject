using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Core.IRepositories.IServices
{
    public interface IEmailService
    {
        Task SendMailAsync(string ToEmail , string Subject , string Message);
    }
}
