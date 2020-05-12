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
        /// <summary>
        /// The YAML configuration to process.
        /// </summary>
        public string Configuration { get; }

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
            var input = new StringReader(Configuration);
            var yaml = new YamlStream();
            yaml.Load(input);
            InfrastructureConfiguration.LoadServers(yaml);
        }

        /// <summary>
        /// Builds infrastructure from the configuration.
        /// </summary>
        public void Process()
        {
            Validate();
        }
    }
}
