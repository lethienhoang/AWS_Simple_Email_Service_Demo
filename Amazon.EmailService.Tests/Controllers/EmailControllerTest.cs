using System.Collections.Generic;
using System.Net;
using Amazon.EmailService.Contract;
using Amazon.EmailService.Controllers;
using Amazon.EmailService.Services;
using Moq;
using NUnit.Framework;

namespace Amazon.EmailService.Tests.Controllers
{
    public class EmailControllerTest
    {
        private EmailController _emailController;
        private Mock<IClaimService> _claimServiceMock;
        private Mock<IEmailService> _emailServiceMock;

        [SetUp]
        public void Setup()
        {
            _claimServiceMock = new Mock<IClaimService>();
            _emailServiceMock = new Mock<IEmailService>();

            _emailController = new EmailController(_claimServiceMock.Object, _emailServiceMock.Object);
        }

        [Test]
        public void SendSingleEmail_DoesNotThrowException()
        {
            var email = new EmailTemplateContract
            {
                Recipients = new List<string>()
                {
                    "test1@gmail.com"
                },
                EmailBodyData = new Dictionary<string, string>()
                {
                    {
                        "name", "test"
                    }
                },
                EmailTemplateCodes = "emailTemplate01",
                Bcc = new List<string>()
                {
                    "bbc@gmail.com"
                },
                Cc = new List<string>()
                {
                    "cc@gmail.com"
                }
            };

            _emailServiceMock.Setup(x => x.SendEmailAsync(It.IsAny<EmailTemplateContract>())).ReturnsAsync(HttpStatusCode.OK);

            Assert.DoesNotThrowAsync(() => _emailController.SendSingleEmail(email));
        }

        [Test]
        public void SendMultipleEmail_DoesNotThrowException()
        {
            var emails = new List<EmailTemplateContract>()
            {
                new EmailTemplateContract
                {
                    Recipients = new List<string>()
                    {
                        "test1@gmail.com"
                    },
                    EmailBodyData = new Dictionary<string, string>()
                    {
                        {
                            "name", "test"
                        }
                    },
                    EmailTemplateCodes = "emailTemplate01",
                    Bcc = new List<string>()
                    {
                        "bbc@gmail.com"
                    },
                    Cc = new List<string>()
                    {
                        "cc@gmail.com"
                    }
                }
            };

            _emailServiceMock.Setup(x => x.SendEmailsAsync(It.IsAny<List<EmailTemplateContract>>())).ReturnsAsync(HttpStatusCode.OK);

            Assert.DoesNotThrowAsync(() => _emailController.SendMultipleEmail(emails));
        }
    }
}
