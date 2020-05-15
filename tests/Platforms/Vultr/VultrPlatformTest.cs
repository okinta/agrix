using agrix.Configuration;
using agrix.Platforms.Vultr;
using OperatingSystem = agrix.Configuration.OperatingSystem;
using System.Collections.Generic;
using System.Net;
using tests.Properties;
using tests.TestHelpers;
using Xunit;

namespace tests.Platforms.Vultr
{
    public class VultrPlatformTest
    {
        private VultrPlatform Platform
        {
            get { return new VultrPlatform(Settings.Default.VultrApiKey); }
        }

        private Plan Plan
        {
            get { return new Plan(1, 1024, "SSD"); }
        }

        private const string Region = "New Jersey";

        [Theory]
        [InlineData(1, "New Jersey")]
        [InlineData(40, "Singapore")]
        public void TestGetRegionID(int id, string name)
        {
            Assert.Equal(id, Platform.GetRegionID(name));
        }

        [Theory]
        [InlineData(204, 4, 8192, "SSD")]
        [InlineData(404, 4, 16384, "HIGHFREQUENCY")]
        public void TestGetPlanID(int id, int cpu, int ram, string type)
        {
            Assert.Equal(id, Platform.GetPlanID(new Plan(cpu, ram, type)));
        }

        [Fact]
        public void TestGetOSApp()
        {
            var server = new Server(
                new OperatingSystem(app: "LAMP on CentOS 7 x64"),
                Plan, Region);
            var os = Platform.GetOS(server);
            Assert.Equal(186, os.OSID);
            Assert.Equal(41, os.APPID);
            Assert.Null(os.ISOID);
            Assert.Null(os.SCRIPTID);
            Assert.Null(os.SNAPSHOTID);
        }

        [Fact]
        public void TestGetOSScript()
        {
            var server = new Server(
                new OperatingSystem(name: "Fedora 32 x64"),
                Plan, Region, startupScript: "setup-ubuntu");
            var os = Platform.GetOS(server);
            Assert.Equal(389, os.OSID);
            Assert.Null(os.APPID);
            Assert.Null(os.ISOID);
            Assert.Null(os.SNAPSHOTID);
            Assert.True(os.SCRIPTID > 0);
        }

        [Theory]
        [InlineData(389, "Fedora 32 x64")]
        [InlineData(366, "OpenBSD 6.6 x64")]
        public void TestGetOS(int id, string name)
        {
            var server = new Server(
                new OperatingSystem(name: name),
                Plan, Region);
            var os = Platform.GetOS(server);
            Assert.Equal(id, os.OSID);
            Assert.Null(os.APPID);
            Assert.Null(os.ISOID);
            Assert.Null(os.SCRIPTID);
            Assert.Null(os.SNAPSHOTID);
        }

        [Fact]
        public void TestGetISO()
        {
            var server = new Server(
                new OperatingSystem(iso: "installcoreos.iso"),
                Plan, Region);
            var os = Platform.GetOS(server);
            Assert.Equal(159, os.OSID);
            Assert.Null(os.APPID);
            Assert.Null(os.SCRIPTID);
            Assert.Null(os.SNAPSHOTID);
            Assert.True(os.ISOID > 0);
        }

        #region Test VultrPlatform.Provision(Server, bool)

        [Fact]
        public void TestProvisionServer()
        {
            var server = new Server(
                new OperatingSystem(name: "Fedora 32 x64"),
                Plan, Region);

            static string CreateServer(
                HttpListenerRequest req,
                HttpListenerResponse rsp,
                Dictionary<string, string> prm)
            {
                Assert.Equal(
                    "DCID=1&VPSPLANID=201&OSID=389&enable_private_network=no&notify_activate=no",
                    req.GetContent());

                return "{\"SUBID\": \"1312965\"}";
            }

            using var requests = new MockVultrRequests(
                new CustomMockHttpHandler("/os/list", Resources.VultrOSList),
                new CustomMockHttpHandler("/regions/list?availability=yes",
                    Resources.VultrRegionsList),
                new CustomMockHttpHandler("/plans/list?type=all",
                    Resources.VultrPlansList),
                // new CustomMockHttpHandler("/server/list"),
                new CustomMockHttpHandler("/server/create", CreateServer)
            );
            requests.Platform.Provision(server);
            requests.AssertAllCalledOnce();
        }

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
            // requests.AssertAllCalledOnce();
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

            static string CreateStartupScript(
                HttpListenerRequest req,
                HttpListenerResponse rsp,
                Dictionary<string, string> prm)
            {
                Assert.Equal(
                    "name=myscript&script=this+is+my+script&type=boot",
                    req.GetContent());

                return "{\"SCRIPTID\": 5}";
            }

            using var requests = new MockVultrRequests(
                new CustomMockHttpHandler("/startupscript/list"),
                new CustomMockHttpHandler(
                    "/startupscript/create", CreateStartupScript)
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
                new CustomMockHttpHandler("/startupscript/list", "")
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

            static string UpdateStartupScript(
                HttpListenerRequest req,
                HttpListenerResponse rsp,
                Dictionary<string, string> prm)
            {
                Assert.Equal(
                    "SCRIPTID=3&script=this+is+my+script",
                    req.GetContent());

                return "";
            }

            using var requests = new MockVultrRequests(
                new CustomMockHttpHandler(
                    "/startupscript/list", Resources.VultrStartupScripts),
                new CustomMockHttpHandler(
                    "/startupscript/update", UpdateStartupScript)
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

            static string DestroyStartupScript(
                HttpListenerRequest req,
                HttpListenerResponse rsp,
                Dictionary<string, string> prm)
            {
                Assert.Equal(
                    "SCRIPTID=3",
                    req.GetContent());

                return "";
            }

            static string CreateStartupScript(
                HttpListenerRequest req,
                HttpListenerResponse rsp,
                Dictionary<string, string> prm)
            {
                Assert.Equal(
                    "name=hello-boot&script=this+is+my+script&type=pxe",
                    req.GetContent());

                return "{\"SCRIPTID\": 5}";
            }

            using var requests = new MockVultrRequests(
                new CustomMockHttpHandler(
                    "/startupscript/list", Resources.VultrStartupScripts),
                new CustomMockHttpHandler(
                    "/startupscript/destroy", DestroyStartupScript),
                new CustomMockHttpHandler(
                    "/startupscript/create", CreateStartupScript)
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
