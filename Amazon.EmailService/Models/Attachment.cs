using System.IO;

namespace Amazon.EmailService.Models
{
    public class Attachment
    {
        public string FileName { get; set; }

        public Stream FileStream { get; set; }
    }
}
