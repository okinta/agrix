using agrix.Configuration.Parsers;
using agrix.Extensions;
using tests.Properties;
using Xunit;

namespace tests.Configuration.Parsers
{
    public class ServerParserTest : BaseTest
    {
        [Fact]
        public void TestParse()
        {
            var servers = LoadYaml().GetSequence("servers");
            var server = new ServerParser().Parse(servers[0]);
            Assert.Equal("http", server.Firewall);
            Assert.True(server.PrivateNetworking);
            Assert.Equal("myscript", server.StartupScript);

            server = new ServerParser().Parse(servers[1]);
            Assert.Equal("Atlanta", server.Region);

            server = new ServerParser().Parse(servers[2]);
            Assert.Equal("Chicago", server.Region);
            Assert.Equal(4096, server.Plan.Memory);
        }

        [Fact]
        public void TestParseUserData()
        {
            var servers = LoadYaml(Resources.UserDataConfig)
                .GetSequence("servers");
            var server = new ServerParser().Parse(servers[0]);
            Assert.Equal("test data", server.UserData);
        }

        [Fact]
        public void TestParseJsonUserData()
        {
            var data = Resources.JSONUserDataConfig;
            var servers = LoadYaml(data).GetSequence("servers");

            var server = new ServerParser().Parse(servers[0]);
            Assert.Equal("{\"my-array\": [\"1\", \"2\"]}", server.UserData);
        }
    }
}
