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
    }
}
