using agrix.Configuration;
using System.IO;
using System.Text;
using tests.Properties;
using Xunit;
using YamlDotNet.RepresentationModel;

namespace tests.Configuration
{
    public class InfrastructureConfigurationTest
    {
        [Fact]
        public void TestLoadServers()
        {
            var input = new StringReader(Encoding.Default.GetString(Resources.agrix));
            var yaml = new YamlStream();
            yaml.Load(input);
            var servers = InfrastructureConfiguration.LoadServers(yaml);

            Assert.Equal(3, servers.Count);
        }
    }
}
