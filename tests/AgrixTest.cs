using agrix;
using agrix.Exceptions;
using System;
using Xunit;

namespace tests
{
    public class AgrixTest
    {
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
            var agrix = new Agrix(@"platform: vultr
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

        private const string SimpleConfig = @"platform: vultr
servers:";
    }
}
