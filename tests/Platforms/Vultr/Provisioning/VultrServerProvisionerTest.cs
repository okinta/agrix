using agrix.Configuration;
using agrix.Platforms.Vultr.Provisioners;
using Vultr.API;
using Xunit;

namespace tests.Platforms.Vultr.Provisioning
{
    public class VultrServerProvisionerTest
    {
        private static Plan Plan => new Plan(1, 1024, "SSD");

        private const string Region = "New Jersey";

        private static VultrServerProvisioner Platform
        {
            get
            {
                var client = new VultrClient(Settings.Default.VultrApiKey);
                return new VultrServerProvisioner(client);
            }
        }

        [Theory]
        [InlineData(1, "New Jersey")]
        [InlineData(40, "Singapore")]
        public void TestGetRegionId(int id, string name)
        {
            Assert.Equal(id, Platform.GetRegionId(name));
        }

        [Theory]
        [InlineData(204, 4, 8192, "SSD")]
        [InlineData(404, 4, 16384, "HIGHFREQUENCY")]
        public void TestGetPlanId(int id, int cpu, int ram, string type)
        {
            Assert.Equal(id, Platform.GetPlanId(
                new Plan(cpu, ram, type)));
        }

        [Fact]
        public void TestGetOsApp()
        {
            var server = new Server(
                new OperatingSystem(app: "LAMP on CentOS 7 x64"),
                Plan, Region);
            var os = Platform.GetOs(server);
            Assert.Equal(186, os.OsId);
            Assert.Equal(41, os.Appid);
            Assert.Null(os.IsoId);
            Assert.Null(os.ScriptId);
            Assert.Null(os.SnapshotId);
        }

        [Fact]
        public void TestGetOsScript()
        {
            var server = new Server(
                new OperatingSystem(name: "Fedora 32 x64"),
                Plan, Region, startupScript: "setup-ubuntu");
            var os = Platform.GetOs(server);
            Assert.Equal(389, os.OsId);
            Assert.Null(os.Appid);
            Assert.Null(os.IsoId);
            Assert.Null(os.SnapshotId);
            Assert.True(os.ScriptId > 0);
        }

        [Theory]
        [InlineData(389, "Fedora 32 x64")]
        [InlineData(366, "OpenBSD 6.6 x64")]
        public void TestGetOs(int id, string name)
        {
            var server = new Server(
                new OperatingSystem(name: name),
                Plan, Region);
            var os = Platform.GetOs(server);
            Assert.Equal(id, os.OsId);
            Assert.Null(os.Appid);
            Assert.Null(os.IsoId);
            Assert.Null(os.ScriptId);
            Assert.Null(os.SnapshotId);
        }

        [Fact]
        public void TestGetIso()
        {
            var server = new Server(
                new OperatingSystem(iso: "installcoreos.iso"),
                Plan, Region);
            var os = Platform.GetOs(server);
            Assert.Equal(159, os.OsId);
            Assert.Null(os.Appid);
            Assert.Null(os.ScriptId);
            Assert.Null(os.SnapshotId);
            Assert.True(os.IsoId > 0);
        }
    }
}
