using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Amazon.EmailService.Contract;
using Amazon.EmailService.Infrastructure;
using Amazon.EmailService.Infrastructure.Appsettings;
using Amazon.EmailService.Models;
using Amazon.EmailService.Services;
using Amazon.Framework.Files;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;

namespace Amazon.EmailService.Tests.ServiceTests
{
    public class EmailServiceTest
    {
        private Mock<ILogger<Amazon.EmailService.Services.EmailService>> _loggerMock;
        private Mock<IViewRenderService> _viewRenderServiceMock;
        private Mock<IAWSEmailService> _awsEmailServiceMock;
        private Mock<IFileServices> _fileServicesMock;
        private IOptions<CommonSettings> _commonSettingsOptions;

        private IEmailService _emailService;

        [SetUp]
        public void Setup()
        {
            _loggerMock = new Mock<ILogger<Amazon.EmailService.Services.EmailService>>();
            _viewRenderServiceMock = new Mock<IViewRenderService>();
            _awsEmailServiceMock = new Mock<IAWSEmailService>();
            _fileServicesMock = new Mock<IFileServices>();
            _commonSettingsOptions = Options.Create(new CommonSettings
            {
                BucketName = "bucketName",
            });

            _emailService = new Amazon.EmailService.Services.EmailService(_awsEmailServiceMock.Object,
                                                                                  _fileServicesMock.Object,
                                                                                  _viewRenderServiceMock.Object,
                                                                                  _commonSettingsOptions,
                                                                                  _loggerMock.Object);

            var key = "key";
            string renderBodyResult = "<div>Welcome test to </div>";

            var path = $"EmailTemplates\\UserCreated.cshtml";
            byte[] bytes = System.IO.File.ReadAllBytes(path);

            var stream = new MemoryStream(bytes);
            _fileServicesMock.Setup(x => x.StoreFileAsync(
                                                "bucketName",
                                                "UserCreated.cshtml"
                                                )).ReturnsAsync(key);

            _fileServicesMock.Setup(x => x.StreamFileAsync(
                                                "UserCreated.cshtml",
                                                "bucketName"
                                                )).ReturnsAsync(stream);

            _viewRenderServiceMock.Setup(x => x.RenderToStringAsync(stream, new Dictionary<string, string>()
                                                        {
                                                            {
                                                                "UserName", "test"
                                                            }
                                                        })).ReturnsAsync(renderBodyResult);
        }

        [TestCaseSource(typeof(EmailServiceTest), nameof(SingleEmailTemplateWithNoAttachment))]
        public void SendEmailNoAttachment_Success(EmailTemplateContract input)
        {
            _awsEmailServiceMock.Setup(x => x.SendEmailAsync(It.IsAny<IEnumerable<string>>(),
                                                             It.IsAny<string>(),
                                                             It.IsAny<string>(),
                                                             It.IsAny<bool>(),
                                                             null,
                                                             null)).ReturnsAsync(HttpStatusCode.OK);

            var httpStatusCode = _emailService.SendEmailAsync(input);

            Assert.AreEqual(httpStatusCode.Result, HttpStatusCode.OK);
        }

        [TestCaseSource(typeof(EmailServiceTest), nameof(SingleEmailTemplateWithNoAttachment))]
        public void SendEmailNoAttachment_Failure(EmailTemplateContract input)
        {
            _awsEmailServiceMock.Setup(x => x.SendEmailAsync(It.IsAny<IEnumerable<string>>(),
                                                             It.IsAny<string>(),
                                                             It.IsAny<string>(),
                                                             It.IsAny<bool>(),
                                                             null,
                                                             null)).ReturnsAsync(HttpStatusCode.BadRequest);

            var httpStatusCode = _emailService.SendEmailAsync(input);

            Assert.AreEqual(httpStatusCode.Result, HttpStatusCode.BadRequest);
        }

        [TestCaseSource(typeof(EmailServiceTest), nameof(SingleEmailTemplateWithSingleAttachment))]
        public void SendEmailSingleAttachment_Success(EmailTemplateContract input)
        {
            _awsEmailServiceMock.Setup(x => x.SendEmailWithAttachmentStreamsAsync(It.IsAny<IEnumerable<string>>(),
                                                                                It.IsAny<string>(),
                                                                                It.IsAny<string>(),
                                                                                true,
                                                                                It.IsAny<IEnumerable<Attachment>>(),
                                                                                null,
                                                                                null)).ReturnsAsync(HttpStatusCode.OK);
            var httpStatusCode = _emailService.SendEmailAsync(input);
            Assert.AreEqual(httpStatusCode.Result, HttpStatusCode.OK);
        }

        [TestCaseSource(typeof(EmailServiceTest), nameof(SingleEmailTemplateWithSingleAttachment))]
        public void SendEmailSingleAttachment_Failure(EmailTemplateContract input)
        {
            _awsEmailServiceMock.Setup(x => x.SendEmailWithAttachmentStreamsAsync(It.IsAny<IEnumerable<string>>(),
                                                                                It.IsAny<string>(),
                                                                                It.IsAny<string>(),
                                                                                true,
                                                                                It.IsAny<IEnumerable<Attachment>>(),
                                                                                null,
                                                                                null)).ReturnsAsync(HttpStatusCode.BadRequest);


            var httpStatusCode = _emailService.SendEmailAsync(input);

            Assert.AreEqual(httpStatusCode.Result, HttpStatusCode.BadRequest);
        }

        [TestCaseSource(typeof(EmailServiceTest), nameof(SingleEmailTemplateWithMultipleAttachment))]
        public void SendEmailMultipleAttachment_Success(EmailTemplateContract input)
        {
            _awsEmailServiceMock.Setup(x => x.SendEmailWithAttachmentStreamsAsync(It.IsAny<IEnumerable<string>>(),
                                                                                     It.IsAny<string>(),
                                                                                     It.IsAny<string>(),
                                                                                     true,
                                                                                     It.IsAny<IEnumerable<Attachment>>(),
                                                                                     null,
                                                                                     null)).ReturnsAsync(HttpStatusCode.OK);
            var httpStatusCode = _emailService.SendEmailAsync(input);

            Assert.AreEqual(httpStatusCode.Result, HttpStatusCode.OK);
        }

        [TestCaseSource(typeof(EmailServiceTest), nameof(SingleEmailTemplateWithMultipleAttachment))]
        public void SendEmailMultipleAttachment_Failure(EmailTemplateContract input)
        {
            _awsEmailServiceMock.Setup(x => x.SendEmailWithAttachmentStreamsAsync(It.IsAny<IEnumerable<string>>(),
                                                                                     It.IsAny<string>(),
                                                                                     It.IsAny<string>(),
                                                                                     true,
                                                                                     It.IsAny<IEnumerable<Attachment>>(),
                                                                                     null,
                                                                                     null)).ReturnsAsync(HttpStatusCode.BadRequest);
            var httpStatusCode = _emailService.SendEmailAsync(input);

            Assert.AreEqual(httpStatusCode.Result, HttpStatusCode.BadRequest);
        }

        [TestCaseSource(typeof(EmailServiceTest), nameof(MutipleEmailTemplateWithNoAttachment))]
        public void SendMutiEmailNoAttachment_Success(IEnumerable<EmailTemplateContract> input)
        {
            _awsEmailServiceMock.Setup(x => x.SendEmailAsync(It.IsAny<IEnumerable<string>>(),
                                                             It.IsAny<string>(),
                                                             It.IsAny<string>(),
                                                             It.IsAny<bool>(),
                                                             null,
                                                             null)).ReturnsAsync(HttpStatusCode.OK);
            var httpStatusCode = _emailService.SendEmailsAsync(input);

            Assert.AreEqual(httpStatusCode.Result, HttpStatusCode.OK);
        }        

        [TestCaseSource(typeof(EmailServiceTest), nameof(MutipleEmailTemplateWithMultipleAttachment))]
        public void SendMutilEmailMultiAttachment_Success(IEnumerable<EmailTemplateContract> input)
        {
            _awsEmailServiceMock.Setup(x => x.SendEmailWithAttachmentStreamsAsync(It.IsAny<IEnumerable<string>>(),
                                                                                    It.IsAny<string>(),
                                                                                    It.IsAny<string>(),
                                                                                    true,
                                                                                    It.IsAny<IEnumerable<Attachment>>(),
                                                                                    null,
                                                                                    null)).ReturnsAsync(HttpStatusCode.OK);
            var httpStatusCode = _emailService.SendEmailsAsync(input);

            Assert.AreEqual(httpStatusCode.Result, HttpStatusCode.OK);
        }

        [TestCaseSource(typeof(EmailServiceTest), nameof(EmailTemplate_Failure))]
        public void SendEmail_Failure(EmailTemplateContract input)
        {
            var httpStatusCode = _emailService.SendEmailAsync(input);

            Assert.AreEqual(httpStatusCode.Result, HttpStatusCode.BadRequest);
        }

        [TestCaseSource(typeof(EmailServiceTest), nameof(EmailTemplate_Failure))]
        public void SendEmails_Failure(IEnumerable<EmailTemplateContract> input)
        {
            var httpStatusCode = _emailService.SendEmailsAsync(input);

            Assert.AreEqual(httpStatusCode.Result, HttpStatusCode.BadRequest);
        }

        #region Data for send email
        public static IEnumerable SingleEmailTemplateWithNoAttachment
        {
            get
            {
                yield return new TestCaseData(new EmailTemplateContract()
                {
                    Recipients = new List<string>()
                    {
                        "test1@gmail.com",
                        "test2@gmail.com"
                    },
                    Subject = "test",
                    EmailBodyData = new Dictionary<string, string>()
                    {
                        {
                            "UserName", "test"
                        }
                    },
                    EmailTemplateCodes = "UserCreated.cshtml"
                });
            }
        }

        public static IEnumerable<TestCaseData> MutiEmailTemplateWithNoAttachment
        {
            get
            {
                yield return new TestCaseData(new List<EmailTemplateContract>()
                {
                    new EmailTemplateContract()
                    {
                        Recipients = new List<string>()
                        {
                            "test1@gmail.com",
                            "test2@gmail.com"
                        },
                        Subject = "test",
                        EmailBodyData = new Dictionary<string, string>()
                        {
                            {
                                "UserName", "test"
                            }
                        },
                        EmailTemplateCodes = "UserCreated.cshtml"
                    }
                });
            }
        }

        public static IEnumerable SingleEmailTemplateWithSingleAttachment
        {
            get
            {
                yield return new TestCaseData(new EmailTemplateContract()
                {
                    Recipients = new List<string>()
                    {
                        "test1@gmail.com",
                        "test2@gmail.com"
                    },
                    Subject = "test",
                    EmailBodyData = new Dictionary<string, string>()
                    {
                        {
                            "UserName", "test"
                        }
                    },
                    EmailTemplateCodes = "UserCreated.cshtml",
                    AttachmentFileNames = new List<string>()
                    {
                        "UserCreated.cshtml"
                    }
                });
            }
        }

        public static IEnumerable<TestCaseData> MutipleEmailTemplateWithNoAttachment
        {
            get
            {
                yield return new TestCaseData(new List<EmailTemplateContract>()
                {
                    new EmailTemplateContract()
                    {
                        Recipients = new List<string>()
                        {
                            "test1@gmail.com",
                            "test2@gmail.com"
                        },
                        Subject = "test",
                        EmailBodyData = new Dictionary<string, string>()
                        {
                            {
                                "UserName", "test"
                            }
                        },
                        EmailTemplateCodes = "UserCreated.cshtml",
                        AttachmentFileNames = new List<string>()
                        {
                            "UserCreated.cshtml"
                        }
                    }
                });
            }
        }

        public static IEnumerable SingleEmailTemplateWithMultipleAttachment
        {
            get
            {
                yield return new TestCaseData(new EmailTemplateContract()
                {
                    Recipients = new List<string>()
                    {
                        "test1@gmail.com",
                        "test2@gmail.com"
                    },
                    Subject = "test",
                    EmailBodyData = new Dictionary<string, string>()
                    {
                        {
                            "UserName", "test"
                        }
                    },
                    EmailTemplateCodes = "UserCreated.cshtml",
                    AttachmentFileNames = new List<string>()
                    {
                        "UserCreated.cshtml",
                        "UserCreated.cshtml"
                    }
                });
            }
        }

        public static IEnumerable<TestCaseData> MutipleEmailTemplateWithMultipleAttachment
        {
            get
            {
                yield return new TestCaseData(new List<EmailTemplateContract>()
                {
                    new EmailTemplateContract()
                    {
                        Recipients = new List<string>()
                        {
                            "test1@gmail.com",
                            "test2@gmail.com"
                        },
                        Subject = "test",
                        EmailBodyData = new Dictionary<string, string>()
                        {
                            {
                                "UserName", "test"
                            }
                        },
                        EmailTemplateCodes = "UserCreated.cshtml",
                        AttachmentFileNames = new List<string>()
                        {
                            "email01_attachment",
                            "email02_attachment"
                        }
                    }
                });
            }
        }

        public static IEnumerable<TestCaseData> EmailTemplate_Failure
        {
            get
            {
                yield return new TestCaseData(null);
            }
        }
        #endregion
    }
}
