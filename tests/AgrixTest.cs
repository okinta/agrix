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
            var agrix = new Agrix(SimpleConfig, "abc");
            Assert.Throws<AgrixValidationException>(() => agrix.Validate());
        }

        [Fact]
        public void TestValidateInvalidServer()
        {
            var agrix = new Agrix(
@"platform: vultr
servers:
  - label: test", Settings.Default.VultrApiKey);
            Assert.Throws<AgrixValidationException>(() => agrix.Validate());
        }

        [Fact]
        public void TestValidate()
        {
            var agrix = new Agrix(SimpleConfig, Settings.Default.VultrApiKey);
            agrix.Validate();
        }

        [Theory]
        [InlineData(
@"platform: vultr
servers:
  - os:
      name: Fedora 32 x64
    plan:
      cpu: 2
      memory: 4096
      type: SSD
    region: Atlanta")]
        [InlineData(
@"platform: vultr
servers:
  - os:
      iso: alpine.iso
    plan:
      cpu: 2
      memory: 4096
      type: SSD
    region: Chicago
    userdata:
      my-array:
        - 1
        - 2")]
        public void TestValidateServer(string config)
        {
            var agrix = new Agrix(config, Settings.Default.VultrApiKey);
            agrix.Validate();
        }

        private const string SimpleConfig =
@"platform: vultr
servers:";
    }
}
