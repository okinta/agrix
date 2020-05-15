using agrix.Configuration;
using agrix.Platforms.Vultr;
using System.Collections.Generic;
using System.IO;
using System.Net;
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

        [Fact]
        public void TestProvisionDryRun()
        {
            var server = new Server(
                new OperatingSystem(name: "Fedora 32 x64"),
                Plan, Region);
            Platform.Provision(server, true);
        }

        [Fact]
        public void TestProvisionScript()
        {
            var script = new Script("myscript", ScriptType.Boot, "this is my script");

            using var requests = new MockVultrRequests(
                new CustomMockHttpHandler(
                    "/startupscript/list", "GET", (req, rsp, prm) => ""),
                new CustomMockHttpHandler(
                    "/startupscript/create", "POST", CreateStartupScript)
            );
            requests.Platform.Provision(script);

            Assert.Equal(1, requests[0].Called);
            Assert.Equal(1, requests[1].Called);
        }

        private string CreateStartupScript(
            HttpListenerRequest req,
            HttpListenerResponse rsp,
            Dictionary<string, string> prm)
        {
            using var reader = new StreamReader(req.InputStream);
            Assert.Equal(
                "name=myscript&script=this+is+my+script&type=boot",
                reader.ReadToEnd());

            return "{\"SCRIPTID\": 5}";
        }
    }
}
