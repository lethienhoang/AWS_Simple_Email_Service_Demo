using System.Collections.Generic;

namespace Amazon.EmailService.Models
{
    public class Email
    {     
        public string EmailTemplateCode { get; set; }

        public List<string> Recipients { get; set; }

        public List<string> Cc { get; set; }

        public List<string> Bcc { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }
    }

    
}
