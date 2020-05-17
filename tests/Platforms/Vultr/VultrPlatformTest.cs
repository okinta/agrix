using agrix.Platforms.Vultr;
using System.Net.Http;
using tests.Properties;
using tests.TestHelpers;
using Xunit;

namespace tests.Platforms.Vultr
{
    public class VultrPlatformTest
    {
        [Fact]
        public void TestTestConnection()
        {
            using var requests = new MockVultrRequests(
                new CustomMockHttpHandler(
                    "/account/info", Resources.VultrAccountInfo));
            new VultrPlatform("abc123", requests.Url).TestConnection();
        }

        [Fact]
        public void TestTestConnectionFail()
        {
            using var requests = new MockVultrRequests(
                new CustomMockHttpHandler("/", ""));
            Assert.Throws<HttpRequestException>(() =>
                new VultrPlatform("abc123", requests.Url).TestConnection());
        }
    }
}
