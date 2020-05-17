using agrix.Platforms.Vultr;
using agrix;
using System.Text;
using tests.Properties;
using Xunit;

namespace tests.Configuration
{
    // TODO: Rename
    public class InfrastructureConfigurationTest : BaseTest
    {
        [Fact]
        public void TestLoadPlatform()
        {
            var agrix = new Agrix(Encoding.Default.GetString(Resources.agrix), ApiKey);
            var platform = agrix.LoadPlatform();
            Assert.Equal(typeof(VultrPlatform), platform.GetType());
        }
    }
}
