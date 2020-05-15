using agrix.Configuration;
using agrix.Platforms.Vultr;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System;
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
            Assert.Equal(ScriptType.Boot, scripts[0].Type);
            Assert.Equal("this is a test script", scripts[0].Content);
            Assert.Equal("bash-script", scripts[1].Name);
            Assert.Equal(ScriptType.Boot, scripts[1].Type);
            Assert.Equal(string.Join('\n',
                "#!/usr/bin/env bash",
                "echo hello"), scripts[1].Content);
        }

        [Fact]
        public void TestLoadInvalidTypeScript()
        {
            Assert.Throws<ArgumentException>(() =>
                InfrastructureConfiguration.LoadScripts(LoadYaml(
@"scripts:
  - name: test
    type: tony
    content: this is a test script"), AgrixConfig));
        }

        [Fact]
        public void TestLoadFirewalls()
        {
            var firewalls = InfrastructureConfiguration.LoadFirewalls(LoadYaml(
@"firewalls:
  - name: ssh
    rules:
      - ip-type: v4
        protocol: tcp
        source: 0.0.0.0/0
        port: 22

      - ip-type: v4
        protocol: tcp
        source: 0.0.0.0/0
        port: 3389

  - name: myapp
    rules:
      - ip-type: v4
        protocol: udp
        source: 172.0.24.1/20
        ports: 8000 - 8100"), AgrixConfig);

            Assert.Equal(2, firewalls.Count);
            Assert.Equal("ssh", firewalls[0].Name);
            Assert.Equal(2, firewalls[0].Rules.Count);

            Assert.Equal(IPType.V4, firewalls[0].Rules[0].IPType);
            Assert.Equal(Protocol.TCP, firewalls[0].Rules[0].Protocol);
            Assert.Equal("0.0.0.0/0", firewalls[0].Rules[0].Source);
            Assert.Empty(firewalls[0].Rules[0].Ports.Except(new List<int>() { 22 }));

            Assert.Equal(IPType.V4, firewalls[0].Rules[1].IPType);
            Assert.Equal(Protocol.TCP, firewalls[0].Rules[1].Protocol);
            Assert.Equal("0.0.0.0/0", firewalls[0].Rules[1].Source);
            Assert.Empty(firewalls[0].Rules[1].Ports.Except(new List<int>() { 3389 }));

            Assert.Equal("myapp", firewalls[1].Name);
            Assert.Equal(1, firewalls[1].Rules.Count);

            Assert.Equal(IPType.V4, firewalls[1].Rules[0].IPType);
            Assert.Equal(Protocol.UDP, firewalls[1].Rules[0].Protocol);
            Assert.Equal("172.0.24.1/20", firewalls[1].Rules[0].Source);
            Assert.Empty(firewalls[1].Rules[0].Ports.Except(Enumerable.Range(8000, 8100)));
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
