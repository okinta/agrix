using agrix.Platforms.Vultr.Destroyers;
using MockHttp.Net;
using OperatingSystem = agrix.Configuration.OperatingSystem;
using Plan = agrix.Configuration.Plan;
using Server = agrix.Configuration.Server;
using tests.Properties;
using Xunit;

namespace tests.Platforms.Vultr.Destroyers
{
    public class VultrServerDestroyerTest
    {
        private static Plan Plan => new Plan(1, 1024, "SSD");

        private const string Region = "New Jersey";

        [Fact]
        public void TestDestroyServer()
        {
            var server = new Server(
                new OperatingSystem(name: "Fedora 32 x64"),
                Plan, Region, userData: "test", label: "my new server");

            using var requests = new MockVultrRequests(
                new HttpHandler("/regions/list?availability=yes",
                    Resources.VultrRegionsList),
                new HttpHandler("/server/list", Resources.VultrServerList),
                new HttpHandler("/server/destroy",
                    "SUBID=576965",
                    "")
            );
            new VultrServerDestroyer(requests.Client).Destroy(server);
            requests.AssertAllCalledOnce();
        }

        [Fact]
        public void TestDestroyServerDoesNotExist()
        {
            var server = new Server(
                new OperatingSystem(name: "Fedora 32 x64"),
                Plan, Region, userData: "test", label: "my other server");

            using var requests = new MockVultrRequests(
                new HttpHandler("/regions/list?availability=yes",
                    Resources.VultrRegionsList),
                new HttpHandler("/server/list", Resources.VultrServerList));
            new VultrServerDestroyer(requests.Client).Destroy(server);
            requests.AssertAllCalledOnce();
        }

        [Fact]
        public void TestDestroyServerDryrun()
        {
            var server = new Server(
                new OperatingSystem(name: "Fedora 32 x64"),
                Plan, Region, userData: "test", label: "my new server");

            using var requests = new MockVultrRequests(
                new HttpHandler("/regions/list?availability=yes",
                    Resources.VultrRegionsList),
                new HttpHandler("/server/list", Resources.VultrServerList));
            new VultrServerDestroyer(requests.Client).Destroy(server, true);
            requests.AssertAllCalledOnce();
        }
    }
}
