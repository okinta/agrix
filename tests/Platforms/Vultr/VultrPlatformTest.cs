using agrix.Configuration;
using agrix.Platforms.Vultr;
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
            get { return new Plan(2, 1024, "SSD"); }
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
    }
}
