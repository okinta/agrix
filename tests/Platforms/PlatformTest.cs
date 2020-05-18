using agrix.Configuration;
using agrix.Platforms;
using System.Collections.Generic;
using System;
using tests.Configuration;
using tests.Properties;
using Xunit;

namespace tests.Platforms
{
    internal class CustomPlatform : Platform
    {
        public IReadOnlyList<Tuple<Klout, bool>> Provisions => _provisions;

        private readonly List<Tuple<Klout, bool>> _provisions =
            new List<Tuple<Klout, bool>>();

        public CustomPlatform() : this("abc", null)
        {
        }

        public CustomPlatform(string apiKey, string apiUrl) : base(apiKey, apiUrl)
        {
            AddNullParser("empty");
            AddParser("klouts", new KloutParser().Parse);
            AddProvisioner<Klout>(Provision);
        }

        public override void TestConnection() { }

        private void Provision(Klout klout, bool dryrun = false)
        {
            _provisions.Add(new Tuple<Klout, bool>(klout, dryrun));
        }
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
            Assert.Equal("Ubuntu 20.04 x64", server.Os.Name);

            server = (Server)servers[1];
            Assert.Equal("compute", server.Plan.Type);

            server = (Server)servers[2];
            Assert.Equal("coreos.iso", server.Os.Iso);
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

        [Fact]
        public void TestProvision()
        {
            var platform = new CustomPlatform();

            var infrastructure = platform.Load(LoadYaml(Resources.KloutsConfig));
            platform.Provision(infrastructure);

            Assert.Equal(3, platform.Provisions.Count);
            Assert.Equal(99, platform.Provisions[1].Item1.Score);
            Assert.False(platform.Provisions[2].Item2);
        }

        [Fact]
        public void TestProvisionDryrun()
        {
            var platform = new CustomPlatform();

            var infrastructure = platform.Load(LoadYaml(Resources.KloutsConfig));
            platform.Provision(infrastructure, true);

            Assert.Equal(3, platform.Provisions.Count);
            Assert.Equal(78, platform.Provisions[2].Item1.Score);
            Assert.True(platform.Provisions[0].Item2);
        }
    }
}
