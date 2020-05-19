using agrix.Platforms.Vultr;
using MockHttp.Net;
using tests.Properties;
using Xunit;

namespace tests.Platforms.Vultr
{
    public class VultrOperatingSystemTest
    {
        [Theory]
        [InlineData(37, "Docker on Ubuntu 18.04 x64")]
        [InlineData(38, "cPanel on CentOS 7 x64")]
        public void TestCreateApp(int id, string name)
        {
            using var requests = new MockVultrRequests(
                new HttpHandler(
                    "/app/list", Resources.VultrAppList));

            var app = VultrOperatingSystem.CreateApp(name, requests.Client);
            Assert.Equal(186, app.OsId);
            Assert.Equal(id, app.Appid);
            Assert.Null(app.IsoId);
            Assert.Null(app.ScriptId);
            Assert.Null(app.SnapshotId);
        }

        [Theory]
        [InlineData("server2019.iso", 732320)]
        [InlineData("installcoreos.iso", 743054)]
        public void TestCreateIso(string name, int id)
        {
            using var requests = new MockVultrRequests(
                new HttpHandler(
                    "/iso/list", Resources.VultrISOList));

            var iso = VultrOperatingSystem.CreateIso(name, requests.Client);
            Assert.Equal(159, iso.OsId);
            Assert.Equal(id, iso.IsoId);
            Assert.Null(iso.Appid);
            Assert.Null(iso.ScriptId);
            Assert.Null(iso.SnapshotId);
        }

        [Theory]
        [InlineData(230, "FreeBSD 11 x64")]
        [InlineData(240, "Windows 2016 x64")]
        public void TestCreateOs(int id, string name)
        {
            using var requests = new MockVultrRequests(
                new HttpHandler(
                    "/os/list", Resources.VultrOSList));

            var os = VultrOperatingSystem.CreateOs(name, requests.Client);
            Assert.Equal(id, os.OsId);
            Assert.Null(os.Appid);
            Assert.Null(os.IsoId);
            Assert.Null(os.ScriptId);
            Assert.Null(os.SnapshotId);
        }

        [Theory]
        [InlineData("5359435d28b9a")]
        [InlineData("5359435dc1df3")]
        public void TestCreateSnapshot(string id)
        {
            using var requests = new MockVultrRequests(
                new HttpHandler(
                    "/snapshot/list", Resources.VultrSnapshotList));

            var snapshot = VultrOperatingSystem.CreateSnapshot(id, requests.Client);
            Assert.Equal(164, snapshot.OsId);
            Assert.Equal(id, snapshot.SnapshotId);
            Assert.Null(snapshot.Appid);
            Assert.Null(snapshot.IsoId);
            Assert.Null(snapshot.ScriptId);
        }

        [Theory]
        [InlineData("hello-boot", 3)]
        public void TestCreateScript(string name, int id)
        {
            using var requests = new MockVultrRequests(
                new HttpHandler(
                    "/os/list", Resources.VultrOSList),
                new HttpHandler(
                    "/startupscript/list", Resources.VultrStartupScripts));

            var script = VultrOperatingSystem.CreateScript(
                "FreeBSD 11 x64", name, requests.Client);
            Assert.Equal(230, script.OsId);
            Assert.Equal(id, script.ScriptId);
            Assert.Null(script.Appid);
            Assert.Null(script.IsoId);
            Assert.Null(script.SnapshotId);
        }
    }
}
