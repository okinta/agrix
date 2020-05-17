using agrix.Configuration;
using agrix.Platforms.Vultr.Provisioners;
using agrix.Platforms.Vultr;
using OperatingSystem = agrix.Configuration.OperatingSystem;
using System.Net.Http;
using tests.Properties;
using tests.TestHelpers;
using Xunit;

namespace tests.Platforms.Vultr
{
    public class VultrPlatformTest
    {
        [Fact]
        public void TestTestConnection()
        {
            using var requests = new MockVultrRequests(
                new CustomMockHttpHandler(
                    "/account/info", Resources.VultrAccountInfo));
            new VultrPlatform("abc123", requests.Url).TestConnection();
        }

        [Fact]
        public void TestTestConnectionFail()
        {
            using var requests = new MockVultrRequests(
                new CustomMockHttpHandler("/", ""));
            Assert.Throws<HttpRequestException>(() =>
                new VultrPlatform("abc123", requests.Url).TestConnection());
        }

        private static Plan Plan => new Plan(1, 1024, "SSD");

        private const string Region = "New Jersey";

        #region Test VultrPlatform.Provision(Server, bool)

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
                new CustomMockHttpHandler(
                    "/os/list", Resources.VultrOSList),
                new CustomMockHttpHandler("/regions/list?availability=yes",
                    Resources.VultrRegionsList),
                new CustomMockHttpHandler("/plans/list?type=all",
                    Resources.VultrPlansList),
                new CustomMockHttpHandler("/server/list"),
                new CustomMockHttpHandler("/server/create",
                    "DCID=1&VPSPLANID=201&OSID=389&enable_private_network=no&userdata=dGVzdA%3D%3D&notify_activate=no",
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
                new CustomMockHttpHandler(
                    "/os/list", Resources.VultrOSList),
                new CustomMockHttpHandler("/regions/list?availability=yes",
                    Resources.VultrRegionsList),
                new CustomMockHttpHandler("/plans/list?type=all",
                    Resources.VultrPlansList),
                new CustomMockHttpHandler("/server/list")
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
                new CustomMockHttpHandler(
                    "/os/list", Resources.VultrOSList),
                new CustomMockHttpHandler("/regions/list?availability=yes",
                    Resources.VultrRegionsList),
                new CustomMockHttpHandler("/plans/list?type=all",
                    Resources.VultrPlansList),
                new CustomMockHttpHandler(
                    "/server/list", Resources.VultrServerList),
                new CustomMockHttpHandler(
                    "/server/destroy", "SUBID=576965", ""),
                new CustomMockHttpHandler("/server/create",
                    "DCID=1&VPSPLANID=201&OSID=389&enable_private_network=no&label=my+new+server&notify_activate=no",
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
                new CustomMockHttpHandler(
                    "/os/list", Resources.VultrOSList),
                new CustomMockHttpHandler("/regions/list?availability=yes",
                    Resources.VultrRegionsList),
                new CustomMockHttpHandler("/plans/list?type=all",
                    Resources.VultrPlansList),
                new CustomMockHttpHandler(
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
                new CustomMockHttpHandler(
                    "/os/list", Resources.VultrOSList),
                new CustomMockHttpHandler("/regions/list?availability=yes",
                    Resources.VultrRegionsList),
                new CustomMockHttpHandler("/plans/list?type=all",
                    Resources.VultrPlansList),
                new CustomMockHttpHandler(
                    "/server/list", Resources.VultrServerList)
            );
            new VultrServerProvisioner(requests.Client).Provision(server);
            requests.AssertAllCalledOnce();
        }

        #endregion

        #region Test VultrPlatform.Provision(Script, bool)

        /// <summary>
        /// Tests that VultrPlatform.Provision(Script) creates a new script.
        /// </summary>
        [Fact]
        public void TestProvisionScript()
        {
            var script = new Script(
                "myscript", ScriptType.Boot, "this is my script");

            using var requests = new MockVultrRequests(
                new CustomMockHttpHandler("/startupscript/list"),
                new CustomMockHttpHandler(
                    "/startupscript/create",
                    "name=myscript&script=this+is+my+script&type=boot",
                    "{\"SCRIPTID\": 5}")
            );
            new VultrScriptProvisioner(requests.Client).Provision(script);
            requests.AssertAllCalledOnce();
        }

        /// <summary>
        /// Tests that VultrPlatform.Provision(Script, dryrun: true) doesn't create a new
        /// script.
        /// </summary>
        [Fact]
        public void TestProvisionScriptDryrun()
        {
            var script = new Script(
                "myscript", ScriptType.Boot, "this is my script");

            using var requests = new MockVultrRequests(
                new CustomMockHttpHandler("/startupscript/list")
            );
            new VultrScriptProvisioner(requests.Client).Provision(script, true);

            requests.AssertAllCalledOnce();
        }

        /// <summary>
        /// Tests that VultrPlatform.Provision(Script) updates the content of an existing
        /// script.
        /// </summary>
        [Fact]
        public void TestProvisionUpdateScriptContent()
        {
            var script = new Script(
                "hello-boot", ScriptType.Boot, "this is my script");

            using var requests = new MockVultrRequests(
                new CustomMockHttpHandler(
                    "/startupscript/list", Resources.VultrStartupScripts),
                new CustomMockHttpHandler(
                    "/startupscript/update",
                    "SCRIPTID=3&script=this+is+my+script",
                    "")
            );
            new VultrScriptProvisioner(requests.Client).Provision(script);

            requests.AssertAllCalledOnce();
        }

        /// <summary>
        /// Tests that VultrPlatform.Provision(Script, dryrun: true) doesn't update
        /// the content of an existing script.
        /// </summary>
        [Fact]
        public void TestProvisionUpdateScriptContentDryrun()
        {
            var script = new Script(
                "hello-boot", ScriptType.Boot, "this is my script");

            using var requests = new MockVultrRequests(
                new CustomMockHttpHandler(
                    "/startupscript/list", Resources.VultrStartupScripts)
            );
            new VultrScriptProvisioner(requests.Client).Provision(script, true);

            requests.AssertAllCalledOnce();
        }

        /// <summary>
        /// Tests that VultrPlatform.Provision(Script) updates the script type and content.
        /// </summary>
        [Fact]
        public void TestProvisionUpdateScriptType()
        {
            var script = new Script(
                "hello-boot", ScriptType.PXE, "this is my script");

            using var requests = new MockVultrRequests(
                new CustomMockHttpHandler(
                    "/startupscript/list", Resources.VultrStartupScripts),
                new CustomMockHttpHandler(
                    "/startupscript/destroy", "SCRIPTID=3", ""),
                new CustomMockHttpHandler(
                    "/startupscript/create",
                    "name=hello-boot&script=this+is+my+script&type=pxe",
                    "{\"SCRIPTID\": 5}")
            );
            new VultrScriptProvisioner(requests.Client).Provision(script);

            requests.AssertAllCalledOnce();
        }

        /// <summary>
        /// Tests that VultrPlatform.Provision(Script, dryrun: true) doesn't update
        /// the script type or content.
        /// </summary>
        [Fact]
        public void TestProvisionUpdateScriptTypeDryrun()
        {
            var script = new Script(
                "hello-boot", ScriptType.PXE, "this is my script");

            using var requests = new MockVultrRequests(
                new CustomMockHttpHandler(
                    "/startupscript/list", Resources.VultrStartupScripts)
            );
            new VultrScriptProvisioner(requests.Client).Provision(script, true);

            requests.AssertAllCalledOnce();
        }

        /// <summary>
        /// Tests that VultrPlatform.Provision(Script) doesn't change a script that
        /// already matches what the configuration defines.
        /// </summary>
        [Fact]
        public void TestProvisionDontUpdateUnchangedScript()
        {
            var script = new Script("hello-boot", ScriptType.Boot,
                "#!/bin/bash echo Hello World > /root/hello");

            using var requests = new MockVultrRequests(
                new CustomMockHttpHandler(
                    "/startupscript/list", Resources.VultrStartupScripts)
            );
            new VultrScriptProvisioner(requests.Client).Provision(script);

            requests.AssertAllCalledOnce();
        }

        #endregion
    }
}
