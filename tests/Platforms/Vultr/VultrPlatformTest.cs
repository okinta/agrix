﻿using agrix.Configuration;
using OperatingSystem = agrix.Configuration.OperatingSystem;
using tests.Properties;
using tests.TestHelpers;
using Xunit;

namespace tests.Platforms.Vultr
{
    public class VultrPlatformTest
    {
        private Plan Plan { get { return new Plan(1, 1024, "SSD"); } }

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
                Plan, Region);

            using var requests = new MockVultrRequests(
                new CustomMockHttpHandler("/os/list", Resources.VultrOSList),
                new CustomMockHttpHandler("/regions/list?availability=yes",
                    Resources.VultrRegionsList),
                new CustomMockHttpHandler("/plans/list?type=all",
                    Resources.VultrPlansList),
                new CustomMockHttpHandler("/server/list"),
                new CustomMockHttpHandler("/server/create",
                    "DCID=1&VPSPLANID=201&OSID=389&enable_private_network=no&notify_activate=no",
                    "{\"SUBID\": \"1312965\"}")
            );
            requests.Platform.Provision(server);
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
                new CustomMockHttpHandler("/os/list", Resources.VultrOSList),
                new CustomMockHttpHandler("/regions/list?availability=yes",
                    Resources.VultrRegionsList),
                new CustomMockHttpHandler("/plans/list?type=all",
                    Resources.VultrPlansList),
                new CustomMockHttpHandler("/server/list")
            );
            requests.Platform.Provision(server, dryrun: true);
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
                new CustomMockHttpHandler("/os/list", Resources.VultrOSList),
                new CustomMockHttpHandler("/regions/list?availability=yes",
                    Resources.VultrRegionsList),
                new CustomMockHttpHandler("/plans/list?type=all",
                    Resources.VultrPlansList),
                new CustomMockHttpHandler("/server/list", Resources.VultrServerList),
                new CustomMockHttpHandler("/server/destroy", "SUBID=576965", ""),
                new CustomMockHttpHandler("/server/create",
                    "DCID=1&VPSPLANID=201&OSID=389&enable_private_network=no&label=my+new+server&notify_activate=no",
                    "{\"SUBID\": \"1312965\"}")
            );
            requests.Platform.Provision(server);
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
                new CustomMockHttpHandler("/os/list", Resources.VultrOSList),
                new CustomMockHttpHandler("/regions/list?availability=yes",
                    Resources.VultrRegionsList),
                new CustomMockHttpHandler("/plans/list?type=all",
                    Resources.VultrPlansList),
                new CustomMockHttpHandler("/server/list", Resources.VultrServerList)
            );
            requests.Platform.Provision(server, dryrun: true);
            requests.AssertAllCalledOnce();
        }

        /// <summary>
        /// Tests that VultrPlatform.Provision(Server) doesn't change a server that
        /// already matches what the configuration defines.
        /// </summary>
        [Fact]
        public void TestProvisionDontUpdateUnchangedServer()
        {
            var server = new Server(
                new OperatingSystem(name: "CentOS 6 x64"),
                new Plan(2, 4096, "SSD"), Region, label: "my new server", tag: "mytag"
            );

            using var requests = new MockVultrRequests(
                new CustomMockHttpHandler("/os/list", Resources.VultrOSList),
                new CustomMockHttpHandler("/regions/list?availability=yes",
                    Resources.VultrRegionsList),
                new CustomMockHttpHandler("/plans/list?type=all",
                    Resources.VultrPlansList),
                new CustomMockHttpHandler("/server/list", Resources.VultrServerList)
            );
            requests.Platform.Provision(server);
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
            var script = new Script("myscript", ScriptType.Boot, "this is my script");

            using var requests = new MockVultrRequests(
                new CustomMockHttpHandler("/startupscript/list"),
                new CustomMockHttpHandler(
                    "/startupscript/create",
                    "name=myscript&script=this+is+my+script&type=boot",
                    "{\"SCRIPTID\": 5}")
            );
            requests.Platform.Provision(script);
            requests.AssertAllCalledOnce();
        }

        /// <summary>
        /// Tests that VultrPlatform.Provision(Script, dryrun: true) doesn't create a new
        /// script.
        /// </summary>
        [Fact]
        public void TestProvisionScriptDryrun()
        {
            var script = new Script("myscript", ScriptType.Boot, "this is my script");

            using var requests = new MockVultrRequests(
                new CustomMockHttpHandler("/startupscript/list")
            );
            requests.Platform.Provision(script, dryrun: true);

            requests.AssertAllCalledOnce();
        }

        /// <summary>
        /// Tests that VultrPlatform.Provision(Script) updates the content of an existing
        /// script.
        /// </summary>
        [Fact]
        public void TestProvisionUpdateScriptContent()
        {
            var script = new Script("hello-boot", ScriptType.Boot, "this is my script");

            using var requests = new MockVultrRequests(
                new CustomMockHttpHandler(
                    "/startupscript/list", Resources.VultrStartupScripts),
                new CustomMockHttpHandler(
                    "/startupscript/update",
                    "SCRIPTID=3&script=this+is+my+script",
                    "")
            );
            requests.Platform.Provision(script);

            requests.AssertAllCalledOnce();
        }

        /// <summary>
        /// Tests that VultrPlatform.Provision(Script, dryrun: true) doesn't update
        /// the content of an existing script.
        /// </summary>
        [Fact]
        public void TestProvisionUpdateScriptContentDryrun()
        {
            var script = new Script("hello-boot", ScriptType.Boot, "this is my script");

            using var requests = new MockVultrRequests(
                new CustomMockHttpHandler(
                    "/startupscript/list", Resources.VultrStartupScripts)
            );
            requests.Platform.Provision(script, dryrun: true);

            requests.AssertAllCalledOnce();
        }

        /// <summary>
        /// Tests that VultrPlatform.Provision(Script) updates the script type and content.
        /// </summary>
        [Fact]
        public void TestProvisionUpdateScriptType()
        {
            var script = new Script("hello-boot", ScriptType.PXE, "this is my script");

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
            requests.Platform.Provision(script);

            requests.AssertAllCalledOnce();
        }

        /// <summary>
        /// Tests that VultrPlatform.Provision(Script, dryrun: true) doesn't update
        /// the script type or content.
        /// </summary>
        [Fact]
        public void TestProvisionUpdateScriptTypeDryrun()
        {
            var script = new Script("hello-boot", ScriptType.PXE, "this is my script");

            using var requests = new MockVultrRequests(
                new CustomMockHttpHandler(
                    "/startupscript/list", Resources.VultrStartupScripts)
            );
            requests.Platform.Provision(script, dryrun: true);

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
            requests.Platform.Provision(script);

            requests.AssertAllCalledOnce();
        }

        #endregion
    }
}
