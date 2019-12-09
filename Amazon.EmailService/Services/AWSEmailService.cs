using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Amazon.SimpleEmailV2;
using Amazon.SimpleEmailV2.Model;
using Amazon.EmailService.Infrastructure.Appsettings;
using Amazon.EmailService.Models;
using Amazon.Framework.Utils.Helpers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Amazon.EmailService.Services
{
    public class AWSEmailService : IAWSEmailService
    {
        private readonly IAmazonSimpleEmailServiceV2 _amazonSimpleEmailService;
        private readonly ILogger<AWSEmailService> _logger;
        private readonly AWSEmailServiceOptions _emailOptions;

        public AWSEmailService(IAmazonSimpleEmailServiceV2 amazonSimpleEmailService, ILogger<AWSEmailService> logger, IOptions<AWSEmailServiceOptions> emailOptions)
        {
            _amazonSimpleEmailService = amazonSimpleEmailService;
            _emailOptions = emailOptions.Value;
            _logger = logger;
        }

        public Task<HttpStatusCode> SendEmailAsync(IEnumerable<string> to,
                                                   string subject,
                                                   string body,
                                                   bool isHtmlBody,
                                                   IEnumerable<string> cc = null,
                                                   IEnumerable<string> bcc = null)
        {
            var emailTemplate = BuildEmailTemplate(to, subject, cc, bcc, body, isHtmlBody);

            return SendEmailAsync(emailTemplate);
        }

        public Task<HttpStatusCode> SendEmailWithAttachmentPathAsync(IEnumerable<string> to,
                                                                     string subject,
                                                                     string body,
                                                                     bool isHtmlBody,
                                                                     string fileAttachmentPath,
                                                                     IEnumerable<string> cc = null,
                                                                     IEnumerable<string> bcc = null)
        {
            var emailTemplate = BuildEmailTemplateWithAttachment(to,
                                                                  subject,
                                                                  cc,
                                                                  bcc,
                                                                  body,
                                                                  isHtmlBody,
                                                                  fileAttachmentPath);
            return SendEmailAsync(emailTemplate);
        }

        public Task<HttpStatusCode> SendEmailWithAttachmentStreamAsync(IEnumerable<string> to,
                                                                       string subject,
                                                                       string body,
                                                                       bool isHtmlBody,
                                                                       string fileName,
                                                                       Stream fileAttachmentStream,
                                                                       IEnumerable<string> cc = null,
                                                                       IEnumerable<string> bcc = null)
        {
            var emailTemplate = BuildEmailTemplateWithAttachment(to,
                                                                  subject,
                                                                  cc,
                                                                  bcc,
                                                                  body,
                                                                  isHtmlBody,
                                                                  string.Empty,
                                                                  fileName,
                                                                  fileAttachmentStream);
            return SendEmailAsync(emailTemplate);
        }

        public Task<HttpStatusCode> SendEmailWithAttachmentPathsAsync(IEnumerable<string> to,
                                                                         string subject,
                                                                         string body,
                                                                         bool isHtmlBody,
                                                                         IEnumerable<string> fileAttachmentPaths,
                                                                         IEnumerable<string> cc = null,
                                                                         IEnumerable<string> bcc = null)
        {
            var emailTemplate = BuildEmailTemplateWithAttachments(to,
                                                                  subject,
                                                                  cc,
                                                                  bcc,
                                                                  body,
                                                                  isHtmlBody,
                                                                  fileAttachmentPaths,
                                                                  null);
            return SendEmailAsync(emailTemplate);
        }

        public Task<HttpStatusCode> SendEmailWithAttachmentStreamsAsync(IEnumerable<string> to,
                                                                           string subject,
                                                                           string body,
                                                                           bool isHtmlBody,
                                                                           IEnumerable<Attachment> attachments,
                                                                           IEnumerable<string> cc = null,
                                                                           IEnumerable<string> bcc = null)
        {
            var emailTemplate = BuildEmailTemplateWithAttachments(to,
                                                                 subject,
                                                                 cc,
                                                                 bcc,
                                                                 body,
                                                                 isHtmlBody,
                                                                 null,
                                                                 attachments);
            return SendEmailAsync(emailTemplate);
        }

        #region Helpers
        private MimeMessage BuildEmailTemplate(IEnumerable<string> to,
                                               string subject,
                                               IEnumerable<string> cc,
                                               IEnumerable<string> bcc,
                                               string body,
                                               bool isHtmlBody)
        {
            var emailMessage = BuildEmailHeaders(to, cc, bcc, subject);
            var emailBody = BuildEmailBody(body, isHtmlBody);

            emailMessage.Body = emailBody.ToMessageBody();

            return emailMessage;
        }

        private MimeMessage BuildEmailTemplateWithAttachment(IEnumerable<string> to,
                                                              string subject,
                                                              IEnumerable<string> cc,
                                                              IEnumerable<string> bcc,
                                                              string body,
                                                              bool isHtmlBody,
                                                              string fileAttachmentPath = "",
                                                              string fileName = "",
                                                              Stream fileAttachmentStream = null)
        {
            var emailMessage = BuildEmailHeaders(to, cc, bcc, subject);
            var emailBody = BuildEmailBodyWithAttachment(body,
                                                          isHtmlBody,
                                                          fileAttachmentPath,
                                                          fileName,
                                                          fileAttachmentStream);

            emailMessage.Body = emailBody.ToMessageBody();

            return emailMessage;
        }

        private MimeMessage BuildEmailTemplateWithAttachments(IEnumerable<string> to,
                                                             string subject,
                                                             IEnumerable<string> cc,
                                                             IEnumerable<string> bcc,
                                                             string body,
                                                             bool isHtmlBody,
                                                             IEnumerable<string> fileAttachmentPaths = null,
                                                             IEnumerable<Attachment> attachments = null)
        {
            var emailMessage = BuildEmailHeaders(to, cc, bcc, subject);
            var emailBody = BuildEmailBodyWithAttachments(body,
                                                          isHtmlBody,
                                                          fileAttachmentPaths,
                                                          attachments);

            emailMessage.Body = emailBody.ToMessageBody();

            return emailMessage;
        }

        private BodyBuilder BuildEmailBody(string body, bool isHtmlBody)
        {
            var bodyBuilder = new BodyBuilder();

            if (isHtmlBody)
            {
                bodyBuilder.HtmlBody = body;
            }
            else
            {
                bodyBuilder.TextBody = body;
            }

            return bodyBuilder;
        }

        private BodyBuilder BuildEmailBodyWithAttachment(string body, bool isHtmlBody, string fileAttachmentPath, string fileName, Stream fileAttachmentStream)
        {
            var bodyBuilder = BuildEmailBody(body, isHtmlBody);

            if (fileAttachmentStream != null && !string.IsNullOrEmpty(fileName))
            {
                bodyBuilder.Attachments.Add(fileName, fileAttachmentStream);
            }
            else if (!string.IsNullOrEmpty(fileAttachmentPath))
            {
                bodyBuilder.Attachments.Add(fileAttachmentPath);
            }

            return bodyBuilder;
        }

        private BodyBuilder BuildEmailBodyWithAttachments(string body, bool isHtmlBody, IEnumerable<string> fileAttachmentPaths = null, IEnumerable<Attachment> attachments = null)
        {
            var bodyBuilder = BuildEmailBody(body, isHtmlBody);

            if (attachments != null)
            {
                foreach (var attachment in attachments)
                {
                    if (attachment.FileStream != null && !string.IsNullOrEmpty(attachment.FileName))
                    {
                        bodyBuilder.Attachments.Add(attachment.FileName, attachment.FileStream);
                    }
                };
            }

            if (fileAttachmentPaths != null)
            {
                foreach (var attachmentPath in fileAttachmentPaths)
                {
                    if (!string.IsNullOrEmpty(attachmentPath))
                    {
                        bodyBuilder.Attachments.Add(attachmentPath);
                    }
                }
            }

            return bodyBuilder;
        }

        private MimeMessage BuildEmailHeaders(IEnumerable<string> to, IEnumerable<string> cc, IEnumerable<string> bcc, string subject)
        {
            var message = new MimeMessage();

            message.Subject = subject;
            message.Date = DateTimeHelper.GenerateTodayUTC();
            message.From.Add(new MailboxAddress(_emailOptions.Sender));

            if (to != null && to.Any())
            {
                message.To.AddRange(to.Select(address => new MailboxAddress(address)));
            }

            if (cc != null && cc.Any())
            {
                message.Cc.AddRange(cc.Select(address => new MailboxAddress(address)));
            }

            if (bcc != null && bcc.Any())
            {
                message.Bcc.AddRange(bcc.Select(address => new MailboxAddress(address)));
            }

            return message;
        }

        private async Task<HttpStatusCode> SendEmailAsync(MimeMessage message)
        {
            try
            {
                using (var memoryStream = new MemoryStream())
                {
                    await message.WriteToAsync(memoryStream);
                    memoryStream.Position = 0;

                    var sendRequest = new SendEmailRequest
                    {
                        Destination = new Destination(),
                        FromEmailAddress = _emailOptions.Sender,
                        Content = new EmailContent
                        {
                            Raw = new RawMessage()
                            {
                                Data = memoryStream
                            }
                        }
                    };

                    var response = await _amazonSimpleEmailService.SendEmailAsync(sendRequest);

                    if (response.HttpStatusCode == HttpStatusCode.OK)
                    {
                        _logger.LogInformation($"The email with message Id {response.MessageId} sent successfully to {message.To} on {DateTimeHelper.GenerateTodayUTC()}");
                    }
                    else
                    {
                        _logger.LogError($"Failed to send email with message Id {response.MessageId} to {message.To} on {DateTimeHelper.GenerateTodayUTC()} due to {response.HttpStatusCode}.");
                    }

                    return response.HttpStatusCode;
                }

            }
            catch (Exception e)
            {
                return HttpStatusCode.BadRequest;
            }
        }
        #endregion
    }
}
