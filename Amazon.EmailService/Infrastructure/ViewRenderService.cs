using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Amazon.EmailService.Infrastructure
{
    public class ViewRenderService : IViewRenderService
    {
        public ViewRenderService()
        {
        }

        public async Task<string> RenderToStringAsync(Stream fileStream,  Dictionary<string, string> properties)
        {
            using (StreamReader reader = new StreamReader(fileStream))
            {
                string line = await reader.ReadToEndAsync();
                foreach (var property in properties)
                {
                    var name = property.Key;
                    var value = property.Value;
                    line = line.Replace($@"[{name}]", value);
                }

                return line;
            }
        }
    }
}
