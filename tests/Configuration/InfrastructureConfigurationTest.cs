﻿using agrix.Configuration;
using agrix.Platforms;
using System.IO;
using System.Text;
using tests.Properties;
using Xunit;
using YamlDotNet.RepresentationModel;

namespace tests.Configuration
{
    public class InfrastructureConfigurationTest
    {
        [Fact]
        public void TestLoadServers()
        {
            var servers = InfrastructureConfiguration.LoadServers(LoadYaml());
            Assert.Equal(3, servers.Count);
        }

        [Fact]
        public void TestLoadPlatform()
        {
            var platform = InfrastructureConfiguration.LoadPlatform(LoadYaml());
            Assert.Equal(typeof(Vultr), platform.GetType());
        }

        private YamlStream LoadYaml()
        {
            var input = new StringReader(Encoding.Default.GetString(Resources.agrix));
            var yaml = new YamlStream();
            yaml.Load(input);
            return yaml;
        }
    }
}