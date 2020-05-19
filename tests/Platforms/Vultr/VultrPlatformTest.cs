using agrix.Platforms.Vultr;
using MockHttp.Net;
using System.Net.Http;
using tests.Properties;
using Xunit;

namespace tests.Platforms.Vultr
{
    public class VultrPlatformTest
    {
        [Fact]
        public void TestTestConnection()
        {
            using var requests = new MockVultrRequests(
                new HttpHandler(
                    "/account/info", Resources.VultrAccountInfo));
            new VultrPlatform("abc123", requests.Url).TestConnection();
        }

        [Fact]
        public void TestTestConnectionFail()
        {
            using var requests = new MockVultrRequests(
                new HttpHandler("/", ""));
            Assert.Throws<HttpRequestException>(() =>
                new VultrPlatform("abc123", requests.Url).TestConnection());
        }
    }
}
