using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Amazon.EmailService.Infrastructure
{
    public interface IViewRenderService
    {
        Task<string> RenderToStringAsync(Stream fileStream, Dictionary<string, string> model);
    }
}
