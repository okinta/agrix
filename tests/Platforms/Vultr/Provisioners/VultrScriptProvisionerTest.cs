using agrix.Configuration;
using agrix.Platforms.Vultr.Provisioners;
using MockHttp.Net;
using tests.Properties;
using Xunit;

namespace tests.Platforms.Vultr.Provisioners
{
    public class VultrScriptProvisionerTest
    {
        /// <summary>
        /// Tests that VultrPlatform.Provision(Script) creates a new script.
        /// </summary>
        [Fact]
        public void TestProvisionScript()
        {
            var script = new Script(
                "myscript", ScriptType.Boot, "this is my script");

            using var requests = new MockVultrRequests(
                new HttpHandler("/startupscript/list"),
                new HttpHandler(
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
                new HttpHandler("/startupscript/list")
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
                new HttpHandler(
                    "/startupscript/list", Resources.VultrStartupScripts),
                new HttpHandler(
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
                new HttpHandler(
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
                new HttpHandler(
                    "/startupscript/list", Resources.VultrStartupScripts),
                new HttpHandler(
                    "/startupscript/destroy", "SCRIPTID=3", ""),
                new HttpHandler(
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
                new HttpHandler(
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
        public void TestProvisionDoNotUpdateUnchangedScript()
        {
            var script = new Script("hello-boot", ScriptType.Boot,
                "#!/bin/bash echo Hello World > /root/hello");

            using var requests = new MockVultrRequests(
                new HttpHandler(
                    "/startupscript/list", Resources.VultrStartupScripts)
            );
            new VultrScriptProvisioner(requests.Client).Provision(script);

            requests.AssertAllCalledOnce();
        }
    }
}
