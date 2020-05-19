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

        private AgrixSettings TestSettings { get; } =
            new AgrixSettings("abc", assembly: TestAssembly);

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

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void TestApiKeyCannotBeEmpty(string apiKey)
        {
            Assert.Throws<ArgumentNullException>(
                () => new Agrix("config", new AgrixSettings(apiKey)));
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
            var agrix = new Agrix("platform: test", TestSettings);
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
            using var requests = new MockVultrRequests(
                new HttpHandler("/account/info"));
            var agrix = new Agrix(Resources.TestPlatformConfig,
                new AgrixSettings("abc", requests.Url, TestAssembly));
            agrix.Process(dryrun);

            var platform = TestPlatform.LastInstance;
            Assert.Equal(1, platform.Provisions.Count);

            var server = platform.Provisions[0].Item1;
            Assert.Equal("Fedora 32 x64", server.Os.Name);
            Assert.Equal(dryrun, platform.Provisions[0].Item2);
        }
    }
}
