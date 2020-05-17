using System.Collections.Generic;
using agrix.Configuration;
using Xunit;

namespace tests.Configuration
{
    public class InfrastructureTest
    {
        [Fact]
        public void TestTypesEmpty()
        {
            Assert.Empty(new Infrastructure().Types);
        }

        [Fact]
        public void TestTypesDoesntAddEmpty()
        {
            var infrastructure = new Infrastructure();
            infrastructure.AddItems(new List<Server>());
            Assert.Empty(infrastructure.Types);
        }

        [Fact]
        public void TestTypes()
        {
            var infrastructure = SetupExampleInfrastructure();
            Assert.Equal(2, infrastructure.Types.Count);
            Assert.Equal(typeof(Server), infrastructure.Types[0]);
            Assert.Equal(typeof(Firewall), infrastructure.Types[1]);
        }

        [Fact]
        public void TestGetItems()
        {
            var infrastructure = SetupExampleInfrastructure();
            Assert.Equal(
                "Chicago",
                ((Server)infrastructure.GetItems(typeof(Server))[0]).Region);
        }

        private Infrastructure SetupExampleInfrastructure()
        {
            var infrastructure = new Infrastructure();

            infrastructure.AddItems(new List<Server>()
            {
                new Server(
                    new OperatingSystem(app: "docker"),
                    new Plan(4, 1024, "SSD"),
                    "Chicago")
            });

            infrastructure.AddItems(new List<Firewall>()
            {
                new Firewall("test", new List<FirewallRule>()
                {
                    new FirewallRule(IPType.V4, Protocol.TCP, "80", "cloudflare")
                })
            });

            return infrastructure;
        }
    }
}
