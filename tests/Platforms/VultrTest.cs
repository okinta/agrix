using Xunit;

namespace tests.Platforms
{
    public class VultrTest
    {
        [Fact]
        public void TestGetRegionID()
        {
            var vultr = new agrix.Platforms.Vultr(Settings.Default.VultrApiKey);
            Assert.Equal(1, vultr.GetRegionID("New Jersey"));
        }
    }
}
