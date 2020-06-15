using agrix.Configuration;
using agrix.Platforms.Vultr.Destroyers;
using MockHttp.Net;
using tests.Properties;
using Xunit;

namespace tests.Platforms.Vultr.Destroyers
{
    public class VultrScriptDestroyerTest
    {
        [Fact]
        public void TestDestroyScript()
        {
            var script = new Script(
                "hello-boot", ScriptType.Boot, "this is my script");

            using var requests = new MockVultrRequests(
                new HttpHandler(
                    "/startupscript/list", Resources.VultrStartupScripts),
                new HttpHandler(
                    "/startupscript/destroy",
                    "SCRIPTID=3",
                    "")
            );
            new VultrScriptDestroyer(requests.Client).Destroy(script);
            requests.AssertAllCalledOnce();
        }

        [Fact]
        public void TestDestroyScriptDoesNotExist()
        {
            var script = new Script(
                "hello-boot", ScriptType.PXE, "this is my script");

            using var requests = new MockVultrRequests(
                new HttpHandler(
                    "/startupscript/list", Resources.VultrStartupScripts)
            );
            new VultrScriptDestroyer(requests.Client).Destroy(script);
            requests.AssertAllCalledOnce();
        }

        [Fact]
        public void TestDestroyScriptDryrun()
        {
            var script = new Script(
                "hello-boot", ScriptType.Boot, "this is my script");

            using var requests = new MockVultrRequests(
                new HttpHandler(
                    "/startupscript/list", Resources.VultrStartupScripts)
            );
            new VultrScriptDestroyer(requests.Client).Destroy(script, true);
            requests.AssertAllCalledOnce();
        }
    }
}
