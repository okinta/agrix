using agrix.Configuration;
using agrix.Platforms.Vultr;
using Xunit;

namespace tests.Platforms.Vultr
{
    public class VultrPlatformTest
    {
        [Theory]
        [InlineData(1, "New Jersey")]
        [InlineData(40, "Singapore")]
        public void TestGetRegionID(int id, string name)
        {
            var vultr = new VultrPlatform(Settings.Default.VultrApiKey);
            Assert.Equal(id, vultr.GetRegionID(name));
        }

        [Theory]
        [InlineData(204, 4, 8192, "SSD")]
        [InlineData(404, 4, 16384, "HIGHFREQUENCY")]
        public void TestGetPlanID(int id, int cpu, int ram, string type)
        {
            var vultr = new VultrPlatform(Settings.Default.VultrApiKey);
            Assert.Equal(id, vultr.GetPlanID(new Plan(cpu, ram, type)));
        }
    }
}
