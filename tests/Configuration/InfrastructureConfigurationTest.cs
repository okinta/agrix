using agrix.Configuration.Parsers;
using agrix.Configuration;
using agrix.Extensions;
using agrix.Platforms.Vultr;
using agrix;
using System.Text;
using System;
using tests.Properties;
using Xunit;

namespace tests.Configuration
{
    // TODO: Rename
    public class InfrastructureConfigurationTest : BaseTest
    {
        [Fact]
        public void TestLoadScripts()
        {
            var node = LoadYaml(Resources.ScriptsConfig).GetSequence("scripts");

            var scripts = new Parser().Load(
                "scripts",
                node,
                new ScriptParser().Parse);

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
            var node = LoadYaml(Resources.InvalidScriptTypeConfig).GetSequence("scripts");

            Assert.Throws<ArgumentException>(() =>
                new Parser().Load(
                    "scripts",
                    node,
                    new ScriptParser().Parse));
        }

        [Fact]
        public void TestLoadFirewalls()
        {
            var node = LoadYaml(Resources.FirewallsConfig).GetSequence("firewalls");

            var firewalls = new Parser().Load(
                "firewalls",
                node,
                new FirewallParser().Parse);

            Assert.Equal(2, firewalls.Count);

            var firewall = firewalls[0];
            Assert.Equal("ssh", firewalls[0].Name);
            Assert.Equal(2, firewalls[0].Rules.Count);

            var rule = firewall.Rules[0];
            Assert.Equal(IPType.V4, rule.IPType);
            Assert.Equal(Protocol.TCP, rule.Protocol);
            Assert.Equal("", rule.Source);
            Assert.Equal("0.0.0.0", rule.Subnet);
            Assert.Equal(0, rule.SubnetSize);
            Assert.Equal("22", rule.Ports);

            rule = firewall.Rules[1];
            Assert.Equal(IPType.V4, rule.IPType);
            Assert.Equal(Protocol.TCP, rule.Protocol);
            Assert.Equal("", rule.Source);
            Assert.Equal("0.0.0.0", rule.Subnet);
            Assert.Equal(0, rule.SubnetSize);
            Assert.Equal("3389", rule.Ports);

            firewall = firewalls[1];
            Assert.Equal("myapp", firewall.Name);
            Assert.Equal(2, firewall.Rules.Count);

            rule = firewall.Rules[0];
            Assert.Equal(IPType.V4, rule.IPType);
            Assert.Equal(Protocol.UDP, rule.Protocol);
            Assert.Equal("", rule.Source);
            Assert.Equal("172.0.24.1", rule.Subnet);
            Assert.Equal(20, rule.SubnetSize);
            Assert.Equal("8000:8100", rule.Ports);

            rule = firewall.Rules[1];
            Assert.Equal(IPType.V4, rule.IPType);
            Assert.Equal(Protocol.TCP, rule.Protocol);
            Assert.Equal("cloudflare", rule.Source);
            Assert.Equal("", rule.Subnet);
            Assert.Null(rule.SubnetSize);
            Assert.Equal("80", rule.Ports);
        }

        [Fact]
        public void TestLoadPlatform()
        {
            var agrix = new Agrix(Encoding.Default.GetString(Resources.agrix), ApiKey);
            var platform = agrix.LoadPlatform();
            Assert.Equal(typeof(VultrPlatform), platform.GetType());
        }
    }
}
