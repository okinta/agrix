using agrix.Configuration;
using agrix.Platforms.Vultr;
using System.IO;
using System.Text;
using tests.Properties;
using Xunit;
using YamlDotNet.RepresentationModel;

namespace tests.Configuration
{
    public class InfrastructureConfigurationTest
    {
        private const string ApiKey = "abc123";

        private static readonly IAgrixConfig AgrixConfig = new VultrAgrixConfig();

        [Fact]
        public void TestLoadServers()
        {
            var servers = InfrastructureConfiguration.LoadServers(LoadYaml(), AgrixConfig);
            Assert.Equal(3, servers.Count);
        }

        [Fact]
        public void TestLoadEmptyServers()
        {
            var servers = InfrastructureConfiguration.LoadServers(
                LoadYaml("platform: vultr"), AgrixConfig);
            Assert.Equal(0, servers.Count);
        }

        [Fact]
        public void TestLoadUserData()
        {
            var servers = InfrastructureConfiguration.LoadServers(LoadYaml(
@"servers:
  - os:
      iso: alpine.iso
    plan:
      cpu: 2
      memory: 4096
      type: SSD
    region: Chicago
    userdata: test data"), AgrixConfig);
            Assert.Equal(1, servers.Count);
            Assert.Equal("test data", servers[0].UserData);
        }

        [Fact]
        public void TestLoadJsonUserData()
        {
            var servers = InfrastructureConfiguration.LoadServers(LoadYaml(
@"servers:
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
        - 2"), AgrixConfig);
            Assert.Equal(1, servers.Count);
            Assert.Equal("{\"my-array\": [\"1\", \"2\"]}", servers[0].UserData);
        }

        [Fact]
        public void TestLoadEmptyScripts()
        {
            var scripts = InfrastructureConfiguration.LoadScripts(LoadYaml(), AgrixConfig);
            Assert.Equal(0, scripts.Count);
        }

        [Fact]
        public void TestLoadScripts()
        {
            var scripts = InfrastructureConfiguration.LoadScripts(LoadYaml(
@"scripts:
  - name: test
    type: boot
    content: this is a test script

  - name: bash-script
    type: boot
    content: |
      #!/usr/bin/env bash
      echo hello"), AgrixConfig);
            Assert.Equal(2, scripts.Count);
            Assert.Equal("test", scripts[0].Name);
            Assert.Equal("boot", scripts[0].Type);
            Assert.Equal("this is a test script", scripts[0].Content);
            Assert.Equal("bash-script", scripts[1].Name);
            Assert.Equal("boot", scripts[1].Type);
            Assert.Equal(string.Join('\n',
                "#!/usr/bin/env bash",
                "echo hello"), scripts[1].Content);
        }

        [Fact]
        public void TestLoadPlatform()
        {
            var platform = InfrastructureConfiguration.LoadPlatform(LoadYaml(), ApiKey);
            Assert.Equal(typeof(VultrPlatform), platform.GetType());
        }

        private YamlStream LoadYaml()
        {
            var input = new StringReader(Encoding.Default.GetString(Resources.agrix));
            var yaml = new YamlStream();
            yaml.Load(input);
            return yaml;
        }

        private YamlStream LoadYaml(string content)
        {
            var input = new StringReader(content);
            var yaml = new YamlStream();
            yaml.Load(input);
            return yaml;
        }
    }
}
