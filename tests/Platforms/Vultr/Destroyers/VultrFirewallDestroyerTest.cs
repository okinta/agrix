using agrix.Configuration;
using agrix.Platforms.Vultr.Destroyers;
using MockHttp.Net;
using tests.Properties;
using Xunit;

namespace tests.Platforms.Vultr.Destroyers
{
    public class VultrFirewallDestroyerTest
    {
        [Fact]
        public void TestDestroyFirewall()
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
                    "/firewall/rule_delete",
                    "FIREWALLGROUPID=1234abcd&rulenumber=1",
                    ""),
                new HttpHandler("/firewall/group_delete",
                    "FIREWALLGROUPID=1234abcd",
                    ""));
            new VultrFirewallDestroyer(requests.Client).Destroy(firewall);
            requests.AssertAllCalledOnce();
        }

        [Fact]
        public void TestDestroyFirewallDoesNotExist()
        {
            var firewall = new Firewall("my http firewall",
                new FirewallRule(
                    IpType.V4, Protocol.TCP, "8080", "192.0.0.1", 20),
                new FirewallRule(
                    IpType.V4, Protocol.TCP, "80", "cloudflare")
            );

            using var requests = new MockVultrRequests(
                new HttpHandler(
                    "/firewall/group_list", "{}"));
            new VultrFirewallDestroyer(requests.Client).Destroy(firewall);
            requests.AssertAllCalledOnce();
        }

        [Fact]
        public void TestDestroyFirewallNoRules()
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
                new HttpHandler("/firewall/group_delete",
                    "FIREWALLGROUPID=1234abcd",
                    ""));
            new VultrFirewallDestroyer(requests.Client).Destroy(firewall);
            requests.AssertAllCalledOnce();
        }

        [Fact]
        public void TestDestroyFirewallDryrun()
        {
            var firewall = new Firewall("my http firewall",
                new FirewallRule(
                    IpType.V4, Protocol.TCP, "8080", "192.0.0.1", 20)
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
                    "{}"));
            new VultrFirewallDestroyer(requests.Client).Destroy(firewall, true);
            requests.AssertAllCalledOnce();
        }
    }
}
