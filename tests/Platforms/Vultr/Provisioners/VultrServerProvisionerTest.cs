using agrix.Configuration;
using agrix.Extensions;
using agrix.Platforms.Vultr.Provisioners;
using MockHttp.Net;
using tests.Properties;
using Xunit;

namespace tests.Platforms.Vultr.Provisioners
{
    public class VultrServerProvisionerTest
    {
        private static Plan Plan => new Plan(1, 1024, "SSD");

        private const string Region = "New Jersey";

        /// <summary>
        /// Tests that VultrPlatform.Provision(Server) stands up a basic server.
        /// </summary>
        [Fact]
        public void TestProvisionServer()
        {
            var server = new Server(
                new OperatingSystem(name: "Fedora 32 x64"),
                Plan, Region, userData: "test");

            using var requests = new MockVultrRequests(
                new HttpHandler(
                    "/os/list", Resources.VultrOSList),
                new HttpHandler("/regions/list?availability=yes",
                    Resources.VultrRegionsList),
                new HttpHandler("/plans/list?type=all",
                    Resources.VultrPlansList),
                new HttpHandler("/sshkey/list", "{}"),
                new HttpHandler("/server/list"),
                new HttpHandler("/server/create",
                    "DCID=1&VPSPLANID=201&OSID=389&enable_private_network=no&userdata=dGVzdA%3D%3D&notify_activate=no&FIREWALLGROUPID=0",
                    "{\"SUBID\": \"1312965\"}")
            );
            new VultrServerProvisioner(requests.Client).Provision(server);
            requests.AssertAllCalledOnce();
        }

        /// <summary>
        /// Tests that VultrPlatform.Provision(Server, dryrun: true) doesn't stand up
        /// a basic server.
        /// </summary>
        [Fact]
        public void TestProvisionServerDryRun()
        {
            var server = new Server(
                new OperatingSystem(name: "Fedora 32 x64"),
                Plan, Region);

            using var requests = new MockVultrRequests(
                new HttpHandler(
                    "/os/list", Resources.VultrOSList),
                new HttpHandler("/regions/list?availability=yes",
                    Resources.VultrRegionsList),
                new HttpHandler("/plans/list?type=all",
                    Resources.VultrPlansList),
                new HttpHandler("/sshkey/list", "{}"),
                new HttpHandler("/server/list")
            );
            new VultrServerProvisioner(requests.Client).Provision(server, true);
            requests.AssertAllCalledOnce();
        }

        /// <summary>
        /// Tests that VultrPlatform.Provision(Server) updates an existing server.
        /// </summary>
        [Fact]
        public void TestProvisionUpdateServer()
        {
            var server = new Server(
                new OperatingSystem(name: "Fedora 32 x64"),
                Plan, Region, label: "my new server"
            );

            using var requests = new MockVultrRequests(
                new HttpHandler(
                    "/os/list", Resources.VultrOSList),
                new HttpHandler("/regions/list?availability=yes",
                    Resources.VultrRegionsList),
                new HttpHandler("/plans/list?type=all",
                    Resources.VultrPlansList),
                new HttpHandler("/sshkey/list", "{}"),
                new HttpHandler(
                    "/server/list", Resources.VultrServerList),
                new HttpHandler(
                    "/server/destroy", "SUBID=576965", ""),
                new HttpHandler("/server/create",
                    "DCID=1&VPSPLANID=201&OSID=389&enable_private_network=no&label=my+new+server&notify_activate=no&FIREWALLGROUPID=0",
                    "{\"SUBID\": \"1312965\"}")
            );
            new VultrServerProvisioner(requests.Client).Provision(server);
            requests.AssertAllCalledOnce();
        }

        /// <summary>
        /// Tests that VultrPlatform.Provision(Server, dryrun: true) doesn't update an
        /// existing server.
        /// </summary>
        [Fact]
        public void TestProvisionUpdateServerDryrun()
        {
            var server = new Server(
                new OperatingSystem(name: "Fedora 32 x64"),
                Plan, Region, label: "my new server"
            );

            using var requests = new MockVultrRequests(
                new HttpHandler(
                    "/os/list", Resources.VultrOSList),
                new HttpHandler("/regions/list?availability=yes",
                    Resources.VultrRegionsList),
                new HttpHandler("/plans/list?type=all",
                    Resources.VultrPlansList),
                new HttpHandler("/sshkey/list", "{}"),
                new HttpHandler(
                    "/server/list", Resources.VultrServerList)
            );
            new VultrServerProvisioner(requests.Client).Provision(server, true);
            requests.AssertAllCalledOnce();
        }

        /// <summary>
        /// Tests that VultrPlatform.Provision(Server) doesn't change a server that
        /// already matches what the configuration defines.
        /// </summary>
        [Fact]
        public void TestProvisionDoNotUpdateUnchangedServer()
        {
            var server = new Server(
                new OperatingSystem(name: "CentOS 6 x64"),
                new Plan(2, 4096, "SSD"),
                Region, label: "my new server", tag: "mytag"
            );

            using var requests = new MockVultrRequests(
                new HttpHandler(
                    "/os/list", Resources.VultrOSList),
                new HttpHandler("/regions/list?availability=yes",
                    Resources.VultrRegionsList),
                new HttpHandler("/plans/list?type=all",
                    Resources.VultrPlansList),
                new HttpHandler("/sshkey/list", "{}"),
                new HttpHandler(
                    "/server/list", Resources.VultrServerList)
            );
            new VultrServerProvisioner(requests.Client).Provision(server);
            requests.AssertAllCalledOnce();
        }

        [Theory]
        [InlineData(1, "New Jersey")]
        [InlineData(40, "Singapore")]
        public void TestGetRegionId(int id, string name)
        {
            using var requests = new MockVultrRequests(
                new HttpHandler(
                    "/regions/list", Resources.VultrRegionsList));

            Assert.Equal(id, requests.Client.Region.GetRegionId(name));
        }

        [Theory]
        [InlineData(204, 4, 8192, "SSD")]
        [InlineData(404, 4, 16384, "HIGHFREQUENCY")]
        public void TestGetPlanId(int id, int cpu, int ram, string type)
        {
            using var requests = new MockVultrRequests(
                new HttpHandler(
                    "/plans/list", Resources.VultrPlansList));

            var provisioner = new VultrServerProvisioner(requests.Client);
            Assert.Equal(id, provisioner.GetPlanId(
                new Plan(cpu, ram, type)));
        }

        [Fact]
        public void TestGetOsApp()
        {
            using var requests = new MockVultrRequests(
                new HttpHandler(
                    "/app/list", Resources.VultrAppList));

            var provisioner = new VultrServerProvisioner(requests.Client);
            var server = new Server(
                new OperatingSystem("LAMP on CentOS 7 x64"),
                Plan, Region);
            var os = provisioner.GetOs(server);
            Assert.Equal(186, os.OsId);
            Assert.Equal(41, os.Appid);
            Assert.Null(os.IsoId);
            Assert.Null(os.ScriptId);
            Assert.Null(os.SnapshotId);
        }

        [Fact]
        public void TestGetOsScript()
        {
            using var requests = new MockVultrRequests(
                new HttpHandler(
                    "/os/list", Resources.VultrOSList),
                new HttpHandler(
                    "/startupscript/list", Resources.VultrStartupScripts));

            var provisioner = new VultrServerProvisioner(requests.Client);
            var server = new Server(
                new OperatingSystem(name: "Fedora 32 x64"),
                Plan, Region, startupScript: "hello-boot");
            var os = provisioner.GetOs(server);
            Assert.Equal(389, os.OsId);
            Assert.Equal(3, os.ScriptId);
            Assert.Null(os.Appid);
            Assert.Null(os.IsoId);
            Assert.Null(os.SnapshotId);
        }

        [Theory]
        [InlineData(389, "Fedora 32 x64")]
        [InlineData(366, "OpenBSD 6.6 x64")]
        public void TestGetOs(int id, string name)
        {
            using var requests = new MockVultrRequests(
                new HttpHandler(
                    "/os/list", Resources.VultrOSList));

            var provisioner = new VultrServerProvisioner(requests.Client);
            var server = new Server(
                new OperatingSystem(name: name),
                Plan, Region);
            var os = provisioner.GetOs(server);
            Assert.Equal(id, os.OsId);
            Assert.Null(os.Appid);
            Assert.Null(os.IsoId);
            Assert.Null(os.ScriptId);
            Assert.Null(os.SnapshotId);
        }

        [Fact]
        public void TestGetIso()
        {
            using var requests = new MockVultrRequests(
                new HttpHandler(
                    "/iso/list", Resources.VultrISOList));

            var provisioner = new VultrServerProvisioner(requests.Client);
            var server = new Server(
                new OperatingSystem(iso: "installcoreos.iso"),
                Plan, Region);
            var os = provisioner.GetOs(server);
            Assert.Equal(159, os.OsId);
            Assert.Null(os.Appid);
            Assert.Null(os.ScriptId);
            Assert.Null(os.SnapshotId);
            Assert.True(os.IsoId > 0);
        }
    }
}
