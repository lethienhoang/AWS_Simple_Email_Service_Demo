using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Amazon.EmailService.Models;

namespace Amazon.EmailService.Services
{
    public interface IAWSEmailService
    {
        Task<HttpStatusCode> SendEmailAsync(IEnumerable<string> to, string subject, string body, bool isHtmlBody, IEnumerable<string> cc = null, IEnumerable<string> bcc = null);

        Task<HttpStatusCode> SendEmailWithAttachmentPathAsync(IEnumerable<string> to, string subject, string body, bool isHtmlBody, string fileAttachmentPath, IEnumerable<string> cc = null, IEnumerable<string> bcc = null);

        Task<HttpStatusCode> SendEmailWithAttachmentStreamAsync(IEnumerable<string> to, string subject, string body, bool isHtmlBody, string fileName, Stream fileAttachmentStream, IEnumerable<string> cc = null, IEnumerable<string> bcc = null);

        Task<HttpStatusCode> SendEmailWithAttachmentPathsAsync(IEnumerable<string> to, string subject, string body, bool isHtmlBody, IEnumerable<string> fileAttachmentPaths, IEnumerable<string> cc = null, IEnumerable<string> bcc = null);

        Task<HttpStatusCode> SendEmailWithAttachmentStreamsAsync(IEnumerable<string> to, string subject, string body, bool isHtmlBody, IEnumerable<Attachment> attachments, IEnumerable<string> cc = null, IEnumerable<string> bcc = null);
    }
}
