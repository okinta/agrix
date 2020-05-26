using agrix.Configuration;
using agrix.Platforms.Vultr.Provisioners;
using MockHttp.Net;
using System.Diagnostics.CodeAnalysis;
using tests.Properties;
using Xunit;

namespace tests.Platforms.Vultr.Provisioners
{
    public class VultrFirewallProvisionerTest
    {
        [Fact]
        [SuppressMessage("ReSharper", "StringLiteralTypo")]
        public void TestProvisionFirewall()
        {
            var firewall = new Firewall("my-firewall",
                new FirewallRule(
                    IpType.V4, Protocol.TCP, "8080", "192.0.0.1", 20),
                new FirewallRule(
                    IpType.V4, Protocol.TCP, "80", "cloudflare"));

            using var requests = new MockVultrRequests(
                new HttpHandler("/firewall/group_list", "{}"),
                new HttpHandler(
                    "/firewall/group_create", "description=my-firewall",
                    "{\"FIREWALLGROUPID\": \"1234\"}"),
                new HttpHandler(
                    "/firewall/rule_create",
                    new ValidateRequestHandler(
                        "FIREWALLGROUPID=1234&direction=in&ip_type=v4&" +
                        "protocol=tcp&subnet=192.0.0.1&subnet_size=20&port=8080",
                        "{\"rulenumber\": 2}"),
                    new ValidateRequestHandler(
                        "FIREWALLGROUPID=1234&direction=in&ip_type=v4&" +
                        "protocol=tcp&subnet=0.0.0.0&subnet_size=0&port=80&" +
                        "source=cloudflare",
                        "{\"rulenumber\": 3}")));
            new VultrFirewallProvisioner(requests.Client).Provision(firewall);
            requests.AssertAllCalledOnce();
        }

        [Fact]
        [SuppressMessage("ReSharper", "StringLiteralTypo")]
        public void AddRuleToFirewall()
        {
            var firewall = new Firewall("my http firewall",
                new FirewallRule(
                    IpType.V4, Protocol.TCP, "8080", "192.0.0.1", 20),
                new FirewallRule(
                    IpType.V4, Protocol.TCP, "80", "cloudflare")
            );

            using var requests = new MockVultrRequests(
                new HttpHandler(
                    "/firewall/group_list", Resources.VultrFirewallGroupList),
                new HttpHandler(
                    "/firewall/rule_list?FIREWALLGROUPID=1234abcd&direction=in&ip_type=v4",
                    "{}"),
                new HttpHandler(
                    "/firewall/rule_list?FIREWALLGROUPID=1234abcd&direction=in&ip_type=v6",
                    "{}"),
                new HttpHandler(
                    "/firewall/rule_create",
                    new ValidateRequestHandler(
                        "FIREWALLGROUPID=1234abcd&direction=in&ip_type=v4&" +
                        "protocol=tcp&subnet=192.0.0.1&subnet_size=20&port=8080",
                        "{\"rulenumber\": 2}"),
                    new ValidateRequestHandler(
                        "FIREWALLGROUPID=1234abcd&direction=in&ip_type=v4&" +
                        "protocol=tcp&subnet=0.0.0.0&subnet_size=0&port=80&" +
                        "source=cloudflare",
                        "{\"rulenumber\": 3}")));
            new VultrFirewallProvisioner(requests.Client).Provision(firewall);
            requests.AssertAllCalledOnce();
        }

        [Fact]
        [SuppressMessage("ReSharper", "StringLiteralTypo")]
        public void DoNotAddExistingRuleToFirewall()
        {
            var firewall = new Firewall("my http firewall",
                new FirewallRule(
                    IpType.V4, Protocol.TCP, "8080", "192.0.0.1", 20),
                new FirewallRule(
                    IpType.V4, Protocol.TCP, "80", "cloudflare")
            );

            using var requests = new MockVultrRequests(
                new HttpHandler(
                    "/firewall/group_list", Resources.VultrFirewallGroupList),
                new HttpHandler(
                    "/firewall/rule_list?FIREWALLGROUPID=1234abcd&direction=in&ip_type=v4",
                    @"
                    {
                        ""1"": {
                            ""rulenumber"": 1,
                            ""action"": ""accept"",
                            ""protocol"": ""tcp"",
                            ""port"": ""8080"",
                            ""subnet"": ""192.0.0.1"",
                            ""subnet_size"": 20,
                            ""source"": """",
                            ""notes"": """"
                        }
                    }"),
                new HttpHandler(
                    "/firewall/rule_list?FIREWALLGROUPID=1234abcd&direction=in&ip_type=v6",
                    "{}"),
                new HttpHandler(
                    "/firewall/rule_create",
                    "FIREWALLGROUPID=1234abcd&direction=in&ip_type=v4&" +
                        "protocol=tcp&subnet=0.0.0.0&subnet_size=0&port=80&" +
                        "source=cloudflare",
                        "{\"rulenumber\": 3}"));
            new VultrFirewallProvisioner(requests.Client).Provision(firewall);
            requests.AssertAllCalledOnce();
        }

        [Fact]
        [SuppressMessage("ReSharper", "StringLiteralTypo")]
        public void UpdateExistingRule()
        {
            var firewall = new Firewall("my http firewall",
                new FirewallRule(
                    IpType.V4, Protocol.TCP, "8080", "192.0.0.1", 20),
                new FirewallRule(
                    IpType.V4, Protocol.TCP, "80", "cloudflare")
            );

            using var requests = new MockVultrRequests(
                new HttpHandler(
                    "/firewall/group_list", Resources.VultrFirewallGroupList),
                new HttpHandler(
                    "/firewall/rule_list?FIREWALLGROUPID=1234abcd&direction=in&ip_type=v4",
                    @"
                    {
                        ""1"": {
                            ""rulenumber"": 1,
                            ""action"": ""accept"",
                            ""protocol"": ""tcp"",
                            ""port"": ""8081"",
                            ""subnet"": ""192.0.0.1"",
                            ""subnet_size"": 20,
                            ""source"": """",
                            ""notes"": """"
                        },
                        ""2"": {
                            ""rulenumber"": 1,
                            ""action"": ""accept"",
                            ""protocol"": ""tcp"",
                            ""port"": ""80"",
                            ""subnet"": ""0.0.0.0"",
                            ""subnet_size"": 0,
                            ""source"": ""cloudflare"",
                            ""notes"": """"
                        }
                    }"),
                new HttpHandler(
                    "/firewall/rule_list?FIREWALLGROUPID=1234abcd&direction=in&ip_type=v6",
                    "{}"),
                new HttpHandler(
                    "/firewall/rule_delete",
                    "FIREWALLGROUPID=1234abcd&rulenumber=1",
                    ""),
                new HttpHandler(
                    "/firewall/rule_create",
                    "FIREWALLGROUPID=1234abcd&direction=in&ip_type=v4&" +
                    "protocol=tcp&subnet=192.0.0.1&subnet_size=20&port=8080",
                    "{\"rulenumber\": 2}"));
            new VultrFirewallProvisioner(requests.Client).Provision(firewall);
            requests.AssertAllCalledOnce();
        }
    }
}
