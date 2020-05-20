using agrix.Configuration;
using agrix.Platforms.Vultr.Provisioners;
using MockHttp.Net;
using System.Diagnostics.CodeAnalysis;
using Xunit;

namespace tests.Platforms.Vultr.Provisioners
{
    public class VultrFirewallProvisionerTest
    {
        [Fact]
        [SuppressMessage("ReSharper", "StringLiteralTypo")]
        public void TestProvisionFirewall()
        {
            var firewall = new Firewall("my-firewall", new[]
            {
                new FirewallRule(
                    IpType.V4, Protocol.TCP, "8080", "192.0.0.1", 20)
            });

            using var requests = new MockVultrRequests(
                new HttpHandler("/firewall/group_list", "{}"),
                new HttpHandler(
                    "/firewall/group_create", "description=my-firewall",
                    "{\"FIREWALLGROUPID\": \"1234\"}"),
                new HttpHandler(
                    "/firewall/rule_create",
                    "FIREWALLGROUPID=1234&direction=in&ip_type=v4&" +
                    "protocol=tcp&subnet=192.0.0.1&subnet_size=20&port=8080",
                    "{\"rulenumber\": 2}"));
            new VultrFirewallProvisioner(requests.Client).Provision(firewall);
            requests.AssertAllCalledOnce();
        }
    }
}
