﻿using agrix.Configuration.Parsers;
using agrix.Configuration;
using agrix.Extensions;
using tests.Properties;
using Xunit;

namespace tests.Configuration.Parsers
{
    public class FirewallParserTest : BaseTest
    {
        [Fact]
        public void TestParse()
        {
            var firewalls = LoadYaml(Resources.FirewallsConfig)
                .GetSequence("firewalls");
            var firewall = new FirewallParser().Parse(firewalls[0]);
            Assert.Equal("ssh", firewall.Name);
            Assert.Equal(2, firewall.Rules.Count);
        }

        [Fact]
        public void TestParse2()
        {
            var firewalls = LoadYaml(Resources.FirewallsConfig)
                .GetSequence("firewalls");
            var firewall = new FirewallParser().Parse(firewalls[0]);
            var rule = firewall.Rules[0];
            Assert.Equal(IpType.V4, rule.IpType);
            Assert.Equal(Protocol.TCP, rule.Protocol);
            Assert.Equal("", rule.Source);
            Assert.Equal("0.0.0.0", rule.Subnet);
            Assert.Equal(0, rule.SubnetSize);
            Assert.Equal("22", rule.Ports);

            rule = firewall.Rules[1];
            Assert.Equal(IpType.V4, rule.IpType);
            Assert.Equal(Protocol.TCP, rule.Protocol);
            Assert.Equal("", rule.Source);
            Assert.Equal("0.0.0.0", rule.Subnet);
            Assert.Equal(0, rule.SubnetSize);
            Assert.Equal("3389", rule.Ports);
        }

        [Fact]
        public void TestParse3()
        {
            var firewalls = LoadYaml(Resources.FirewallsConfig)
                .GetSequence("firewalls");
            var firewall = new FirewallParser().Parse(firewalls[1]);
            Assert.Equal("myapp", firewall.Name);
            Assert.Equal(2, firewall.Rules.Count);

            var rule = firewall.Rules[0];
            Assert.Equal(IpType.V4, rule.IpType);
            Assert.Equal(Protocol.UDP, rule.Protocol);
            Assert.Equal("", rule.Source);
            Assert.Equal("172.0.24.1", rule.Subnet);
            Assert.Equal(20, rule.SubnetSize);
            Assert.Equal("8000:8100", rule.Ports);

            rule = firewall.Rules[1];
            Assert.Equal(IpType.V4, rule.IpType);
            Assert.Equal(Protocol.TCP, rule.Protocol);
            Assert.Equal("cloudflare", rule.Source);
            Assert.Equal("0.0.0.0", rule.Subnet);
            Assert.Equal("80", rule.Ports);
        }
    }
}
