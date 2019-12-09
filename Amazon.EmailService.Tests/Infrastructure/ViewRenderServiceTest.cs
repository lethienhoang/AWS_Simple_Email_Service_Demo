using Amazon.EmailService.Infrastructure;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Amazon.EmailService.Tests.Infrastructure
{
    public class ViewRenderServiceTest
    {
        private ViewRenderService _viewRenderService;

        [SetUp]
        public void Setup()
        {
            _viewRenderService = new ViewRenderService();
        }

        [Test]
        public void RenderToStringAsync_ReturnString()
        {
            string content = @"<div>Welcome [UserName] to </div>";

            byte[] byteArray = Encoding.UTF8.GetBytes(content);
            var steam = new MemoryStream(byteArray);

            var bodyData = new Dictionary<string, string>();
            bodyData.Add("UserName", "userTest");

            var result = _viewRenderService.RenderToStringAsync(steam, bodyData);

            Assert.IsInstanceOf(typeof(string), result.Result);
            Assert.AreEqual("<div>Welcome userTest to </div>", result.Result);
        }

    }
}
