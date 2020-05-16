using agrix.Exceptions;
using agrix.Platforms.Vultr;
using agrix;
using System.Reflection;
using System;
using tests.Properties;
using Xunit;

namespace tests
{
    public class AgrixTest
    {
        private readonly Assembly TestAssembly = Assembly.GetExecutingAssembly();

        [Fact]
        public void TestConfigurationCannotBeEmpty()
        {
            Assert.Throws<ArgumentNullException>(() => new Agrix("", "abc"));
            Assert.Throws<ArgumentNullException>(() => new Agrix(null, "abc"));
        }

        [Fact]
        public void TestApiKeyCannotBeEmpty()
        {
            Assert.Throws<ArgumentNullException>(() => new Agrix("config", ""));
            Assert.Throws<ArgumentNullException>(() => new Agrix("config", null));
        }

        [Fact]
        public void TestLoadPlatform()
        {
            var agrix = new Agrix("platform: vultr", "abc");
            Assert.Equal(typeof(VultrPlatform), agrix.LoadPlatform().GetType());
        }

        [Fact]
        public void TestLoadCustomPlatform()
        {
            var agrix = new Agrix("platform: test", "abc");
            Assert.Equal(typeof(TestPlatform), agrix.LoadPlatform(TestAssembly).GetType());
        }

        [Fact]
        public void TestValidatePlatformIsRequired()
        {
            var agrix = new Agrix("servers:", "abc");
            Assert.Throws<AgrixValidationException>(() => agrix.Validate());
        }

        [Fact]
        public void TestValidateUnsupportedPlatform()
        {
            var agrix = new Agrix("platform:", "abc");
            Assert.Throws<AgrixValidationException>(() => agrix.Validate());
        }

        [Fact]
        public void TestValidateInvalidKey()
        {
            var agrix = new Agrix(Resources.SimpleConfig, "abc");
            Assert.Throws<AgrixValidationException>(() => agrix.Validate());
        }

        [Fact]
        public void TestValidateInvalidServer()
        {
            var agrix = new Agrix(
                Resources.InvalidServerConfig, Settings.Default.VultrApiKey);
            Assert.Throws<AgrixValidationException>(() => agrix.Validate());
        }

        [Fact]
        public void TestValidate()
        {
            var agrix = new Agrix(Resources.SimpleConfig, Settings.Default.VultrApiKey);
            agrix.Validate();
        }

        [Theory]
        [InlineData("ValidFedoraConfig")]
        [InlineData("ValidAlpineConfig")]
        public void TestValidateServer(string configName)
        {
            var config = Resources.ResourceManager.GetString(configName);
            var agrix = new Agrix(config, Settings.Default.VultrApiKey);
            agrix.Validate();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void TestProcess(bool dryrun)
        {
            var agrix = new Agrix(Resources.TestPlatformConfig, "abc123");
            agrix.Process(dryrun, TestAssembly);

            var platform = TestPlatform.LastInstance;
            Assert.Equal(1, platform.Provisions.Count);

            var server = platform.Provisions[0].Item1;
            Assert.Equal("Fedora 32 x64", server.OS.Name);
            Assert.Equal(dryrun, platform.Provisions[0].Item2);
        }
    }
}
