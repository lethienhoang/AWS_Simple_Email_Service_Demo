using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.EmailService.Contract;
using Amazon.EmailService.Services;
using Amazon.Framework.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Amazon.EmailService.Controllers
{
    public class EmailController : ApiControllerBase
    {
        private readonly IEmailService _emailService;
        public EmailController(IClaimService claimService, IEmailService emailService) : base(claimService)
        {
            _emailService = emailService;
        }

        [HttpPost]
        [Route("single/send")]
        [AllowAnonymous]
        public async Task<IActionResult> SendSingleEmail([FromBody] EmailTemplateContract contract)
        {
            return Ok(await _emailService.SendEmailAsync(contract));
        }

        [HttpPost]
        [Route("multiple/send")]
        [AllowAnonymous]
        public async Task<IActionResult> SendMultipleEmail([FromBody] IEnumerable<EmailTemplateContract> emailTemplateContracts)
        {
            return Ok(await _emailService.SendEmailsAsync(emailTemplateContracts));
        }
    }
}
