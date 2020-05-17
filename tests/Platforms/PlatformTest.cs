using agrix.Configuration;
using agrix.Platforms;
using tests.Configuration;
using tests.Properties;
using Xunit;

namespace tests.Platforms
{
    internal class CustomPlatform : Platform
    {
        public CustomPlatform()
        {
            AddNullParser("empty");
            AddParser("klouts", new KloutParser().Parse);
        }

        public override void TestConnection() { }
    }

    public class PlatformTest : BaseTest
    {
        [Fact]
        public void TestLoad()
        {
            var platform = new CustomPlatform();
            var infrastructure = platform.Load(LoadYaml());
            Assert.Equal(1, infrastructure.Types.Count);

            var servers = infrastructure.GetItems(typeof(Server));
            Assert.Equal(3, servers.Count);

            var server = (Server)servers[0];
            Assert.Equal("Ubuntu 20.04 x64", server.OS.Name);

            server = (Server)servers[1];
            Assert.Equal("compute", server.Plan.Type);

            server = (Server)servers[2];
            Assert.Equal("coreos.iso", server.OS.ISO);
        }

        [Fact]
        public void TestCustomParserLoad()
        {
            var platform = new CustomPlatform();

            var infrastructure = platform.Load(LoadYaml(Resources.KloutsConfig));
            Assert.Equal(1, infrastructure.Types.Count);

            var klouts = infrastructure.GetItems(typeof(Klout));
            Assert.Equal(3, klouts.Count);
            Assert.Equal(1, ((Klout)klouts[0]).Score);
            Assert.Equal(99, ((Klout)klouts[1]).Score);
            Assert.Equal(78, ((Klout)klouts[2]).Score);
        }
    }
}
