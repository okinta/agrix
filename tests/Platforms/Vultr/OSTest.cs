using agrix.Platforms.Vultr;
using Vultr.API;
using Xunit;

namespace tests.Platforms.Vultr
{
    public class OSTest
    {
        [Theory]
        [InlineData(37, "Docker on Ubuntu 18.04 x64")]
        [InlineData(38, "cPanel on CentOS 7 x64")]
        public void TestCreateApp(int id, string name)
        {
            var client = new VultrClient(Settings.Default.VultrApiKey);
            var app = VultrOS.CreateApp(name, client);

            Assert.Equal(186, app.OSID);
            Assert.Equal(id, app.APPID);
            Assert.Null(app.ISOID);
            Assert.Null(app.SCRIPTID);
            Assert.Null(app.SNAPSHOTID);
        }

        [Theory]
        [InlineData("server2019.iso")]
        [InlineData("installcoreos.iso")]
        public void TestCreateISO(string name)
        {
            var client = new VultrClient(Settings.Default.VultrApiKey);
            var iso = VultrOS.CreateISO(name, client);

            Assert.Equal(159, iso.OSID);
            Assert.True(iso.ISOID > 0);
            Assert.Null(iso.APPID);
            Assert.Null(iso.SCRIPTID);
            Assert.Null(iso.SNAPSHOTID);
        }

        [Theory]
        [InlineData(230, "FreeBSD 11 x64")]
        [InlineData(240, "Windows 2016 x64")]
        public void TestCreateOS(int id, string name)
        {
            var client = new VultrClient(Settings.Default.VultrApiKey);
            var os = VultrOS.CreateOS(name, client);

            Assert.Equal(id, os.OSID);
            Assert.Null(os.APPID);
            Assert.Null(os.ISOID);
            Assert.Null(os.SCRIPTID);
            Assert.Null(os.SNAPSHOTID);
        }

        [Theory]
        [InlineData("setup-ubuntu")]
        public void TestCreateScript(string name)
        {
            var client = new VultrClient(Settings.Default.VultrApiKey);
            var script = VultrOS.CreateScript("FreeBSD 11 x64", name, client);

            Assert.Equal(230, script.OSID);
            Assert.Null(script.APPID);
            Assert.Null(script.ISOID);
            Assert.Null(script.SNAPSHOTID);
            Assert.True(script.SCRIPTID > 0);
        }
    }
}
