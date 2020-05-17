using agrix.Configuration.Parsers;
using agrix.Configuration;
using agrix.Extensions;
using System;
using tests.Properties;
using Xunit;

namespace tests.Configuration.Parsers
{
    public class FirewallRuleParserTest : BaseTest
    {
        [Fact]
        public void TestParse()
        {
            var rules = LoadYaml(Resources.FirewallRulesConfig).GetSequence("rules");
            var rule = new FirewallRuleParser().Parse(rules[0]);
            Assert.Equal(Protocol.TCP, rule.Protocol);
            Assert.Equal("0.0.0.0", rule.Subnet);
            Assert.Equal(0, rule.SubnetSize);
            Assert.Equal("22", rule.Ports);
        }

        [Fact]
        public void TestParse2()
        {
            var rules = LoadYaml(Resources.FirewallRulesConfig).GetSequence("rules");
            var rule = new FirewallRuleParser().Parse(rules[1]);
            Assert.Equal(Protocol.TCP, rule.Protocol);
            Assert.Equal("0.0.0.0", rule.Subnet);
            Assert.Equal(0, rule.SubnetSize);
            Assert.Equal("3389", rule.Ports);
        }

        [Fact]
        public void TestParseMultiplePorts()
        {
            var rules = LoadYaml(Resources.FirewallRulesConfig).GetSequence("rules");
            var rule = new FirewallRuleParser().Parse(rules[2]);
            Assert.Equal(Protocol.UDP, rule.Protocol);
            Assert.Equal("172.0.24.1", rule.Subnet);
            Assert.Equal(20, rule.SubnetSize);
            Assert.Equal("8000:8100", rule.Ports);
        }

        [Fact]
        public void TestParseCloudflare()
        {
            var rules = LoadYaml(Resources.FirewallRulesConfig).GetSequence("rules");
            var rule = new FirewallRuleParser().Parse(rules[3]);
            Assert.Equal(Protocol.TCP, rule.Protocol);
            Assert.Equal("cloudflare", rule.Source);
            Assert.Equal("80", rule.Ports);
        }

        [Fact]
        public void TestParseIPv6()
        {
            var rules = LoadYaml(Resources.FirewallRulesConfig).GetSequence("rules");
            var rule = new FirewallRuleParser().Parse(rules[4]);
            Assert.Equal(Protocol.ICMP, rule.Protocol);
            Assert.Equal("2002:4559:1FE2::4559:1FE2", rule.Subnet);
            Assert.Equal(20, rule.SubnetSize);
            Assert.Equal("9100:9105", rule.Ports);
        }

        [Fact]
        public void TestParseInvalid()
        {
            var rules = LoadYaml(Resources.FirewallRulesConfig).GetSequence("rules");
            Assert.Throws<ArgumentException>(() =>
                new FirewallRuleParser().Parse(rules[5]));
        }
    }
}
