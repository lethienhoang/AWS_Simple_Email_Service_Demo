using System;
using System.Collections.Generic;
using System.Text;

namespace Amazon.EmailService.Contract
{
    public class EmailTemplateContract
    {
        public EmailTemplateContract()
        {
            EmailBodyData = new Dictionary<string, string>();
            AttachmentFileNames = new List<string>();
            Recipients = new List<string>();
        }

        public List<string> Recipients { get; set; }

        public List<string> Cc { get; set; }

        public List<string> Bcc { get; set; }

        public string Subject { get; set; }

        public string EmailTemplateCodes { get; set; }

        public Dictionary<string, string> EmailBodyData { get; set; }

        public IEnumerable<string> AttachmentFileNames { get; set; }
    }
}
