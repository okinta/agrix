﻿using System;
using System.IO;
using agrix.Configuration;
using YamlDotNet.RepresentationModel;

namespace agrix
{
    /// <summary>
    /// Provides an interface to convert code to infrastructure.
    /// </summary>
    internal class Agrix
    {
        private YamlStream yaml;

        /// <summary>
        /// The YAML configuration to process, as a string.
        /// </summary>
        public string Configuration { get; }

        /// <summary>
        /// The YAML configuration to process, as a YamlStream.
        /// </summary>
        public YamlStream YAML
        {
            get
            {
                if (yaml is null)
                {
                    yaml = new YamlStream();
                    yaml.Load(new StringReader(Configuration));
                }

                return yaml;
            }
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="configuration">The YAML configuration to process.</param>
        public Agrix(string configuration)
        {
            if (string.IsNullOrEmpty(configuration))
            {
                throw new ArgumentNullException("configuration", "must not be empty");
            }

            Configuration = configuration;
        }

        /// <summary>
        /// Validates the YAML configuration to ensure correctness.
        /// </summary>
        public void Validate()
        {
            InfrastructureConfiguration.LoadPlatform(YAML);
            InfrastructureConfiguration.LoadServers(YAML);
        }

        /// <summary>
        /// Builds infrastructure from the configuration.
        /// </summary>
        public void Process()
        {
            var platform = InfrastructureConfiguration.LoadPlatform(YAML);
            var servers = InfrastructureConfiguration.LoadServers(YAML);

            foreach (var server in servers)
            {
                platform.Provision(server);
            }
        }
    }
}
