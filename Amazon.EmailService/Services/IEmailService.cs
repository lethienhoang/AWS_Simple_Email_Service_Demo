using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Amazon.EmailService.Contract;

namespace Amazon.EmailService.Services
{
    public interface IEmailService
    {
        Task<HttpStatusCode> SendEmailAsync(EmailTemplateContract emailTemplateContract);

        Task<HttpStatusCode> SendEmailsAsync(IEnumerable<EmailTemplateContract> emailTemplateContracts);
    }
}
