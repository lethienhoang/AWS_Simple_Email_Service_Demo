using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Amazon.EmailService.Contract;
using Amazon.EmailService.Infrastructure;
using Amazon.EmailService.Infrastructure.Appsettings;
using Amazon.EmailService.Models;
using Amazon.Framework.Files;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Amazon.EmailService.Services
{
    public class EmailService : IEmailService
    {
        private readonly IAWSEmailService _awsEmailService;
        private readonly IFileServices _fileServices;
        private readonly CommonSettings _commonSettings;
        private readonly IViewRenderService _viewRenderService;
        private readonly ILogger<EmailService> _logger;
        public EmailService(IAWSEmailService awsEmailService,
                            IFileServices fileServices,
                            IViewRenderService viewRenderService,
                            IOptions<CommonSettings> bulkUserSettings,
                            ILogger<EmailService> logger)
        {
            _awsEmailService = awsEmailService;
            _fileServices = fileServices;
            _viewRenderService = viewRenderService;
            _commonSettings = bulkUserSettings.Value;
            _logger = logger;
        }

        public async Task<HttpStatusCode> SendEmailAsync(EmailTemplateContract emailTemplateContract)
        {
            try
            {
                var body = await GenerateEmailBodyTemplate(emailTemplateContract.EmailTemplateCodes, emailTemplateContract.EmailBodyData);
                var attachments = await GetEmailAttachments(emailTemplateContract.AttachmentFileNames);

                if (attachments.Any())
                {
                    return await _awsEmailService.SendEmailWithAttachmentStreamsAsync(emailTemplateContract.Recipients, emailTemplateContract.Subject, body, true, attachments, emailTemplateContract.Cc, emailTemplateContract.Bcc);
                }

                return await _awsEmailService.SendEmailAsync(emailTemplateContract.Recipients, emailTemplateContract.Subject, body, true, emailTemplateContract.Cc, emailTemplateContract.Bcc);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return HttpStatusCode.BadRequest;
            }
        }

        public async Task<HttpStatusCode> SendEmailsAsync(IEnumerable<EmailTemplateContract> emailTemplateContracts)
        {
            try
            {
                foreach (var email in emailTemplateContracts)
                {
                    var body = await GenerateEmailBodyTemplate(email.EmailTemplateCodes, email.EmailBodyData);
                    var attachments = await GetEmailAttachments(email.AttachmentFileNames);

                    if (attachments.Any())
                    {
                        await _awsEmailService.SendEmailWithAttachmentStreamsAsync(email.Recipients, email.Subject, body, true, attachments, email.Cc, email.Bcc);
                    }
                    else
                    {
                        await _awsEmailService.SendEmailAsync(email.Recipients, email.Subject, body, true, email.Cc, email.Bcc);
                    }
                }

                return HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return HttpStatusCode.BadRequest;
            }
        }

        #region Helpers  
        private async Task<string> GenerateEmailBodyTemplate(string emailTemplateCode, Dictionary<string, string> properties)
        {
            var bodyStream = await GetFileStream(emailTemplateCode);
            return await _viewRenderService.RenderToStringAsync(bodyStream, properties);
        }

        private async Task<Stream> GetFileStream(string fileName)
        {
            return await _fileServices.StreamFileAsync(fileName, _commonSettings.BucketName);
        }

        private async Task<IList<Attachment>> GetEmailAttachments(IEnumerable<string> fileNames)
        {
            var attachments = new List<Attachment>();

            if (!fileNames.Any())
            {
                return attachments;
            }

            foreach (var fileName in fileNames)
            {
                var fileStream = await GetFileStream(fileName);

                attachments.Add(new Attachment
                {
                    FileName = fileName,
                    FileStream = fileStream
                });
            }

            return attachments;
        }
        #endregion
    }
}
