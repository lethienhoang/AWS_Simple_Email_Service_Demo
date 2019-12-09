using System.IO;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.Framework.Files;

namespace Amazon.EmailService.Tests
{
    public class FileServicesStub : IFileServices
    {
        public Task DeleteFileAsync(string fileName)
        {
            return Task.FromResult<string>("Success");
        }

        public Task<string> StoreFileAsync(string filePath, string fileName)
        {
            return Task.FromResult<string>("Success");
        }

        public Task<string> StoreFileAsync(Stream inputStream, string bucket, string fileName)
        {
            return Task.FromResult<string>("Success");
        }

        public Task<string> StoreFileTemporaryAsync(Stream stream, string fileName)
        {
            return Task.FromResult<string>("Success");
        }

        public Task<string> StoreFileWithACLAsync(string filePath, string fileName, S3CannedACL cannedACL)
        {
            return Task.FromResult<string>("Success");
        }

        public Task<Stream> StreamFileAsync(string fileName, string bucketName)
        {
            return Task.FromResult<Stream>(new MemoryStream());
        }
    }
}
