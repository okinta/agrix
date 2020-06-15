using agrix.Exceptions;
using agrix.Platforms.Vultr;
using agrix;
using MockHttp.Net;
using System.Reflection;
using System;
using tests.Properties;
using Xunit;

namespace tests
{
    public class AgrixTest
    {
        private static readonly Assembly TestAssembly = Assembly.GetExecutingAssembly();

        private AgrixSettings BasicSettings { get; } = new AgrixSettings("abc");

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void TestConfigurationCannotBeEmpty(string configuration)
        {
            Assert.Throws<ArgumentNullException>(
                () => new Agrix(configuration, BasicSettings));
        }

        [Fact]
        public void TestSettingsCannotBeEmpty()
        {
            Assert.Throws<ArgumentNullException>(
                () => new Agrix("config", null));
        }

        [Fact]
        public void TestLoadPlatform()
        {
            var agrix = new Agrix(
                "platform: vultr", BasicSettings);
            Assert.Equal(
                typeof(VultrPlatform), agrix.LoadPlatform().GetType());
        }

        [Fact]
        public void TestLoadCustomPlatform()
        {
            var agrix = new Agrix("platform: test",
                new AgrixSettings("abc", "http://example.org/", TestAssembly));
            Assert.Equal(typeof(TestPlatform), agrix.LoadPlatform().GetType());
        }

        [Fact]
        public void TestValidatePlatformIsRequired()
        {
            var agrix = new Agrix("servers:", BasicSettings);
            Assert.Throws<AgrixValidationException>(() => agrix.Validate());
        }

        [Fact]
        public void TestValidateUnsupportedPlatform()
        {
            var agrix = new Agrix("platform:", BasicSettings);
            Assert.Throws<AgrixValidationException>(() => agrix.Validate());
        }

        [Fact]
        public void TestValidateInvalidKey()
        {
            var agrix = new Agrix(Resources.SimpleConfig, BasicSettings);
            Assert.Throws<AgrixValidationException>(() => agrix.Validate());
        }

        [Fact]
        public void TestValidateInvalidServer()
        {
            var agrix = new Agrix(Resources.InvalidServerConfig, BasicSettings);
            Assert.Throws<AgrixValidationException>(() => agrix.Validate());
        }

        [Fact]
        public void TestValidate()
        {
            using var requests = new MockVultrRequests(
                new HttpHandler("/account/info"));
            var agrix = new Agrix(
                Resources.SimpleConfig, new AgrixSettings("abc", requests.Url));
            agrix.Validate();
        }

        [Theory]
        [InlineData("ValidFedoraConfig")]
        [InlineData("ValidAlpineConfig")]
        public void TestValidateServer(string configName)
        {
            using var requests = new MockVultrRequests(
                new HttpHandler("/account/info"));
            var config = Resources.ResourceManager.GetString(configName);
            var agrix = new Agrix(config, new AgrixSettings("abc", requests.Url));
            agrix.Validate();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void TestProcess(bool dryrun)
        {
            var expected = "plan.cpu=2&plan.memory=4096&plan.type=SSD&" +
                                 "os.name=Fedora%2032%20x64&os.app=Fedora%2032%20x64&" +
                                 $"os.iso=&dryrun={dryrun}";
            using var requests = new MockVultrRequests(
                new HttpHandler("provision", expected, ""));
            var agrix = new Agrix(Resources.TestPlatformConfig,
                new AgrixSettings("abc", requests.Url, TestAssembly));
            agrix.Process(dryrun);
            requests.AssertAllCalledOnce();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void TestDestroy(bool dryrun)
        {
            using var requests = new MockVultrRequests(
                new HttpHandler(
                    "destroy",
                    $"label=myserver&dryrun={dryrun}",
                    ""));
            var agrix = new Agrix(Resources.TestPlatformConfig,
                new AgrixSettings("abc", requests.Url, TestAssembly));
            agrix.Destroy(dryrun);
            requests.AssertAllCalledOnce();
        }
    }
}
