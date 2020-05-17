using agrix.Platforms.Vultr;
using Vultr.API.Models;
using Xunit;

namespace tests.Platforms.Vultr
{
    public class VultrServerExtensionsTest
    {
        [Fact]
        public void TestIsEquivalent()
        {
            Assert.True(new Server
            {
                APPID = "123",
                tag = "test"
            }.IsEquivalent(new Server
            {
                APPID = "123",
                tag = "test"
            }));

            Assert.True(new Server
            {
                APPID = "123",
                tag = "test"
            }.IsEquivalent(new Server
            {
                APPID = "123",
                tag = "test",
                default_password = "698hrg"
            }));
        }

        [Fact]
        public void TestIsNotEquivalent()
        {
            Assert.False(new Server
            {
                APPID = "123",
                tag = "tet"
            }.IsEquivalent(new Server
            {
                APPID = "123",
                tag = "test"
            }));
        }
    }
}
