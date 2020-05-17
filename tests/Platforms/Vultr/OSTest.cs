using agrix.Platforms.Vultr;
using Vultr.API;
using Xunit;

namespace tests.Platforms.Vultr
{
    public class OsTest
    {
        [Theory]
        [InlineData(37, "Docker on Ubuntu 18.04 x64")]
        [InlineData(38, "cPanel on CentOS 7 x64")]
        public void TestCreateApp(int id, string name)
        {
            var client = new VultrClient(Settings.Default.VultrApiKey);
            var app = VultrOs.CreateApp(name, client);

            Assert.Equal(186, app.OsId);
            Assert.Equal(id, app.Appid);
            Assert.Null(app.IsoId);
            Assert.Null(app.ScriptId);
            Assert.Null(app.SnapshotId);
        }

        [Theory]
        [InlineData("server2019.iso")]
        [InlineData("installcoreos.iso")]
        public void TestCreateIso(string name)
        {
            var client = new VultrClient(Settings.Default.VultrApiKey);
            var iso = VultrOs.CreateIso(name, client);

            Assert.Equal(159, iso.OsId);
            Assert.True(iso.IsoId > 0);
            Assert.Null(iso.Appid);
            Assert.Null(iso.ScriptId);
            Assert.Null(iso.SnapshotId);
        }

        [Theory]
        [InlineData(230, "FreeBSD 11 x64")]
        [InlineData(240, "Windows 2016 x64")]
        public void TestCreateOs(int id, string name)
        {
            var client = new VultrClient(Settings.Default.VultrApiKey);
            var os = VultrOs.CreateOs(name, client);

            Assert.Equal(id, os.OsId);
            Assert.Null(os.Appid);
            Assert.Null(os.IsoId);
            Assert.Null(os.ScriptId);
            Assert.Null(os.SnapshotId);
        }

        [Theory]
        [InlineData("setup-ubuntu")]
        public void TestCreateScript(string name)
        {
            var client = new VultrClient(Settings.Default.VultrApiKey);
            var script = VultrOs.CreateScript("FreeBSD 11 x64", name, client);

            Assert.Equal(230, script.OsId);
            Assert.Null(script.Appid);
            Assert.Null(script.IsoId);
            Assert.Null(script.SnapshotId);
            Assert.True(script.ScriptId > 0);
        }
    }
}
