using System.Collections.Generic;
using System.Net;
using System.Threading;
using Amazon.SimpleEmailV2;
using Amazon.SimpleEmailV2.Model;
using Amazon.EmailService.Infrastructure.Appsettings;
using Amazon.EmailService.Models;
using Amazon.EmailService.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using Moq;
using NUnit.Framework;

namespace Amazon.EmailService.Tests.ServiceTests
{
    public class AWSEmailServiceTest
    {
        private Mock<IAmazonSimpleEmailServiceV2> _amazonSimpleEmailServiceMock;
        private Mock<ILogger<AWSEmailService>> _loggerMock;
        private Mock<MimeMessage> _mimeMessageMock;
        private IOptions<AWSEmailServiceOptions> _emailOptionsMock;

        private AWSEmailService _awsEmailService;

        private IEnumerable<string> to;
        private IEnumerable<string> cc;
        private IEnumerable<string> bcc;
        private IEnumerable<string> attahcmentPaths;
        private IEnumerable<Attachment> attachments;

        [SetUp]
        public void Setup()
        {
            _emailOptionsMock = Options.Create(new AWSEmailServiceOptions
            {
                Sender = "sender@gmail.com"
            });

            _loggerMock = new Mock<ILogger<AWSEmailService>>();
            _amazonSimpleEmailServiceMock = new Mock<IAmazonSimpleEmailServiceV2>();
            _mimeMessageMock = new Mock<MimeMessage>();

            _awsEmailService = new AWSEmailService(_amazonSimpleEmailServiceMock.Object, _loggerMock.Object, _emailOptionsMock);

            InitData();
        }

        private void InitData()
        {
            to = new List<string>()
            {
                "demo@gmail.com"
            };

            cc = new List<string>()
            {
                "cc@gmail.com"
            };

            bcc = new List<string>()
            {
                "bcc@gmail.com"
            };

            attahcmentPaths = new List<string>()
            {
                $"EmailTemplates\\UserCreated.cshtml",
            };

            attachments = new List<Attachment>()
            {
                new Attachment
                {
                    FileName = "file1",
                    FileStream = new System.IO.MemoryStream()
                }
            };

            var response = new SendEmailResponse()
            {
                HttpStatusCode = HttpStatusCode.OK
            };

            _amazonSimpleEmailServiceMock.Setup(x => x.SendEmailAsync(It.IsAny<SendEmailRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(response);
        }

        [Test]
        public void SendEmail_Success()
        {
            var httpStatusCode = _awsEmailService.SendEmailAsync(to, "SendEmail_Success", "SendEmail_Success", true, cc, bcc);

            Assert.AreEqual(httpStatusCode.Result, HttpStatusCode.OK);
        }

        [Test]
        public void SendEmailWithAttachmentPath_Success()
        {
            var httpStatusCode =  _awsEmailService.SendEmailWithAttachmentPathAsync(to, "SendEmail_Success", "SendEmail_Success", true, $"EmailTemplates\\UserCreated.cshtml");

            Assert.AreEqual(httpStatusCode.Result, HttpStatusCode.OK);
        }

        [Test]
        public void SendEmailWithAttachmentStream_Success()
        {
            var httpStatusCode = _awsEmailService.SendEmailWithAttachmentStreamAsync(to, "SendEmail_Success", "SendEmail_Success", true, "file1", new System.IO.MemoryStream());

            Assert.AreEqual(httpStatusCode.Result, HttpStatusCode.OK);
        }

        [Test]
        public void SendEmailWithMutiAttachmentPath_Success()
        {
            var httpStatusCode = _awsEmailService.SendEmailWithAttachmentPathsAsync(to, "SendEmail_Success", "SendEmail_Success", true, attahcmentPaths);

            Assert.AreEqual(httpStatusCode.Result, HttpStatusCode.OK);
        }

        [Test]
        public void SendEmailWithMutiAttachmentStream_Success()
        {
            var httpStatusCode = _awsEmailService.SendEmailWithAttachmentStreamsAsync(to, "SendEmail_Success", "SendEmail_Success", true, attachments);

            Assert.AreEqual(httpStatusCode.Result, HttpStatusCode.OK);
        }
    }
}
